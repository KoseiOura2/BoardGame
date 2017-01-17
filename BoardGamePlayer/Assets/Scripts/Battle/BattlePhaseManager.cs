using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Common;
using PlayerData;

public class BattlePhaseManager : MonoBehaviour {

	public const string DICE_PHASE_MESSAGE  = "ダイスを振ります\n(今回はダイスのアニメーションはありません)";		//ダイスフェイズのメッセージ
	public const string FIELD_NAVI_MESSAGE  = "上画面に注目してください";									        //フィールド画面に誘導するメッセージ
	public const string PLAYER_WAIT_MESSAGE = "対戦相手を待っています";										        //対戦相手を待つ際のメッセージ

	public GameObject _canvas_Root;				    //キャンバスを取得

	private PlayerManager _player_manager;          //プレイヤーマネージャーを取得
    private Text _battle_timer_text;                //制限時間のテキストを取得
    [SerializeField]
	private int _hand_max = 6;					    //手札の限界を設定	
    private bool _draw_end = false;                 //ドローが終了したか
	private bool _initial_setting = false;		    //フェイズ毎の初期設定フラグ
	private bool _draw_card_use = false;		    //ドローカードを使用したか
	private bool _select_confirm = false;		    //ボタンを押したか
	private bool _generate_complate = false;	    //生成が終了したかどうか
	private bool _card_select_start = false;	    //カードセレクトが始まるかどうか
	private bool _select_push = false;              //確定を押したかどうか
    private bool _wait_phase = false;               //フェイズチェンジの待機フラグ
    private float _battle_time = 60.0f;			    //バフ選択時間
	private float _interval_time = 1.0f;		    //テキスト表示の時間
	private float _now_time;                        //経過時間を取得

    private Vector3 _set_button_position            = new Vector3 ( 120, -120, 0 );         //ボタンの座標を設定
    private Vector3 discard_text_position           = new Vector3 ( 0, 201, 0 );            //ディスカードフェイズでのテキストウィンドウの座標
    private Vector3 discard_selec_area_position     = new Vector3 ( -125, -16, 0 );         //ディスカードフェイズでのエリアの座標
    private Vector3 discard_select_button_position  = new Vector3 ( -140, -7, 0 );          //ディスカードフェイズでの選択ボタンの座標
    private Vector3 _set_select_area_position       = new Vector3 ( 133, 40, 0 );           //セレクトエリアの座標

    private List < OBJECT_DATA > _battle_phase_objects;	//バトルフェイズオブジェクトをまとめる


	// Use this for initialization
	void Awake( ) {

        //プレイヤーマネージャーを取得
        if ( _player_manager == null ) {
            _player_manager = GameObject.Find ( "PlayerManager" ).GetComponent< PlayerManager > ( );
        }

        //リストの初期化
        _battle_phase_objects = new List< OBJECT_DATA > ( );

		//各種オブジェクトのロード
		objectLoad( PLAYER_OBJECT_LIST.YES_BUTTON, ( GameObject )Resources.Load ( "Prefab/Button" ) );
		objectLoad( PLAYER_OBJECT_LIST.NO_BUTTON, ( GameObject )Resources.Load ( "Prefab/Button" ) );
		objectLoad( PLAYER_OBJECT_LIST.TEXT_WINDOW, ( GameObject )Resources.Load ( "Prefab/TextWindow" ) );
		objectLoad( PLAYER_OBJECT_LIST.BLACKOUT_PANEL, ( GameObject )Resources.Load ( "Prefab/blackBackGround" ) );
		objectLoad( PLAYER_OBJECT_LIST.DISCARD_AREA, ( GameObject )Resources.Load ( "Prefab/DisCardArea" ) );
		objectLoad( PLAYER_OBJECT_LIST.DISCARD_BUTTON, ( GameObject )Resources.Load ( "Prefab/disCardSelectButton" ) );
        objectLoad( PLAYER_OBJECT_LIST.BATTLE_SELECT_AREA, ( GameObject )Resources.Load ( "Prefab/BattlePhaseSelectArea" ) );
    }

