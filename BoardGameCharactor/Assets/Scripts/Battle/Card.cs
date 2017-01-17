using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common;

public class Card : MonoBehaviour {

	private Vector3 _init_card_position;    				//初期位置座標
	private Vector3 _mouse_position;    					// マウス位置座標
	private Vector3 _world_position;    				    // マウスポイントをワールド位置座標に

	private BattlePhaseManager _battle_phase_manager;       //BattleManagerの取得
    private PlayerManager _player_manager;				    //プレイヤーマネージャーの取得
	private GameObject _card_front;						    //カードの前面を取得
    [SerializeField]
	private CARD_DATA _own_card_data;    				    //自身のカードデータを取得
    private Material _material;							    //フロントに貼るマテリアルを取得

	private bool _in_select_area = false;    				//自身がセレクトエリアに入っているか
	private int _select_area_use_iD = -1;   				//セレクトエリアのどこを使用しているか -1はどこも使用してない

    // Use this for initialization
    void Awake() {

        //バトルマネージャーの取得
        if ( _battle_phase_manager == null ) {
            _battle_phase_manager = GameObject.Find ( "BattlePhaseManager" ).GetComponent< BattlePhaseManager > ( );
        }
        //プレイヤーマネージャーの取得
        if ( _player_manager == null ) {
            _player_manager = GameObject.Find ( "PlayerManager" ).GetComponent< PlayerManager > ( );
        };

        //カードの前面オブジェクトを取得
        if( _material == null ) {
            _card_front = gameObject.transform.FindChild ( "Front" ).gameObject;

        }

    }

    void Start() {
        //初期位置を取得
        _init_card_position = transform.position;
    }

    // Update is called once per frame
    void Update() {
        getMousePos ( );
        onPointUp ( );
        drag ();
    }

    //カードデータを設定する関数
    public void SetCardData(CARD_DATA setData) {
        //カードタイプを見て画像を設定
		switch (setData.enchant_type) {
		case "enhance":
                _material = Resources.Load<Material>( "Materials/Cards/card_dagger" );
                break;
		case "drow":
                _material = Resources.Load<Material> ( "Materials/Cards/card_drug" );
                break;
		case "turn":
                _material = Resources.Load<Material> ( "Materials/Cards/card_hat" );
                break;
        case "UNAVAILABLE":
                _material = Resources.Load<Material> ( "Materials/Cards/card_sword" );
                break;
        case "CARD_TYPE_INSURANCE":
                _material = Resources.Load<Material> ( "Materials/Cards/card_boots" );
                break;
        }
        //前面のマテリアルを変更
        _card_front.GetComponent<Renderer> ( ).material = _material;

        //カードデータを設定
        _own_card_data = setData;
    }

    void getMousePos ( ) {
        Vector3 objectPointInScreen = Camera.main.WorldToScreenPoint ( this.transform.position );

        Vector3 mousePointInScreen = new Vector3 ( Input.mousePosition.x, Input.mousePosition.y, objectPointInScreen.z );

        _world_position = Camera.main.ScreenToWorldPoint ( mousePointInScreen );
        _world_position.z = this.transform.position.z;
    }

   void drag( ) {
        //カードセレクト中か
        if ( _battle_phase_manager.getCardSelectStart( ) ) {

            //左クリックされているなら
            if ( Input.GetMouseButton ( ( 0 ) ) ) {
                //現在のマウスカーソルの場所を取得
                Vector3 mousePos = Input.mousePosition;
                //マウスカーソルの場所へ飛ばすRayの生成
                Ray ray = Camera.main.ScreenPointToRay( mousePos );
                RaycastHit hit = new RaycastHit( );
                //ヒットしたなら
                if ( Physics.Raycast( ray, out hit ) ) {
                    if ( hit.collider.tag == "Card" ) {
                        //マウスの位置へカードが移動
                        this.transform.position = _world_position;
                    }
                }
            }
        }
    }
    void onPointUp ( ) {
        //カードセレクト中か
        if ( _battle_phase_manager.getCardSelectStart ( ) ) {
            //左ボタンが放されたら
            if ( Input.GetMouseButtonUp ( 0 ) ) {
                //現在のマウスカーソルの場所を取得
                Vector3 mousePos = Input.mousePosition;

                //マウスカーソルの場所へ飛ばすRayの生成
                Ray ray = Camera.main.ScreenPointToRay ( mousePos );
                RaycastHit hit = new RaycastHit ( );
                //ヒットしたなら
                if ( Physics.Raycast ( ray, out hit ) ) {
                    //セレクトエリアに当たったなら特定の位置へ移動それ以外なら戻す
                    if ( hit.collider.tag == "SelectArea" ) {
                        //セレクトエリアフラグをON
                        _in_select_area = true;

                        //セレクトエリアのポジションに移動できたかどうかを取得
                        bool isSelectAreaCheck = _player_manager.setSelectAreaPosition ( _own_card_data );

                        //セレクトエリアのカードが全て使用されて移動できなかったら初期位置へ
                        if ( !isSelectAreaCheck ) {
                            cardReturn ( );
                        }
                    } else {
                        //セレクトエリアから出たので自身を選択エリアから解除する
                        _player_manager.setSelectAreaOut ( _own_card_data );
                        //セレクトエリア以外に当たった
                        cardReturn ( );
                    }
                } else {
                    //セレクトエリアから出たので自身を選択エリアから解除する
                    _player_manager.setSelectAreaOut ( _own_card_data );
                    //どこにも当たらなかった
                    cardReturn ( );
                }
            }
        }
    }

    //セレクトエリアに入ってるかどうかを取得
    public bool getInSelectArea() {
        return _in_select_area;
    }

	//セレクトエリアの使用箇所を設定
    public void setSelectAreaUseId( int setUseID ){
        _select_area_use_iD = setUseID;
    }

	//自身がどのセレクトエリアを使用しているのか取得
    public int getSelectUseId() {
        return _select_area_use_iD;
    }

    void cardReturn() {
        //初期位置へ
		this.transform.position = _init_card_position;
        //セレクトエリアフラグをOFFに
        _in_select_area = false;
    }
}
