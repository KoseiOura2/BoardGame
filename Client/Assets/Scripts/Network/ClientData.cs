using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Common;

public class ClientData : NetworkBehaviour {

	[ SerializeField ]
	private SERVER_STATE _server_state = SERVER_STATE.STATE_NONE;
    
	[ SerializeField ]
	private NETWORK_PLAYER_DATA _player_data;
    
    public int MAX_USE_CARD_NUM = 6;

    private bool _change_satatus = false;

    void Awake( ) {
		_player_data.changed_scene    = false;
        _player_data.changed_phase    = false;
        _player_data.dice_value       = -1;
        _player_data.ready            = false;
		_player_data.used_card_list   = new int[ MAX_USE_CARD_NUM ];
		_player_data.turned_card_list = new int[ MAX_USE_CARD_NUM ];
        _player_data.player_power     = 0;
        _player_data.hand_num         = 0;
        _player_data.battle_ready     = false;
        _player_data.mass_adjust      = MASS_ADJUST.NO_ADJUST;
        _player_data.ok_event         = false;
        _player_data.connect_ready    = false;
    }

	// Use this for initialization
	void Start( ) {
		if ( isLocalPlayer == true ) {
			_server_state = SERVER_STATE.STATE_CLIANT;
		} else {
			_server_state = SERVER_STATE.STATE_HOST;
		}
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}
    
    /// <summary>
    /// 通信準備完了かどうかを送る
    /// </summary>
    /// <param name="flag"></param>
	[ Command ]
    public void CmdSetSendConnectReady( bool flag ) { 
		_player_data.connect_ready = flag;
    }

    /// <summary>
    /// シーンが変化したかどうかを設定
    /// </summary>
    /// <param name="flag"></param>
	[ Command ]
    public void CmdSetSendChangedScene( bool flag ) { 
		_player_data.changed_scene = flag;
    }
    
	[ Client ]
    public void setChangedScene( bool flag ) { 
		_player_data.changed_scene = flag;
    }
    
    /// <summary>
    /// フェイズが変化したかどうかを設定
    /// </summary>
    /// <param name="flag"></param>
	[ Command ]
    public void CmdSetSendChangedPhase( bool flag ) { 
		_player_data.changed_phase = flag;

    }
    
	[ Client ]
    public void setChangedPhase( bool flag ) { 
		_player_data.changed_phase = flag;

    }
    
	[ Command ]
	public void CmdSetSendStatus( int player_power, int hand_num ) { 
        _player_data.player_power = player_power;
        _player_data.hand_num     = hand_num;
    }
    
	[ Client ]
	public void setStatus( int player_power, int hand_num ) { 
        _player_data.player_power = player_power;
        _player_data.hand_num     = hand_num;

        _change_satatus = false;
    }

    /// <summary>
    /// さいの目を送る
    /// </summary>
    /// <param name="value"></param>
	[ Command ]
    public void CmdSetSendDiceValue( int value ) { 
		_player_data.dice_value = value;

    }
    
	[ Client ]
    public void setDiceValue( int value ) { 
		_player_data.dice_value = value;

    }
    
    /// <summary>
    /// 準備完了を送信
    /// </summary>
    /// <param name="flag"></param>
	[ Command ]
    public void CmdSetSendReady( bool flag ) { 
		_player_data.ready = flag;
    }
    
	[ Client ]
    public void setReady( bool flag ) { 
		_player_data.ready = flag;
    }
    
    /// <summary>
    /// 準備完了を送信
    /// </summary>
    /// <param name="flag"></param>
	[ Command ]
    public void CmdSetSendBattleReady( bool flag ) { 
		_player_data.battle_ready = flag;
    }
    
	[ Client ]
    public void setBattleReady( bool flag ) { 
		_player_data.battle_ready = flag;
    }

    /// <summary>
    /// 戦闘結果を送信
    /// </summary>
    /// <param name="ready"></param>
    /// <param name="status"></param>
    /// <param name="card_list"></param>
	[ Command ]
	public void CmdSetSendBattleData( bool ready, int player_power, int[ ] used_card_list, int[ ] turned_card_list ) { 
        _player_data.player_power = player_power;
		for ( int i = 0; i < used_card_list.Length; i++ ) {
			_player_data.used_card_list[ i ] = used_card_list[ i ];
		}
		for ( int i = 0; i < turned_card_list.Length; i++ ) {
			_player_data.turned_card_list[ i ] = turned_card_list[ i ];
		}
		_player_data.battle_ready  = ready;
    }
    
	[ Client ]
	public void setBattleData( bool ready, int player_power, int[ ] used_card_list, int[ ] turned_card_list ) { 
        _player_data.player_power = player_power;
		for ( int i = 0; i < used_card_list.Length; i++ ) {
			_player_data.used_card_list[ i ] = used_card_list[ i ];
		}
		for ( int i = 0; i < turned_card_list.Length; i++ ) {
			_player_data.turned_card_list[ i ] = turned_card_list[ i ];
		}
		_player_data.battle_ready  = ready;
    }
    
    /// <summary>
    /// マス調整の結果を送る
    /// </summary>
    /// <param name="ready"></param>
    /// <param name="advance"></param>
	[ Command ]
    public void CmdSetSendMassAdjust( bool ready, MASS_ADJUST adjust ) { 
        _player_data.ready = ready;
        _player_data.mass_adjust = adjust;
    }
    
	[ Client ]
    public void setMassAdjust( bool ready, MASS_ADJUST adjust ) { 
        _player_data.ready = ready;
        _player_data.mass_adjust = adjust;
    }
    
	[ Command ]
    public void CmdSetSendOkEvent( bool ok ) { 
        _player_data.ok_event = ok;
    }
    
	[ Client ]
    public void setOkEvent( bool ok ) { 
        _player_data.ok_event = ok;
    }

	public NETWORK_PLAYER_DATA getRecvData( ) {
		return _player_data;
	}

    /// <summary>
    /// シーンが変化したかどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool isChangeFieldScene( ) {
		if ( _player_data.changed_scene == true ) {
			_player_data.changed_scene = false;
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// フェイズが変化したかどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool isChangeFieldPhase( ) {
		if ( _player_data.changed_phase == true ) {
			_player_data.changed_phase = false;
            return true;
        }

        return false;
    }

    public bool isChangeStatus( ) {
        return _change_satatus;
    }

	public bool isLocal( ) {

		return isLocalPlayer;
	}

	public SERVER_STATE getServerState( ) {
		return _server_state;
	}
}
