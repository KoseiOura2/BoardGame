using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Common;

public class HostData : NetworkBehaviour {

	[ SerializeField ]
	private SERVER_STATE _server_state = SERVER_STATE.STATE_NONE;

    // networkdata
	[ SyncVar ]
    public int _network_scene_data;
	[ SyncVar ]
    public int _network_phase_data;
	[ SyncVar ]
    public bool _network_change_scene;

    private NETWORK_FIELD_DATA _field_data;

    void Awake( ) {
        _network_scene_data = 0;
        _network_phase_data = 0;
        _network_change_scene = false;

        _field_data.scene = ( SCENE )_network_scene_data;
        _field_data.main_game_phase = ( MAIN_GAME_PHASE )_network_phase_data;
        _field_data.change_scene = _network_change_scene;
    }

	// Use this for initialization
	void Start( ) {
		if ( isLocalPlayer == true ) {
			_server_state = SERVER_STATE.STATE_HOST;
			this.gameObject.tag = "HostObj";
		} else {
			_server_state = SERVER_STATE.STATE_CLIANT;
			this.gameObject.tag = "ClientObj";
		}
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    /// <summary>
    /// scenedataのセット
    /// </summary>
	/// <param name="data"></param>
	[ Server ]
    public void setSendScene( SCENE data ) {
        if ( isLocalPlayer ) {
            _field_data.scene = data;
            _network_scene_data = ( int )data;
        }
    }

	/// <summary>
	/// phasedataのセット
	/// </summary>
	/// <param name="data"></param>
	[ Server ]
	public void setSendGamePhase( MAIN_GAME_PHASE data ) {
		if ( isLocalPlayer ) {
			_field_data.main_game_phase = data;
            _network_phase_data = ( int )data;
		}
	}

    /// <summary>
    /// シーンが変化したかどうかを設定
    /// </summary>
	/// <param name="flag"></param>
	[ Server ]
    public void setSendChangeFieldScene( bool flag ) { 
        _field_data.change_scene = flag;
        _network_change_scene = flag;
    }

	[ Client ]
	public NETWORK_FIELD_DATA getRecvData( ) {
		return _field_data;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns><c>true</c>, if change field scene was ised, <c>false</c> otherwise.</returns>
	[ Client ]
    public bool isChangeFieldScene( ) {
        if ( _network_change_scene == true ) {
            _field_data.scene = ( SCENE )_network_scene_data;
            return true;
        }

        return false;
    }

	public bool isLocal( ) {

		return isLocalPlayer;
	}

	public SERVER_STATE getServerState( ) {
		return _server_state;
	}
}
