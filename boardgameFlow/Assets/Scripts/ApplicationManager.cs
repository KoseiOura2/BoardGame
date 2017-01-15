using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;

public class ApplicationManager : Manager< ApplicationManager > {
	//
    enum PROGRAM_MODE {
		MODE_NO_CONNECT,
        MODE_ONE_CONNECT,
		MODE_TWO_CONNECT,
    };

	[ SerializeField ]
	private NetworkMNG _network_manager;
	[ SerializeField ]
	private PhaseManager _phase_manager;
    [ SerializeField ]
    private FileManager _file_manager;
	[ SerializeField ]
	private CardManager _card_manager;
    [ SerializeField ]
    private PlayerManager _player_manager;
    [ SerializeField ]
    private StageManager _stage_manager;
	[ SerializeField ]
	private CameraManager _camera_manager;

    [ SerializeField ]
    private NetworkGUIControll _network_gui_controll;
    [ SerializeField ]
    private HostData _host_data;
    [ SerializeField ]
    private ClientData[ ] _client_data = new ClientData[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
    
	[ SerializeField ]
	private PROGRAM_MODE _mode = PROGRAM_MODE.MODE_NO_CONNECT;
	[ SerializeField ]
	private SCENE _scene = SCENE.SCENE_CONNECT;
	private int[ ] _event_count = new int[ ]{ 0, 0 };        //イベントを起こす回数 
    [ SerializeField ]
    private int[ ] _dice_value = new int[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
    private bool _game_playing = false;
    private bool _goal_flag = false;

	public Text _scene_text;
	public Text[ ] _reside_text = new Text[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //残りマス用テキスト
	public Text[ ] _environment = new Text[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //環境情報用テキスト

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

		_card_manager.init( );
	}

	void referManager( ) {
		try {
			if ( _network_manager == null ) {
				_network_manager = GameObject.Find( "NetworkManager" ).GetComponent< NetworkMNG >( );
			}
			if ( _phase_manager == null ) {
				_phase_manager = GameObject.Find( "PhaseManager" ).GetComponent< PhaseManager >( );
			}
			if ( _card_manager == null ) {
				_card_manager = GameObject.Find( "CardManager" ).GetComponent< CardManager >( );
			}
			if ( _player_manager == null ) {
				_player_manager = GameObject.Find( "PlayerManager" ).GetComponent< PlayerManager >( );
			}
			if ( _stage_manager == null ) {
				_stage_manager = GameObject.Find( "StageManager" ).GetComponent< StageManager >( );
			}
			if ( _camera_manager == null ) {
				_camera_manager = Camera.main.GetComponent< CameraManager >( );
			}
			_network_gui_controll = GameObject.Find( "NetworkManager" ).GetComponent< NetworkGUIControll >( );
		}
		catch {
			Debug.Log( "参照に失敗しました。" );
		}
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
        
		if ( _mode == PROGRAM_MODE.MODE_ONE_CONNECT ) {
			if ( _host_data == null && _network_manager.getHostObj( ) != null ) {
				_host_data = _network_manager.getHostObj( ).GetComponent< HostData >( );
			}
			if ( _client_data[ 0 ] == null && _network_manager.getClientObj( 0 ) != null ) {
				_client_data[ 0 ] = _network_manager.getClientObj( 0 ).GetComponent< ClientData >( );
			}
		} else if ( _mode == PROGRAM_MODE.MODE_TWO_CONNECT ) {
			if ( _host_data == null && _network_manager.getHostObj( ) != null ) {
				_host_data = _network_manager.getHostObj( ).GetComponent< HostData >( );
			}
			if ( _client_data[ 0 ] == null && _network_manager.getClientObj( 0 ) != null ) {
				_client_data[ 0 ] = _network_manager.getClientObj( 0 ).GetComponent< ClientData >( );
			}
		    if ( _client_data[ 1 ] == null && _network_manager.getClientObj( 1 ) != null ) {
			    _client_data[ 1 ] = _network_manager.getClientObj( 1 ).GetComponent< ClientData >( );
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

		if ( _host_data != null && _client_data[ 0 ] != null /* && _client_data[ 1 ] != null */ ) {
			if ( _mode == PROGRAM_MODE.MODE_ONE_CONNECT ) {
				// player側のシーン変更が完了したかどうか
				if ( _client_data[ 0 ].getRecvData( ).changed_scene == true ) {
					_host_data.setSendChangeFieldScene( false );
				}
				// player側のフェイズ変更が完了したかどうか
				if ( _client_data[ 0 ].getRecvData( ).changed_phase == true ) {
					_host_data.setSendChangeFieldPhase( false );
				}
			} else if ( _mode == PROGRAM_MODE.MODE_TWO_CONNECT ) {
				// player側のシーン変更が完了したかどうか
				if ( _client_data[ 0 ].getRecvData( ).changed_scene == true ) {
					_host_data.setSendChangeFieldScene( false );
				}
				// player側のフェイズ変更が完了したかどうか
				if ( _client_data[ 0 ].getRecvData( ).changed_phase == true ) {
					_host_data.setSendChangeFieldPhase( false );
				}
                // player側のシーン変更が完了したかどうか
			    if ( _client_data[ 1 ].getRecvData( ).changed_scene == true ) {
				    _host_data.setSendChangeFieldScene( false );
			    }
                // player側のフェイズ変更が完了したかどうか
			    if ( _client_data[ 1 ].getRecvData( ).changed_phase == true ) {
				    _host_data.setSendChangeFieldPhase( false );
			    }
            }
 		}
	}

	/// <summary>
	/// ConnectSceneの更新
	/// </summary>
	private void updateConnectScene( ) {
		if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				_scene = SCENE.SCENE_TITLE;
				_scene_text.text = "SCENE_TITLE";
				_network_gui_controll.setShowGUI( false );
			}
		} else if ( _mode == PROGRAM_MODE.MODE_ONE_CONNECT ) {
			if ( _network_manager.getPlayerNum( ) >= 1 ) {
				_scene = SCENE.SCENE_TITLE;
				_scene_text.text = "SCENE_TITLE";
				_network_gui_controll.setShowGUI( false );
				try {
					_host_data.setSendScene( _scene );
	            	_host_data.setSendChangeFieldScene( true );
				}
				catch {
					Debug.Log( "通信に失敗しまいました" );
				}
			}
		} else if ( _mode == PROGRAM_MODE.MODE_TWO_CONNECT ) {
			if ( _network_manager.getPlayerNum( ) >= 2 ) {
				_scene = SCENE.SCENE_TITLE;
				_scene_text.text = "SCENE_TITLE";
				_network_gui_controll.setShowGUI( false );
				try {
					_host_data.setSendScene( _scene );
	            	_host_data.setSendChangeFieldScene( true );
				}
				catch {
					Debug.Log( "通信に失敗しまいました" );
				}
			}
		}
	}

	/// <summary>
	/// TitleSceneの更新
	/// </summary>
	private void updateTitleScene( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_GAME;
			_scene_text.text = "SCENE_GAME";

			_player_manager.init( _file_manager.getMassCoordinate( 0 ) );

			//マスの生成
			for( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
				int num = _stage_manager.getMassCount( );
				_stage_manager.massCreate( num, _file_manager.getFileData( ).mass[ num ].type, _file_manager.getMassCoordinate( num ) );
				_stage_manager.increaseMassCount( );
			}
			_stage_manager.init( );

			if ( _mode != PROGRAM_MODE.MODE_NO_CONNECT ) {
				try {
					_host_data.setSendScene( _scene );
					_host_data.setSendChangeFieldScene( true );
				} catch {
					Debug.Log( "通信に失敗しまいました" );
				}
			}

			_network_gui_controll.setShowGUI( false );
		}
	}

	/// <summary>
	/// FinishSceneの更新
	/// </summary>
	private void updateFinishScene( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_TITLE;
			_scene_text.text = "SCENE_TITLE";
			if ( _mode != PROGRAM_MODE.MODE_NO_CONNECT ) {
				try {
					_host_data.setSendScene( _scene );
					_host_data.setSendChangeFieldScene( true );
				} catch {
					Debug.Log( "通信に失敗しまいました" );
				}
			}
		}
	}

	/// <summary>
	/// GameSceneの更新
	/// </summary>
	private void updateGameScene( ) {
		// フェイズごとの更新
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

		// 通信データのセット
		if ( _phase_manager.isPhaseChanged( ) && _mode != PROGRAM_MODE.MODE_NO_CONNECT ) {
			_host_data.setSendGamePhase( _phase_manager.getMainGamePhase( ) );
			_host_data.setSendChangeFieldPhase( true );
		}

        // playerの環境情報を更新
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			if ( _file_manager.getEnvironment( _player_manager.getPlayerCount( i, _stage_manager.getMassCount( ) ) ) != "" ) {
				string environment = _file_manager.getEnvironment ( _player_manager.getPlayerCount( i, _stage_manager.getMassCount( ) ) );
				playerEnvironment( environment, i );
			}
		}

        int[ ] count = getResideCount( );
        _player_manager.dicisionTopAndLowestPlayer( ref count );

        // カメラの位置更新
		_camera_manager.moveCameraPos( _player_manager.getTopPlayer( PLAYER_RANK.RANK_FIRST ), _player_manager.getLastPlayer( ) );
	}

