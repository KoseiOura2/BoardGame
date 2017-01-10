using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class ApplicationManager : MonoBehaviour {

	[ SerializeField ]
	private SCENE _scene = SCENE.SCENE_CONNECT;
	[ SerializeField ]
	private NetworkMNG _network_manager;
	[ SerializeField ]
	private PhaseManager _phase_manager;
	[ SerializeField ]
	private CardManager _card_manager;
    //[ SerializeField ]
    //private NetworkGUIControll _network_gui_controll;
    [ SerializeField ]
    private HostData _host_data;
    [ SerializeField ]
    private ClientData _client_data;

    [ SerializeField ]
    private int _player_num = 0;

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
			//_network_gui_controll = GameObject.Find( "NetworkManager" ).GetComponent< NetworkGUIControll >( );
		}
		catch {
			Debug.Log( "参照に失敗しました。" );
		}
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
		if ( _host_data == null && _network_manager.getHostObj( ) != null ) {
            _host_data = _network_manager.getHostObj( ).GetComponent< HostData >( );
		}

        if ( _client_data == null && _network_manager.getClientObj( ) != null ) {
            _client_data = _network_manager.getClientObj( ).GetComponent< ClientData >( );
        }

		// デバッグ
        /*
		if ( _network_data != null && !_network_data.isLocal( ) ) {
			_scene = _network_data.getRecvData( ).scene;
			_phase_manager.setPhase(_network_data.getRecvData ().main_game_phase);
		}
        */
        
        if ( _host_data != null && _client_data != null ) {
            // シーンの切り替え
            sceneChange( );
            // 切り替え完了を送る
            if ( _client_data.getRecvData( ).changed_scene == true && _host_data.getRecvData( ).change_scene == false ) {
                _client_data.CmdSetSendChangedScene( false );
                _client_data.setChangedScene( false );
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

    /// <summary>
    /// フェイズの切り替え
    /// </summary>
    private void phaseChange( ) { 
        if ( _host_data.isChangeFieldPhase( ) ) {
            _phase_manager.setPhase( _host_data.getRecvData( ).main_game_phase );
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
        int value = 1;
        // さいの目を決定する処理を加える
        if ( Input.GetKeyDown( KeyCode.A ) ) {
            _client_data.CmdSetSendDiceValue( value );
            _client_data.setDiceValue( value );
        }
	}
    
	/// <summary>
	/// MovePhaseの更新
	/// </summary>
	private void updateMovePhase( ) {
		if ( _client_data.getRecvData( ).dice_value > 0 ) {
            // さいの目を―1に初期化
            _client_data.CmdSetSendDiceValue( -1 );
            _client_data.setDiceValue( -1 );
        }
	}
    
	/// <summary>
	/// DrawPhaseの更新
	/// </summary>
	private void updateDrawPhase( ) {
        if ( _player_num == 0 ) {
		    if ( _host_data.getRecvData( ).card_list_0.Count > 0 ) {
                // 手札にカードを加える処理
            }
        }
        if ( Input.GetKeyDown( KeyCode.A ) ) {
            // 準備完了を送る
            _client_data.CmdSetSendReady( true );
            _client_data.setReady( true );
        }
	}
    
	/// <summary>
	/// ButtlePhaseの更新
	/// </summary>
	private void updateButtlePhase( ) {
		if ( _client_data.getRecvData( ).ready == true ) {
            // 準備完了を初期化
            _client_data.CmdSetSendReady( false );
            _client_data.setReady( false );
        }
	}
    
	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
		
	}
    
	/// <summary>
	/// EventPhaseの更新
	/// </summary>
	private void updateEventPhase( ) {

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
