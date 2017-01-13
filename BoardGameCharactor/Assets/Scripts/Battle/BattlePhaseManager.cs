using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Common;

public class BattlePhaseManager : MonoBehaviour {

	public const string DICE_PHASE_MESSAGE  = "ダイスを振ります\n(今回はダイスのアニメーションはありません)";		//ダイスフェイズのメッセージ
	public const string FIELD_NAVI_MESSAGE  = "上画面に注目してください";									//フィールド画面に誘導するメッセージ
	public const string PLAYER_WAIT_MESSAGE = "対戦相手を待っています";										//対戦相手を待つ際のメッセージ

	private FadeManager _fade_anager;						//フェードマネージャーを取得
	private PlayerNetWorkManager _player_netWork_manager;	//プレイヤーのネットワークマネージャーを取得
	private PlayerManager _player_manager;					//プレイヤーマネージャーを取得

	[SerializeField]
	private int _hand_Max = 6;					//手札の限界を設定	
	private bool _initial_setting = false;		//フェイズ毎の初期設定フラグ
	private bool _drowCard_Use = false;			//ドローカードを使用したか
	private bool _select_Confirm = false;		//ボタンを押したか
	private bool _generate_Complate = false;	//生成が終了したかどうか
	private bool _card_Select_Start = false;	//カードセレクトが始まるかどうか
	private bool _Select_Push = false;			//確定を押したかどうか
	private float _battleTime = 60.0f;			//バフ選択時間
	private float _intervalTime = 1.0f;			//テキスト表示の時間
	private float _nowTime;						//経過時間を取得

	private Vector3 _setButton_Position = new Vector3 (120, -120, 0);	//ボタンの座標を設定
	private Vector3 discard_TextPosition = new Vector3 (0, 201, 0);	//ディスカードフェイズでのテキストウィンドウの座標
	private Vector3 discard_SelecAreaPosition = new Vector3 ( -125, -16, 0 );	//ディスカードフェイズでのエリアの座標
	private Vector3 discard_SelectButtonPosition = new Vector3 ( -140, -7, 0);	//ディスカードフェイズでの選択ボタンの座標

	private List < OBJECT_DATA > _battlePhaseObjects;	//バトルフェイズオブジェクトをまとめる
	public GameObject _canvas_Root;					//キャンバスを取得

	//オブジェクト管理用の構造体
	struct OBJECT_DATA {
		public GameObject obj;
		public int id;
	};

	/// <summary>
	/// オブジェクトのリスト
	/// </summary>
	private enum OBJECT_LIST{
		TEXT_WINDOW,
		YES_BUTTON,
		NO_BUTTON,


	}


	// Use this for initialization
	void Awake( ) {

		if ( _player_manager == null ){
			GameObject _Player_Manager_Obj = GameObject.Find( "PlayerManager" );
			_player_manager = _Player_Manager_Obj.GetComponent< PlayerManager >( );
		}

		//リストの初期化
		_battlePhaseObjects = new List<  OBJECT_DATA >( );
	}