	public void drawPhase( ) {
        if ( !_wait_phase ) {
            //初期設定が済んでなければ行う
            if ( !_initial_setting ) {
                objectDraw( PLAYER_OBJECT_LIST.BATTLE_SELECT_AREA, _set_select_area_position );

                //共通の初期設定
                phaseInit(  );

                //エネミーのテキストを設定
                _player_manager.setEnemyObject( );

                //プレイヤーによって変わる部分を変更
                _player_manager.setPlayerObject( );

                //初期設定完了フラグ
                _initial_setting = true;
            } else {
                //ドローが終了しているか
                if ( _draw_end ) {
                    //ドローカードタイプがあればドローカードを使うかどうかを選択させる
                    if ( !_generate_complate ) {

                        //ボタンオブジェクトを描画
                        objectDraw( PLAYER_OBJECT_LIST.YES_BUTTON, _set_button_position );
                        objectDraw( PLAYER_OBJECT_LIST.NO_BUTTON, new Vector3( -_set_button_position.x, _set_button_position.y, _set_button_position.z ) );

                        //テキストウィンドウを描画
                        objectDraw ( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );

                        //テキストを設定
                        textSet( PLAYER_OBJECT_LIST.YES_BUTTON, "YES" );
                        textSet( PLAYER_OBJECT_LIST.NO_BUTTON, "NO" );
                        textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, "ドローをしますか？" );

                        //生成フラグを立てる
                        _generate_complate = true;
                    } else {
                        //ボタンフラグをクリア
                        _select_confirm = true;
                    }

                    //buttonをクリックしました
                    if ( _select_confirm ) {

                        //オブジェクトを削除
                        objectDelete( );

                        //選択の結果ドローカードを使ったかどうか
                        if ( _draw_card_use ) {

                        } else {
                            //テキストウィンドウを描画
                            objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );

                            //テキストを設定
                            textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, PLAYER_WAIT_MESSAGE );

                            //初期設定フラグをOFF
                            _initial_setting = false;

                            //フェイズを待機状態に
                            _wait_phase = true;
                        }
                    }
                }
            }
            //初期設定完了フラグ
            _initial_setting = false;
            //フェイズを待機状態に
            _wait_phase = true;
        }
	}

	public void cardPhase( ) {
        if ( !_wait_phase ) {
            //初期設定が済んでなければ行う
            if ( !_initial_setting ) {
                //共通の初期設定
                phaseInit( );

                //テキストウィンドウを描画
                objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
                //テキストを設定
                textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, "戦闘開始" );
                //カード設定
                _battle_timer_text = GameObject.Find( "Timer" ).GetComponentInChildren< Text >( );
                Debug.Log( _battle_timer_text );
                //初期設定完了フラグ
                _initial_setting = true;

            } else {
                //カードセレクトが始まる前に戦闘開始を表示する
                if ( !_card_select_start ) {
                    //1秒後テキストを消す
                    _now_time += Time.deltaTime;
                    if ( _now_time >= _interval_time ) {
                        //経過時間をバトルタイマーにセット
                        _now_time = 0;
                        //カードセレクトを開始
                        _card_select_start = true;
                        //オブジェクトを削除
                        objectDelete( PLAYER_OBJECT_LIST.BATTLE_SELECT_AREA );
                    }
                } else if ( _now_time > _battle_time || _select_push ) {
                    //セレクトエリアにセットされたカードをセレクトエリアカードに
                    _player_manager.SetSelectAreaCard( );
                    //テキストウィンドウを描画
                    objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
                    //テキストを設定
                    textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, PLAYER_WAIT_MESSAGE );
                    //確定フラグ
                    _wait_phase = true;

                } else {
                    //時間を加算
                    _now_time += Time.deltaTime;
                    //制限時間から経過時間を引いた数を取得
                    float _count_dawn = _battle_time - _now_time;

                    //テキストを設定
                    _battle_timer_text.text = "残り時間 " + _count_dawn.ToString( "00" );
                }
            }
        }
	}

	void disCardPhase( ) {
		//初期設定が済んでなければ行う
		if ( !_initial_setting ) {
            phaseInit( );

            //黒背景にする
            objectDraw( PLAYER_OBJECT_LIST.BLACKOUT_PANEL, Vector3.zero );
			//確定ボタン
			objectDraw( PLAYER_OBJECT_LIST.DISCARD_BUTTON, discard_select_button_position );
			//セレクトエリア
			objectDraw( PLAYER_OBJECT_LIST.DISCARD_AREA, discard_selec_area_position );

			//セレクトカードを手札のカード数-6にして生成
			int _select_area_number = _player_manager.getHandListNumber( ) - 6;

            for ( int i = 0; i < _select_area_number; i++ ) {
                //セレクトカードはサイズ/さっきの数字の場所で作る
                //DisCard_Object[1][selectAreaPosition = DisCard_Object[1].GetComponent< RectTransform >().sizeDelta / SelectAreaNumber * i ;
            }

			//テキストウィンドウ
			objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, discard_text_position );
			textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, "捨てるカードを選んでください" );

			//初期設定完了フラグ
			_initial_setting = true;
		} else {
			//確定ボタンが押されたらセレクトリストにおいたものを削除
		}
	}


	void inductionPhase( ) {
		//初期設定が済んでなければ行う
		if ( !_initial_setting ) {
            phaseInit( );

            //フィールド画面に誘導するテキストを表示
            objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
			textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, FIELD_NAVI_MESSAGE );

			//初期設定完了フラグ
			_initial_setting = true;
		}
	}


    public void resultPhase ( BATTLE_RESULT _Result ) {
        if ( !_wait_phase ) {
            //初期設定が済んでなければ行う
            if ( !_initial_setting ) {
                phaseInit( );

                //キャンバスにテキストウィンドウを作成
                objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
                //リザルト結果によってテキスト変更
                switch ( _Result ) {
                    case BATTLE_RESULT.WIN:
                        textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, BATTLE_RESULT.WIN.ToString( ) );
                        break;

                    case BATTLE_RESULT.DRAW:
                        textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, BATTLE_RESULT.DRAW.ToString( ) );
                        break;

                    case BATTLE_RESULT.LOSE:
                        textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, BATTLE_RESULT.DRAW.ToString( ) );
                        break;
                }

                //初期設定完了フラグ
                _initial_setting = true;
            } else {
                //経過時間を加算
                _now_time += Time.deltaTime;
                //表示時間を経過時間が超えたら
                if ( _now_time >= _interval_time ) {
                    //オブジェクトを削除
                    objectDelete( );
                    //経過時間をバトルタイマーにセット
                    _now_time = 0;
                    //テキストを設定
                    textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, PLAYER_WAIT_MESSAGE );
                    //フェイズを待機状態に
                    _wait_phase = true;
                }
            }
        }
    }

    //フェイズ開始に実行
    void phaseInit( ) {
        //各種フラグや数値を初期化
        _initial_setting   = false;     //フェイズ毎の初期設定フラグ
        _draw_card_use     = false;     //ドローカードを使用したか
        _select_confirm    = false;     //ボタンを押したか
        _generate_complate = false;     //生成が終了したかどうか
        _card_select_start = false;     //カードセレクトが始まるかどうか
        _select_push       = false;     //確定を押したかどうか
        _wait_phase        = false;     //フェイズチェンジの待機フラグ

        //生成したオブジェクトを非表示に
        objectDelete( PLAYER_OBJECT_LIST.BATTLE_SELECT_AREA  );
    }

    public void phaseReset( ) {
        _wait_phase      = false;
        _initial_setting = false;
    }


    //描画されているオブジェクトを削除
    public void objectDelete( PLAYER_OBJECT_LIST _ignore_type = PLAYER_OBJECT_LIST.NONE_OBJECT ) {
		//オブジェクトを削除
		for ( int i = 0; i < _battle_phase_objects.Count; i++ ) {
            if ( _battle_phase_objects[ i ].type != _ignore_type ) {
                Destroy( _battle_phase_objects[ i ].obj );
            }
		}
	}

    //オブジェクトリストのオブジェクトを描画
    void objectDraw( PLAYER_OBJECT_LIST _obj_type, Vector3 _set_pos ) {
        OBJECT_DATA obj_data = new OBJECT_DATA( );
        //オブジェクトのサーチ
        for ( int i = 0; i < _battle_phase_objects.Count; i++ ) {
            if ( _obj_type == _battle_phase_objects[ i ].type ) {
                //データを保存
                obj_data = _battle_phase_objects[ i ];
                //オブジェクトにインスタンスを生成
                obj_data.obj = ( GameObject )Instantiate( _battle_phase_objects[ i ].resource );
                //オブジェクトをキャンバスに移動
                obj_data.obj.transform.SetParent( _canvas_Root.transform, false );
                //オブジェクトに座標を設定
                obj_data.obj.GetComponent< RectTransform >( ).anchoredPosition3D = _set_pos;

                //対象の現データを削除
                _battle_phase_objects.RemoveAt( i );
            }
        }
        //新たに追記したコピーデータを書き込み
        _battle_phase_objects.Add( obj_data );
    }

	//オブジェクトリストにプレハブとオブジェクトの種類を保存
	void objectLoad(  PLAYER_OBJECT_LIST _set_type, GameObject _load_resouce ) {
		//オブジェクト構造体
		OBJECT_DATA obj = new OBJECT_DATA( );

		//オブジェクトとIDとオブジェクトタイプを設定
		obj.resource = _load_resouce;
		obj.id       = _battle_phase_objects.Count;
		obj.type     = _set_type;

        //リストに追加
        _battle_phase_objects.Add( obj );

	}

	//指定されたオブジェクトタイプの子のテキストを設定したメッセージに変更
	void textSet( PLAYER_OBJECT_LIST _obj_type, string _message ) {
        //オブジェクトのサーチ
        for ( int i = 0; i < _battle_phase_objects.Count; i++ ) {
            if ( _obj_type == _battle_phase_objects[ i ].type ) {
                //テキストを指定したメッセージに変更します
                Text _Text = _battle_phase_objects[ i ].obj.GetComponentInChildren< Text >( );
                _Text.text = _message;
            }
        }
	}

    //フェイズが待機状態になっているかを取得
    public bool getPhaseWait( ) {
        return _wait_phase;
    }

    //ドローが終了したかどうかを取得
    public void drawEnd( bool is_End ) {
        _draw_end = is_End;
    }

    //プレイヤーにカードIDを渡すため
    public void getCardId( int _card_id ) {
        //プレイヤーにカードIDを渡す
        _player_manager.deckCardList( _card_id );
    }

	//カードセレクトフラグを取得する関数です、カードを必要な時に動かせないようにしています
	public bool getCardSelectStart( ) {
		//カードセレクトが始まったかどうかを取得します
		return _card_select_start;
	}

	//ドローカードを使うかどうかを決める関数です、ボタンを押した際のはいといいえの挙動に使います
	public bool drowCardUse( bool _set_use ) {
		//ドローカードを使うかどうかを取得します
		_draw_card_use = _set_use;
		//ボタンを押したフラグをON
		_select_confirm = true;
		return _select_confirm;
	}

	//決定ボタンを押したかどうかを取得する関数です。押した状態でカードセレクトであれば決定フラグが立ちます
	public void select_push( ) {
		//カード選択フェイズで押されると反応をします
		if ( _card_select_start ) {
			_select_push = true;
		}
	}
}
