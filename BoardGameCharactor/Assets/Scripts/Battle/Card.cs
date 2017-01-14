using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common;

public class Card : MonoBehaviour {

    //初期位置座標
    private Vector3 initCardPosition;

    // マウス位置座標
    private Vector3 Mouseposition;

    // ビューポイント座標をキャンバスの座標に変換した位置座標
    private Vector3 _world_position;

    //RectTransfromの取得
    private RectTransform uI_Element;

    //キャンバスのRectTransfromを取得
    private RectTransform CanvasRect;

    //BattleManagerの取得
    private BattlePhaseManager _battle_Phase_Manager;

    private PlayerManager _player_Manager;

    [SerializeField]
    //自身のカードデータを取得
    private CARD_DATA ownCardData;

    //カードの画像を取得
    private Material _material;

    //自身がセレクトエリアに入っているか
    private bool InSelectArea = false;

    //セレクトエリアのどこを使用しているか -1はどこも使用してない
    private int SelectAreaUseID = -1;

    // Use this for initialization
    void Awake() {

        //バトルマネージャーの取得
        if (_battle_Phase_Manager == null) {
            GameObject _battle_Manager_Obj = GameObject.Find("BattlePhaseManager");
            if (_battle_Manager_Obj != null) {
                _battle_Phase_Manager = _battle_Manager_Obj.GetComponent<BattlePhaseManager>();
            }
        }
        //プレイヤーマネージャーの取得
        if (_player_Manager == null) {
            GameObject _player_Manager_Obj = GameObject.Find("PlayerManager");
            _player_Manager = _player_Manager_Obj.GetComponent<PlayerManager>();
        };

    }

    void Start() {
        //初期位置を取得
        initCardPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        Vector3 objectPointInScreen
                    = Camera.main.WorldToScreenPoint ( this.transform.position );

        Vector3 mousePointInScreen
            = new Vector3 ( Input.mousePosition.x,
                          Input.mousePosition.y,
                          objectPointInScreen.z );

        _world_position = Camera.main.ScreenToWorldPoint ( mousePointInScreen );
        _world_position.z = this.transform.position.z;

        //マウスの位置へカードが移動
        this.transform.position = _world_position;
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
        this.GetComponent<Renderer> ( ).material = _material;

        //カードデータを設定
        ownCardData = setData;
    }

    public void drag() {
        //バトルフェイズマネージャーでカードセレクトが始まっているなら動くように、存在しなければ動かさない
        if (_battle_Phase_Manager != null) {
            if (_battle_Phase_Manager.getCardSelectStart()) {

            }
        }
    }
    public void onPointUp() {
        //バトルフェイズマネージャーでカードセレクトが始まっているなら動くように、存在しなければ動かさない
        if (_battle_Phase_Manager != null) {
            //カードセレクト中か
            if (_battle_Phase_Manager.getCardSelectStart()) {
                //現在のマウスカーソルの場所を取得
                Vector3 mousePos = Input.mousePosition;

                //マウスカーソルの場所へ飛ばすRayの生成
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit = new RaycastHit();
                //ヒットしたなら
                if (Physics.Raycast(ray, out hit)) {
                    //セレクトエリアに当たったなら特定の位置へ移動それ以外なら戻す
                    if (hit.collider.tag == "SelectArea") {
                        //セレクトエリアフラグをON
                        InSelectArea = true;

                        //セレクトエリアのポジションに移動できたかどうかを取得
                        bool isSelectAreaCheck = _player_Manager.SetSelectAreaPosition(ownCardData);

                        //セレクトエリアのカードが全て使用されて移動できなかったら初期位置へ
                        if (!isSelectAreaCheck) {
                            cardReturn();
                        }
                    } else {
						//セレクトエリアから出たので自身を選択エリアから解除する
						_player_Manager.SetSelectAreaOut( ownCardData );
						//セレクトエリア以外に当たった
                        cardReturn();
                    }
                } else {
					//セレクトエリアから出たので自身を選択エリアから解除する
					_player_Manager.SetSelectAreaOut( ownCardData );
					//どこにも当たらなかった
                    cardReturn();
                }
            }
        }
    }

    //セレクトエリアに入ってるかどうかを取得
    public bool getInSelectArea() {
        return InSelectArea;
    }

	//セレクトエリアの使用箇所を設定
    public void setSelectAreaUseId( int setUseID ){
        SelectAreaUseID = setUseID;
    }

	//自身がどのセレクトエリアを使用しているのか取得
    public int getSelectUseId() {
        return SelectAreaUseID;
    }

    void cardReturn() {
        //初期位置へ
        uI_Element.anchoredPosition = initCardPosition;
        //セレクトエリアフラグをOFFに
        InSelectArea = false;
    }
}
