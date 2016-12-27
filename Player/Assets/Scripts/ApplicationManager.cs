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
    [SerializeField]
    private ClientData _client_data;

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
	void Update( ) {
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
        // シーンの切り替え
        if ( _host_data != null && _client_data != null ) {
            sceneChange( );
            if ( _client_data.getRecvData( ).changed_scene == true && _host_data.getRecvData( ).change_scene == false ) {
                _client_data.CmdSetSendChangedScene( false );
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
    
    void sceneChange( ) { 
        if ( _host_data.isChangeFieldScene( ) ) {
            Debug.Log( _host_data.getRecvData( ).scene.ToString( ) );
            _scene = _host_data.getRecvData( ).scene;
            _client_data.CmdSetSendChangedScene( true );
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

	}

	/// <summary>
	/// DicePhaseの更新
	/// </summary>
	private void updateDicePhase( ) {

	}

	/// <summary>
	/// BuffPhaseの更新
	/// </summary>
	private void updateBuffPhase( ) {
		
	}

	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
		
	}

	/// <summary>
	/// MovePhaseの更新
	/// </summary>
	private void updateMovePhase( ) {
		
	}

	/// <summary>
	/// GimmickPhaseの更新
	/// </summary>
	private void updateGimmickPhase( ) {
		
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
