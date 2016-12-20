using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Common;

public class NetworkData : NetworkBehaviour {

    [ SyncVar ]
    private NETWORK_FIELD_DATA _send_data;

    private NETWORK_PLAYER_DATA _recieve_data;

    void Awake( ) {
        _send_data.scene = SCENE.SCENE_CONNECT;
        _send_data.main_game_phase = MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY;
    }

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// scenedataのセット
    /// </summary>
    /// <param name="data"></param>
    public void setSendScene( SCENE data ) {
        _send_data.scene = data;
    }
}