	/// <summary>
	/// NoPlayPhaseの更新
	/// </summary>
	private void updateNoPlayPhase( ) {
        // サイコロフェイズへの移行
		StartCoroutine( "gameStart" );
        _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_DICE, "DicePhase" );
	}
    
    private IEnumerator gameStart( ) {
        yield return new WaitForSeconds( 3.0f );
    }

	/// <summary>
	/// DicePhaseの更新
	/// </summary>
	private void updateDicePhase( ) {
		if ( _mode == PROGRAM_MODE.MODE_TWO_CONNECT ) {
            // 送られてきた賽の目の数
            int[ ] dice_value = new int[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
            dice_value[ 0 ] = _client_data[ 0 ].getRecvData( ).dice_value;
            dice_value[ 1 ] = _client_data[ 1 ].getRecvData( ).dice_value;
		    // ダイスを振ったら(通信)
		    if ( dice_value[ 0 ] > 0 && dice_value[ 1 ] > 0  ) {
                _dice_value[ 0 ] = dice_value[ 0 ];
                _dice_value[ 1 ] = dice_value[ 1 ];
                // キャラクター移動フェイズへの移行
                _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER, "MovePhase" );
            }
		} else if ( _mode == PROGRAM_MODE.MODE_ONE_CONNECT ) {
            // 送られてきた賽の目の数
            int[ ] dice_value = new int[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
            dice_value[ 0 ] = _client_data[ 0 ].getRecvData( ).dice_value;
		    // ダイスを振ったら(通信)
		    if ( dice_value[ 0 ] > 0 ) {
                _dice_value[ 0 ] = dice_value[ 0 ];
                // キャラクター移動フェイズへの移行
                _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER, "MovePhase" );
            }
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				// 送られてきた賽の目の数
				int[ ] dice_value = new int[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
				for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
					dice_value[ i ] = 2;//( int )Random.Range( 1.0f, 4.0f );
                    _dice_value[ i ] = dice_value[ i ];
				}
				// キャラクター移動フェイズへの移行
				_phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER, "MovePhase" );
			}
		}
	}

	/// <summary>
	/// MovePhaseの更新
	/// </summary>
	private void updateMovePhase( ) {
		if ( _mode == PROGRAM_MODE.MODE_TWO_CONNECT || _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
            if ( _player_manager.isPlayerMoveStart( 0 ) == false ) {
                // 1Pを動かす
				if ( _player_manager.getPlayerOnMove( 0 ) ) {
					_player_manager.setPlayerID( 0 );
					_player_manager.setLimitValue( _dice_value[ 0 ] );
					_player_manager.setAdvanceFlag( true );
				} else {
					_player_manager.setPlayerOnMove( 0, true );
				}
				_event_count[ 0 ] = 0;
			} else if ( _player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
                // 2Pを動かす
				if ( _player_manager.getPlayerOnMove( 1 ) ) {
					_player_manager.setPlayerID( 1 );
					_player_manager.setLimitValue( _dice_value[ 1 ] );
					_player_manager.setAdvanceFlag( true );
				} else {
					_player_manager.setPlayerOnMove( 1, true );
				}
				_event_count[ 0 ] = 0;
            }
		} else if ( _mode == PROGRAM_MODE.MODE_ONE_CONNECT ) {
            if ( _player_manager.isPlayerMoveStart( 0 ) == false ) {
                // 1Pを動かす
				if ( _player_manager.getPlayerOnMove( 0 ) ) {
					_player_manager.setPlayerID( 0 );
					_player_manager.setLimitValue( _dice_value[ 0 ] );
					_player_manager.setAdvanceFlag( true );
				}else {
					_player_manager.setPlayerOnMove( 0, true );
				}
				_event_count[ 1 ] = 0;
			} else if ( _player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
                // 2Pを動かす
				if ( _player_manager.getPlayerOnMove( 1 ) ) {
					_player_manager.setPlayerID( 1 );
					_player_manager.setLimitValue( _dice_value[ 0 ] );
					_player_manager.setAdvanceFlag( true );
				} else {
					_player_manager.setPlayerOnMove( 1, true );
				}
				_event_count[ 1 ] = 0;
            }
		}
		
		_player_manager.movePhaseUpdate( getResideCount( ),
            _stage_manager.getTargetMass( _player_manager.getTargetMassID( _stage_manager.getMassCount( ) ) ) );
        
       
        // ゴールまでの残りマスを表示
		resideCount( );

        // 両方の移動が終わったら次のフェイズへ
        if ( _player_manager.isPlayerMoveFinish( 0 ) == true && _player_manager.isPlayerMoveFinish( 1 ) == true ) {
            _player_manager.movedRefresh( );
            _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_DRAW_CARD, "DrawPhase" );
        } 
	}

	/// <summary>
	/// DrawPhaseの更新
	/// </summary>
	private void updateDrawPhase( ) {
        List< int > card_list = new List< int >( );

		if ( _mode == PROGRAM_MODE.MODE_TWO_CONNECT ) {
            // 1Pにカード配布
            if ( _host_data.getRecvData( ).card_list_one.Length == 0 ) {
		        for ( int i = 0; i < _dice_value[ 0 ]; i++ ) {
			        // デッキのカード数が０になったらリフレッシュ
			        if ( _card_manager.getDeckCardNum( ) <= 0 ) {
				        _card_manager.createDeck( );
			        }
                    card_list.Add( _card_manager.distributeCard( ).id );
		        }
                _host_data.setSendCardlist( ( int )PLAYER_ORDER.PLAYER_ONE, card_list );
                // カードリストを初期化
                card_list.Clear( );
            }
            
            // 2Pにカード配布
            if ( _host_data.getRecvData( ).card_list_two.Length == 0 ) {
		        for ( int i = 0; i < _dice_value[ 1 ]; i++ ) {
			        // デッキのカード数が０になったらリフレッシュ
			        if ( _card_manager.getDeckCardNum( ) <= 0 ) {
				        _card_manager.createDeck( );
			        }
                    card_list.Add( _card_manager.distributeCard( ).id );
		        }
                _host_data.setSendCardlist( ( int )PLAYER_ORDER.PLAYER_TWO, card_list );
            }

            // ドローカード処理を追加

            // 両方の準備が終わったら次のフェイズへ
            if ( _client_data[ 0 ].getRecvData( ).ready == true && _client_data[ 1 ].getRecvData( ).ready == true ) {
                _host_data.refreshCardList( 0 );
                _host_data.refreshCardList( 1 );
                _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_BATTLE, "ButtlePhase" );
            }
		} else if ( _mode == PROGRAM_MODE.MODE_ONE_CONNECT ) {
            // 1Pにカード配布
            if ( _host_data.getRecvData( ).card_list_one.Length == 0 ) {
		        for ( int i = 0; i < _dice_value[ 0 ]; i++ ) {
			        // デッキのカード数が０になったらリフレッシュ
			        if ( _card_manager.getDeckCardNum( ) <= 0 ) {
				        _card_manager.createDeck( );
			        }
                    card_list.Add( _card_manager.distributeCard( ).id );
		        }
                _host_data.setSendCardlist( ( int )PLAYER_ORDER.PLAYER_ONE, card_list );
            }
            
            // ドローカード処理を追加

            // 準備が終わったら次のフェイズへ
            if ( _client_data[ 0 ].getRecvData( ).ready == true ) {
                _host_data.refreshCardList( 0 );
                _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_BATTLE, "ButtlePhase" );
            }
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			// 準備が終わったら次のフェイズへ
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				_phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_BATTLE, "ButtlePhase" );
			}
		}
	}

	/// <summary>
	/// ButtlePhaseの更新
	/// </summary>
	private void updateButtlePhase( ) {
		if ( _mode == PROGRAM_MODE.MODE_TWO_CONNECT ) {
            if ( _client_data[ 0 ].getRecvData( ).battle_ready == true &&
				_client_data[ 1 ].getRecvData( ).battle_ready == true )  {
				// 1Pのステータスを設定
				_player_manager.setPlayerPower( 0, _client_data[ 0 ].getRecvData( ).player_status );
				for ( int i = 0; i < _client_data[ 0 ].getRecvData( ).used_card_list.Length; i++ ) {
					_player_manager.playCard( _card_manager.getCardData( _client_data[ 0 ].getRecvData( ).used_card_list[ i ] ) );
				}
				_player_manager.endStatus( 0 );
				Debug.Log( "1Pのpower:" + _player_manager.getPlayerPower( )[ 0 ].ToString( ) );
				// プラスバリューの初期化
				_player_manager.plusValueInit( );

				// 2Pのステータスを設定
				_player_manager.setPlayerPower( 1, _client_data[ 1 ].getRecvData( ).player_status );
				for ( int i = 0; i < _client_data[ 1 ].getRecvData( ).used_card_list.Length; i++ ) {
					_player_manager.playCard( _card_manager.getCardData( _client_data[ 1 ].getRecvData( ).used_card_list[ i ] ) );
				}
				_player_manager.endStatus( 1 );
				Debug.Log( "2Pのpower:" + _player_manager.getPlayerPower( )[ 1 ].ToString( ) );
				// プラスバリューの初期化
				_player_manager.plusValueInit( );

                // 攻撃力を比較
				_player_manager.attackTopAndLowestPlayer( _player_manager.getPlayerPower( ) );
                // 次のフェイズへ
                _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_RESULT, "ResultPhase" );
            }
		} else if ( _mode == PROGRAM_MODE.MODE_ONE_CONNECT ) {
			if ( _client_data[ 0 ].getRecvData( ).battle_ready == true )  {
				// 1Pのステータスを設定
				_player_manager.setPlayerPower( 0, _client_data[ 0 ].getRecvData( ).player_status );
				for ( int i = 0; i < _client_data[ 0 ].getRecvData( ).used_card_list.Length; i++ ) {
					_player_manager.playCard( _card_manager.getCardData( _client_data[ 0 ].getRecvData( ).used_card_list[ i ] ) );
				}
				_player_manager.endStatus( 0 );
				Debug.Log( "1Pのpower:" + _player_manager.getPlayerPower( )[ 0 ].ToString( ) );
				// プラスバリューの初期化
				_player_manager.plusValueInit( );

				// 2Pのステータスを設定
				_player_manager.setPlayerPower( 1, _client_data[ 0 ].getRecvData( ).player_status );
				for ( int i = 0; i < _client_data[ 0 ].getRecvData( ).used_card_list.Length; i++ ) {
					_player_manager.playCard( _card_manager.getCardData( _client_data[ 0 ].getRecvData( ).used_card_list[ i ] ) );
				}
				_player_manager.endStatus( 1 );
				Debug.Log( "2Pのpower:" + _player_manager.getPlayerPower( )[ 1 ].ToString( ) );
				// プラスバリューの初期化
				_player_manager.plusValueInit( );

                // 攻撃力を比較
				_player_manager.attackTopAndLowestPlayer( _player_manager.getPlayerPower( ) );
                // 次のフェイズへ
                _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_RESULT, "ResultPhase" );
            }
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			if ( Input.GetKeyDown( KeyCode.A ) )  {
				// 次のフェイズへ
				_phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_RESULT, "ResultPhase" );
			}
		}
	}

	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
        if (_mode != PROGRAM_MODE.MODE_NO_CONNECT)
        {
            // 戦闘結果を送信
            if (_host_data.getRecvData().send_result == false)
            {
                _host_data.setSendBattleResult(_player_manager.getPlayerResult(0), _player_manager.getPlayerResult(1), true);
            }
        }

		if ( _mode == PROGRAM_MODE.MODE_TWO_CONNECT ) {
            if ( _client_data[ 0 ].getRecvData( ).ready == true &&
                 _client_data[ 1 ].getRecvData( ).ready == true )  {
                if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.ADVANCE &&
                     _player_manager.isPlayerMoveStart( 0 ) == false ) {
                    // 1Pを前に動かす
		            _player_manager.setPlayerID( 0 );
		            _player_manager.setLimitValue( 1 );
		            _player_manager.setAdvanceFlag( true );
					_event_count[ 0 ] = 0;
                } else if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.BACK &&
                     _player_manager.isPlayerMoveStart( 0 ) == false ) {
                    // 1Pを後ろに動かす
		            _player_manager.setPlayerID( 0 );
		            _player_manager.setLimitValue( 1 );
		            _player_manager.setAdvanceFlag( false );
					_event_count[ 0 ] = 0;
                } else if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.NO_ADJUST &&
                     _player_manager.isPlayerMoveStart( 0 ) == false ) {
                    // 1Pを動かさない
		            _player_manager.setMoveFinish( 0, true );
                    _player_manager.setMoveStart( 0, true );
                } else if ( _client_data[ 1 ].getRecvData( ).mass_adjust == MASS_ADJUST.ADVANCE &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
                    // 2Pを前に動かす
		            _player_manager.setPlayerID( 1 );
		            _player_manager.setLimitValue( 1 );
		            _player_manager.setAdvanceFlag( true );
					_event_count[ 1 ] = 0;
                } else if ( _client_data[ 1 ].getRecvData( ).mass_adjust == MASS_ADJUST.BACK &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
                    // 2Pを後ろに動かす
		            _player_manager.setPlayerID( 1 );
		            _player_manager.setLimitValue( 1 );
		            _player_manager.setAdvanceFlag( false );
					_event_count[ 1 ] = 0;
                } else if ( _client_data[ 1 ].getRecvData( ).mass_adjust == MASS_ADJUST.NO_ADJUST &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true  ) {
                    // 2Pを動かさない
		            _player_manager.setMoveFinish( 1, true );
                    _player_manager.setMoveStart( 1, true );
                }
            }
		} else if ( _mode == PROGRAM_MODE.MODE_ONE_CONNECT ) {
            if ( _client_data[ 0 ].getRecvData( ).ready == true )  {
                if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.ADVANCE &&
                     _player_manager.isPlayerMoveStart( 0 ) == false ) {
                    // 1Pを前に動かす
		            _player_manager.setPlayerID( 0 );
		            _player_manager.setLimitValue( 1 );
		            _player_manager.setAdvanceFlag( true );
                    _event_count[ 0 ] = 0;
                } else if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.BACK &&
                     _player_manager.isPlayerMoveStart( 0 ) == false ) {
                    // 1Pを後ろに動かす
		            _player_manager.setPlayerID( 0 );
		            _player_manager.setLimitValue( 1 );
		            _player_manager.setAdvanceFlag( false );
					_event_count[ 0 ] = 0;
                } else if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.NO_ADJUST &&
                     _player_manager.isPlayerMoveStart( 0 ) == false ) {
                    // 1Pを動かさない
                    _player_manager.setMoveStart( 0, true );
                    _player_manager.setMoveFinish( 0, true );
                } else if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.ADVANCE &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
                    // 2Pを前に動かす
		            _player_manager.setPlayerID( 1 );
		            _player_manager.setLimitValue( 1 );
		            _player_manager.setAdvanceFlag( true );
					_event_count[ 1 ] = 0;
                } else if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.BACK &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
                    // 2Pを後ろに動かす
		            _player_manager.setPlayerID( 1 );
		            _player_manager.setLimitValue( 1 );
		            _player_manager.setAdvanceFlag( false );
					_event_count[ 1 ] = 0;
                } else if ( _client_data[ 0 ].getRecvData( ).mass_adjust == MASS_ADJUST.NO_ADJUST &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
                    // 2Pを動かさない
                    _player_manager.setMoveStart( 1, true );
                    //_player_manager.setMoveFinish( 1, true );
                }
            }
		} else if ( _mode == PROGRAM_MODE.MODE_NO_CONNECT ) {
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				MASS_ADJUST[ ] adjust = new MASS_ADJUST[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
				for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
					adjust[ i ] = ( MASS_ADJUST )( ( int )Random.Range( 0.0f, 3.0f ) );
				}

				if ( adjust[ 0 ] == MASS_ADJUST.ADVANCE &&
					_player_manager.isPlayerMoveStart( 0 ) == false ) {
					// 1Pを前に動かす
					_player_manager.setPlayerID( 0 );
					_player_manager.setLimitValue( 1 );
					_player_manager.setAdvanceFlag( true );
					_event_count[ 0 ] = 0;
				} else if ( adjust[ 0 ] == MASS_ADJUST.BACK &&
					_player_manager.isPlayerMoveStart( 0 ) == false ) {
					// 1Pを後ろに動かす
					_player_manager.setPlayerID( 0 );
					_player_manager.setLimitValue( 1 );
					_player_manager.setAdvanceFlag( false );
					_event_count[ 0 ] = 0;
				} else if ( adjust[ 0 ] == MASS_ADJUST.NO_ADJUST &&
					_player_manager.isPlayerMoveStart( 0 ) == false ) {
					// 1Pを動かさない
					_player_manager.setMoveFinish( 0, true );
					_player_manager.setMoveStart( 0, true );
				} else if ( adjust[ 1 ] == MASS_ADJUST.ADVANCE &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
					// 2Pを前に動かす
					_player_manager.setPlayerID( 1 );
					_player_manager.setLimitValue( 1 );
					_player_manager.setAdvanceFlag( true );
					_event_count[ 1 ] = 0;
				} else if ( adjust[ 1 ] == MASS_ADJUST.BACK &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
					// 2Pを後ろに動かす
					_player_manager.setPlayerID( 1 );
					_player_manager.setLimitValue( 1 );
					_player_manager.setAdvanceFlag( false );
					_event_count[ 1 ] = 0;
				} else if ( adjust[ 1 ] == MASS_ADJUST.NO_ADJUST &&
					_player_manager.isPlayerMoveStart( 1 ) == false && _player_manager.isPlayerMoveFinish( 0 ) == true ) {
					// 2Pを動かさない
					_player_manager.setMoveFinish( 1, true );
					_player_manager.setMoveStart( 1, true );
				}
			}
		}
        if ( _player_manager.getPlayerID( ) > -1 ) {
            _player_manager.movePhaseUpdate(getResideCount( ), _stage_manager.getTargetMass( _player_manager.getTargetMassID( _stage_manager.getMassCount( ) ) ) );
        }
        // ゴールまでの残りマスを表示
		resideCount( );

        // 両方の移動が終わったら次のフェイズへ
        if ( _player_manager.isPlayerMoveFinish( 0 ) == true && _player_manager.isPlayerMoveFinish( 1 ) == true ) {
			if ( _mode != PROGRAM_MODE.MODE_NO_CONNECT ) {
				_host_data.setSendBattleResult( BATTLE_RESULT.BATTLE_RESULT_NONE, BATTLE_RESULT.BATTLE_RESULT_NONE, false );
			}
            _phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_EVENT, "EventPhase" );
            _player_manager.movedRefresh( );
        }
	}

	/// <summary>
	/// EventPhaseの更新
	/// </summary>
	private void updateEventPhase( ) {
		if ( _player_manager.isEventStart( 0 ) == false ) {
			massEvent( _player_manager.getPlayerCount( 0, _stage_manager.getMassCount( ) ), 0 );
		} else if ( _player_manager.isEventFinish( 0 ) == true && _player_manager.isEventStart( 1 ) == false ) {
			massEvent (_player_manager.getPlayerCount( 1, _stage_manager.getMassCount( ) ), 1 );
		}

		// マス移動終了時にイベントフラグをfalseにしてもう一度イベントが発生するようにする
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
            if( _player_manager.getEventType( i ) == EVENT_TYPE.EVENT_MOVE ){
			    if ( _player_manager.isPlayerMoveFinish( i ) == true ) {
				    _player_manager.setEventStart( i, false );
				    _player_manager.movedRefresh( );
			    }
            }

		}
        if (_player_manager.getPlayerID() > -1) {
            _player_manager.movePhaseUpdate( getResideCount( ), _stage_manager.getTargetMass( _player_manager.getTargetMassID( _stage_manager.getMassCount( ) ) ) );
        }

		if ( _player_manager.isEventFinish( 0 ) == true && _player_manager.isEventFinish( 1 ) == true && _goal_flag == false) {
			_player_manager.setEventStart( 0, false );
			_player_manager.setEventStart( 1, false );
			_player_manager.setEventFinish( 0, false );
			_player_manager.setEventFinish( 1, false );
            if ( _mode != PROGRAM_MODE.MODE_NO_CONNECT ) {
                _host_data.refreshCardList( 0 );
                _host_data.refreshCardList( 1 );
            }
			_phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_DICE, "DisePhase" );
		}
	}

	/// <summary>
	/// マスイベントの処理
	/// </summary>
	/// <param name="i">The index.</param>
	public void massEvent( int i, int id ) {
		_player_manager.setEventStart( id, true );
		switch ( _file_manager.getFileData( ).mass[ i ].type ) {
		case "draw":
			int value = _file_manager.getMassValue( i )[ 0 ];
			List< int > card_list = new List< int >( );
			Debug.Log( "カード" + value + "ドロー" );
			for ( int j = 0; j < value; j++ ) {
				// デッキのカード数が０になったらリフレッシュ
				if ( _card_manager.getDeckCardNum( ) <= 0 ) {
					_card_manager.createDeck( );
				}
				card_list.Add( _card_manager.distributeCard( ).id );
			}
            if ( _mode != PROGRAM_MODE.MODE_NO_CONNECT ) {
			    _host_data.setSendCardlist( id, card_list );
            }
			// カードリストを初期化
			card_list.Clear( );
			_player_manager.setEventFinish( id, true );
            _player_manager.setEventType( id, EVENT_TYPE.EVENT_DRAW );
			break;
		case "trap1":
			Debug.Log ("トラップ発動");
			Debug.Log ("カード" + _file_manager.getMassValue( i )[ 1 ] + "捨てる");
			Debug.Log (_file_manager.getMassValue( i )[ 0 ] + "マス進む");
			_player_manager.setPlayerID( id );
			_player_manager.setAdvanceFlag( true );
			_player_manager.setLimitValue( _file_manager.getMassValue( i )[ 0 ] );
            _player_manager.setEventType( id, EVENT_TYPE.EVENT_MOVE );
			break;
		case "trap2":
			Debug.Log( "トラップ発動");
			Debug.Log( "カード"+_file_manager.getMassValue( i )[ 0 ] + "ドロー");
			Debug.Log( _file_manager.getMassValue( i )[ 1 ] + "マス戻る" );
			_player_manager.setPlayerID( id );
			_player_manager.setAdvanceFlag( false );
			_player_manager.setLimitValue( _file_manager.getMassValue( i )[ 1 ] );
            _player_manager.setEventType( id, EVENT_TYPE.EVENT_MOVE );
			break;
		case "advance":
			Debug.Log(_file_manager.getMassValue( i )[ 0 ] + "マス進む" );
			_player_manager.setPlayerID( id );
			_player_manager.setAdvanceFlag( true );
			_player_manager.setLimitValue( _file_manager.getMassValue( i )[ 0 ] );
            _player_manager.setEventType( id, EVENT_TYPE.EVENT_MOVE );
			break;
		case "event":
			Debug.Log( "イベント発生!!" );
			_player_manager.setEventFinish( id, true );
            _player_manager.setEventType( id, EVENT_TYPE.EVENT_ACTION );
			break;
		case "goal":
			if( _player_manager.getPlayerResult( id ) == BATTLE_RESULT.WIN ){
				_phase_manager.changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_FINISH, "FinishPhase" );
				Debug.Log( "プレイヤー" + ( id + 1 ) +":Goal!!" );
				_goal_flag = true;
				_player_manager.setEventFinish( id, true );
                _player_manager.setEventType( id, EVENT_TYPE.EVENT_GOAL );
			} else if ( _player_manager.getPlayerResult( id ) == BATTLE_RESULT.LOSE || _player_manager.getPlayerResult( id ) == BATTLE_RESULT.DRAW ) {
				_player_manager.setPlayerID( id );
				_player_manager.setAdvanceFlag ( false );
				_player_manager.setLimitValue( 1 );
                _player_manager.setEventType( id, EVENT_TYPE.EVENT_MOVE );
			}
			break;
		case "selectDraw":
			int cardType = _file_manager.getCardID( i );
			_card_manager.getCardData( cardType );
			break;
		case "Buff":
			int buffValue = _file_manager.getMassValue( i )[ 0 ];
			Debug.Log( "プレイヤーのパラメーターを" + buffValue.ToString( ) + "上昇" );
			break;
		case "MoveSeal":
			Debug.Log( "行動停止" );
			_player_manager.setPlayerOnMove( id, false );
			_player_manager.setEventFinish( id, true );
			_player_manager.setEventType( id, EVENT_TYPE.EVENT_DRAW );
			break;
		}  
	}

	/// <summary>
	/// FinishPhaseの更新
	/// </summary>
	private void updateFinishPhase( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_FINISH;
			_scene_text.text = "SCENEFINISH";
			if ( _mode != PROGRAM_MODE.MODE_NO_CONNECT ) {
				try {
					_host_data.setSendScene( _scene );
					_host_data.setSendChangeFieldScene( true );
				} catch {
					Debug.Log( "通信に失敗しまいました" );
				}
			}
		}
	}

	public void OnGUI( ) {
		if ( _host_data != null && _scene == SCENE.SCENE_CONNECT ) {
			drawConnectScene( );
		}
	}

	/// <summary>
	/// ConnectSceneの描画
	/// </summary>
	private void drawConnectScene( ) {
		if ( _mode != PROGRAM_MODE.MODE_NO_CONNECT ) {
			if ( !_network_manager.isConnected( ) && _host_data.getServerState( ) != SERVER_STATE.STATE_HOST ) {
				//_network_manager.noConnectDraw( );
			}

			if ( _host_data.getServerState( ) == SERVER_STATE.STATE_HOST ) {
				_network_manager.hostStateDraw( );
			}
		}
	}

	/// <summary>
	/// シーン情報を返す
	/// </summary>
	/// <returns>The scene.</returns>
	public SCENE getScene( ) {
		return _scene;
	}


	/// <summary>
	/// プレイヤーの現在位置（環境）
	/// </summary>
	/// <param name="environment"></param>
	/// <param name="num"></param>
	public void playerEnvironment( string environment, int num ) {
		_environment[ num ].text = "プレイヤー" + ( num + 1 ) + ":" + environment;
	}

	/// <summary>
	/// ゴールまでの残りマスを表示
	/// </summary>
	public void resideCount( ) {
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			_reside_text[ i ].text = "プレイヤー" + i.ToString( ) + "：残り" + getResideCount( )[ i ].ToString( ) + "マス";
		}
	}
    
    /// <summary>
    /// ゴールまでどれくらい残っているか取得
    /// </summary>
    /// <returns></returns>
    public int[ ] getResideCount( ) {
		int[ ] count = new int[ 2 ];
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			count[ i ] = _file_manager.getMassCount( ) - 1 - _player_manager.getPlayerCount( i, _stage_manager.getMassCount( ) );
		}
		return count;
    }

	public void setEventCount( int id, int count ) {
		_event_count[ id ] = count;
	}

}
