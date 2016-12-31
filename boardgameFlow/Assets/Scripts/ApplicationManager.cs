using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class ApplicationManager : MonoBehaviour {


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
    private NetworkGUIControll _network_gui_controll;
    [ SerializeField ]
    private HostData _host_data;
    [ SerializeField ]
    private ClientData _client_data;
    
	[ SerializeField ]
	private SCENE _scene = SCENE.SCENE_CONNECT;
	public Text _scene_text;
    private int _event_count = 0;        //イベントを起こす回数   

    void Awake( ) {
        if ( isError( ) ) {
            return;
        }
        DontDestroyOnLoad( this.gameObject );

        _player_manager.init( _file_manager.getMassCoordinate( 0 ) );
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
			_network_gui_controll = GameObject.Find( "NetworkManager" ).GetComponent< NetworkGUIControll >( );
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
        
		if ( _client_data == null && _network_manager.getClientObj( 0 ) != null ) {
			_client_data = _network_manager.getClientObj( 0 ).GetComponent< ClientData >( );
		}

		// デバッグ
        /*
		if ( _network_data != null && !_network_data.isLocal( ) ) {
			_scene = _network_data.getRecvData( ).scene;
			_phase_manager.setPhase(_network_data.getRecvData ().main_game_phase);
		}
        */

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

		if ( _host_data != null && _client_data != null ) {
			if ( _client_data.getRecvData( ).changed_scene == true ) {
				_host_data.setSendChangeFieldScene( false );
				Debug.Log( "ok" );
			}
 		}
	}

	/// <summary>
	/// ConnectSceneの更新
	/// </summary>
	private void updateConnectScene( ) {
		if ( _network_manager.isConnected( ) ||  Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_TITLE;
			_scene_text.text = "SCENE_TITLE";
			_host_data.setSendScene( _scene );
            _host_data.setSendChangeFieldScene( true );
		}
	}

	/// <summary>
	/// TitleSceneの更新
	/// </summary>
	private void updateTitleScene( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_GAME;
			_scene_text.text = "SCENE_GAME";
			_host_data.setSendScene( _scene );
            _host_data.setSendChangeFieldScene( true );
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
			_host_data.setSendScene( _scene );
            _host_data.setSendChangeFieldScene( true );
		}
	}

	/// <summary>
	/// GameSceneの更新
	/// </summary>
	private void updateGameScene( ) {
		// フェイズチェンジ
		_phase_manager.changeMainGamePhase( );
		// 通信データのセット
		if ( _phase_manager.isPhaseChanged( ) ) {
			_host_data.setSendGamePhase( _phase_manager.getMainGamePhase( ) );
		}

		// フェイズごとの更新
		switch( _phase_manager.getMainGamePhase( ) ) {
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

        // playerの環境情報を更新
		for ( int i = 0; i < _player_manager.getPlayerNum( ); i++ ) {
			if ( _file_manager.getEnvironment( _player_manager.getPlayerCount( i ) ) != "" ) {
				string environment = _file_manager.getEnvironment ( _player_manager.getPlayerCount( i ) );
				_player_manager.playerEnvironment( environment, i );
			}
		}
	}

	/// <summary>
	/// DicePhaseの更新
	/// </summary>
	private void updateDicePhase( ) {
		// ダイスを振ったら(通信)
		if ( Input.GetMouseButtonDown( 0 ) ) {
			int sai = 3;	// 送られてきた賽の目の数
			for ( int i = 0; i < sai; i++ ) {
				// デッキのカード数が０になったらリフレッシュ
				if ( _card_manager.getDeckCardNum( ) <= 0 ) {
					_card_manager.createDeck( );
				}
				Debug.Log( _card_manager.distributeCard( ) );
			}
		}
	}

	/// <summary>
	/// MovePhaseの更新
	/// </summary>
	private void updateMovePhase( ) {
		_player_manager.movePhaseUpdate( getResideCount( _player_manager.getPlayerID( ) ) );
	}

	/// <summary>
	/// BuffPhaseの更新
	/// </summary>
	private void updateDrawPhase( ) {
		
	}

	/// <summary>
	/// GimmickPhaseの更新
	/// </summary>
	private void updateButtlePhase( ) {
		
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
		eventPhaseUpdate( );
	}
    
    public void eventPhaseUpdate( ) {
		Debug.Log( "マスイベント！" );
		if (_event_count < 2) {
			_stage_manager.massEvent( _player_manager.getPlayerCount( _player_manager.getPlayerID( ) ) );
			_event_count++;
		}
    }

	/// <summary>
	/// FinishPhaseの更新
	/// </summary>
	private void updateFinishPhase( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_FINISH;
			_scene_text.text = "SCENEFINISH";
			_host_data.setSendScene( _scene );
            _host_data.setSendChangeFieldScene( true );
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

    /// <summary>
    /// ゴールまでどれくらい残っているか取得
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public int getResideCount( int i ) {
        return _file_manager.getMassCount( ) - 1 - _player_manager.getPlayerCount( i );
    }

}
