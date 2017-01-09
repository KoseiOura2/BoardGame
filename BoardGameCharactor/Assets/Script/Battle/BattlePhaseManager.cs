using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Common;

public class BattlePhaseManager : MonoBehaviour {

	//フェイズ毎の初期設定フラグ
	bool initial_setting = false;

	//通信データ受信フラグ
	private bool netData_Reception = false;

	//通信データ送信フラグ
	private bool netData_Send = false;

	//現在のフェイズ
	private MAIN_GAME_PHASE _current_Phase;

	//キャンバスを取得
	public GameObject _canvas_Root;

	//テキストウィンドウ
	public GameObject _textWindow_Prefab;

	//ボタンプレハブ
	public GameObject _button_Prefab;

	//黒背景
	public GameObject _blackout_Prefab;

	//表示の待ち時間を設定
	public float intervalTime = 1.0f;

	public float BattleTime = 60.0f;

	public Text TimeText;

	//シーン読み込み中かどうか
	private bool nowSceneLoad = false;

	//フェードマネージャーを取得
	private FadeManager _fade_Manager;

	//プレイヤーのネットワークマネージャーを取得
	private PlayerNetWorkManager _player_NetWork_Manager;

	//プレイヤーマネージャーを取得
	private PlayerManager _player_Manager;

	//ボタンはい、いいえ取得
	private GameObject _yes_button;
	private GameObject _no_button;

	//黒背景を取得
	private GameObject _blackOut;

	//ボタンの座標を設定
	private Vector3 _setButton_Position = new Vector3 (120, -120, 0);

	//テキストウィンドウを取得
	private GameObject _textWindow;

	//ドローカードを使用したか
	private bool _drowCard = false;

	//はいかいいえのボタンを押したか
	private bool _selectConfirm = false;

	//生成が終了したかどうか
	private bool _generateComplate = false;

	//ドロー出来るカードを取得
	private CARD_DATA _DrowCard;

	//経過時間を取得
	private float nowTime;

	//カードセレクトが始まるかどうか
	private bool _card_Select_Start = false;

	//確定を押したかどうか
	private bool _Select_Push = false;

	private RESULT _Result;

	// Use this for initialization
	void Awake () {

		//各種マネージャーのロード
		if (_fade_Manager == null) {
			GameObject _fade_Manager_Obj = GameObject.Find ("FadeManager");
			_fade_Manager = _fade_Manager_Obj.GetComponent<FadeManager> ();
		}
		if ( _player_NetWork_Manager == null){
			GameObject _player_NetWork_Manager_Obj = GameObject.Find ("PlayerNetWorkManager");
			_player_NetWork_Manager = _player_NetWork_Manager_Obj.GetComponent<PlayerNetWorkManager> ();
		}
		if ( _player_Manager == null){
			GameObject _Player_Manager_Obj = GameObject.Find ("PlayerManager");
			_player_Manager = _Player_Manager_Obj.GetComponent<PlayerManager> ();
		}


		//最初のフェイズをロード
		_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_DROW;
	}
	
	// Update is called once per frame
	void Update () {
		if (!nowSceneLoad) {
			//フェイズ読み込み
			phaseLoad ();
		}
	}

	void phaseLoad (){
		//現在のシーンによって読み込みを変える
		switch (_current_Phase) {

		case MAIN_GAME_PHASE.GAME_PHASE_DROW:
			drowPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_DIS_CARD:
			disCardPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF:
			cardPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_RESULT_BATTLE:
			resultPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION:
			inductionPhase ();
			break;

		default:
			Debug.LogError ("errorCase:" + _current_Phase);
			break;
		}
	}

