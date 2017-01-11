using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class massData : MonoBehaviour {

    //テキストの取得
    private Text _effect_Window_Text;

	//_Content_Rootを取得
	private RectTransform _Content;

	//Canvasを取得
	private GameObject _canvas_Root;

	//プレイヤーフェイズマネージャーを取得
	private PlayerPhaseManager _player_Phase_Manager;

	//プレイヤーマネージャーを取得
	private PlayerManager _player_Manager;

	//マスの場所を保存
	private int MassID;

	//自身のイメージ
	private Image img_Source;

	//書き換えスプライト
	private Sprite img_Sprite;

    [SerializeField]
	//書き換えテキスト
	private string _effect_Text;

    //書き換える前のテキスト
    private string _effect_Return_Text;

	void Awake () {
		img_Source = GetComponent<Image> ();

		//キャンバスを取得
		if (_canvas_Root == null) {
			_canvas_Root = GameObject.Find ("Canvas");
		}
		//Scrollのコンテンツを取得
		if (_Content == null) {
			GameObject _Content_Obj = GameObject.Find ("Content");
			_Content = _Content_Obj.GetComponent<RectTransform> ();
		}
		//プレイヤーフェイズのマネージャーを取得
		if (_player_Phase_Manager == null) {
			GameObject _player_Phase_Manager_Obj = GameObject.Find ("PlayerPhaseManager");
			_player_Phase_Manager = _player_Phase_Manager_Obj.GetComponent<PlayerPhaseManager> ();
		}

		//プレイヤーマネージャーを取得
		if ( _player_Manager == null){
			GameObject _Player_Manager_Obj = GameObject.Find ("PlayerManager");
			_player_Manager = _Player_Manager_Obj.GetComponent<PlayerManager> ();
		}

        //エフェクトウィンドウのテキストを取得
        if (_effect_Window_Text == null) {
            GameObject _effect_Window_Obj = GameObject.Find("EffectWindow");
            _effect_Window_Text = _effect_Window_Obj.GetComponentInChildren<Text>();
        }

        //書き換え前のテキストを取得
        _effect_Return_Text = _effect_Window_Text.text;
	}

	void Update(){
		//プレイヤーのマス調整が可能なら
		if (_player_Phase_Manager.getTroutAdjustment ()) {
			//自身を光らせる
		}
	}

	public void SetMassData( string SetData, int setID ){
		//マスのスプライトを取得,設定をします。テキストを設定をします　マスの場所（ID）を取得します
		switch (SetData)
		{
		case "start":
                _effect_Text = "開始マス";
                img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_yellow");
            break;
		case "goal":
                _effect_Text = "ゴールマス";
                img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_yellow");
            break;
		case "draw":
                _effect_Text = "ドローマス";
                img_Sprite = Resources.Load<Sprite>("Graphec/Sprite/masu/masu_blue");
			break;
		case "advance":
                _effect_Text = "アドバンスマス";
			    img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_blue");
			break;
		case "trap1":
		case "trap2":
                _effect_Text = "トラップマス";
                img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_red");
			break;
		case "event":
                _effect_Text = "イベントマス";
                img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_green");
			break;
		}
		img_Source.sprite = img_Sprite;
        MassID = setID;
	}

	public void EffectWindowDrow(){
        //エフェクトウィンドウの効果を入力
        _effect_Window_Text.text = _effect_Text;
	}
		
	public void EffectWindowReturn(){
        //吹き出しの削除
        _effect_Window_Text.text = _effect_Return_Text;
	}

	public void massClick(){
		//プレイヤーフェイズのマス調整が可能なら
		if (_player_Phase_Manager.getTroutAdjustment ()) {
            //プレイヤーとマスの差を取得
            int player = _player_Manager.getPlayerHere();
            int PlayerWhile = player - MassID;
			//プレイヤーとマスの位置を見て-1から+1までか
			if (PlayerWhile >= -1 && PlayerWhile <= 1) {
				_player_Phase_Manager.SetClick (MassID);
			}
		}
	}
		
}