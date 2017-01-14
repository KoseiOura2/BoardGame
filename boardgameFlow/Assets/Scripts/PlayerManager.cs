using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common;

public class PlayerManager : MonoBehaviour {
    
    public float ADJUST_FIRST_PLAYER_Y_POS = 0.3f;          // プレイヤー初期生成時の修正Y座標
    public float ADJUST_FIRST_PLAYER_Z_POS = 0.1f;          // プレイヤー初期生成時の修正Z座標

    [ SerializeField ]
    private PLAYER_ORDER _player_order;     // どのプレイヤーが行動中か
	private PLAYER_DATA[ ] _players = new PLAYER_DATA[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];

    private Vector3 _start_position;        //現在位置を設定
	private Vector3 _end_position;          //到達位置を設定
	private GameObject[ ] _player_pref = new GameObject[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //プレイヤーのモデルをロード
    private GameObject _target;             //進む先のターゲットを設定
	private GameObject _firstest_player;
	private GameObject _latest_player;
	private GameObject _winner_player;
	private GameObject _loser_player;
	[ SerializeField ]
    private int _player_id     = -1;    //動かすプレイヤーID設定
    [ SerializeField ]
	private int _limit_value   = -1;    //進むマス数設定
	[ SerializeField ]
	private int _defalut_draw = 0;
	[ SerializeField ]
	private int _defalut_power = 0;
	private int _plus_draw;
	private int _plus_power;
    private float _time = 1;
    private float _startTime;
	[ SerializeField ]
    private bool _move_flag    = false;     //動かす時のフラグが立っているか
	[ SerializeField ]
    private bool[ ] _move_start = new bool[ ]{ false, false };
	[ SerializeField ]
    private bool[ ] _move_finish = new bool[ ]{ false, false };
	[ SerializeField ]
	private bool[ ] _event_start = new bool[ ]{ false, false };
	[ SerializeField ]
	private bool[ ] _event_finish = new bool[ ]{ false, false };
    private bool _advance_flag = true;   	//前に進むか後ろに戻るか

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="first_pos"></param>
    public void init( Vector3 first_pos ) {

		_player_pref[ 0 ] = ( GameObject )Resources.Load( "Prefabs/Player1" );
		_player_pref[ 1 ] = ( GameObject )Resources.Load( "Prefabs/Player2" );

        createObj( first_pos );
        // playerオブジェクトの色替え
        
        _players[ 0 ].obj.GetComponent< Renderer>( ).material.color = Color.magenta;
        _players[ 1 ].obj.GetComponent< Renderer>( ).material.color = Color.green;

		// ステータス値の初期化
		setDefalutStatus( );
		plusValueInit( );

    }
    
    /// <summary>
    /// ゲーム開始時プレイヤーを生成
    /// </summary>
    public void createObj( Vector3 first_pos ) {
        _player_order = PLAYER_ORDER.PLAYER_ONE;

        for( int i = 0; i < _player_pref.Length; i++ ) {
			// 位置の決定
            first_pos.y = ADJUST_FIRST_PLAYER_Y_POS;
            switch ( _player_order ) {
                case PLAYER_ORDER.PLAYER_ONE:
                    first_pos.z += ADJUST_FIRST_PLAYER_Z_POS;
                    _player_order = PLAYER_ORDER.PLAYER_TWO;
                    break;
                case PLAYER_ORDER.PLAYER_TWO:
                    first_pos.z -= ADJUST_FIRST_PLAYER_Z_POS;
                    _player_order = PLAYER_ORDER.NO_PLAYER;
                    break;
            }

			_players[ i ].obj = ( GameObject )Instantiate( _player_pref[ i ], first_pos, Quaternion.identity );
            _players[ i ].obj.transform.parent = transform;
            _players[ i ].obj.name = "Player" + i;
            _players[ i ].event_type = EVENT_TYPE.EVENT_NONE;
			_players[ i ].onMove = true;
        }
    }

	/// <summary>
	/// プレイヤーのステータスを初期値へ戻す
	/// </summary>
	public void setDefalutStatus( ) {
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			_players[ i ].draw = _defalut_draw;
			_players[ i ].power = _defalut_power;
		}
	}

	/// <summary>
	/// 強化値を初期化
	/// </summary>
	public void plusValueInit( ) {
		_plus_power = 0;
		_plus_draw = 0;
	}

	// Use this for initialization
	void Start( ) {
	
	}

	// Update is called once per frame
	void Update( ) {
	
	}
    