	void drowPhase( CARD_DATA drowCard ){
		//初期設定が済んでなければ行う
		if( !_initial_setting ) {
			Debug.Log( "ドローフェイズです" );
			//エネミーのテキストを設定
			_player_manager.setEnemyObject( );

			//プレイヤーによって変わる部分を変更
			_player_manager.setPlayerObject( );

			//初期設定完了フラグ
			_initial_setting = true;
		} else {
			//ドローカードタイプがあればドローカードを使うかどうかを選択させる
			if( !_generate_Complate ) {

				//ボタンオブジェクトをリストに追加
				OBJECT_DATA _yes_Button = objectLoad( ( GameObject )Resources.Load( "Prefab/Button" ), 
				                                    new Vector3( -_setButton_Position.x, _setButton_Position.y, _setButton_Position.z ) );

				OBJECT_DATA _no_Button = objectLoad( ( GameObject )Resources.Load( "Prefab/Button" ),
				                                   new Vector3( -_setButton_Position.x, _setButton_Position.y, _setButton_Position.z ) );

				//テキストを設定
				textSet( _yes_Button.id, "YES" );
				textSet( _no_Button.id, "NO" );

				//ドローカードを使用するかのテキストを表示
				OBJECT_DATA _textWindow = objectLoad( ( GameObject )Resources.Load( "Prefab/TextWindow" ), Vector3.zero );
				textSet( _textWindow.obj, "ドローをしますか？" );

				//生成フラグを立てる
				_generate_Complate = true;
			}

			//buttonをクリックしました
			if( _select_Confirm ) {
				//オブジェクトを削除
				for( int i = 0; i < _battlePhaseObjects.Count; i++ ) {
					Destroy( _battlePhaseObjects[ i ] );
					_battlePhaseObjects.RemoveAt( i );				
				}

				//選択の結果ドローカードを使ったかどうか
				if( _drowCard_Use ) {
					
				}
			}
		}
	}

	void cardPhase( )
	{
		//初期設定が済んでなければ行う
		if ( !_initial_setting ) {
			Debug.Log( "カード選択フェイズです" );

			OBJECT_DATA _textWindow = objectLoad( ( GameObject )Resources.Load( "Prefab/TextWindow" ), Vector3.zero );
			textSet( _textWindow.obj, "戦闘開始" );

			//初期設定完了フラグ
			_initial_setting = true;
		} else {
			//カードセレクトが始まる前に戦闘開始を表示する
			if ( !_card_Select_Start ) {
				//1秒後テキストを消す
				_nowTime += Time.deltaTime;
				if ( _nowTime >= _intervalTime ) {
					//経過時間をバトルタイマーにセット
					_nowTime = 0;
					//カードセレクトを開始
					_card_Select_Start = true;
					//オブジェクトを削除
					for( int i = 0; i < _battlePhaseObjects.Count; i++ ) {
						Destroy( _battlePhaseObjects[ i ] );
						_battlePhaseObjects.RemoveAt( i );				
					}
				}
			} else if ( _nowTime > _battleTime || _Select_Push ) {
				//セレクトエリアにセットされたカードを処理
				_player_manager.SetSelectAreaCard( );
				//テキストウィンドウを表示
				canvasSet( ( GameObject )Resources.Load( "Prefab/TextWindow" ), Vector3.zero );
				textSet( _textWindow, PLAYER_WAIT_MESSAGE );
				//確定フラグ

			} else {
				nowTime += Time.deltaTime;
				float CountDawn = BattleTime - nowTime;
				//タイムテキストを取得
				TimeText.text = "残り時間 " + CountDawn.ToString( "00" );
			}
		}
	}

	void disCardPhase( ){
		//初期設定が済んでなければ行う
		if ( !initial_setting ) {
			Debug.Log ( "カードを捨てるフェイズです" );
			//黒背景にする
			_blackOut = canvasSet ( _blackout_Prefab, Vector3.zero );
			_blackOut.GetComponent< RectTransform >( ).sizeDelta = Vector2.zero;
			//確定ボタン
			DisCard_Object.Add ( canvasSet ( _discardSelectButton_Prefab, discard_SelectButtonPosition ) );

			//セレクトエリア
			DisCard_Object.Add( canvasSet ( _discardArea_Prefab, discard_SelecAreaPosition ) );
			//セレクトカードを手札のカード数-6にして生成
			int SelectAreaNumber = _player_Manager.getHandListNumber( ) - 6;

			for( int i =0; i <  SelectAreaNumber; i++ ){
				//セレクトカードはサイズ/さっきの数字の場所で作る
				//DisCard_Object[1][selectAreaPosition = DisCard_Object[1].GetComponent< RectTransform >().sizeDelta / SelectAreaNumber * i ;
			}

			//テキストウィンドウ
			_textWindow = canvasSet ( ( GameObject )Resources.Load( "Prefab/TextWindow" ), discard_TextPosition );
			textSet (_textWindow, "捨てるカードを選んでください");

			//初期設定完了フラグ
			initial_setting = true;
		} else {
			//確定ボタンが押されたら
			if (_discard_Ok) {
				//セレクトリストにおいたものを削除
			}
		}
	}


