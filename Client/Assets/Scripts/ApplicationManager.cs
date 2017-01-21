using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;

public class ApplicationManager : MonoBehaviour {

	enum PROGRAM_MODE {
		MODE_NO_CONNECT,
		MODE_CONNECT,
	};

	enum GAME_PLAY_MODE {
		MODE_NORMAL_PLAY,
		MODE_PLAYER_SELECT,
	};

	private const int MAX_DRAW_NUM = 4;

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
	private NetworkGUIControll _network_gui_controll;
	[ SerializeField ]
	private HostData _host_data;
	[ SerializeField ]
	private ClientData _client_data;

	[SerializeField]
	private PLAYER_ORDER _player_num = PLAYER_ORDER.NO_PLAYER;
	[SerializeField]
	private PROGRAM_MODE _mode = PROGRAM_MODE.MODE_NO_CONNECT;
	[SerializeField]
	private GAME_PLAY_MODE _play_mode = GAME_PLAY_MODE.MODE_NORMAL_PLAY;

	public Text _scene_text;

	void Awake( ) {
		DontDestroyOnLoad( this.gameObject );
	}

	// Use this for initialization
	void Start( ) {
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
				_player_manager = GameObject.Find( "ClientPlayerManager" ).GetComponent< ClientPlayerManager >( );
			}
			if ( _network_gui_controll == null ) {
				_network_gui_controll = GameObject.Find( "NetworkManager" ).GetComponent< NetworkGUIControll >( );
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
				_host_data = _network_manager.getHostObj ( ).GetComponent<HostData> ( );
			}

			if ( _client_data == null && _network_manager.getClientObj( ) != null ) {
				_client_data = _network_manager.getClientObj( ).GetComponent< ClientData >( );
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
				}
			}
		} else {
			_player_num = PLAYER_ORDER.PLAYER_ONE;
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
		if ( _host_data.isChangeFieldScene( ) ) {
			_scene = _host_data.getRecvData( ).scene;
			_scene_text.text = _scene.ToString( );
			_client_data.CmdSetSendChangedScene( true );
			_client_data.setChangedScene( true );
		}
	}

	/// <summary>
	/// ConnectSceneの更新
	/// </summary>
	private void updateConnectScene( ) {

	}

	/// <summary>
	/// TitleSceneの更新
	/// </summary>
	private void updateTitleScene( ) {

	}

	/// <summary>
	/// FinishSceneの更新
	/// </summary>
	private void updateFinishScene( ) {

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

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _host_data != null && _client_data != null ) {
				// フェイズの切り替え
				phaseChange( );

				// 切り替え完了を送る
				if ( _client_data.getRecvData( ).changed_phase == true && _host_data.getRecvData( ).change_phase == false ) {
					_client_data.CmdSetSendChangedPhase( false );
					_client_data.setChangedPhase( false );
				}
			}
		}
	}

	/// <summary>
	/// フェイズの切り替え
	/// </summary>
	private void phaseChange( ) { 
		if ( _host_data.isChangeFieldPhase( ) ) {
			try {
				_phase_manager.setPhase( _host_data.getRecvData( ).main_game_phase );
			}
			catch {
				Debug.Log( "ゲームフェイズの取得に失敗しまし" );
			}
			_client_data.CmdSetSendChangedPhase( true );
			_client_data.setChangedPhase( true );
		}
	}

	/// <summary>
	/// NoPlayPhaseの更新
	/// </summary>
	private void updateNoPlayPhase( ) {
		
	}

	/// <summary>
	/// DicePhaseの更新
	/// </summary>
	private void updateDicePhase( ) {
		int value = 0;
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			// ダイスの目を決定
			_player_manager.dicisionDiceValue( );
			value = _player_manager.getDiceValue( );

			// サーバーにダイスの目を送信
			if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
				_client_data.CmdSetSendDiceValue( value );
				_client_data.setDiceValue( value );
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
		}
	}

	/// <summary>
	/// DrawPhaseの更新
	/// </summary>
	private void updateDrawPhase( ) {
		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			// カードデータを受診したら
			if ( _host_data.getCardListNum( _player_num ) == MAX_DRAW_NUM - _player_manager.getDiceValue( ) &&
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
				_player_manager.updateAllPlayerCard( );

				// サーバーに準備完了を送信
				_client_data.CmdSetSendReady( true );
				_client_data.setReady( true );
			}
		}
	}

	/// <summary>
	/// ButtlePhaseの更新
	/// </summary>
	private void updateButtlePhase( ) {
		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( _client_data.getRecvData( ).ready == true ) {
				// 準備完了を初期化
				_client_data.CmdSetSendReady( false );
				_client_data.setReady( false );
			}
		}

		if ( _mode == PROGRAM_MODE.MODE_CONNECT ) {
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				// 選択結果を送る
				int player_status = _player_manager.getPlayerData( ).power;
				int[ ] card_list = _player_manager.dicisionSelectCard( );
				int[ ] turned_card_list = new int[ ]{ 0, 1, 2 };

				Debug.Log( player_status.ToString( ) );
				_client_data.CmdSetSendBattleData( true, player_status, card_list, turned_card_list );
				_client_data.setBattleData( true, player_status, card_list, turned_card_list );
                _player_manager.refreshSelectCard( );
			}
		}
	}

	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
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
			} else if ( Input.GetKeyDown( KeyCode.B ) ) {
				_phase_manager.setPhase( MAIN_GAME_PHASE.GAME_PHASE_FINISH );
			}
		} 
	}

	/// <summary>
	/// FinishPhaseの更新
	/// </summary>
	private void updateFinishPhase( ) {

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