    /// //////////////////////////////
    // MovePhaseの更新
    public void movePhaseUpdate(int[] count, GameObject target_pos) {
        dicisionTopAndLowestPlayer(ref count);
        if ( _player_id > -1 ) {
            _move_start[ _player_id ] = true;
            if ( _limit_value > 0 ) {
                if ( !_move_flag ) {
                    setTargetPos( ref target_pos );
                } else {
                    playerMove( );
                }
            } else if ( _limit_value == 0 ) {
                Debug.Log("aaa");
                _move_finish[ _player_id ] = true;
                _limit_value--;
                _player_id = -1;
            }
        } else {
			_player_id = -1;
			_target = null;
		}
	}
    ///////////////////////////////////

	/// <summary>
	/// ターゲットの設定
	/// </summary>
	/// <param name="count">Count.</param>
	/// <param name="advance_pos">Advance position.</param>
	/// <param name="back_pos">Back position.</param>
	private void setTargetPos( ref GameObject target_pos ) {
        if ( _time <= 0 ) {
			_players[ _player_id ].obj.transform.position = _end_position;
			_player_id = -1;
            _target = null;
            return;
        }

        _startTime = Time.timeSinceLevelLoad;
		_start_position = _players[ _player_id ].obj.transform.position;
		_target = target_pos;
        _end_position = _target.transform.localPosition;
        _end_position.y += 0.3f;
        _move_flag = true;
    }

	/// <summary>
	/// プレイヤーを動かす処理
	/// </summary>
    private void playerMove( ) {
        var diff = Time.timeSinceLevelLoad - _startTime;
		if ( diff > _time ) {
			_players[ _player_id ].obj.transform.position = _end_position;
            _limit_value--;
			if ( _advance_flag ) {
				_players[ _player_id ].advance_count++;
			} else {
				_players[ _player_id ].advance_count--;
			}
            _move_flag = false;
            _target    = null;
        }

		var rate = diff / _time;

		_players[ _player_id ].obj.transform.position = Vector3.Lerp ( _start_position, _end_position, rate );
    }
    
	/// <summary>
	/// ランク付け関数
	/// </summary>
	public void dicisionTopAndLowestPlayer( ref int[ ] count ) {
		if( !Mathf.Approximately( count[ 0 ], count[ 1 ] ) ) {
			float first = Mathf.Min( count[ 0 ], count[ 1 ] );
			if( first == count[ 0 ] ) {
				_firstest_player	= _players[ 0 ].obj;
				_latest_player = _players[ 1 ].obj;
				_players[ 0 ].rank = PLAYER_RANK.RANK_FIRST;
				_players[ 1 ].rank = PLAYER_RANK.RANK_SECOND;
			} else if( first == count[ 1 ] ) {
				_firstest_player	= _players[ 1 ].obj;
				_latest_player = _players[ 0 ].obj;
				_players[ 0 ].rank = PLAYER_RANK.RANK_SECOND;
				_players[ 1 ].rank = PLAYER_RANK.RANK_FIRST;
			}
		} else {
			_players[ 0 ].rank = PLAYER_RANK.RANK_FIRST;
			_players[ 1 ].rank = PLAYER_RANK.RANK_SECOND;
			_firstest_player	= _players[ 0 ].obj;
			_latest_player = _players[ 1 ].obj;
		}
        /*
		Debug.Log( "プレイヤー1ランク:" + _players[ 0 ].rank );
		Debug.Log( "プレイヤー2ランク:" + _players[ 1 ].rank );
        */
	}
    
	/// <summary>
	/// 攻撃力比較用関数
	/// </summary>
	public void attackTopAndLowestPlayer( int[ ] attack ){
		if( !Mathf.Approximately( attack[ 0 ], attack[ 1 ] ) ) {
			float winner = Mathf.Max( attack[ 0 ], attack[ 1 ] );
			if( winner == attack[ 0 ] ) {
				_winner_player = _players[ 0 ].obj;
				_loser_player = _players[ 1 ].obj;
				_players[ 0 ].battle_result = BATTLE_RESULT.WIN;
				_players[ 1 ].battle_result = BATTLE_RESULT.LOSE;
			} else if( winner == attack[ 1 ] ) {
				_winner_player = _players[ 1 ].obj;
				_loser_player = _players[ 0 ].obj;
				_players[ 1 ].battle_result = BATTLE_RESULT.WIN;
				_players[ 0 ].battle_result = BATTLE_RESULT.LOSE;
			} 
		} else {
			_players[ 0 ].battle_result = BATTLE_RESULT.DRAW;
			_players[ 1 ].battle_result = BATTLE_RESULT.DRAW;
		}
	}

