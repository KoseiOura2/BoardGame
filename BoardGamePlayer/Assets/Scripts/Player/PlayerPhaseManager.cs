using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Common;
using PlayerData;

public class PlayerPhaseManager : MonoBehaviour {

	public const string DICE_PHASE_MESSAGE  = "ダイスを振ります\n(今回はダイスのアニメーションはありません)";		//ダイスフェイズのメッセージ
	public const string FIELD_NAVI_MESSAGE  = "上画面に注目してください";									//フィールド画面に誘導するメッセージ
	public const string PLAYER_WAIT_MESSAGE = "対戦相手を待っています";										//対戦相手を待つ際のメッセージ

	private PlayerManager _player_manager;	    //プレイヤーマネージャーを取得

	private BATTLE_RESULT _is_result;		    //プレイヤーの勝敗

    private int _dice_data;                     //ダイス数値の情報
    private int _player_trout_result;           //プレイヤーのマス調整の結果//ボタンの座標を設定
    private float _interval_time   = 3.0f;		//待ち時間を設定
    private float _now_time;                    //経過時間を取得
    private bool _initial_setting  = false;     //フェイズ毎の初期設定フラグ
    private bool _trout_adjustment = false;		//マス調整が可能か
	private bool _is_click         = false;     //マスがクリックされたか
    private bool _wait_phase       = false;     //フェイズチェンジの待機フラグ

	private List < OBJECT_DATA > _player_phase_objects; //プレイヤーフェイズオブジェクトをまとめる

    private Vector3 _set_dice_button_position = new Vector3 ( 0, -120, 0 ); //ダイスボタンの座標
    private Vector3 _set_select_area_position = new Vector3 ( 133, 40, 0 ); //セレクトエリアの座標

    /// <summary>
    /// 初起動時にオブジェクトを取得する。オブジェクトの取得などを主に行います
    /// </summary>
    public void awake( ) {
		
		//各種マネージャーのロード
		if ( _player_manager == null ) {
			GameObject _player_manager_obj = GameObject.Find( "PlayerManager" );
			_player_manager = _player_manager_obj.GetComponent< PlayerManager >( );
		}

		//リストの初期化
		_player_phase_objects = new List< OBJECT_DATA >( );

		//各種オブジェクトのロード
		objectLoad( PLAYER_OBJECT_LIST.YES_BUTTON,           ( GameObject )Resources.Load( "Prefab/Button" ) );
		objectLoad( PLAYER_OBJECT_LIST.NO_BUTTON,            ( GameObject )Resources.Load( "Prefab/Button" ) );
		objectLoad( PLAYER_OBJECT_LIST.TEXT_WINDOW,          ( GameObject )Resources.Load( "Prefab/TextWindow" ) );
		objectLoad( PLAYER_OBJECT_LIST.BLACKOUT_PANEL,       ( GameObject )Resources.Load( "Prefab/blackBackGround" ) );
		objectLoad( PLAYER_OBJECT_LIST.DISCARD_AREA,         ( GameObject )Resources.Load( "Prefab/DisCardArea" ) );
		objectLoad( PLAYER_OBJECT_LIST.DISCARD_BUTTON,       ( GameObject )Resources.Load( "Prefab/disCardSelectButton" ) );
		objectLoad( PLAYER_OBJECT_LIST.DICE_BUTTON,          ( GameObject )Resources.Load( "Prefab/DiceButton" ) );
        objectLoad( PLAYER_OBJECT_LIST.BATTLE_SELECT_AREA,   ( GameObject )Resources.Load( "Prefab/BattlePhaseSelectArea" ) );
        objectLoad( PLAYER_OBJECT_LIST.MAINGAME_SELECT_AREA, ( GameObject )Resources.Load( "Prefab/PlayerPhaseSelectArea" ) );
    }

	/// <summary>
	/// init
	/// 初起動時に初期設定を行います。ここでは数値の設定などをします
	/// </summary>
	public void init( ) {
        //初期数値を0に
		_dice_data = 0;
    }

	/// <summary>
	/// Waits the phase.
	/// ここでプレイヤーの判別を行い、プレイヤーラベルの色とテキスト表示をエネミーの表示とテキストの表示を変更します
	/// </summary>
	public void waitPhase( ) {
        if ( !_wait_phase ) {
            //プレイヤーの判別を行う
            if ( !_initial_setting ) {
                //共通の初期設定
                phaseInit( );

                //セレクトエリアを表示
                objectDraw( PLAYER_OBJECT_LIST.MAINGAME_SELECT_AREA, _set_select_area_position );
                // プレイヤー吹き出しを表示
                _player_manager.setPlayerPos( );
                //エネミーのテキストを設定
                _player_manager.setEnemyObject( );
                //プレイヤーによって変わる部分を変更
                _player_manager.setPlayerObject( );

                //初期設定完了フラグ
                _initial_setting = true;
            }
        }
	}

