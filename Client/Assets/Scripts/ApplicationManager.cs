using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;

public class ApplicationManager : Manager< ApplicationManager > {

	enum PROGRAM_MODE {
		MODE_NO_CONNECT,
		MODE_CONNECT,
	};

	private const int MAX_DRAW_NUM = 4;
	private const int MAX_SEND_CARD_NUM = 4;

	[ SerializeField ]
	private SCENE _scene = SCENE.SCENE_CONNECT;
	[ SerializeField ]
	private NetworkMNG _network_manager;
	[ SerializeField ]
	private PhaseManager _phase_manager;
	[ SerializeField ]
	private CardManager _card_manager;
	[ SerializeField ]
	private ClientPlayerManager _player_manager;
	[ SerializeField ]
	private BattleManager _battle_manager;
	[ SerializeField ]
	private FileManager _file_manager;
	[ SerializeField ]
	private MapManager _map_manager;
	[ SerializeField ]
	private NetworkGUIControll _network_gui_controll;
	[ SerializeField ]
	private HostData _host_data;
	[ SerializeField ]
	private ClientData _client_data;

	[ SerializeField ]
	private PLAYER_ORDER _player_num = PLAYER_ORDER.NO_PLAYER;
	[ SerializeField ]
	private PROGRAM_MODE _mode = PROGRAM_MODE.MODE_NO_CONNECT;
	[ SerializeField ]
	private GAME_PLAY_MODE _play_mode = GAME_PLAY_MODE.MODE_NORMAL_PLAY;

    private GameObject _event_system;
    
    private GameObject _light_off_pref;
    private GameObject _light_off_obj;
    private GameObject _flush_pref;
    private GameObject _flush_obj;
    private Sprite _game_scene_back_ground;
    private GameObject _back_ground_obj;
    private GameObject _game_scene_select_area_pref;
    private GameObject _game_scene_select_area_obj;
    private GameObject _map_info_pref;
    private GameObject _map_info_obj;
    private GameObject _wait_picture_pref;
    private GameObject _wait_picture_obj;
    private GameObject _select_throw_area_pref;
    private GameObject _select_throw_area_obj;
    [ SerializeField ]
    private GameObject _dice_button_obj;
    [ SerializeField ]
    private GameObject _dice_button_pref;
    private GameObject _complete_button_obj;
	private GameObject _complete_button_pref;

    // 時間テキスト
	private GameObject[ ] _battle_time_image     = new GameObject[ 2 ];
	private GameObject[ ] _sea_deep_count_image  = new GameObject[ 3 ];
	private GameObject[ ] _goal_count_image      = new GameObject[ 2 ];

    private int _change_scene_count = 0;
    private int _change_phase_count = 0;
    private bool _scene_init  = false;
    private bool _phase_init  = false;
	private bool _result_init = false;
    private bool _reject      = false;
    private bool _wait_play   = false;
	[ SerializeField ]
    private int _debug_player_num = 0;
	[ SerializeField ]
    private bool _debug_player_move = false;
    BATTLE_RESULT _debug_result;

	public Text _scene_text;

	// Awake関数の代わり
	protected override void initialize( ) {
		init( );
	}

    void init( ) {
        if ( isError( ) ) {
            return;
        }

		referManager( );
	}

    
    bool isError( ) {
        bool error = false;

        if ( !_file_manager ) {
            try {
                error = true;
                _file_manager = FileManager.getInstance( );
            } catch {
                Debug.LogError( "ファイルマネージャーのインスタンスが取得できませんでした。" );
            }
        }

        return error;
    }

	// Use this for initialization
	void Start( ) {

        referManager( );

        try {
            _card_manager.init( );
        }
        catch {
            Debug.Log( "Failure Init CardManager..." );
        }

		try {
			_battle_manager.init( );
		}
		catch {
			Debug.Log( "Failure Init BattleManager..." );
		}

        try {
            _map_manager.init( );
        }
        catch {
            Debug.Log( "Failure Init MapManager..." );
        }

        try {
            _light_off_pref = Resources.Load< GameObject >( "Prefabs/Background/LightOff" );
        }
        catch {
            Debug.Log( "Failure Load LightOffObj..." );
        }

        try {
            _flush_pref = Resources.Load< GameObject >( "Prefabs/UI/Flush" );
        }
        catch {
            Debug.Log( "Failure Load FlushObj..." );
        }
	}

    private void referManager( ) {
		try {
			if ( _network_manager == null ) {
				_network_manager = GameObject.Find( "NetworkManager" ).GetComponent< NetworkMNG >( );
			}
			if ( _network_gui_controll == null ) {
				_network_gui_controll = GameObject.Find( "NetworkManager" ).GetComponent< NetworkGUIControll >( );
			}
			if ( _phase_manager == null ) {
				_phase_manager = GameObject.Find( "PhaseManager" ).GetComponent< PhaseManager >( );
			}
			if ( _card_manager == null ) {
				_card_manager = GameObject.Find( "CardManager" ).GetComponent< CardManager >( );
			}
			if ( _battle_manager == null ) {
				_battle_manager = GameObject.Find( "BattleManager" ).GetComponent< BattleManager >( );
			}
			if ( _player_manager == null ) {
				_player_manager = GameObject.Find( "ClientPlayerManager" ).GetComponent< ClientPlayerManager >( );
			}
			if ( _map_manager == null ) {
				_map_manager = GameObject.Find( "MapManager" ).GetComponent< MapManager >( );
			}
            if ( _event_system == null ) {
                _event_system = GameObject.Find( "EventSystem" );
            }
            if ( _back_ground_obj == null ) {
                _back_ground_obj = GameObject.Find( "BackGround" );
            }
		}
		catch {
			Debug.Log( "参照に失敗しました。" );
		}
    }

