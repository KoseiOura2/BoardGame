using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class PlayerPhaseManager : MonoBehaviour {

	public const string DICE_PHASE_MESSAGE  = "ダイスを振ります\n(今回はダイスのアニメーションはありません)";		//ダイスフェイズのメッセージ
	public const string FIELD_NAVI_MESSAGE  = "上画面に注目してください";									//フィールド画面に誘導するメッセージ
	public const string PLAYER_WAIT_MESSAGE = "対戦相手を待っています";										//対戦相手を待つ際のメッセージ

	private PlayerManager _player_manager;					//プレイヤーマネージャーを取得
	private PlayerNetWorkManager _player_netWork_manager;	//プレイヤーのネットワークマネージャーを取得

	private BATTLE_RESULT _isResult;		//プレイヤーの勝敗
	private GameObject _textWindow;			//テキストウィンドウを取得
	private GameObject _diceButton;			//ダイスボタンの取得
	private GameObject _blackOut;			//画面を暗くするオブジェクトを取得
	private Vector3 _setDiceButton_Position = new Vector3( 0, -120, 0 );		//ボタンの座標を設定
	private bool initial_setting = false;		//フェイズ毎の初期設定フラグ
	private int _diceData;						//ダイス数値の情報
	private int _player_Trout_Result;			//プレイヤーのマス調整の結果
	private float _nowTime;						//経過時間を取得
	private bool _trout_Adjustment = false;		//マス調整が可能か
	private bool _isComplate = false;
	private bool isButton;		//ボタンのどちらがクリックされたか

	public GameObject _blackOut_Prefab;		//画面を暗くするためのプレハブ
	public GameObject _textWindow_Prefab;	//テキストウィンドウプレハブ	
	public GameObject _diceButton_Prefab;	//ダイスボタンのプレハブ	
	public GameObject _canvas_Root;			//キャンバスを取得
	public float intervalTime = 3.0f;		//待ち時間を設定

	public void awake( ) {
		//各種マネージャーのロード
		if ( _player_netWork_manager == null ) {
			GameObject _player_NetWork_Manager_obj = GameObject.Find( "PlayerNetWorkManager" );
			_player_netWork_manager = _player_NetWork_Manager_obj.GetComponent< PlayerNetWorkManager >( );
		}
		if ( _player_manager == null ) {
			GameObject _Player_Manager_obj = GameObject.Find( "PlayerManager" );
			_player_manager = _Player_Manager_obj.GetComponent< PlayerManager >( );
		}

	}

	public void init( ) {
		//初期数値を0に
		_diceData = 0;
		/*
		//プレイヤーの現在の手札を生成
		_player_manager.AllHandCreate( );
		*/
	}

	/// <summary>
	/// Waits the phase.
	/// ここでプレイヤーの判別を行い、プレイヤーラベルの色とテキスト表示をエネミーの表示とテキストの表示を変更します
	/// </summary>
	void WaitPhase( ) {
		//プレイヤーの判別を行う
		if ( !initial_setting ) {
			// プレイヤー移動（今回は初期設定なので現在地に入れる）
			_player_manager.setPlayerPos( );
			//エネミーのテキストを設定
			_player_manager.setEnemyObject( );
			//プレイヤーによって変わる部分を変更
			_player_manager.setPlayerObject( );

			//初期設定完了フラグ
			initial_setting = true;

		}
	}

	/// <summary>
	/// dice the phase.
	/// ここでdiceボタンを押したらフラグを立てる
	/// </summary>
	void dicePhase (){
		//初期設定が済んでなければ行う
		if ( !initial_setting ) {

			Debug.Log ("ダイスフェイズです");
			//画面を暗くする
            _blackOut = canvasSet( _blackOut_Prefab, Vector3.zero );
            _blackOut.GetComponent< RectTransform >( ).sizeDelta = Vector2.zero;

			//テキストウィンドウを表示
            _textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
			textSet( _textWindow, DICE_PHASE_MESSAGE );

			//サイコロボタンを表示
            _diceButton = canvasSet( _diceButton_Prefab, _setDiceButton_Position );

			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//ダイスデータに1以上の数値が入ったなら実行
			if ( _diceData > 0 ) {
				//3秒ほど経過後
				_nowTime += Time.deltaTime;
				if ( _nowTime >= intervalTime ) {
					//待機メッセージ
					textSet ( _textWindow, PLAYER_WAIT_MESSAGE );
					//経過時間をリセット
					_nowTime = 0;
				}
			}
		}
	}

	/// <summary>
	/// inductionPhase the phase.
	/// 画面上に誘導を行います（いらないのであとで別のものに）
	/// </summary>
    void inductionPhase( ) {
		//初期設定が済んでなければ行う
		if ( !initial_setting ) {
			//フィールド誘導
            _textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
			textSet ( _textWindow, FIELD_NAVI_MESSAGE );
			//初期設定完了フラグ
			initial_setting = true;
		}
	}

	void movePhase( ){
		//プレイヤーの判別を行う
		if ( !initial_setting ) {
			Debug.Log ("フィールド移動画面です");

			//リザルトの結果によってマス調整を出来る、出来ないを判定
			switch ( _isResult ) {
			case BATTLE_RESULT.WIN:
				//ゴールしたかの通信が来る。
		
				//マス調整フラグがOFF
				_trout_Adjustment = true;
				break;
			case BATTLE_RESULT.DRAW:
			case BATTLE_RESULT.LOSE:
				//ゴールしたかの通信が来て負けているなら
				//簡易2Dマップの自分の位置を一マス後ろに移動
				_player_manager.setPlayerPos ( -1 );
				_trout_Adjustment = false;
				break;
			}

			//初期設定完了フラグ
			initial_setting = true;

		} else {

				//マス調整フラグがONなら
			if (_trout_Adjustment) {
				//マスがクリックされた
				if (_isComplate) {
					//テキストウィンドウで表示待機メッセージを表示
					_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
					textSet( _textWindow, PLAYER_WAIT_MESSAGE );
					//簡易2Dマップをクリックマスに移動させる
					_player_manager.setPlayerPos( _player_Trout_Result, true );
				}
			} else {
				//マス調整ができない
				//テキストウィンドウを表示待機メッセージを表示
				_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
				textSet ( _textWindow, PLAYER_WAIT_MESSAGE );
			}
		}
	}

	void filedPhase( ){
		//初期設定が済んでなければ行う
		if ( !initial_setting ) {
			//フィールド画面に誘導するテキストを表示
			_textWindow = canvasSet( _textWindow_Prefab, Vector3.zero );
			textSet ( _textWindow, FIELD_NAVI_MESSAGE );
			//初期設定完了フラグ
			initial_setting = true;
		}
	}

	void finishPhase( ) {
		//初期設定が済んでなければ行う
		if ( !initial_setting ){
			Debug.Log ( "ゲームの終了などを行うフェイズです" );
			//初期設定完了フラグ
			initial_setting = true;
		}
	}

	void phaseChange( bool isBlackOut = false ){
		//フェイズチェンジを行いますブラックアウトがチカチカして気になるのでTrueで表示続行　falseで非表示に
		//初期設定フラグをoffに
		initial_setting = false;

		//各種フラグをoffに
		_isComplate = false;
		_trout_Adjustment = false;
		_player_Trout_Result = 0;

		//生成したオブジェクトを非表示に

        Destroy( _textWindow );

        if ( isBlackOut ) {
            Destroy( _blackOut );
        }
	}

	void textSet( GameObject _textWindow, string _Message ){
		//テキストを指定したメッセージに変更します
		Text _windowText = _textWindow.GetComponentInChildren< Text >( );
		_windowText.text = _Message;
	}

	GameObject canvasSet( GameObject _setPrefab, Vector3 _setPosition ){
		//セットされたプレハブの生成、座標の修正、キャンパスの中に生成します
		GameObject _Setobj = ( GameObject )Instantiate( _setPrefab );
		_Setobj.transform.SetParent ( _canvas_Root.transform );
		_Setobj.GetComponent< RectTransform >( ).anchoredPosition3D = _setPosition;
		_Setobj.GetComponent< RectTransform >( ).localScale = Vector3.one;

		return _Setobj;
	}

	public void SetDiceData( int GetDiceData ){
		//ダイスボタンを消す
		_diceButton.SetActive ( false );
		//テキストを設定
		textSet ( _textWindow, GetDiceData + "が出ました！" );
		switch ( GetDiceData ){
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
	public bool getTroutAdjustment( ){
		return _trout_Adjustment;
	}

	public void SetClick( int massID ){

		//OKならその場所は今プレイヤーから見て前か後ろか同じか？
		//プレイヤーの現在地を取得
		int playerMass = _player_manager.getPlayerHere( );

		//現在地 - 移動先のマスで計算
		_player_Trout_Result = massID - playerMass;

		//クリックされました
		_isComplate = true;
	}
}
