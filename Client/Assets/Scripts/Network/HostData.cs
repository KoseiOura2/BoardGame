using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Common;

public class HostData : NetworkBehaviour {

    [ SerializeField ]
    private SERVER_STATE _server_state = SERVER_STATE.STATE_NONE;
    private bool[ ] _connect = new bool[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];

    // networkdata
    [ SyncVar ]
	public int _network_scene_data;
	[ SyncVar ]
    public int _network_phase_data;

    public SyncListInt _network_card_list_0 = new SyncListInt ( );
    public SyncListInt _network_card_list_1 = new SyncListInt ( );

	[ SyncVar ]
	public int _network_player_num;
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
	[ SyncVar ]
	public bool _network_send_card_one;
	[ SyncVar ]
	public bool _network_send_card_two;
	[ SyncVar ]
	public int _network_mass_count_one;
	[ SyncVar ]
	public int _network_mass_count_two;

    private NETWORK_FIELD_DATA _field_data;

    public int DISTRIBUT_CARD_NUM = 3;

    void Awake ( ) {
        for ( int i = 0; i < _connect.Length; i++ ) {
            _connect[ i ] = false;
        }

        _network_player_num        = -1;
        _network_scene_data        = 0;
        _network_phase_data        = 0;
        _network_change_scene      = false;
        _network_change_phase      = false;
        _network_battle_result_one = 0;
        _network_battle_result_two = 0;
        _network_send_result       = false;
		_network_send_card_one     = false;
		_network_send_card_two     = false;
        _network_mass_count_one    = 0;
        _network_mass_count_two    = 0;

        _field_data.player_num        = _network_player_num;
        _field_data.scene             = ( SCENE )_network_scene_data;
        _field_data.main_game_phase   = ( MAIN_GAME_PHASE )_network_phase_data;
        _field_data.change_scene      = _network_change_scene;
        _field_data.change_phase      = _network_change_phase;
        _field_data.card_list_one     = new int[ DISTRIBUT_CARD_NUM ];
        _field_data.card_list_two     = new int[ DISTRIBUT_CARD_NUM ];
        _field_data.result_player_one = ( BATTLE_RESULT )_network_battle_result_one;
        _field_data.result_player_two = ( BATTLE_RESULT )_network_battle_result_two;
        _field_data.send_result       = _network_send_result;
		_field_data.send_card         = new bool[ ]{ _network_send_card_one, _network_send_card_two };
        _field_data.mass_count        = new int[ ]{ _network_mass_count_one, _network_mass_count_two };
    }

    // Use this for initialization
    void Start ( ) {
        if ( isLocalPlayer == true ) {
            _server_state = SERVER_STATE.STATE_CLIANT;
            this.gameObject.tag = "ClientObj";
        } else {
            _server_state = SERVER_STATE.STATE_HOST;
            this.gameObject.tag = "HostObj";
        }
    }

    // Update is called once per frame
    void Update ( ) {

    }

    [ Server ]
    public void send( ) {
        if ( isLocalPlayer ) {
            // 共通事項
            if ( _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] && _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] ) {
                _network_scene_data        = ( int )_field_data.scene;
                _network_phase_data        = ( int )_field_data.main_game_phase;
                _network_change_scene      = _field_data.change_scene;
                _network_change_phase      = _field_data.change_phase;
                _network_battle_result_one = ( int )_field_data.result_player_one;
                _network_battle_result_two = ( int )_field_data.result_player_two;
                _network_send_result       = _field_data.send_result;
            }

