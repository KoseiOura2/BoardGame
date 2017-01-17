using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class massData : MonoBehaviour {

    //テキストの取得
    private Text _effect_window_text;

	//_content_Rootを取得
	private RectTransform _content;

	//Canvasを取得
	private GameObject _canvas_root;

	//プレイヤーフェイズマネージャーを取得
	private PlayerPhaseManager _player_phase_manager;

	//プレイヤーマネージャーを取得
	private PlayerManager _player_manager;

	//マスの場所を保存
	private int _masu_id;

	//自身のイメージ
	private Image img_source;

	//書き換えスプライト
	private Sprite img_sprite;

    [SerializeField]
	//書き換えテキスト
	private string _effect_Text;

    //書き換える前のテキスト
    private string _effect_return_text;

	void Awake () {
		img_source = GetComponent< Image > ( );

		//キャンバスを取得
		if ( _canvas_root == null ) {
			_canvas_root = GameObject.Find ( "Canvas" );
		}
		//Scrollのコンテンツを取得
		if ( _content == null ) {
			_content = GameObject.Find( "Content" ).GetComponent< RectTransform > ( );
		}
		//プレイヤーフェイズのマネージャーを取得
		if ( _player_phase_manager == null ){
			_player_phase_manager = GameObject.Find ( "PlayerPhaseManager" ).GetComponent< PlayerPhaseManager > ( );
		}

		//プレイヤーマネージャーを取得
		if ( _player_manager == null ){
			_player_manager = GameObject.Find ( "PlayerManager" ).GetComponent< PlayerManager > ( );
		}

        //エフェクトウィンドウのテキストを取得
        if (_effect_window_text == null ) {
            _effect_window_text = GameObject.Find( "EffectWindow" ).GetComponentInChildren< Text >( );
        }

        //書き換え前のテキストを取得
        _effect_return_text = _effect_window_text.text;
	}

	void Update(){
		//プレイヤーのマス調整が可能なら
		if ( _player_phase_manager.getTroutAdjustment ( ) ) {
			//自身を光らせる
		}
	}

	public void setMassData( string setData, int setID ){
		//マスのスプライトを取得,設定をします。テキストを設定をします　マスの場所（ID）を取得します
		switch ( setData )
		{
		case "start":
                _effect_Text = "開始マス";
                img_sprite = Resources.Load< Sprite > ( "Graphec/Sprite/masu/masu_yellow" );
            break;
		case "goal":
                _effect_Text = "ゴールマス";
                img_sprite = Resources.Load< Sprite > ( "Graphec/Sprite/masu/masu_yellow" );
            break;
		case "draw":
                _effect_Text = "ドローマス";
                img_sprite = Resources.Load< Sprite >( "Graphec/Sprite/masu/masu_blue" );
			break;
		case "advance":
                _effect_Text = "アドバンスマス";
			    img_sprite = Resources.Load< Sprite >( "Graphec/Sprite/masu/masu_blue" );
			break;
		case "trap1":
		case "trap2":
                _effect_Text = "トラップマス";
                img_sprite = Resources.Load< Sprite >( "Graphec/Sprite/masu/masu_red" );
			break;
		case "event":
                _effect_Text = "イベントマス";
                img_sprite = Resources.Load< Sprite > ( "Graphec/Sprite/masu/masu_green" );
			break;
		}
		img_source.sprite = img_sprite;
        _masu_id = setID;
	}

	public void effectWindowDrow( ){
        //エフェクトウィンドウの効果を入力
        _effect_window_text.text = _effect_Text;
	}
		
	public void effectWindowReturn( ){
        //吹き出しの削除
        _effect_window_text.text = _effect_return_text;
	}

	public void massClick( ){
		//プレイヤーフェイズのマス調整が可能なら
		if ( _player_phase_manager.getTroutAdjustment ( ) ) {
            //プレイヤーとマスの差を取得
            int player = _player_manager.getPlayerHere( );
            int playerwhile = player - _masu_id;
			//プレイヤーとマスの位置を見て-1から+1までか
			if ( playerwhile >= -1 && playerwhile <= 1 ) {
				_player_phase_manager.setClick ( _masu_id );
			}
		}
	}
		
}