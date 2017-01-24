﻿using UnityEngine;
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
    
    private GameObject _light_off_pref;
    private GameObject _light_off_obj;
    private Sprite _game_scene_back_ground;
    private GameObject _back_ground_obj;
    private GameObject _game_scene_select_area_pref;
    private GameObject _game_scene_select_area_obj;
	[ SerializeField ]
    private GameObject _select_throw_area_pref;
	[ SerializeField ]
    private GameObject _select_throw_area_obj;
    private GameObject _dice_button_obj;
    private GameObject _dice_button_pref;
    private GameObject _complete_button_obj;
    private GameObject _complete_button_pref;

    private int _change_scene_count = 0;
    private int _change_phase_count = 0;
    private bool _scene_init = false;
    private bool _phase_init = false;
    private bool _reject = false;
    private bool _complete = false;
	[ SerializeField ]
    private int _debug_player_num = 0;
	[ SerializeField ]
    private bool _debug_player_move = false;

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
			if ( _player_manager == null ) {
				_player_manager = GameObject.Find( "ClientPlayerManager" ).GetComponent< ClientPlayerManager >( );
			}
			if ( _map_manager == null ) {
				_map_manager = GameObject.Find( "MapManager" ).GetComponent< MapManager >( );
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
            _client_data.CmdSetSendConnectReady( true );

            _scene_init = true;
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

		if ( _player_manager.mouseClick( ) ) {
			//GUIにカード情報表示用
			Debug.Log( _player_manager.getSelectCard( ).name );
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
            //_back_ground_obj.GetComponent< Image >( ).sprite = _game_scene_back_ground;

            createSelectArea( "MapBackground" );

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
			// ダイスの目を決定
			value = _player_manager.getDiceValue( );

			// サーバーにダイスの目を送信
			if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
				_client_data.CmdSetSendDiceValue( value );
				_client_data.setDiceValue( value );
			} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
                _phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER );
            }

            // ダイスオブジェ削除
            Destroy( _dice_button_obj );
            _dice_button_pref = null;
            destroyLightOffObj( );
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
        }
	}

	/// <summary>
	/// DrawPhaseの更新
	/// </summary>
	private void updateDrawPhase( ) {
		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
            int length = 0;
            if ( _player_num == PLAYER_ORDER.PLAYER_ONE ) {
                length = _host_data.getRecvData( ).card_list_one.Length;
            } else if ( _player_num == PLAYER_ORDER.PLAYER_TWO ) {
                length = _host_data.getRecvData( ).card_list_two.Length;
            }

			// カードデータを受診したら
			if ( _host_data.getCardListNum( _player_num ) == length &&
			     _client_data.getRecvData( ).ready == false ) {
                //一度画面上に配置しているカードオブジェクトを削除
                 _player_manager.allDeletePlayerCard();
				for ( int i = 0; i < _host_data.getCardListNum( _player_num ); i++ ) {
					if ( _host_data.getCardList( _player_num )[ i ] < 1 ) {
						continue;
					}

					// カードデータを登録
					_player_manager.addPlayerCard( _host_data.getCardList( _player_num )[ i ] );
				}

				// ダイスの目を初期化
				_player_manager.initDiceValue( );
				// カードを生成
				_player_manager.initAllPlayerCard( );

                if ( _player_manager.getPlayerCardNum( ) > _player_manager.getMaxPlayerCardNum( ) ) {
                    _play_mode = GAME_PLAY_MODE.MODE_PLAYER_SELECT;
                    _player_manager.setPlayMode( _play_mode );
                    return;
                }

                try {
				    // サーバーに準備完了を送信
				    _client_data.CmdSetSendReady( true );
				    _client_data.setReady( true );
                }
                catch {
                    Debug.Log( "Failure Connect..." );
                }
			}
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			if ( Input.GetKeyDown( KeyCode.A ) ) {
                //一度画面上に配置しているカードオブジェクトを削除
                _player_manager.allDeletePlayerCard( );

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
					_player_manager.addPlayerCard( card_list[ i ] );
				}

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

                _phase_init = false;
                _phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_BATTLE );

			}
		}
	}

    public void updateSelectPlayerCard( ) {
        // 初期化処理
        if ( !_phase_init ) {
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
            }
		}
    }

	/// <summary>
	/// ButtlePhaseの更新
	/// </summary>
	private void updateButtlePhase( ) {
        // 初期化処理
        if ( !_phase_init ) {
            destroySelectArea( );
            createSelectArea( "BattleCardBackground" );
            createCompleteButton( );
            _phase_init = true;
        }

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _client_data.getRecvData( ).ready == true ) {
				// 準備完了を初期化
				_client_data.CmdSetSendReady( false );
				_client_data.setReady( false );
			}
		}

		if ( _complete ) {
            destroyCompleteButton( );
            _complete = false;
			if (  _mode == PROGRAM_MODE.MODE_CONNECT) {
				// 選択結果を送る
				int player_status = _player_manager.getPlayerData( ).power;
				int[ ] card_list = _player_manager.dicisionSelectCard( );
				int[ ] turned_card_list = new int[ ]{ 0, 1, 2 };
                if ( card_list.Length > MAX_SEND_CARD_NUM ) {
                    _player_manager.refreshSelectCard( );
                    return;
                }
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
	}

	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
        // 初期化処理
        if ( !_phase_init ) {
            destroySelectArea( );
            createSelectArea( "MapBackground" );
            _phase_init = true;
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

			if ( _player_num == PLAYER_ORDER.PLAYER_ONE ) {
				battle_result = _host_data.getBattleResultOne( );
			} else if ( _player_num == PLAYER_ORDER.PLAYER_TWO ) {
				battle_result = _host_data.getBattleResultTwo( );
			}
			// 勝ちか引き分け時
			if ( battle_result == ( int )BATTLE_RESULT.WIN ||
				battle_result == ( int )BATTLE_RESULT.DRAW ) {
				// マス調整の処理
				if ( Input.GetKey( KeyCode.A ) ) {
					adjust = MASS_ADJUST.ADVANCE;
					flag = true;
				} else if ( Input.GetKey( KeyCode.S ) ) {
					adjust = MASS_ADJUST.BACK;
					flag = true;
				} else if ( Input.GetKey( KeyCode.D ) ) {
					adjust = MASS_ADJUST.NO_ADJUST;
					flag = true;
				}
			} else if ( battle_result == ( int )BATTLE_RESULT.LOSE ) {
				// 負けた場合マス調整不能
				adjust = MASS_ADJUST.NO_ADJUST;
				if ( Input.GetKey( KeyCode.A ) ) {
					flag = true;
				}
			}

			// マスを進ませるかどうかを送信
			if ( flag ) {
				_client_data.CmdSetSendMassAdjust( true, adjust );
				_client_data.setMassAdjust( true, adjust );
			}
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				_phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_EVENT );
			}
		}
	}

	/// <summary>
	/// EventPhaseの更新
	/// </summary>
	private void updateEventPhase( ) {
		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _client_data.getRecvData( ).ready == true ) {
				// 準備完了を初期化
				_client_data.CmdSetSendMassAdjust( false, MASS_ADJUST.NO_ADJUST );
				_client_data.setMassAdjust( false, MASS_ADJUST.NO_ADJUST );
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
            
        _complete_button_obj.GetComponent< Button >( ).onClick.AddListener( readyComplete );
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
        _light_off_obj = null;
        _reject = false;
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

        //_back_ground_obj.transform.SetSiblingIndex( _game_scene_select_area_obj.transform.GetSiblingIndex( ) + 2 );
    }

    /// <summary>
    /// セレクトエリアの削除
    /// </summary>
    private void destroySelectArea( ) {
        Destroy( _game_scene_select_area_obj );
        _game_scene_select_area_obj = null;
        _game_scene_select_area_pref = null;
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

    public void readyComplete( ) {
        _complete = true;
    }

}