            // 1Pに送る
            if ( _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] ) {
                _network_send_card_one  = _field_data.send_card[ 0 ];
                _network_mass_count_two = _field_data.mass_count[ 0 ];

                if ( _field_data.send_card[ 0 ] == true ) {
                    for ( int i = 0; i < _field_data.card_list_one.Length; i++ ) {
                        _network_card_list_0.Add( _field_data.card_list_one[ i ] );
                    }
                    _field_data.send_card[ 0 ] = false;
                }
            }

            // 2Pに送る
            if ( _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] ) {
				_network_send_card_two  = _field_data.send_card[ 1 ];
                _network_mass_count_two = _field_data.mass_count[ 1 ];
                
                if ( _field_data.send_card[ 1 ] == true ) {
                    for ( int i = 0; i < _field_data.card_list_two.Length; i++ ) {
                        _network_card_list_1.Add( _field_data.card_list_two[ i ] );
                    }
                    _field_data.send_card[ 1 ] = false;
                }
            }

            _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] = false;
            _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] = false;
        }
    }

    /// <summary>
    /// 新しいプレイヤー接続時、プレイヤー番号を一つ繰り上げ
    /// </summary>
    [ Server ]
    public void increasePlayerNum ( ) {
        if ( isLocalPlayer ) {
            _field_data.player_num++;
            _network_player_num++;
        }
    }

    /// <summary>
    /// scenedataのセット
    /// </summary>
	/// <param name="data"></param>
	[ Server ]
    public void setSendScene ( SCENE data ) {
        if ( isLocalPlayer ) {
            _field_data.scene = data;
            _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] = true;
            _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] = true;
        }
    }

    /// <summary>
    /// phasedataのセット
    /// </summary>
    /// <param name="data"></param>
    [ Server ]
    public void setSendGamePhase ( MAIN_GAME_PHASE data ) {
        if ( isLocalPlayer ) {
            _field_data.main_game_phase = data;
            
            _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] = true;
            _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] = true;
        }
    }

    /// <summary>
    /// シーンが変化したかどうかを設定
    /// </summary>
	/// <param name="flag"></param>
	[ Server ]
    public void setSendChangeFieldScene ( bool flag ) {
        _field_data.change_scene = flag;
            
        _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] = true;
        _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] = true;
    }

    /// <summary>
    /// フェイズが変化したかどうかを設定
    /// </summary>
    /// <param name="flag"></param>
    [ Server ]
    public void setSendChangeFieldPhase ( bool flag ) {
        _field_data.change_phase = flag;
            
        _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] = true;
        _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] = true;
    }

    /// <summary>
    /// 配布するカードの設定
    /// </summary>
    /// <param name="player_num"></param>
    /// <param name="card_list"></param>
	[ Server ]
    public void setSendCardlist ( int player_num, List<int> card_list ) {
        for ( int i = 0; i < card_list.Count; i++ ) {
            if ( player_num == ( int )PLAYER_ORDER.PLAYER_ONE ) {
                _field_data.card_list_one[ i ] = card_list[ i ];
				_field_data.send_card[ player_num ] = true;

                _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] = true;
            } else if ( player_num == ( int )PLAYER_ORDER.PLAYER_TWO ) {
                _field_data.card_list_two[ i ] = card_list[ i ];
				_field_data.send_card[ player_num ] = true;

                _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] = true;
            }
        }
    }

    [ Server ]
    public void refreshCardList ( int player_num ) {
        if ( player_num == ( int )PLAYER_ORDER.PLAYER_ONE ) {
            for ( int i = 0; i < _field_data.card_list_one.Length; i++ ) {
                _field_data.card_list_one[ i ] = -1;
            }
			_network_card_list_0.Clear( );
			_field_data.send_card[ player_num ] = false;
			_network_send_card_one = false;
        } else if ( player_num == ( int )PLAYER_ORDER.PLAYER_TWO ) {
            for ( int i = 0; i < _field_data.card_list_one.Length; i++ ) {
                _field_data.card_list_two[ i ] = -1;
            }
			_network_card_list_1.Clear ( );
			_field_data.send_card[ player_num ] = false;
			_network_send_card_two = false;
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

        _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] = true;
        _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] = true;
    }
    
    /// <summary>
    /// 現在のマスを送る
    /// </summary>
    /// <param name="player_num"></param>
    /// <param name="count"></param>
	[ Server ]
    public void setSendMassCount( PLAYER_ORDER player_num, int count ) {
        if ( player_num == PLAYER_ORDER.PLAYER_ONE ) {
            _field_data.mass_count[ 0 ] = count;

            _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] = true;
        } else if ( player_num == PLAYER_ORDER.PLAYER_TWO ) {
            _field_data.mass_count[ 1 ] = count;
            
            _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] = true;
        }
    }

    [ Client ]
    public NETWORK_FIELD_DATA getRecvData ( ) {
        _field_data.player_num        = _network_player_num;
        _field_data.scene             = ( SCENE )_network_scene_data;
        _field_data.main_game_phase   = ( MAIN_GAME_PHASE )_network_phase_data;
        _field_data.change_scene      = _network_change_scene;
        _field_data.change_phase      = _network_change_phase;
        _field_data.result_player_one = ( BATTLE_RESULT )_network_battle_result_one;
        _field_data.result_player_two = ( BATTLE_RESULT )_network_battle_result_two;
        _field_data.send_result       = _network_send_result;
		_field_data.send_card[ 0 ]    = _network_send_card_one;
		_field_data.send_card[ 1 ]    = _network_send_card_two;
        _field_data.mass_count[ 0 ]   = _network_mass_count_one;
        _field_data.mass_count[ 1 ]   = _network_mass_count_two;
        
		for ( int i = 0; i < _network_card_list_0.Count; i++ ) {
            _field_data.card_list_one[ i ] = _network_card_list_0[ i ];
        }
        for ( int i = 0; i < _network_card_list_1.Count; i++ ) {
            _field_data.card_list_two[ i ] = _network_card_list_1[ i ];
        }

        return _field_data;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns><c>true</c>, if change field scene was ised, <c>false</c> otherwise.</returns>
    [ Client ]
    public bool isChangeFieldScene ( ) {
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
		
	[ Client ]
	public int[ ] getCardList( PLAYER_ORDER player_num ) {
		int[ ] card_list = new int[ DISTRIBUT_CARD_NUM ];

		if ( player_num == PLAYER_ORDER.PLAYER_ONE ) {
			for ( int i = 0; i < _network_card_list_0.Count; i++ ) {
				card_list[ i ] = _network_card_list_0[ i ];
				_field_data.card_list_one[ i ] = _network_card_list_0[ i ];
			}
		} else if ( player_num == PLAYER_ORDER.PLAYER_TWO ) {
			for ( int i = 0; i < _network_card_list_1.Count; i++ ) {
				card_list[ i ] = _network_card_list_1[ i ];
				_field_data.card_list_two[ i ] = _network_card_list_1[ i ];
			}
		}

		return card_list;
	}

	[ Client ]
	public int getCardListNum( PLAYER_ORDER player_num ) {
		int num = 0;

		if ( player_num == PLAYER_ORDER.PLAYER_ONE ) {
			num = _network_card_list_0.Count;
		} else if ( player_num == PLAYER_ORDER.PLAYER_TWO ) {
			num = _network_card_list_1.Count;
		}

		return num;
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