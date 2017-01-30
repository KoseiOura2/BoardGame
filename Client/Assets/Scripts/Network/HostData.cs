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

    // 1Pのカードリスト
    public SyncListInt _network_card_list_0   = new SyncListInt( );
    // 2Pのカードリスト
    public SyncListInt _network_card_list_1   = new SyncListInt( );
    // プレイヤーのステータスを送信したかどうか
	public SyncListBool _network_send_status  = new SyncListBool( );
    // プレイヤーのパワー
	public SyncListInt _network_player_power  = new SyncListInt( );
    // プレイヤーの手数
	public SyncListInt _network_hand_num      = new SyncListInt( );
    // プレイヤーの戦闘結果を送信したかどうか
	public SyncListInt _network_battle_result = new SyncListInt( );
    // プレイヤーのカードを送信したかどうか
	public SyncListBool _network_send_card    = new SyncListBool( );
    // プレイヤーの現在位置
	public SyncListInt _network_mass_count    = new SyncListInt( );
    // プレイヤーの起こしたイベント
	public SyncListInt _network_event_type    = new SyncListInt( );
    
    // プレイヤー番号
	[ SyncVar ]
	public int _network_player_num;
    // シーン切り替えしたかどうか
	[ SyncVar ]
	public bool _network_change_scene;
    // フェイズ切り替えしたかどうか
	[ SyncVar ]
	public bool _network_change_phase;
    // 勝敗結果を送ったかどうか
	[ SyncVar ]
	public bool _network_send_result;

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
        _network_send_result       = false;

        _field_data.player_num      = _network_player_num;
        _field_data.scene           = ( SCENE )_network_scene_data;
        _field_data.main_game_phase = ( MAIN_GAME_PHASE )_network_phase_data;
        _field_data.change_scene    = _network_change_scene;
        _field_data.change_phase    = _network_change_phase;
        _field_data.send_result     = _network_send_result;
        
    }

    void Start( ) {
        if ( isLocalPlayer == true ) {
            _server_state = SERVER_STATE.STATE_CLIANT;
            this.gameObject.tag = "ClientObj";
        } else {
            _server_state = SERVER_STATE.STATE_HOST;
            this.gameObject.tag = "HostObj";
            Debug.Log( "a" );
        }
    }

    // Use this for initialization
    public void init ( ) {

        // 配列の確保
        _field_data.card_list_one = new int[ DISTRIBUT_CARD_NUM ];
        _field_data.card_list_two = new int[ DISTRIBUT_CARD_NUM ];
        _field_data.send_status   = new bool[ _network_send_status.Count ];
        _field_data.player_power  = new int[ _network_player_power.Count ];
        _field_data.hand_num      = new int[ _network_hand_num.Count ];
        _field_data.result_player = new BATTLE_RESULT[ _network_battle_result.Count ];
		_field_data.send_card     = new bool[ _network_send_card.Count ];
        _field_data.mass_count    = new int[ _network_mass_count.Count ];
        _field_data.event_type    = new EVENT_TYPE[ _network_event_type.Count ];

        for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
            _field_data.send_status[ i ]   = _network_send_status[ i ];
            _field_data.player_power[ i ]  = _network_player_power[ i ]; 
            _field_data.hand_num[ i ]      = _network_hand_num[ i ];
            _field_data.result_player[ i ] = ( BATTLE_RESULT )_network_battle_result[ i ];
            _field_data.send_card[ i ]     = _network_send_card[ i ];
            _field_data.mass_count[ i ]    = _network_mass_count[ i ];
            _field_data.event_type[ i ]    = ( EVENT_TYPE )_network_event_type[ i ];
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
                _network_send_result       = _field_data.send_result;

                for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
                    _network_battle_result[ i ] = ( int )_field_data.result_player[ i ];
                }
            }

            // 1Pに送る
            if ( _connect[ ( int )PLAYER_ORDER.PLAYER_ONE ] ) {
                _network_send_status[ ( int )PLAYER_ORDER.PLAYER_ONE ]  = _field_data.send_status[ ( int )PLAYER_ORDER.PLAYER_ONE ];
                _network_player_power[ ( int )PLAYER_ORDER.PLAYER_ONE ] = _field_data.player_power[ ( int )PLAYER_ORDER.PLAYER_ONE ];
                _network_hand_num[ ( int )PLAYER_ORDER.PLAYER_ONE ]     = _field_data.hand_num[ ( int )PLAYER_ORDER.PLAYER_ONE ];
                _network_send_card[ ( int )PLAYER_ORDER.PLAYER_ONE ]    = _field_data.send_card[ ( int )PLAYER_ORDER.PLAYER_ONE ];
                _network_mass_count[ ( int )PLAYER_ORDER.PLAYER_ONE ]   = _field_data.mass_count[ ( int )PLAYER_ORDER.PLAYER_ONE ];
                _network_event_type[ ( int )PLAYER_ORDER.PLAYER_ONE ]   = ( int )_field_data.event_type[ ( int )PLAYER_ORDER.PLAYER_ONE ];

                if ( _field_data.send_card[ ( int )PLAYER_ORDER.PLAYER_ONE ] ) {
                    for ( int i = 0; i < _field_data.card_list_one.Length; i++ ) {
                        _network_card_list_0.Add( _field_data.card_list_one[ i ] );
                    }
                    _field_data.send_card[ ( int )PLAYER_ORDER.PLAYER_ONE ] = false;
                }
            }

            // 2Pに送る
            if ( _connect[ ( int )PLAYER_ORDER.PLAYER_TWO ] ) {
                _network_send_status[ ( int )PLAYER_ORDER.PLAYER_TWO ]  = _field_data.send_status[ ( int )PLAYER_ORDER.PLAYER_TWO ];
                _network_player_power[ ( int )PLAYER_ORDER.PLAYER_TWO ] = _field_data.player_power[ ( int )PLAYER_ORDER.PLAYER_TWO ];
                _network_hand_num[ ( int )PLAYER_ORDER.PLAYER_TWO ]     = _field_data.hand_num[ ( int )PLAYER_ORDER.PLAYER_TWO ];
                _network_send_card[ ( int )PLAYER_ORDER.PLAYER_TWO ]    = _field_data.send_card[ ( int )PLAYER_ORDER.PLAYER_TWO ];
                _network_mass_count[ ( int )PLAYER_ORDER.PLAYER_TWO ]   = _field_data.mass_count[ ( int )PLAYER_ORDER.PLAYER_TWO ];
                _network_event_type[ ( int )PLAYER_ORDER.PLAYER_TWO ]   = ( int )_field_data.event_type[ ( int )PLAYER_ORDER.PLAYER_TWO ];
                
                if ( _field_data.send_card[ ( int )PLAYER_ORDER.PLAYER_TWO ] ) {
                    for ( int i = 0; i < _field_data.card_list_two.Length; i++ ) {
                        _network_card_list_1.Add( _field_data.card_list_two[ i ] );
                    }
                    _field_data.send_card[ ( int )PLAYER_ORDER.PLAYER_TWO ] = false;
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
            } else if ( player_num == ( int )PLAYER_ORDER.PLAYER_TWO ) {
                _field_data.card_list_two[ i ] = card_list[ i ];
            }
        }
        _connect[ ( int )player_num ]       = true;
		_field_data.send_card[ player_num ] = true;
    }

    [ Server ]
    public void refreshCardList ( int player_num ) {
        if ( player_num == ( int )PLAYER_ORDER.PLAYER_ONE ) {
            for ( int i = 0; i < _field_data.card_list_one.Length; i++ ) {
                _field_data.card_list_one[ i ] = -1;
            }
			_network_card_list_0.Clear( );
        } else if ( player_num == ( int )PLAYER_ORDER.PLAYER_TWO ) {
            for ( int i = 0; i < _field_data.card_list_two.Length; i++ ) {
                _field_data.card_list_two[ i ] = -1;
            }
			_network_card_list_1.Clear ( );
        }
        
		_field_data.send_card[ player_num ] = false;
		_network_send_card[ player_num ]    = false;
    }
    
	[ Server ]
    public void setSendPlayerStatus( int player_num, int power, int hand_num, bool send ) {
        _field_data.player_power[ player_num ] = power;
        _field_data.hand_num[ player_num ]     = hand_num;
        _field_data.send_status[ player_num ]  = send;
        _connect[ player_num ] = true;
    }

    /// <summary>
    /// 戦闘結果を送る
    /// </summary>
    /// <param name="result"></param>
    /// <param name="result_flag"></param>
	[ Server ]
    public void setSendBattleResult( BATTLE_RESULT[ ] result, bool result_flag ) {
        for ( int i = 0; i < result.Length; i++ ) {
            _field_data.result_player[ i ] = result[ i ];
        }
        _field_data.send_result = result_flag;
        
        for ( int i = 0; i < _connect.Length; i++ ) {
            _connect[ i ] = true;
        }
    }
    
    /// <summary>
    /// 現在のマスを送る
    /// </summary>
    /// <param name="player_num"></param>
    /// <param name="count"></param>
	[ Server ]
    public void setSendMassCount( PLAYER_ORDER player_num, int count ) {
        _field_data.mass_count[ ( int )player_num ] = count;
        
        _connect[ ( int )player_num ] = true;
    }
    
	[ Server ]
    public void setSendEventType( PLAYER_ORDER player_num, EVENT_TYPE type ) {
        _field_data.event_type[ ( int )player_num ] = type;
        
        _connect[ ( int )player_num ] = true;
    }

    [ Client ]
    public NETWORK_FIELD_DATA getRecvData ( ) {
        _field_data.player_num        = _network_player_num;
        _field_data.scene             = ( SCENE )_network_scene_data;
        _field_data.main_game_phase   = ( MAIN_GAME_PHASE )_network_phase_data;
        _field_data.change_scene      = _network_change_scene;
        _field_data.change_phase      = _network_change_phase;
        _field_data.send_result       = _network_send_result;

        for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
            _field_data.send_status[ i ]   = _network_send_status[ i ];
            _field_data.player_power[ i ]  = _network_player_power[ i ];
            _field_data.hand_num[ i ]      = _network_hand_num[ i ];
            _field_data.result_player[ i ] = ( BATTLE_RESULT )_network_battle_result[ i ];
            _field_data.send_card[ i ]     = _network_send_card[ i ];
            _field_data.mass_count[ i ]    = _network_mass_count[ i ];
            _field_data.event_type[ i ]    = ( EVENT_TYPE )_network_event_type[ i ];
        }
        
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

    public int getBattleResult( int id ) {
        return _network_battle_result[ id ];
    }
}