	/// <summary>
	/// ここでdiceボタンを押したら送信
	/// </summary>
	public void dicePhase( ) {
        if ( !_wait_phase ) {
            //初期設定が済んでなければ行う
            if ( !_initial_setting ) {
                //共通の初期設定
                phaseInit( );

                //画面を暗くする
                objectDraw( PLAYER_OBJECT_LIST.BLACKOUT_PANEL, Vector3.zero );
                //テキストウィンドウを表示
                objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
                textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, DICE_PHASE_MESSAGE );
                //サイコロボタンを表示
                objectDraw( PLAYER_OBJECT_LIST.DICE_BUTTON, _set_dice_button_position );

                //初期設定完了フラグ
                _initial_setting = true;
            } else {
                //ダイスデータに1以上の数値が入ったなら実行
                if ( _dice_data > 0 ) {
                    //3秒ほど経過後
                    _now_time += Time.deltaTime;
                    if ( _now_time >= _interval_time ) {

                        //テキストを設定
                        textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, PLAYER_WAIT_MESSAGE );
                        //経過時間をリセット
                        _now_time = 0;
                        //フェイズを待機状態に
                        _wait_phase = true;

                    }
                }
            }
        }
	}

	/// <summary>
	/// 画面上に誘導を行います
	/// </summary>
     public void inductionPhase( ) {
        if ( !_wait_phase ) {
            //初期設定が済んでなければ行う
            if ( !_initial_setting ) {
                //共通の初期設定
                phaseInit( );

                //テキストウィンドウを表示
                objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
                //テキストを設定
                textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, FIELD_NAVI_MESSAGE );
                //初期設定完了フラグ
                _initial_setting = true;
                //フェイズを待機状態に
                _wait_phase = true;
            }
        }
	}

    public void movePhase( BATTLE_RESULT _result ) {
        if ( !_wait_phase ) {
            //プレイヤーの判別を行う
            if ( !_initial_setting ) {
                //共通の初期設定
                phaseInit( );

                //セレクトエリアを表示
                objectDraw( PLAYER_OBJECT_LIST.MAINGAME_SELECT_AREA, _set_select_area_position );
                //プレイヤー吹き出しを表示
                _player_manager.setPlayerPos( );
                //リザルトの結果によってマス調整を出来る、出来ないを判定
                switch ( _result ) {
                    case BATTLE_RESULT.WIN:
                        //マス調整フラグがON
                        _trout_adjustment = true;
                        break;
                    case BATTLE_RESULT.DRAW:
                    case BATTLE_RESULT.LOSE:
                        //マス調整フラグがOFF
                        _trout_adjustment = false;
                        break;
                }

                //初期設定完了フラグ
                _initial_setting = true;

            } else {
                //マス調整フラグがONなら
                if ( _trout_adjustment ) {
                    //マスがクリックされた
                    if ( _is_click ) {
                        //テキストウィンドウを表示
                        objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
                        //テキストを設定
                        textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, PLAYER_WAIT_MESSAGE );
                        //プレイヤー吹き出しをクリックマスに移動させる
                        _player_manager.setPlayerPos( _player_trout_result );

                        //フェイズを待機状態に
                        _wait_phase = true;
                    }
                } else {
                    //テキストウィンドウを表示
                    objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
                    textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, PLAYER_WAIT_MESSAGE );

                    //フェイズを待機状態に
                    _wait_phase = true;
                }
            }
        }
    }

    void fieldPhase( ) {
        if ( !_wait_phase ) {
            //初期設定が済んでなければ行う
            if ( !_initial_setting ) {
                //フィールド画面に誘導するテキストを表示
                objectDraw( PLAYER_OBJECT_LIST.TEXT_WINDOW, Vector3.zero );
                //テキストを設定
                textSet( PLAYER_OBJECT_LIST.TEXT_WINDOW, FIELD_NAVI_MESSAGE );
                //初期設定完了フラグ
                _initial_setting = true;
            } else {
                //フェイズを待機状態に
                _wait_phase = true;
            }
        }
	}

	void finishPhase( ) {
		//初期設定が済んでなければ行う
		if ( !_initial_setting ) {
			//初期設定完了フラグ
			_initial_setting = true;
		}
	}

    //フェイズ開始に実行
    void phaseInit( ) {
		//各種フラグや数値を初期化
		_player_trout_result = 0;          //プレイヤーの調整値
		_is_click            = false;      //クリックされたか
		_trout_adjustment    = false;      //マス調整可能か
        _initial_setting     = false;      //フェイズ毎の初期設定フラグ
        _wait_phase          = false;      //フェイズチェンジの待機フラグ

        //生成したセレクトエリア以外のオブジェクトを非表示に
        objectDelete( PLAYER_OBJECT_LIST.MAINGAME_SELECT_AREA );
	}

	//描画されているオブジェクトを削除（引数を指定で対象オブジェクトを除外）
	public void objectDelete( PLAYER_OBJECT_LIST _ignore_type = PLAYER_OBJECT_LIST.NONE_OBJECT ) {
		//オブジェクトを削除
		for ( int i = 0; i < _player_phase_objects.Count; i++ ) {
            if ( _ignore_type != _player_phase_objects[ i ].type ) {
                Destroy( _player_phase_objects[ i ].obj );
            }
		}
	}

    //オブジェクトリストのオブジェクトを描画
    void objectDraw( PLAYER_OBJECT_LIST _obj_type, Vector3 _set_pos ) {
        OBJECT_DATA obj_Data   = new OBJECT_DATA( );
        GameObject canvas_root = GameObject.Find( "Canvas" );

        //オブジェクトのサーチ
        for ( int i = 0; i < _player_phase_objects.Count; i++ ) {
            if ( _obj_type == _player_phase_objects[ i ].type ) {
                //データを保存
                obj_Data = _player_phase_objects[ i ];
                //オブジェクトにインスタンスを生成
                obj_Data.obj = ( GameObject )Instantiate( _player_phase_objects[ i ].resource );
                //オブジェクトをキャンバスに座標を設定、サイズの修正
                obj_Data.obj.transform.SetParent( canvas_root.transform, false );
                //           obj_Data.obj.GetComponent<RectTransform> ( ).anchoredPosition3D = _setPos;
                //           obj_Data.obj.GetComponent<RectTransform> ( ).localScale = Vector3.one;

                //対象の現データを削除
                _player_phase_objects.RemoveAt( i );
            }
        }
        //新たに追記したコピーデータを書き込み
        _player_phase_objects.Add( obj_Data );
    }

	//オブジェクトリストにプレハブとオブジェクトの種類を保存
	void objectLoad(  PLAYER_OBJECT_LIST _set_type, GameObject _load_resouce ) {
		//オブジェクト構造体
		OBJECT_DATA obj = new OBJECT_DATA( );

		//オブジェクトとIDとオブジェクトタイプを設定
		obj.resource = _load_resouce;
		obj.id       = _player_phase_objects.Count;
		obj.type     = _set_type;

        //リストに追加
        _player_phase_objects.Add( obj );

	}

    //指定されたオブジェクトタイプの子のテキストを設定したメッセージに変更
    void textSet ( PLAYER_OBJECT_LIST _obj_type, string _message ) {
        //オブジェクトのサーチ
        for ( int i = 0; i < _player_phase_objects.Count; i++ ) {
            if ( _obj_type == _player_phase_objects[ i ].type ) {
                //テキストを指定したメッセージに変更します
                Text _Text = _player_phase_objects[ i ].obj.GetComponentInChildren< Text >( );
                _Text.text = _message;
            }
        }
    }

    public void phaseReset( ) {

        _wait_phase = false;

        _initial_setting = false;

    }

    //ダイスデータを設定する関数、ダイスボタンが押された際に実行をする
    public void setDiceData( int _get_dice_data ) {
        //ダイスボタンを消す
        for ( int i = 0; i < _player_phase_objects.Count; i++ ) {
            if ( _player_phase_objects[ i ].type == PLAYER_OBJECT_LIST.DICE_BUTTON ) {
                Destroy( _player_phase_objects[ i ].obj );
            }
        }

        //テキストを設定
        textSet ( PLAYER_OBJECT_LIST.TEXT_WINDOW, _get_dice_data + "が出ました！" );

        //プレイヤーの出た目で１、２、３を取得
        switch ( _get_dice_data ) {
            case 1:
                _dice_data = 1;
                break;
            case 2:
                _dice_data = 2;
                break;
            case 3:
                _dice_data = 3;
                break;
        }
    }

    public void initDiceData( ) {
        _dice_data = 0;
    }

	//マス調整が可能か取得する関数、マスをクリックする際の判定で使用
	public bool getTroutAdjustment( ) {
		return _trout_adjustment;
	}

    public int getDiceData( ) {
        return _dice_data;
    }

	//クリックされたマスからプレイヤーはどのマスにいるのか取得
	public void SetClick( int massID ) {

		//OKならその場所は今プレイヤーから見て前か後ろか同じか？
		//プレイヤーの現在地を取得
		int playerMass = _player_manager.getPlayerHere( );

		//現在地 - 移動先のマスで計算
		_player_trout_result = massID - playerMass;

		//クリックされました
		_is_click= true;
	}
}