	void inductionPhase( ) {
		//初期設定が済んでなければ行う
		if ( !initial_setting ) {
			Debug.Log ( "上画面に誘導をするフェイズです" );
			//フィールド画面に誘導するテキストを表示
			_textWindow = canvasSet( ( GameObject )Resources.Load( "Prefab/TextWindow" ), Vector3.zero );
			textSet ( _textWindow, FIELD_NAVI_MESSAGE );
			//初期設定完了フラグ
			initial_setting = true;
		} 
	}

	void resultPhase ( BATTLE_RESULT _Result ){
		//初期設定が済んでなければ行う
		if ( !initial_setting ) {
			Debug.Log ("リザルトフェイズです");

			//キャンバスにテキストウィンドウを作成
			_textWindow = canvasSet ( ( GameObject )Resources.Load( "Prefab/TextWindow" ), Vector3.zero );
			switch ( _Result ) {
			case BATTLE_RESULT.WIN:
				textSet (_textWindow, BATTLE_RESULT.WIN.ToString ( ) );
				break;

			case BATTLE_RESULT.DRAW:
				textSet (_textWindow, BATTLE_RESULT.DRAW.ToString ( ) );
				break;

			case BATTLE_RESULT.LOSE:
				textSet (_textWindow, BATTLE_RESULT.DRAW.ToString ( ) );
				break;
			}

			//初期設定完了フラグ
			_initial_setting = true;
		} else {
			_nowTime += Time.deltaTime;
			if (_nowTime >= _intervalTime ) {
				Destroy ( _textWindow );
				//経過時間をバトルタイマーにセット
				_nowTime = 0;
			}
		}

	}

	void phaseChange( ){
		//フェイズチェンジを行いますブラックアウトがチカチカして気になるのでTrueで表示続行　falseで非表示に
		//初期設定フラグをoffに
		_initial_setting = false;
	}

	canvasSet( GameObject _setPrefab, Vector3 _setPosition ){
		//セットされたプレハブの生成、座標の修正、キャンパスの中に生成します
		GameObject _Setobj = ( GameObject )Instantiate( _setPrefab );
		_Setobj.transform.SetParent ( _canvas_Root.transform );
		_Setobj.GetComponent< RectTransform >( ).anchoredPosition3D = _setPosition;
		_Setobj.GetComponent< RectTransform >( ).localScale = Vector3.one;

		return _Setobj;
	}

	OBJECT_ID objectLoad( GameObject _loadObject, Vector3 _setPos ){
		//オブジェクト構造体
		OBJECT_DATA obj;

		//オブジェクトとIDを設定
		obj.obj = _loadObject;
		obj.id = _battlePhaseObjects.Count;

		//リストに追加
		_battlePhaseObjects.Add (obj);

		return obj.id;
		
	}

	void textSet( int _object_Id, string _Message ){
		//テキストを指定したメッセージに変更します
		Text _Text = _battlePhaseObjects[_object_Id].obj.GetComponentInChildren< Text >( );
		_Text.text = _Message;
	}

	public bool getCardSelectStart( ){
		//カードセレクトが始まったかどうかを取得します
		return _card_Select_Start;
	}

	public bool drowCardUse( bool setUse ){
		//ドローカードを使うかどうかを取得します
		_drowCard_Use = setUse;
		//ボタンを押したフラグをON
		_select_Confirm = true;
		return _select_Confirm;
	}
		
	public void select_Push( ){
		//カード選択フェイズで押されると反応をします
		if ( _card_Select_Start ) {
			_select_Push = true;
		}
	}

}
