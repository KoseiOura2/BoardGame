﻿using UnityEngine;
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
	public FadeManager _fade_Manager;

	//プレイヤーのネットワークマネージャーを取得
	public PlayerNetWorkManager _player_NetWork_Manager;

	//プレイヤーマネージャーを取得
	public PlayerManager _Player_Manager;

	//フェイズ毎の初期設定フラグ
	private bool initial_setting = false;

	//通信データ送信フラグ
	private bool netData_Send = false;

	//通信データ受信フラグ
	private bool netData_Reception = false;

	//現在のフェイズ
	private MAIN_GAME_PHASE _current_Phase;
	[SerializeField]

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
	private float nowTime;

	//画面推移の処理をしているかどうか
	private bool nowSceneLoad;

	void Awake () {

		//最初のフェイズをロード
		_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY;

		//初期数値を0に
		_diceData = 0;

		//画面推移中かどうか
		nowSceneLoad = false;

		//画面を暗くするオブジェクトをセット
		_blackOut = canvasSet(_blackOut_Prefab, Vector3.zero); 
		_blackOut.GetComponent<RectTransform> ().sizeDelta = Vector2.zero;
		_blackOut.SetActive (false);
		//テキストウィンドウをセット
		_textWindow = canvasSet(_textWindow_Prefab, Vector3.zero); 
		textSet (_textWindow, DicePhaseMessage );
		_textWindow.SetActive (false);
		//サイコロボタンをセット
		_diceButton = canvasSet(_diceButton_Prefab, _setDiceButton_Position );
		_diceButton.SetActive (false);

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

		case MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY:
			WaitPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE:
			dicePhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION:
			movePhase ();
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
			//初期手札を生成
			_Player_Manager.setHandCardCreate();
			//エネミーのテキストを設定
			_Player_Manager.SetEnemyObject();
			//プレイヤーによって変わる部分を変更
			_Player_Manager.setPlayerObject();
			//初期設定完了フラグ
			initial_setting = true;

		} else {
			//通信データを送信していないのなら通信データを送信する
			if (!netData_Send) {
				netData_Send = _player_NetWork_Manager.netDataAcross (MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY );
			}
			//受信フラグが立っていないのなら実行受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {
				netData_Reception = _player_NetWork_Manager.netDataReceipt ();
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
			_blackOut.SetActive(true);
			//テキストウィンドウを表示
			_textWindow.SetActive(true); 
			textSet (_textWindow, DicePhaseMessage );
			//サイコロボタンを表示
			_diceButton.SetActive(true);
			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//ダイスデータに1以上の数値が入り通信データを送信していないのなら通信データを送信する
			if (_diceData > 0 && !netData_Send) {
				//3秒ほど経過後通信データを投げる
				nowTime += Time.deltaTime;
				if (nowTime >= intervalTime) {
					//経過時間をリセット
					nowTime = 0;
					netData_Send = _player_NetWork_Manager.netDataAcross (MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE);
				} 
			} else if( netData_Send ){
				textSet (_textWindow, PlayerWaitMessage );
			}
			//受信フラグが立っていないのなら実行受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {
				netData_Reception = _player_NetWork_Manager.netDataReceipt ();
				DebugReceipt ();
			} else {
				phaseChange ( true );
			}
		}
	}

	void movePhase(){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("上画面に誘導をするフェイズです");
			//黒背景
			_blackOut.SetActive(true);
			//フィールド画面に誘導するテキストを表示
			_textWindow.SetActive(true); 
			textSet ( _textWindow, FieldNaviMessage );
			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//受信フラグが立っていないのなら実行、受信フラグが立ったならフェイズをチェンジ
			if (!netData_Reception) {
				netData_Reception = _player_NetWork_Manager.netDataReceipt ();
				DebugReceipt ();
			} else {
				phaseChange (true);
			}
		}
	}

	void phaseChange( bool isBlackOut ){
		//初期設定フラグをoffに
		initial_setting = false;

		//受信、送信フラグをoffに
		netData_Send = false;
		netData_Reception = false;

		//生成したオブジェクトを非表示に
		if ( _diceButton.activeSelf ) {
			_diceButton.SetActive (false);
		}

		if (_textWindow.activeSelf) {
			_textWindow.SetActive (false);
		}

		if (!isBlackOut) {
			if (_blackOut.activeSelf) {
				_blackOut.SetActive (false);
			}
		}

		//次のフェイズに移行する。プレイヤーの移動フェイズで次のフェイズに移行した場合バトルシーンへ
		switch (_current_Phase) {

		case MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY:
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE;
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE:
			_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER;
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION: 
			nowSceneLoad = true;
			_fade_Manager.FadeStart ("Battle");
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
}