	/// <summary>
	/// カード効果適応
	/// </summary>
	/// <param name="card">Card.</param>
	public void playCard( CARD_DATA data ) {
		switch ( data.enchant_type ) {
		case "enhance":
			addPower( data.enchant_value );
			Debug.Log( "強化効果" + data.enchant_value );
			Debug.Log( "power" + _plus_power );
			break;
		case "turn":
			addPower( data.enchant_value );
			Debug.Log( "強化効果" + data.enchant_value );
			Debug.Log( "power" + _plus_power );
			break;
		case "special":
			specialEnhance( data );
			Debug.Log( "スペシャル効果" );
			break;
		case "demerit":
			addPower( -data.enchant_value );
			Debug.Log( "power" + _plus_power );
			Debug.Log( "デメリット効果" + data.enchant_value );
			break;
		}
	}

	/// <summary>
	/// Adds the power.
	/// </summary>
	/// <param name="enhance">Enhance.</param>
	private void addPower( int enhance ) {
		_plus_power += enhance;
	}

	/// <summary>
	/// スペシャルタイプのカード効果
	/// </summary>
	/// <param name="data">Data.</param>
	private void specialEnhance( CARD_DATA data ) {
		if ( data.special_value == ( int )SPECIAL_LIST.ENHANCE_TYPE_DRAW ) {
			_plus_draw += data.enchant_value;
		}
	}

	public void endStatus( int id ) {
		_players[ id ].draw  = _plus_draw;
		_players[ id ].power = _plus_power;
	}

    /// <summary>
    /// プレイヤーがどれくらい進んでいるかを取得
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
	public int getPlayerCount( int i, int length ) {
		if ( i >= 0 ) {
			if( _players[ i ].advance_count < length - 1 ) {
				return _players[ i ].advance_count;
			} else {
				return length - 1;
			}
        }
        return 0;
    }

    /// <summary>
    /// playeridを取得
    /// </summary>
    /// <returns></returns>
    public int getPlayerID( ) {
		return _player_id;
    }

	public bool isEventStart( int id ){
		return _event_start[ id ];
	}

	public bool isEventFinish( int id ){
		return _event_finish[ id ];
	}

	//各プレイヤーの攻撃力を取得
	public int[ ] getPlayerPower( ) {
		int[ ] power = new int[ 2 ];
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			power[ i ] = _players[ i ].power;
		}
		return power;
	}

	public bool getPlayerOnMove( int playerID ) {
		return _players[ playerID ].onMove;
	}

	//ターゲットとなるマスIDを取得
	public int getTargetMassID( int length ) {
		if( _advance_flag ) {
			if( getPlayerCount( getPlayerID( ), length ) < length - 1 ) {
				return getPlayerCount( getPlayerID( ), length ) + 1;
			} else {
                _limit_value = 0;
				return getPlayerCount( getPlayerID( ), length );
			}
		} else {
			return getPlayerCount( getPlayerID( ), length ) - 1;
		}
	}

	//指定ランクプレイヤーのゲームオブジェクトを返す
	public GameObject getTopPlayer( PLAYER_RANK player_rank ) {
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			if ( player_rank == _players[ i ].rank ) {
				return _players[ i ].obj;
			} 
		}
		return null;
	}

	//最下位プレイヤーのゲームオブジェクトを返す
	public GameObject getLastPlayer( ) {
		return _latest_player;
	}

    public EVENT_TYPE getEventType( int id ) {
        return _players[ id ].event_type;
    }

    public bool isPlayerMoveFinish( int i ) {
        return _move_finish[ i ];
    }
    
    public bool isPlayerMoveStart( int i ) {
        return _move_start[ i ];
    }

    public BATTLE_RESULT getPlayerResult( int id ) {
        return _players[ id ].battle_result;
    }

    public void refreshPlayerResult( ) {
        for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
            _players[ i ].battle_result = BATTLE_RESULT.BATTLE_RESULT_NONE;
        }
    }

    public void movedRefresh( ) {
        for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
            _move_finish[ i ] = false;
            _move_start[ i ] = false;
        }
    }

    public void setMoveFinish( int id, bool flag ) {
        _move_finish[ id ] = flag;
    }

    public void setMoveStart( int id, bool flag ) {
		_move_start[ id ] = flag;
	}

	public void setAdvanceFlag( bool flag ) {
		_advance_flag = flag;
	}

	public void setLimitValue( int value ) {
		_limit_value = value;
	}

	public void setPlayerID( int id ) {
		_player_id = id;
	}

	public void setPlayerPower( int id, int power ) {
		_players[ id ].power = power;
	}

	public void setEventStart( int id, bool flag ){
		_event_start[ id ] = flag;
	}

	public void setEventFinish( int id, bool flag ){
		_event_finish[ id ] = flag;
	}

    public void setEventType( int id, EVENT_TYPE event_type ) {
        _players[ id ].event_type = event_type;
    }

	public void setPlayerOnMove( int i, bool onMove ) {
		_players[ i ].onMove = onMove;
	}
}
