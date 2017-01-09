using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class PlayerPhaseManager : MonoBehaviour {

	//画面を暗くするためのプレハブ
	public GameObject _blackOut_Prefab;

	//テキストウィンドウプレハブ
	public GameObject _textWindow_Prefab;

	//ダイスボタンのプレハブ
	public GameObject _diceButton_Prefab;

	//キャンバスを取得
	public GameObject _canvas_Root;

	//待ち時間を設定
	public float intervalTime = 3.0f;

	//フェードマネージャーを取得
	private FadeManager _fade_Manager;

	//プレイヤーのネットワークマネージャーを取得
	private PlayerNetWorkManager _player_NetWork_Manager;

	//プレイヤーマネージャーを取得
	private PlayerManager _player_Manager;

	//フェイズ毎の初期設定フラグ
	private bool initial_setting = false;

	//通信データ送信フラグ
	private bool netData_Send = false;

	//通信データ受信フラグ
	private bool netData_Reception = false;

	[SerializeField]
	//現在のフェイズ
	private MAIN_GAME_PHASE _current_Phase;

	//ボタンの座標を設定
	private Vector3 _setDiceButton_Position = new Vector3 (0, -120, 0);

	//ダイスフェイズのメッセージ
	private string DicePhaseMessage = "ダイスを振ります\n(今回はダイスのアニメーションはありません)";

	//フィールド画面に誘導するメッセージ
	private string FieldNaviMessage = "上画面に注目してください";

	//対戦相手を待つ際のメッセージ
	private string PlayerWaitMessage = "対戦相手を待っています";

	//ダイス数値の情報
	private int _diceData;

	//テキストウィンドウを取得
	private GameObject _textWindow;

	//ダイスボタンの取得
	private GameObject _diceButton;

	//画面を暗くするオブジェクトを取得
	private GameObject _blackOut;

	//経過時間を取得
	private float _nowTime;

	//画面推移の処理をしているかどうか
	private bool _nowScene_Load;

	//プレイヤーの勝敗
	private RESULT _isResult;

	//マス調整が可能か
	private bool _trout_Adjustment = false;

	//プレイヤーのマス調整の結果
	private int _player_Trout_Result;

	private bool _isComplate = false;

	//ボタンのどちらがクリックされたか
	private bool isButton;

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

		//ここでプレイヤーマネージャーを見てマスアクションフラグが立っているなら最初のフェイズロードをマスギミックフェイズに
		if (_player_Manager.getIsActionPhase ()) {
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER;
		} else {
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY;
		}

		//初期数値を0に
		_diceData = 0;

		//画面推移中かどうか
		_nowScene_Load = false;

	}

	// Update is called once per frame
	void Update () {
		if (!_nowScene_Load) {
			//フェイズ読み込み
			phaseLoad ();
		}
	}

	void phaseLoad (){
		//現在のシーンによって読み込みを変える
		switch (_current_Phase) {

		case MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY:
			WaitPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE:
			dicePhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION:
            inductionPhase( );
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER:
			movePhase();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_GIMMICK:
			filedPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FINISH:
			finishPhase ();
			break;

		default:
			Debug.LogError ("errorCase:" + _current_Phase);
			break;
		}
	}

	void WaitPhase(){
		//プレイヤーの判別を行う
		if (!initial_setting) {
			Debug.Log ("プレイヤー画面です");
			//ここでプレイヤーの判別を行い、プレイヤーラベルの色とテキスト表示をエネミーの表示とテキストの表示を変更します

			//プレイヤー移動（今回は初期設定なので現在地に入れる）
			_player_Manager.SetPlayerMove ();

			//エネミーのテキストを設定
			_player_Manager.SetEnemyObject();

			//プレイヤーによって変わる部分を変更
			_player_Manager.setPlayerObject();

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
				phaseChange (false);
			}
		}
	}

	void dicePhase (){
		//初期設定が済んでなければ行う
		if (!initial_setting) {

			Debug.Log ("ダイスフェイズです");
			//画面を暗くする
            _blackOut = canvasSet( _blackOut_Prefab, Vector3.zero );
            _blackOut.GetComponent<RectTransform>( ).sizeDelta = Vector2.zero;

			//テキストウィンドウを表示
            _textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
            textSet( _textWindow, DicePhaseMessage );

			//サイコロボタンを表示
            _diceButton = canvasSet( _diceButton_Prefab, _setDiceButton_Position );

			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//ダイスデータに1以上の数値が入り通信データを送信していないのなら通信データを送信する
			if (_diceData > 0 && !netData_Send) {
				//3秒ほど経過後通信データを投げる
				_nowTime += Time.deltaTime;
				if (_nowTime >= intervalTime) {
					//経過時間をリセット
					_nowTime = 0;
					netData_Send = _player_NetWork_Manager.networkDataAcross ( MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE,_diceData);
				} 
			} else if( netData_Send ){
				textSet (_textWindow, PlayerWaitMessage );
			}
			//受信フラグが立っていないのなら実行受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {
                netData_Reception = _player_NetWork_Manager.networkPhaseChangeReceipt( );
				DebugReceipt ();
			} else {
				phaseChange ( );
			}
		}
	}

    void inductionPhase( ) {
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("上画面に誘導をするフェイズです");
			//フィールド画面に誘導するテキストを表示
            _textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
			textSet ( _textWindow, FieldNaviMessage );
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

	void movePhase(){
		//プレイヤーの判別を行う
		if (!initial_setting) {
			Debug.Log ("フィールド移動画面です");

			//プレイヤーによって変わる部分を変更
			_player_Manager.setPlayerObject();

			//プレイヤーの現在の手札を生成
			_player_Manager.AllHandCreate();

			//勝敗を取得
			_isResult = _player_Manager.getBattleResult ();

			//プレイヤー移動（初期設定なので現在地に入れる）
			_player_Manager.SetPlayerMove ();

			//エネミーのテキストを設定
			_player_Manager.SetEnemyObject();

			//初期設定完了フラグ
			initial_setting = true;

		} else {
			if (!netData_Send) {
				//リザルトの結果によってマス調整を出来る、出来ないを判定
				switch (_isResult) {
				case RESULT.WINNER:
					Debug.Log (_player_Manager.getPlayerHere ());
					//プレイヤーがこの時点でゴールにいれば
					if (_player_Manager.getPlayerHere () == _player_Manager.getGoalPoint ()) {
						netData_Send = _player_NetWork_Manager.networkDataAcross ( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER, 0, true, _player_Trout_Result );
					}
					_trout_Adjustment = true;
					break;
				case RESULT.DROW:
				case RESULT.LOSE:
					//ゴールした地点で負けたら-1マス
					if (_player_Manager.getPlayerHere () == _player_Manager.getGoalPoint ()) {
						_player_Manager.SetPlayerMove ( -1 );
					}
					_trout_Adjustment = false;
					break;
				}
				//マス調整フラグが立っているなら
				if (_trout_Adjustment) {
					//マスがクリックされたらそのマスの場所が前かその場かによって-1～1までを入れる
					if(_isComplate){
						netData_Send = _player_NetWork_Manager.networkDataAcross ( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER, 0, false, _player_Trout_Result );
						//テキストウィンドウを表示
						_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
						textSet( _textWindow, PlayerWaitMessage );
						//裏でここで既に移動を完了させます
						_player_Manager.SetPlayerMove (_player_Trout_Result, true);
					}
				} else {
					//どこに移動したかを送信します（マス調整ができないので0を入れます）
					_player_Trout_Result = 0;
					netData_Send = _player_NetWork_Manager.networkDataAcross ( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER, 0, false, _player_Trout_Result );
					//テキストウィンドウを表示
					_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
					textSet( _textWindow, PlayerWaitMessage );
				}
			}
			//受信フラグが立っていないのなら実行受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {
				//次のフェイズの受信命令がないか確認
				netData_Reception = _player_NetWork_Manager.networkPhaseChangeReceipt ();

				//デバッグ用でPキーを押したら通信完了フラグ
				DebugReceipt ();
			} else {
				phaseChange (false);
			}
		}
	}

	void filedPhase (){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("マス効果などを反映させるフェイズです");
			//フィールド画面に誘導するテキストを表示
			_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
			textSet ( _textWindow, FieldNaviMessage );
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

	void finishPhase( ) {
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("ゲームの終了などを行うフェイズです");
			_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
			textSet ( _textWindow, "ゲームを終了します" );
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

	void phaseChange( bool isBlackOut = false){
		//フェイズチェンジを行いますブラックアウトがチカチカして気になるのでTrueで表示続行　falseで非表示に
		//初期設定フラグをoffに
		initial_setting = false;

		//各種フラグをoffに
		netData_Send = false;
		netData_Reception = false;
		_isComplate = false;
		_trout_Adjustment = false;

		_player_Trout_Result = 0;

		//生成したオブジェクトを非表示に
        Destroy( _diceButton );

        Destroy( _textWindow );

        if ( isBlackOut ) {
            Destroy( _blackOut );
        }

		//次のフェイズに移行する。プレイヤーの移動フェイズで次のフェイズに移行した場合バトルシーンへ
		switch (_current_Phase) {

		case MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY:
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE:
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION: 
			//裏で移動を完了させておく
			_player_Manager.SetPlayerMove (_diceData, true);
			_nowScene_Load = true;
			_fade_Manager.FadeStart ("Battle");
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER:
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_FIELD_GIMMICK;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_GIMMICK:
			//プレイヤーがこの時点でゴールにいれば
			if (_player_Manager.getPlayerHere () == _player_Manager.getGoalPoint ()) {
				//フィニッシュシーンです
				_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_FINISH;
			} else {
				_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE;
			}
			break;
		}
	}

	void DebugReceipt(){
		//仮でPキーを押したら通信完了をさせる
		if (Input.GetKeyDown (KeyCode.P)) {
			netData_Reception = true;
		}
	}


	void textSet( GameObject _textWindow, string _Message ){
		//テキストを指定したメッセージに変更します
		Text _windowText = _textWindow.GetComponentInChildren<Text>();
		_windowText.text = _Message;
	}

	GameObject canvasSet( GameObject _setPrefab, Vector3 _setPosition ){
		//セットされたプレハブの生成、座標の修正、キャンパスの中に生成します
		GameObject _Setobj = ( GameObject )Instantiate( _setPrefab );
		_Setobj.transform.SetParent (_canvas_Root.transform);
		_Setobj.GetComponent<RectTransform> ().anchoredPosition3D = _setPosition;
		_Setobj.GetComponent<RectTransform> ().localScale = Vector3.one;

		return _Setobj;
	}

	public void SetDiceData( int GetDiceData ){
		//ダイスボタンを消す
		_diceButton.SetActive (false);
		//テキストを設定
		textSet (_textWindow, GetDiceData + "が出ました！" );
		switch (GetDiceData){
		case 1:
		case 2:
			_diceData = 1;
			break;
		case 3:
		case 4:
			_diceData = 2;
			break;
		case 5:
		case 6:
			_diceData = 3;
			break;
		default:
			break;
		}
	}

	//マス調整が可能か取得
	public bool getTroutAdjustment (){
		return _trout_Adjustment;
	}

	public void SetClick( int massID ){

		//OKならその場所は今プレイヤーから見て前か後ろか同じか？
		//プレイヤーの現在地を取得
		int playerMass = _player_Manager.getPlayerHere();
		//現在地 - 移動先のマスで計算
		_player_Trout_Result = massID - playerMass;
		//クリックされました
		_isComplate = true;
	}
}
