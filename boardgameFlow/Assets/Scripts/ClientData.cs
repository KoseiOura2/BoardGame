using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Common;

public class ClientData : NetworkBehaviour {

	[ SyncVar ]
    private NETWORK_FIELD_DATA _field_data;

    void Awake( ) {
        _field_data.scene = SCENE.SCENE_CONNECT;
        _field_data.main_game_phase = MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY;
        _field_data.change_scene = false;
    }

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    /// <summary>
    /// scenedataのセット
    /// </summary>
    /// <param name="data"></param>
    public void setSendScene( SCENE data ) {
        if ( isLocalPlayer ) {
            _field_data.scene = data;
        }
    }

	/// <summary>
	/// phasedataのセット
	/// </summary>
	/// <param name="data"></param>
	public void setSendGamePhase( MAIN_GAME_PHASE data ) {
		if ( isLocalPlayer ) {
			_field_data.main_game_phase = data;
		}
	}

    /// <summary>
    /// シーンが変化したかどうかを設定
    /// </summary>
    /// <param name="flag"></param>
    public void setSendChangeFieldScene( bool flag ) { 
        _field_data.change_scene = flag;
    }

	public NETWORK_FIELD_DATA getRecvData( ) {
		
		return _field_data;
	}

    public bool isChangeFieldScene( ) {
        if ( _field_data.change_scene == true ) {
            _field_data.change_scene = false;
            return true;
        }

        return false;
    }

	public bool isLocal( ) {

		return isLocalPlayer;
	}
}