	// Update is called once per frame
	void FixedUpdate( ) {
		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _host_data == null && _network_manager.getHostObj( ) != null ) {
				_host_data = _network_manager.getHostObj ( ).GetComponent< HostData >( );
			}

			if ( _client_data == null && _network_manager.getClientObj( ) != null ) {
				_client_data = _network_manager.getClientObj( ).GetComponent< ClientData >( );
                if ( _client_data != null ) {
                    _client_data.CmdSetSendConnectReady( true );
                }
			}

			if ( _host_data != null && _client_data != null ) {
				if ( _player_num == PLAYER_ORDER.NO_PLAYER ) {
					_player_num = ( PLAYER_ORDER )_host_data.getRecvData( ).player_num;
				}

				// シーンの切り替え
				sceneChange ( );
				// 切り替え完了を送る
				if ( _client_data.getRecvData( ).changed_scene == true && _host_data.getRecvData( ).change_scene == false ) {
					_client_data.CmdSetSendChangedScene( false );
					_client_data.setChangedScene( false );
                    _change_scene_count = 0;
				}
			}
		} 

		switch( _scene ) {
		case SCENE.SCENE_CONNECT:
			updateConnectScene( );
			break;
		case SCENE.SCENE_TITLE:
			updateTitleScene( );
			break;
		case SCENE.SCENE_GAME:
			updateGameScene( );
			break;
		case SCENE.SCENE_FINISH:
			updateFinishScene( );
			break;
		}
	}

	/// <summary>
	/// シーンの切り替え
	/// </summary>
	private void sceneChange( ) { 
		if ( _host_data.getRecvData( ).change_scene && _change_scene_count == 0 ) {
            _client_data.CmdSetSendConnectReady( false );
			_scene = _host_data.getRecvData( ).scene;
			_scene_text.text = _scene.ToString( );
			_client_data.CmdSetSendChangedScene( true );
			_client_data.setChangedScene( true );
            _scene_init = false;
            if ( _scene == SCENE.SCENE_TITLE ) {
                _network_gui_controll.setShowGUI( false );
            }
            _client_data.CmdSetSendConnectReady( true );
            _change_scene_count = 1;
		}
	}

	/// <summary>
	/// ConnectSceneの更新
	/// </summary>
	private void updateConnectScene( ) {
        if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
            if ( Input.GetKeyDown( KeyCode.A ) ) {
			    _scene = SCENE.SCENE_TITLE;
			    _scene_text.text = _scene.ToString( );
			    _player_num = PLAYER_ORDER.PLAYER_ONE;
                _network_gui_controll.setShowGUI( false );
            }
        }
	}

	/// <summary>
	/// TitleSceneの更新
	/// </summary>
	private void updateTitleScene( ) {
        if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
            if ( Input.GetKeyDown( KeyCode.A ) ) {
			    _scene = SCENE.SCENE_GAME;
			    _scene_text.text = _scene.ToString( );
			    //マスの生成
			    for( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
				    int num = _map_manager.getMassCount( );
                    try {
				        _map_manager.createMassObj( num, _file_manager.getFileData( ).mass[ num ].type, _file_manager.getMassCoordinate( num ) );
				        _map_manager.increaseMassCount( );
                    }
                    catch {
                        Debug.Log( "Failure Create Mass..." );
                    }
			    }
                _map_manager.createMiniMass( );
                _map_manager.massPosAdustBasePos( );
                _map_manager.allMassReject( );

                _scene_init = false;
            } 
        }
	}

	/// <summary>
	/// FinishSceneの更新
	/// </summary>
	private void updateFinishScene( ) {
        if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
            if ( Input.GetKeyDown( KeyCode.A ) ) {
			    _scene = SCENE.SCENE_TITLE;
			    _scene_text.text = _scene.ToString( );
            } 
        }
	}

	/// <summary>
	/// GameSceneの更新
	/// </summary>
	private void updateGameScene( ) {
        if ( !_scene_init ) {
            if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
                _client_data.CmdSetSendConnectReady( false );
			    //マスの生成
			    for( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
				    int num = _map_manager.getMassCount( );
                    try {
				        _map_manager.createMassObj( num, _file_manager.getFileData( ).mass[ num ].type, _file_manager.getMassCoordinate( num ) );
                        //Debug.Log( "Clear Create Mass..." );
				        _map_manager.increaseMassCount( );
                    }
                    catch {
                        Debug.Log( "Failure Create Mass..." );
                    }
			    }
                _map_manager.createMiniMass( );
                _map_manager.massPosAdustBasePos( );
                _map_manager.allMassReject( );
                _client_data.CmdSetSendConnectReady( true );

                
            }
            // プレイヤーカードの生成
            _player_manager.createProfileCard( );
            _map_manager.bindSprite( _player_num );
            _scene_init = true;
        }

        if ( _wait_play ) {
            _event_system.SetActive( false );
        } else { 
            _event_system.SetActive( true );
        }

		// フェイズごとの更新
        if ( _play_mode == GAME_PLAY_MODE.MODE_NORMAL_PLAY ) {
		    switch( _phase_manager.getMainGamePhase( ) ) {
		    case MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY:
			    updateNoPlayPhase( );
			    break;
		    case MAIN_GAME_PHASE.GAME_PHASE_DICE:
			    updateDicePhase( );
			    break;
		    case MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER:
			    updateMovePhase( );
			    break;
		    case MAIN_GAME_PHASE.GAME_PHASE_DRAW_CARD:
			    updateDrawPhase( );
			    break;
		    case MAIN_GAME_PHASE.GAME_PHASE_BATTLE:
			    updateButtlePhase( );
			    break;
		    case MAIN_GAME_PHASE.GAME_PHASE_RESULT:
			    updateResultPhase( );
			    break;
		    case MAIN_GAME_PHASE.GAME_PHASE_EVENT:
			    updateEventPhase( );
			    break;
		    case MAIN_GAME_PHASE.GAME_PHASE_FINISH:
			    updateFinishPhase( );
			    break;
		    }
        } else if ( _play_mode == GAME_PLAY_MODE.MODE_PLAYER_SELECT ) {
            updateSelectPlayerCard( );
        }

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _host_data != null && _client_data != null ) {
				// フェイズの切り替え
				phaseChange( );

				// 切り替え完了を送る
				if ( _client_data.getRecvData( ).changed_phase == true && _host_data.getRecvData( ).change_phase == false ) {
					_client_data.CmdSetSendChangedPhase( false );
					_client_data.setChangedPhase( false );
                    _change_phase_count = 0;
				}
			}
		}

        // ターゲットの設定
        if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
            if ( _map_manager.getPlayerPosNum( ) != _host_data.getRecvData( ).mass_count[ ( int )_player_num ] ) {
                _map_manager.dicisionMoveTarget( _host_data.getRecvData( ).mass_count[ ( int )_player_num ] );
            }
        }

        if ( _debug_player_move ) {
            _debug_player_move = false;
            _map_manager.dicisionMoveTarget( _debug_player_num );
        }

        // マスの移動
        if ( _map_manager.isMassMove( ) ) {
            _map_manager.massMove( );
        }

		if ( _goal_count_image[ 0 ] != null && _goal_count_image[ 1 ] != null ) {
			_map_manager.changeGoalImageNum( _goal_count_image[ 0 ], _goal_count_image[ 1 ] );
		}

		_map_manager.adbanceSea( );

		if ( _sea_deep_count_image[ 0 ] != null && _sea_deep_count_image[ 1 ] != null && _sea_deep_count_image[ 2 ] != null ) {
			_map_manager.changeSeaDeepNum( _sea_deep_count_image[ 2 ], _sea_deep_count_image[ 0 ], _sea_deep_count_image[ 1 ] );
		}
	}

	/// <summary>
	/// フェイズの切り替え
	/// </summary>
	private void phaseChange( ) { 
		if ( _host_data.getRecvData( ).change_phase && _change_phase_count == 0 ) {
			try {
				_phase_manager.setPhase( _host_data.getRecvData( ).main_game_phase );
			}
			catch {
				Debug.Log( "ゲームフェイズの取得に失敗しまし" );
			}
            _client_data.CmdSetSendConnectReady( true );
			_client_data.CmdSetSendChangedPhase( true );
			_client_data.setChangedPhase( true );
            _phase_init = false;
            Debug.Log( "GamePhase Change..." );
            _change_phase_count = 1;
		}
	}

	/// <summary>
	/// NoPlayPhaseの更新
	/// </summary>
	private void updateNoPlayPhase( ) {
        // 初期化処理
        if ( !_phase_init ) {
            switch ( _player_num ) {
                case PLAYER_ORDER.PLAYER_ONE:
                    _game_scene_back_ground = Resources.Load< Sprite >( "Graphics/Background/bg_P1" );
                    break;
                case PLAYER_ORDER.PLAYER_TWO:
                    _game_scene_back_ground = Resources.Load< Sprite >( "Graphics/Background/bg_P2" );
                    break;
            }
            _back_ground_obj.GetComponent< Image >( ).sprite = _game_scene_back_ground;

            createSelectArea( "MapBackground" );
            createMapInfo( );

			bindMapCountImage( );
			_map_manager.changeGoalImageNum( _goal_count_image[ 0 ], _goal_count_image[ 1 ] );
            _phase_init = true;
        }

        if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
            if ( Input.GetKeyDown( KeyCode.A ) ) {
			    _phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_DICE );
                _phase_init = false;
            } 
        }
	}

	/// <summary>
	/// DicePhaseの更新
	/// </summary>
	private void updateDicePhase( ) {
		int value = 0;
        if ( !_phase_init ) {

            if ( _wait_play ) {
                destroyWaitImage( );
            }

            createLightOffObj( false );

            _dice_button_pref = Resources.Load< GameObject >( "Prefabs/UI/DiceButton" );

            _dice_button_obj = ( GameObject )Instantiate( _dice_button_pref );
            _dice_button_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
            _dice_button_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
            _dice_button_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 0.03f, 0.03f, 1 );
            _dice_button_obj.GetComponent< RectTransform >( ).localPosition = new Vector3( 0, 0, -600 );

            _dice_button_obj.GetComponent< Button >( ).onClick.AddListener( _player_manager.dicisionDiceValue );

            _phase_init = true;
        }

        if ( _player_manager.isDiceRoll( ) ) {

            // ダイスオブジェ削除
            Destroy( _dice_button_obj );
            _dice_button_pref = null;
            _dice_button_obj  = null;
            destroyLightOffObj( );

            createWaitImage( "InductionOver" );

            // ダイスの目を決定
            value = _player_manager.getDiceValue( );

            // サーバーにダイスの目を送信
            if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
                _client_data.CmdSetSendDiceValue( value );
                _client_data.setDiceValue(value);
            }  else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
                _phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER );

                _phase_init = false;
            }
        }
	}

	/// <summary>
	/// MovePhaseの更新
	/// </summary>
	private void updateMovePhase( ) {
		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _client_data.getRecvData( ).dice_value > 0 ) {
				// さいの目を―1に初期化
				_client_data.CmdSetSendDiceValue ( -1 );
				_client_data.setDiceValue( -1 );
			}
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
            _phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_DRAW_CARD );
            _map_manager.dicisionMoveTarget( _map_manager.getPlayerPosNum( ) + _player_manager.getDiceValue( ) );
        }
	}

	/// <summary>
	/// DrawPhaseの更新
	/// </summary>
	private void updateDrawPhase( ) {
        if ( !_phase_init ) {
            destroyWaitImage( );
            createLightOffObj( true );
            _phase_init = true;
        }

        // カードドロー演出
        if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
            drawEventAction( false );
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
            drawEventAction( MAIN_GAME_PHASE.GAME_PHASE_BATTLE );
        }
	}

    /// <summary>
    /// 通信時のドロー演出
    /// </summary>
    private void drawEventAction( bool event_phase ) {
        int length = 0;
        if ( _player_num == PLAYER_ORDER.PLAYER_ONE ) {
            length = _host_data.getRecvData( ).card_list_one.Length;
        } else if ( _player_num == PLAYER_ORDER.PLAYER_TWO ) {
            length = _host_data.getRecvData( ).card_list_two.Length;
        }

		// カードデータを受診したら
		if ( _host_data.getCardListNum( _player_num ) == length &&
			    _client_data.getRecvData( ).ready == false ) {
            if ( !_player_manager.isDrawCard( ) ) {
				for ( int i = 0; i < _host_data.getCardListNum( _player_num ); i++ ) {
					if ( _host_data.getCardList( _player_num )[ i ] < 1 ) {
						continue;
					}

					// カードデータを登録
					_player_manager.addDrawCard( _host_data.getCardList( _player_num )[ i ], i, _host_data.getCardListNum( _player_num ) );
				}
                _player_manager.setDrawCardFlag( true );
            } else {
                drawProductionUpdate( );
            }
        }

        // カードが送られて来たら
        if ( _player_manager.getDrawCardAction( ) == ClientPlayerManager.DRAW_CARD_ACTION.MOVE_FOR_GET_ACTION ) {
            if ( _player_manager.isArrivedAllDrawCard( ) ) {
                createFlushObj( );
            }
        }

        // カードの回転が終わったら
        if ( _player_manager.isFinishRotateAllDrawCard( ) ) {
            finishRotateCard( );
            destroyFlushObj( );
        }

        if ( _player_manager.getDrawCardAction( ) == ClientPlayerManager.DRAW_CARD_ACTION.MOVE_FOR_HAND_ACTION ) {
            if ( _player_manager.isArrivedAllDrawCard( ) ) {
                finishDrawUpdate( );

                if ( event_phase ) {
                    try {
                        // イベント処理完了を送信
                        _client_data.CmdSetSendOkEvent( true );
                        _client_data.setOkEvent( true );
                    }
                    catch {
                        Debug.Log( "Failure Connect..." );
                    }
                    _player_manager.setDrawCardFlag( false );
                    destroyLightOffObj( );
                }
                else {
                    try {
				        // サーバーに準備完了を送信
				        _client_data.CmdSetSendReady( true );
				        _client_data.setReady( true );
                    }
                    catch {
                        Debug.Log( "Failure Connect..." );
                    }
                }
            }
        }
    }

    /// <summary>
    /// 非通信時のドロー演出
    /// </summary>
    /// <param name="phase"></param>
    private void drawEventAction( MAIN_GAME_PHASE phase ) {
        if ( !_player_manager.isDrawCard( ) ) {
            // 配するカードを決定
            List< int > card_list = new List< int >( );
            for ( int i = 0; i < 4 - _player_manager.getDiceValue( ); i++ ) {
                if ( _card_manager.getDeckCardNum( ) <= 0 ) {
                    _card_manager.createDeck( );
                }
				card_list.Add( _card_manager.distributeCard( ).id );
			}

            // カード配布
			for ( int i = 0; i < card_list.Count; i++ ) {
				// カードデータを登録
				_player_manager.addDrawCard( card_list[ i ], i, card_list.Count );
                //_player_manager.moveStartDrawCard( i );
			}
            _player_manager.setDrawCardFlag( true );
        } else {
            drawProductionUpdate( );
        }
        // カードが送られて来たら
        if ( _player_manager.getDrawCardAction( ) == ClientPlayerManager.DRAW_CARD_ACTION.MOVE_FOR_GET_ACTION ) {
            if ( _player_manager.isArrivedAllDrawCard( ) ) {
                createFlushObj( );
            }
        }

        // カードの回転が終わったら
        if ( _player_manager.isFinishRotateAllDrawCard( ) ) {
            finishRotateCard( );
            destroyFlushObj( );
        }
        if ( _player_manager.getDrawCardAction( ) == ClientPlayerManager.DRAW_CARD_ACTION.MOVE_FOR_HAND_ACTION ) {
            if ( _player_manager.isArrivedAllDrawCard( ) ) {
                finishDrawUpdate( );
                    
                _phase_init = false;
                _phase_manager.setPhase( phase );
                    
            }
        }
    }

    /// <summary>
    /// DrawPhaseの演出処理
    /// </summary>
    private void drawProductionUpdate( ) {
        for ( int i = 0; i < _player_manager.getDrawCardNum( ); i++ ) {
            if ( _player_manager.getDrawCardAction( ) == ClientPlayerManager.DRAW_CARD_ACTION.MOVE_FOR_GET_ACTION ||
                    _player_manager.getDrawCardAction( ) == ClientPlayerManager.DRAW_CARD_ACTION.MOVE_FOR_HAND_ACTION ) {
                _player_manager.moveDrawCard( i );
            } else if ( _player_manager.getDrawCardAction( ) == ClientPlayerManager.DRAW_CARD_ACTION.ROTATE_ACTION ) {
                _player_manager.rotateDrawCard( i );
            } 
        }
    }

    /// <summary>
    /// DrawUpdate終了処理
    /// </summary>
    private void finishDrawUpdate( ) {
        //一度画面上に配置しているカードオブジェクトを削除
        _player_manager.allDeletePlayerCard( );
        // ダイスの目を初期化
        _player_manager.initDiceValue( );
        // カードを生成
        _player_manager.initAllPlayerCard( );

        if ( _player_manager.getPlayerCardNum( ) > _player_manager.getMaxPlayerCardNum( ) ) {
            _play_mode = GAME_PLAY_MODE.MODE_PLAYER_SELECT;
            _player_manager.setPlayMode( _play_mode );
            _player_manager.initAllPlayerCard( );
            _phase_init = false;
            return;
        }
    }

    /// <summary>
    /// カードの回転終了処理
    /// </summary>
    private void finishRotateCard( ) {
        Vector3 target_pos = new Vector3( 0, 0, 0 );
		float hand_area_postion_y = 0.0f;
		float hand_area_postion_z = 0.0f;
        GameObject area = _player_manager.getPlayerCardArea( );
		float start_card_point = area.transform.position.x - area.transform.localScale.x / 2;
		float card_potision_x = 0.0f;
		hand_area_postion_y = area.transform.position.y;
		hand_area_postion_z = area.transform.position.z;
                

        for ( int i = 0; i < _player_manager.getDrawCardNum( ); i++ ) {
            card_potision_x = start_card_point + _player_manager.getCardWidth( ) * ( _player_manager.getPlayerCardNum( ) - 1 + i + 1 );
            //位置を設定する
		    target_pos = new Vector3( card_potision_x,  hand_area_postion_y, hand_area_postion_z );

            _player_manager.setDrawCardMoveTarget( i, target_pos );
        }
    }

    private void updateSelectPlayerCard( ) {
        // 初期化処理
        if ( !_phase_init ) {
            _player_manager.setDrawCardFlag( false );
            _player_manager.playerCardEnable( false );
            destroyLightOffObj( );
            createLightOffObj( true );
            _select_throw_area_pref = Resources.Load< GameObject >( "Prefabs/Background/SelectThrowArea" );
            Vector3 pos = _select_throw_area_pref.GetComponent< RectTransform >( ).localPosition;
            
            _select_throw_area_obj = ( GameObject )Instantiate( _select_throw_area_pref );
            _select_throw_area_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
            _select_throw_area_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
            _select_throw_area_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
            _select_throw_area_obj.GetComponent< RectTransform >( ).localPosition = pos;

            _select_throw_area_obj.GetComponentInChildren< Button >( ).onClick.AddListener( _player_manager.dicisionSelectThrowCard );
            _phase_init = true;
        }

        if ( _player_manager.mouseClick( ) && !_wait_play ) {
			//GUIにカード情報表示用
			Debug.Log( _player_manager.getSelectCard( ).name );
		}

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
            if ( _player_manager.isSelectThrowComplete( ) ) {
                _play_mode = GAME_PLAY_MODE.MODE_NORMAL_PLAY;
                _player_manager.allSelectInit( );
                // カードを更新
                _player_manager.initAllPlayerCard( );
                // サーバーに準備完了を送信
                _client_data.CmdSetSendReady( true );
                _client_data.setReady( true );
                Destroy( _select_throw_area_obj );
                _select_throw_area_pref = null;
                destroyLightOffObj( );
                _player_manager.refreshSelectCard( );
                _player_manager.setPlayMode( _play_mode );
                _player_manager.initAllPlayerCard( );
                _player_manager.playerCardEnable( true );
            }
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
            if ( _player_manager.isSelectThrowComplete( ) ) {
                _play_mode = GAME_PLAY_MODE.MODE_NORMAL_PLAY;
                _player_manager.allSelectInit( );
				// カードを更新
				_player_manager.initAllPlayerCard( );
				_phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_BATTLE );
                Destroy( _select_throw_area_obj );
                _select_throw_area_pref = null;
                _phase_init = false;
                destroyLightOffObj( );
                _player_manager.refreshSelectCard( );
                _player_manager.setPlayMode( _play_mode );
                _player_manager.initAllPlayerCard( );
                _player_manager.playerCardEnable( true );
            }
		}
    }

	/// <summary>
	/// ButtlePhaseの更新
	/// </summary>
	private void updateButtlePhase( ) {
        // 初期化処理
        if ( !_phase_init ) {
            _player_manager.setDrawCardFlag( false );
            destroyLightOffObj( );
            destroySelectArea( );
            destroyMapInfo( );
			freeMapCountImage( );
            createSelectArea( "BattleCardBackground" );
			bindBattleTimeImage( );
            createCompleteButton( );
            _map_manager.allMassVisible( false );
            _map_manager.setVisibleSprite( false );
            _battle_manager.refreshBattleTime( );
            _phase_init = true;
        }

        if ( _player_manager.mouseClick( ) && !_wait_play ) {
			//GUIにカード情報表示用
			Debug.Log( _player_manager.getSelectCard( ).name );
		}

        // 時間更新
		_battle_manager.changeBattleTimeImageNum( _battle_time_image[ 0 ], _battle_time_image[ 1 ] );
        _battle_manager.battleTimeCount( );

		if ( _battle_manager.isComplete( ) ) {
            createWaitImage( "WaitOpponent" );
            destroyCompleteButton( );
			if (  _mode == PROGRAM_MODE.MODE_CONNECT) {
				// 選択結果を送る
				int player_status = _player_manager.getPlayerData( ).power;
				int[ ] card_list = _battle_manager.resultSelectCard( _player_manager.dicisionSelectCard( ) );
				int[ ] turned_card_list = new int[ ]{ 0, 1, 2 };

				_client_data.CmdSetSendBattleData( true, player_status, card_list, turned_card_list );
				_client_data.setBattleData( true, player_status, card_list, turned_card_list );
                _player_manager.refreshSelectCard( );
				// カードを更新
				_player_manager.initAllPlayerCard( );
			} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
				int[ ] card_list = _player_manager.dicisionSelectCard( );
                if ( card_list.Length > MAX_SEND_CARD_NUM ) {
                    _player_manager.refreshSelectCard( );
                    return;
                }
                _player_manager.refreshSelectCard( );
			    _phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_RESULT );
                _phase_init = false;
            } 
        }

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _client_data.getRecvData( ).ready == true ) {
				// 準備完了を初期化
				_client_data.CmdSetSendReady( false );
				_client_data.setReady( false );
			}
		}
	}

	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
        // 初期化処理
        if ( !_phase_init ) {
            destroySelectArea( );
			freeBattleTimeImage( );
			createSelectArea( "MapBackground" );
            createMapInfo( );
			bindMapCountImage( );
			_map_manager.changeGoalImageNum( _goal_count_image[ 0 ], _goal_count_image[ 1 ] );
            _map_manager.allMassVisible( true );
            
            for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
                _map_manager.setMassColor( i, new Color( 0.5f, 0.5f, 0.5f ) );
                _map_manager.allMassReject( );
            }
            _map_manager.setVisibleSprite( true );
            _debug_result = ( BATTLE_RESULT )( ( int )Random.Range( 1, 3 ) );
            _phase_init  = true;
			_result_init = false;
        }

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _client_data.getRecvData( ).battle_ready == true ) {
				// 準備完了を初期化
				_client_data.CmdSetSendBattleReady( false );
				_client_data.setBattleReady( false );
			}
		}

		MASS_ADJUST adjust = MASS_ADJUST.NO_ADJUST;

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			bool flag = false;

			int battle_result = 0;

            battle_result = _host_data.getBattleResult( ( int )_player_num );


			if ( !_result_init && battle_result != 0 ) {
                destroyWaitImage( );
				createLightOffObj( false );
				_battle_manager.createResultImage( ( BATTLE_RESULT )battle_result );
				_result_init = true;
			}

			// 左クリックでResultを消す
			if ( Input.GetMouseButtonDown( 0 ) ) {
				_battle_manager.clearResult( );
				_battle_manager.deleteResultImage( );
				destroyLightOffObj( );

				// 前後一マス以内のマスを明るくする
				for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
					if ( i >= _map_manager.getPlayerPosNum( ) - 1 && i <= _map_manager.getPlayerPosNum( ) + 1 ) {
						_map_manager.setMassColor( i, Color.white );
						_map_manager.setMassNotReject( i );
					}
				}
			}

			// リザルト画面が消えていたら実行できる
			if ( !_battle_manager.isResultOpen( ) ) {
				// 勝ちか引き分け時
				if ( battle_result == ( int )BATTLE_RESULT.WIN ||
				    battle_result == ( int )BATTLE_RESULT.DRAW ) {
					// マス調整の処理
					int num = _map_manager.isSelect( );

					if ( num == _map_manager.getPlayerPosNum( ) + 1 ) {
						adjust = MASS_ADJUST.ADVANCE;
						flag = true;
						for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
							_map_manager.setMassColor( i, new Color( 0.5f, 0.5f, 0.5f ) );
							_map_manager.allMassReject( );
						}
					} else if ( num == _map_manager.getPlayerPosNum( ) - 1 ) {
						adjust = MASS_ADJUST.BACK;
						flag = true;
						for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
							_map_manager.setMassColor( i, new Color( 0.5f, 0.5f, 0.5f ) );
							_map_manager.allMassReject( );
						}
					} else if ( num == _map_manager.getPlayerPosNum( ) ) {
						adjust = MASS_ADJUST.NO_ADJUST;
						flag = true;
						for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
							_map_manager.setMassColor( i, new Color( 0.5f, 0.5f, 0.5f ) );
							_map_manager.allMassReject( );
						}
					}
				} else if ( battle_result == ( int )BATTLE_RESULT.LOSE ) {
					// 負けた場合マス調整不能
					adjust = MASS_ADJUST.NO_ADJUST;
					flag = true;
				}
			}

			// マスを進ませるかどうかを送信
			if ( flag ) {
				_client_data.CmdSetSendMassAdjust( true, adjust );
				_client_data.setMassAdjust( true, adjust );
                _map_manager.allMassReject( );
                createWaitImage( "WaitOpponent" );
			}
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			bool flag = false;
            int num = 0;

			if ( !_result_init ) {
                destroyWaitImage( );
				createLightOffObj( false );
				_battle_manager.createResultImage( _debug_result );
				_result_init = true;
			}

			// 左クリックでResultを消す
			if ( Input.GetMouseButtonDown( 0 ) ) {
				_battle_manager.clearResult( );
				_battle_manager.deleteResultImage( );
				destroyLightOffObj( );

				// 前後一マス以内のマスを明るくする
				for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
					if ( i >= _map_manager.getPlayerPosNum( ) - 1 && i <= _map_manager.getPlayerPosNum( ) + 1 ) {
						_map_manager.setMassColor( i, Color.white );
						_map_manager.setMassNotReject( i );
					}
				}
			}

			// リザルト画面が消えていたら実行できる
			if ( !_battle_manager.isResultOpen( ) ) {
				if ( _debug_result == BATTLE_RESULT.WIN || _debug_result == BATTLE_RESULT.DRAW ) {
					// マス調整の処理
					num = _map_manager.isSelect( );

					if ( num == _map_manager.getPlayerPosNum( ) + 1 ) {
						for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
							_map_manager.setMassColor( i, new Color( 0.5f, 0.5f, 0.5f ) );
							_map_manager.allMassReject( );
							flag = true;
						}
					} else if ( num == _map_manager.getPlayerPosNum( ) - 1 ) {
						for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
							_map_manager.setMassColor( i, new Color( 0.5f, 0.5f, 0.5f ) );
							_map_manager.allMassReject( );
							flag = true;
						}
					} else if ( num == _map_manager.getPlayerPosNum( ) ) {
						for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
							_map_manager.setMassColor( i, new Color( 0.5f, 0.5f, 0.5f ) );
							_map_manager.allMassReject( );
							flag = true;
						}
					}
				} else if ( _debug_result == BATTLE_RESULT.LOSE ) {
					// 負けた場合マス調整不能
					flag = true;
				}
			}

            if ( flag ) {
                if ( _debug_result != BATTLE_RESULT.LOSE ) {
                    _map_manager.dicisionMoveTarget( num );
                }
			    _phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_EVENT );
                _map_manager.allMassReject( );
                createWaitImage( "WaitOpponent" );
                _phase_init = false;
            }
		}
	}

	/// <summary>
	/// EventPhaseの更新
	/// </summary>
	private void updateEventPhase( ) {
        // 初期化処理
        if ( !_phase_init ) {
            for ( int i = 0; i < _map_manager.getMassCount( ); i++ ) {
                _map_manager.setMassColor( i, Color.white );
            }
            destroyWaitImage( );
            createWaitImage( "InductionOver" );
            _phase_init = true;
        }

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _client_data.getRecvData( ).ready == true ) {
				// 準備完了を初期化
				_client_data.CmdSetSendMassAdjust( false, MASS_ADJUST.NO_ADJUST );
				_client_data.setMassAdjust( false, MASS_ADJUST.NO_ADJUST );
			}

            switch ( _host_data.getRecvData( ).event_type[ ( int )_player_num ] ) {
                case EVENT_TYPE.EVENT_DRAW:
                    drawEventAction( true );
                    break;
            }

            if ( _host_data.getRecvData( ).event_type[ ( int )_player_num ] == EVENT_TYPE.EVENT_NONE ||
                 _client_data.getRecvData( ).ok_event ) {
                // イベント処理完了を初期化
                _client_data.CmdSetSendOkEvent( false );
                _client_data.setOkEvent( false );
            }
            
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				_phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_DICE );
                _phase_init = false;
			} else if ( Input.GetKeyDown( KeyCode.B ) ) {
				_phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_FINISH );
                _phase_init = false;
			}
		} 
	}

	/// <summary>
	/// FinishPhaseの更新
	/// </summary>
	private void updateFinishPhase( ) {

	}
    
    /// <summary>
    /// 完了ボタンを作成
    /// </summary>
    public void createCompleteButton( ) {
        _complete_button_pref = Resources.Load< GameObject >( "Prefabs/UI/CompleteButton" );
        
        _complete_button_obj = ( GameObject )Instantiate( _complete_button_pref );
        _complete_button_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
        _complete_button_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
        _complete_button_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );

        Vector3 pos = _complete_button_pref.GetComponent< RectTransform >( ).localPosition;
        _complete_button_obj.GetComponent< RectTransform >( ).localPosition = pos;
            
        _complete_button_obj.GetComponent< Button >( ).onClick.AddListener( _battle_manager.readyComplete );
    }
    
    /// <summary>
    /// 完了ボタンを削除
    /// </summary>
    private void destroyCompleteButton( ) {
        Destroy( _complete_button_obj );
        _complete_button_obj = null;
        _complete_button_pref = null;
    }

    /// <summary>
    /// 暗くなる画面を生成
    /// </summary>
    private void createLightOffObj( bool throw_card ) {
        Vector3 pos = _light_off_pref.GetComponent< RectTransform >( ).localPosition;
            
        _light_off_obj = ( GameObject )Instantiate( _light_off_pref );
        _light_off_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
        _light_off_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );

        _reject = true;

        if ( !throw_card ) {
            pos = new Vector3( pos.x, pos.y, -550.0f );
        }
        _light_off_obj.GetComponent< RectTransform >( ).localPosition = pos;
        _light_off_obj.GetComponent< RectTransform >( ).offsetMax = new Vector2( 0, 0 );
        _light_off_obj.GetComponent< RectTransform >( ).offsetMin = new Vector2( 0, 0 );

    }
    
    /// <summary>
    /// 暗くなる画面を削除
    /// </summary>
    private void destroyLightOffObj( ) {
        Destroy( _light_off_obj );
        _light_off_obj  = null;
        _reject = false;
    }
    
    /// <summary>
    /// フラッシュ画像を作成
    /// </summary>
    private void createFlushObj( ) {
        Vector3 pos = _flush_pref.GetComponent< RectTransform >( ).localPosition;
            
        _flush_obj = ( GameObject )Instantiate( _flush_pref );
        _flush_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
        _flush_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
        _flush_obj.GetComponent< RectTransform >( ).localPosition = pos;

    }
    
    /// <summary>
    /// フラッシュ画像を削除
    /// </summary>
    private void destroyFlushObj( ) {
        Destroy( _flush_obj );
        _flush_obj  = null;
    }

    /// <summary>
    /// セレクトエリアの生成
    /// </summary>
    /// <param name="data_path"></param>
    private void createSelectArea( string data_path ) {
        _game_scene_select_area_pref = Resources.Load< GameObject >( "Prefabs/Background/" + data_path );
        Vector3 pos = _game_scene_select_area_pref.GetComponent< RectTransform >( ).localPosition;
            
        _game_scene_select_area_obj = ( GameObject )Instantiate( _game_scene_select_area_pref );
        _game_scene_select_area_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
        _game_scene_select_area_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
        _game_scene_select_area_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
        _game_scene_select_area_obj.GetComponent< RectTransform >( ).localPosition = pos;

        int num = GameObject.Find( "MassBasePoint" ).transform.GetSiblingIndex( );
        _game_scene_select_area_obj.transform.SetSiblingIndex( num );

        _back_ground_obj.transform.SetSiblingIndex( _game_scene_select_area_obj.transform.GetSiblingIndex( ) + 2 );
    }

    /// <summary>
    /// セレクトエリアの削除
    /// </summary>
    private void destroySelectArea( ) {
        Destroy( _game_scene_select_area_obj );
        _game_scene_select_area_obj = null;
        _game_scene_select_area_pref = null;
    }
    
    /// <summary>
    /// マップ情報の生成
    /// </summary>
    private void createMapInfo( ) {
        _map_info_pref = Resources.Load< GameObject >( "Prefabs/Background/MapInfo" );
        Vector3 pos = _game_scene_select_area_pref.GetComponent< RectTransform >( ).localPosition;
            
        _map_info_obj = ( GameObject )Instantiate( _map_info_pref );
        _map_info_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
        _map_info_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
        _map_info_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
        _map_info_obj.GetComponent< RectTransform >( ).localPosition = pos;
    }

    /// <summary>
    /// マップ情報の削除
    /// </summary>
    private void destroyMapInfo( ) {
        Destroy( _map_info_obj );
        _map_info_obj = null;
        _map_info_pref = null;
    }

    /// <summary>
    /// 待機画面の作成
    /// </summary>
    /// <param name="data_path"></param>
    private void createWaitImage( string data_path ) {
        createLightOffObj( false );

        _wait_picture_pref = Resources.Load< GameObject >( "Prefabs/UI/" + data_path );
        Vector3 pos = _wait_picture_pref.GetComponent< RectTransform >( ).localPosition;
            
        _wait_picture_obj = ( GameObject )Instantiate( _wait_picture_pref );
        _wait_picture_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
        _wait_picture_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
        _wait_picture_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
        _wait_picture_obj.GetComponent< RectTransform >( ).localPosition = pos;

        _wait_play = true;
    }

    /// <summary>
    /// 待機画面の削除
    /// </summary>
    private void destroyWaitImage( ) {
        destroyLightOffObj( );
        Destroy( _wait_picture_obj );
        _wait_picture_obj = null;
        _wait_picture_pref = null;

        _wait_play = false;
    }

	private void bindMapCountImage( ) {
		if ( _goal_count_image[ 0 ] == null ) {
			_goal_count_image[ 0 ] = GameObject.Find( "mass_ten_digit" );
		}
		if ( _goal_count_image[ 1 ] == null ) {
			_goal_count_image[ 1 ] = GameObject.Find( "mass_digit" );
		}
		if ( _sea_deep_count_image[ 0 ] == null ) {
			_sea_deep_count_image[ 0 ] = GameObject.Find( "deep_hudred_digit" );
		}
		if ( _sea_deep_count_image[ 1 ] == null ) {
			_sea_deep_count_image[ 1 ] = GameObject.Find( "deep_ten_digit" );
		}
		if ( _sea_deep_count_image[ 2 ] == null ) {
			_sea_deep_count_image[ 2 ] = GameObject.Find( "deep_digit" );
		}
	}

	private void freeMapCountImage( ) {
		for ( int i = 0; i < _goal_count_image.Length; i++ ) {
			_goal_count_image[ i ] = null;
		}
		for ( int i = 0; i < _sea_deep_count_image.Length; i++ ) {
			_sea_deep_count_image[ i ] = null;
		}
	}

	private void bindBattleTimeImage( ) {
		if ( _battle_time_image[ 0 ] == null ) {
			_battle_time_image[ 0 ] = GameObject.Find( "ten_time_digit" );
		}
		if ( _battle_time_image[ 1 ] == null ) {
			_battle_time_image[ 1 ] = GameObject.Find( "time_digit" );
		}
	}

	private void freeBattleTimeImage( ) {
		for ( int i = 0; i < _battle_time_image.Length; i++ ) {
			_battle_time_image[ i ] = null;
		}
	}

	public void OnGUI( ) {
		if ( _scene == SCENE.SCENE_CONNECT && _host_data != null ) {
			drawConnectScene( );
		}
	}

	/// <summary>
	/// ConnectSceneの描画
	/// </summary>
	private void drawConnectScene( ) {

		if( !_network_manager.isConnected( ) && _host_data.getServerState( ) != SERVER_STATE.STATE_HOST ) {
			//_network_manager.noConnectDraw( );
		}

		if ( _host_data.getServerState( ) == SERVER_STATE.STATE_HOST ) {
			_network_manager.hostStateDraw( );
		}

	}

	/// <summary>
	/// シーン情報を返す
	/// </summary>
	/// <returns>The scene.</returns>
	public SCENE getScene( ) {
		return _scene;
	}

}