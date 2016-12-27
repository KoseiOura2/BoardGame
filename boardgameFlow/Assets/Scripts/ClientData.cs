using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Common;

public class ClientData : NetworkBehaviour {

	[ SerializeField ]
	private SERVER_STATE _server_state = SERVER_STATE.STATE_NONE;

	private NETWORK_PLAYER_DATA _player_data;

    void Awake( ) {
		_player_data.changed_scene = false;
    }

	// Use this for initialization
	void Start( ) {
		if ( isLocalPlayer == true ) {
			_server_state = SERVER_STATE.STATE_HOST;
		} else {
			_server_state = SERVER_STATE.STATE_CLIANT;
		}
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    /// <summary>
    /// シーンが変化したかどうかを設定
    /// </summary>
    /// <param name="flag"></param>
	[ Command ]
    public void CmdSetSendChangedScene( bool flag ) { 
		_player_data.changed_scene = flag;

    }

	public NETWORK_PLAYER_DATA getRecvData( ) {
		return _player_data;
	}

    public bool isChangeFieldScene( ) {
		if ( _player_data.changed_scene == true ) {
			_player_data.changed_scene = false;
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
