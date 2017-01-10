﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Common;

public class HostData : NetworkBehaviour {

	[ SerializeField ]
	private SERVER_STATE _server_state = SERVER_STATE.STATE_NONE;

    // networkdata
	[ SyncVar ]
    public int _network_scene_data;
	[ SyncVar ]
    public int _network_phase_data;

    public SyncListInt _network_card_list_0 = new SyncListInt( );
    public SyncListInt _network_card_list_1 = new SyncListInt( );

	[ SyncVar ]
    public bool _network_change_scene;
	[ SyncVar ]
    public bool _network_change_phase;
	[ SyncVar ]
    public int _network_battle_result_one;
	[ SyncVar ]
    public int _network_battle_result_two;
	[ SyncVar ]
    public bool _network_send_result;

    private NETWORK_FIELD_DATA _field_data;

    public int DISTRIBUT_CARD_NUM = 3;

    void Awake( ) {
        _network_scene_data = 0;
        _network_phase_data = 0;
        _network_change_scene = false;
        _network_change_phase = false;
        _network_battle_result_one = 0;
        _network_battle_result_two = 0;
        _network_send_result = false;

        _field_data.scene = ( SCENE )_network_scene_data;
        _field_data.main_game_phase = ( MAIN_GAME_PHASE )_network_phase_data;
        _field_data.change_scene = _network_change_scene;
        _field_data.change_phase = _network_change_phase;
        _field_data.card_list_one = new int[ DISTRIBUT_CARD_NUM ];
        _field_data.card_list_two = new int[ DISTRIBUT_CARD_NUM ];
        _field_data.result_player_one = ( BATTLE_RESULT )_network_battle_result_one;
        _field_data.result_player_two = ( BATTLE_RESULT )_network_battle_result_two;
        _field_data.send_result = _network_send_result;
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
    
    /// <summary>
    /// フェイズが変化したかどうかを設定
    /// </summary>
	/// <param name="flag"></param>
	[ Server ]
    public void setSendChangeFieldPhase( bool flag ) { 
        _field_data.change_phase = flag;
        _network_change_phase = flag;
    }

    /// <summary>
    /// 配布するカードの設定
    /// </summary>
    /// <param name="player_num"></param>
    /// <param name="card_list"></param>
	[ Server ]
    public void setSendCardlist( int player_num, List< int > card_list ) { 
        for ( int i = 0; i < card_list.Count; i++ ) {
            if ( player_num == ( int )PLAYER_ORDER.PLAYER_ONE ) {
                _field_data.card_list_one[ i ] = card_list[ i ];
                _network_card_list_0.Add( card_list[ i ] );
            } else if ( player_num == ( int )PLAYER_ORDER.PLAYER_TWO ) {
                _field_data.card_list_two[ i ] = card_list[ i ];
                _network_card_list_1.Add( card_list[ i ] );
            }
        }
    }
    
	[ Server ]
    public void refreshCardList( int player_num ) { 
        if ( player_num == ( int )PLAYER_ORDER.PLAYER_ONE ) {
            for ( int i = 0; i < _field_data.card_list_one.Length; i++ ) {
                _field_data.card_list_one[ i ] = -1;
            }
            _network_card_list_0.Clear( );
        } else if ( player_num == ( int )PLAYER_ORDER.PLAYER_TWO ) {
            for ( int i = 0; i < _field_data.card_list_one.Length; i++ ) {
                _field_data.card_list_two[ i ] = -1;
            }
            _network_card_list_1.Clear( );
        }
    }

    /// <summary>
    /// 戦闘結果を送る
    /// </summary>
    /// <param name="result_one"></param>
    /// <param name="result_two"></param>
    /// <param name="result"></param>
	[ Server ]
    public void setSendBattleResult( BATTLE_RESULT result_one, BATTLE_RESULT result_two, bool result ) { 
        _field_data.result_player_one = result_one;
        _field_data.result_player_two = result_two;
        _field_data.send_result = result;
        _network_battle_result_one = ( int )result_one;
        _network_battle_result_two = ( int )result_two;
        _network_send_result = result;
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

	/// <summary>
	/// 
	/// </summary>
	/// <returns><c>true</c>, if change field scene was ised, <c>false</c> otherwise.</returns>
	[ Client ]
    public bool isChangeFieldPhase( ) {
        if ( _network_change_phase == true ) {
            _field_data.main_game_phase = ( MAIN_GAME_PHASE )_network_phase_data;
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

    public int getBattleResultOne( ) {
        return _network_battle_result_one;
    }
    
    public int getBattleResultTwo( ) {
        return _network_battle_result_two;
    }
}