	void drowPhase (){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("ドローフェイズです");
			//エネミーのテキストを設定
			_player_Manager.SetEnemyObject();

			//プレイヤーによって変わる部分を変更
			_player_Manager.setPlayerObject();

			//プレイヤーの現在の手札を生成
			_player_Manager.AllHandCreate();

			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//受信フラグが立っていないのなら実行受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {

				//ドローが終わったか確認
				if (!_player_NetWork_Manager.getDrowCompletion( ) ) {
					//ドローカードタイプがあればドローカードを使うかどうかを選択させる
					if (_player_Manager.getHandSerach (CARD_TYPE.CARD_TYPE_NONE_TYPE)) {

						//ボタンやテキストウィンドウが生成されてないなら
						if (!_generateComplate) {
							_DrowCard = _player_Manager.getHandData (CARD_TYPE.CARD_TYPE_NONE_TYPE);

							//テキストウィンドウセット　YESかNOのボタンセット
							_yes_button = canvasSet (_button_Prefab, new Vector3 (-_setButton_Position.x, _setButton_Position.y, _setButton_Position.z));
							_no_button = canvasSet (_button_Prefab, new Vector3 (_setButton_Position.x, _setButton_Position.y, _setButton_Position.z));
							textSet (_yes_button, "YES");
							textSet (_no_button, "NO");

							//ドローカードを使用するかのテキストを表示
							_textWindow = canvasSet (_textWindow_Prefab, Vector3.zero);
							textSet (_textWindow, "ドローをしますか？");

							//生成フラグを立てる
							_generateComplate = true;
						}
					}
				}

				//ドローカードを使うか使わないか選びました
				if (_selectConfirm) {
					
					//オブジェクトを削除
					Destroy (_yes_button);
					Destroy (_no_button);
					Destroy (_textWindow);

					//選択の結果ドローカードを使ったかどうか
					if (_drowCard) {
						//使用したカードのデータを送信する
						_player_NetWork_Manager.networkDataAcross ( MAIN_GAME_PHASE.GAME_PHASE_DROW, 0, 0, _DrowCard );
					}
				}

				_player_Manager.DebugCardDrow();

				//次のフェイズの受信命令がないか確認
				netData_Reception = _player_NetWork_Manager.networkPhaseChangeReceipt ();

				//デバッグ用でPキーを押したら通信完了フラグ
				DebugReceipt ();
			} else {
				phaseChange ();
			}
		}
	}

	void cardPhase (){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("カード選択フェイズです");

			_textWindow = canvasSet (_textWindow_Prefab, Vector3.zero);
			textSet (_textWindow, "戦闘開始");

			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//データ送信をまだ行っていないなら
			if (!netData_Send) {
				//カードセレクトが始まる前に戦闘開始を表示するフラグ
				if (!_card_Select_Start) {
					//1秒後テキストを消す
					//通信データを投げる
					nowTime += Time.deltaTime;
					if (nowTime >= intervalTime) {
						//経過時間をバトルタイマーにセット
						nowTime = 0;
						_card_Select_Start = true;
						Destroy (_textWindow);
					}
				} else if (nowTime > BattleTime || _Select_Push) {
					//セレクトエリアにセットされたカードを処理
					_player_Manager.SetSelectAreaCard ();
					//テキストウィンドウを表示
					_textWindow = canvasSet (_textWindow_Prefab, Vector3.zero);
					textSet (_textWindow, "対戦相手を待っています");
					//データを送信
					netData_Send = _player_NetWork_Manager.networkDataAcross (MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF);

				} else {
					nowTime += Time.deltaTime;
					float CountDawn = BattleTime - nowTime;
					TimeText.text = "残り時間 " + CountDawn.ToString ("00");
				}
			}
			//受信フラグが立っていないのなら実行受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {
				
				//次のフェイズの受信命令がないか確認
				netData_Reception = _player_NetWork_Manager.networkPhaseChangeReceipt ();

				//デバッグ用でPキーを押したら通信完了フラグ
				DebugReceipt ();
			} else {
				phaseChange ();
			}
		}
	}

	void disCardPhase(){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("カードを捨てるフェイズです");
			//黒背景にする
			_blackOut = canvasSet (_blackout_Prefab, Vector3.zero);

			//確定ボタン

			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//受信フラグが立っていないのなら実行受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {

				//次のフェイズの受信命令がないか確認
				netData_Reception = _player_NetWork_Manager.networkPhaseChangeReceipt ();

				//デバッグ用でPキーを押したら通信完了フラグ
				DebugReceipt ();
			} else {
				phaseChange ();
			}
		}
	}


	void inductionPhase( ) {
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("上画面に誘導をするフェイズです");
			//フィールド画面に誘導するテキストを表示
			_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
			textSet ( _textWindow, "上画面に注目してください");
			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//受信フラグが立っていないのなら実行、受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {
				netData_Reception = _player_NetWork_Manager.networkPhaseChangeReceipt( );
				DebugReceipt ();
			} else {
				phaseChange ();
			}
		}
	}

	void resultPhase (){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("リザルトフェイズです");
			//プレイヤーからリザルトを取得
			_Result = _player_Manager.getBattleResult( );
			//キャンバスにテキストウィンドウを作成
			_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
			switch( _Result ){
			case RESULT.WINNER:
				textSet (_textWindow, "WINNER");
				break;

			case RESULT.DROW:
				textSet (_textWindow, "DROW");
				break;

			case RESULT.LOSE:
				textSet (_textWindow, "RESULT");
				break;
			}

			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//受信フラグが立っていないのなら実行、受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {
				netData_Reception = _player_NetWork_Manager.networkPhaseChangeReceipt( );
				DebugReceipt ();
			} else {
				phaseChange ();
			}
		}
	}

	void phaseChange( ){
		//フェイズチェンジを行いますブラックアウトがチカチカして気になるのでTrueで表示続行　falseで非表示に
		//初期設定フラグをoffに
		initial_setting = false;

		//受信フラグをoffに
		netData_Reception = false;

		Destroy( _textWindow );

		//次のフェイズに移行する。リザルトフェイズで次のフェイズに移行した場合プレイヤーシーンへ、その時のフラグでMassActionフラグを渡す
		switch (_current_Phase) {

		case MAIN_GAME_PHASE.GAME_PHASE_DROW:
			//手札数が6枚以上の場合disCardPhaseに
			if (_player_Manager.getHandListNumber () > 6) {
				_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_DIS_CARD;
			} else {
				_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF;
			}
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_DIS_CARD:
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF:
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION: 
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_RESULT_BATTLE;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_RESULT_BATTLE:
			//フラグをプレイヤーに渡す
			nowSceneLoad = true;
			//マスアクションフェイズに行くようにフラグをセットする
			_player_Manager.SetIsActionPhase( );
			_fade_Manager.FadeStart ("Player");
			break;
		}
	}

	GameObject canvasSet( GameObject _setPrefab, Vector3 _setPosition ){
		//セットされたプレハブの生成、座標の修正、キャンパスの中に生成します
		GameObject _Setobj = ( GameObject )Instantiate( _setPrefab );
		_Setobj.transform.SetParent (_canvas_Root.transform);
		_Setobj.GetComponent<RectTransform> ().anchoredPosition3D = _setPosition;
		_Setobj.GetComponent<RectTransform> ().localScale = Vector3.one;

		return _Setobj;
	}

	void textSet( GameObject _textWindow, string _Message ){
		//テキストを指定したメッセージに変更します
		Text _windowText = _textWindow.GetComponentInChildren<Text>();
		_windowText.text = _Message;
	}

	void DebugReceipt(){
		//仮でPキーを押したら通信完了をさせる
		if (Input.GetKeyDown (KeyCode.P)) {
			netData_Reception = true;
		}
	}

	public MAIN_GAME_PHASE GetMainGamePhase(){
		//現在のフェイズを返します
		return _current_Phase;
	}

	public bool getCardSelectStart(){
		//カードセレクトが始まったかどうかを取得します
		return _card_Select_Start;
	}

	public bool DrowCardUse( bool setUse ){
		//ドローカードを使うかどうかを取得します
		_drowCard = setUse;
		//ボタンを押したフラグをON
		_selectConfirm = true;
		return _selectConfirm;
	}
		
	public void Select_Push(){
		//カード選択フェイズで押されると反応をします
		if (_card_Select_Start) {
			_Select_Push = true;
		}
	}
}
