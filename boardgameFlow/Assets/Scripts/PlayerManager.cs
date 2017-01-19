using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common;

public class PlayerManager : MonoBehaviour {
    
    public float ADJUST_FIRST_PLAYER_Y_POS = 0.3f;          // プレイヤー初期生成時の修正Y座標
    public float ADJUST_PLAYER_POS = 0.6f;          // プレイヤー初期生成時の修正Z座標

    [ SerializeField ]
    private PLAYER_ORDER _player_order;     // どのプレイヤーが行動中か
	private PLAYER_DATA[ ] _players = new PLAYER_DATA[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
    
    private Vector3[ ] _start_position = new Vector3[ 2 ];        //現在位置を設定
    [ SerializeField ]
    private Vector3[ ] _end_position = new Vector3[ 2 ];          //到達位置を設定
	private GameObject[ ] _player_pref = new GameObject[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //プレイヤーのモデルをロード
    [ SerializeField ]
    private GameObject[ ] _target = new GameObject[ 2 ];             //進む先のターゲットを設定
	private GameObject _firstest_player;
	private GameObject _latest_player;
	private GameObject _winner_player;
	private GameObject _loser_player;
	[ SerializeField ]
    private int _player_id     = -1;    //動かすプレイヤーID設定
    [ SerializeField ]
	private int _limit_value;    //進むマス数設定
    [ SerializeField ]
	private int _limit_value_adjustment;    //進むマス数設定
	[ SerializeField ]
	private int _defalut_draw = 0;
	[ SerializeField ]
	private int _defalut_power = 0;
	private int _plus_draw;
	private int _plus_power;
    private float _time = 0.3f;
    private Vector3 _velocity = Vector3.zero;
    private float[] _startTime = new float[2];
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
    [ SerializeField ]
    private bool _current_flag = false;
    
    private bool _accel_init = false;
    private float _accel = 0.0f;
    private float _speed = 0.0f;
    private float _first_speed = 0.0f;
    [ SerializeField ]
    private bool _adjustment_flag = false;

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
                    first_pos.z += ADJUST_PLAYER_POS;
                    _player_order = PLAYER_ORDER.PLAYER_TWO;
                    break;
                case PLAYER_ORDER.PLAYER_TWO:
                    first_pos.z -= ADJUST_PLAYER_POS;
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

    #if UNITY_EDITOR
    /// <summary>
    /// Unityエディタ上でのみデバッグ機能を有効
    /// </summary>
    void Update( ) {
    
        if ( Input.GetKeyDown( KeyCode.P ) ) {
            startBonusMode( 1, GAME_STAGE.BONUS );
        }
        if ( Input.GetKeyDown( KeyCode.Q ) ) {
            endBonusMode( 1, GAME_STAGE.NORMAL );
        }
    }
    #endif


    // MovePhaseの更新
    public void movePhaseUpdate( int[ ] count, GameObject target_pos ) {
        dicisionTopAndLowestPlayer( ref count );
        if ( _player_id > -1 ) {
            _move_start[ _player_id ] = true;
            //if( _target != null )
               // adjustmentUpdate( target_pos );
            if ( _limit_value > 0 ) {
                if ( !_move_flag ) {
                    setTargetPos( 0, _player_id, ref target_pos );
                } else {
                    playerMove( 0, _player_id );
                }
            } else if ( _limit_value == 0 ) {
                _move_finish[ _player_id ] = true;
                _limit_value--;
                _limit_value_adjustment--;
                _adjustment_flag = false;
            } else {
                 _move_finish[ _player_id ] = true;
                _player_id = -1;
            }
        } else {
			_player_id = -1;
			_target[ 0 ] = null;
		}
	}

    /*private void adjustmentUpdate( GameObject target_pos ) {
        if ( _limit_value_adjustment == 0 ) {
            _limit_value_adjustment--;
            _adjustment_flag = false;
        } else {
            if( _limit_value_adjustment < 0 ) {
                if ( !_adjustment_flag ) {
                    playerPostionAdjustment( target_pos );
                }
            } else if( _limit_value_adjustment > 0 ) {
                //playerAdjustment( );
            }
        }
    }

    private void setAdjustmentTargetPos( int i, int id, ref GameObject target_pos ) {
        _startTime[ i ] = Time.timeSinceLevelLoad;
		_start_position[ i ] = _players[ id ].obj.transform.position;
        _target[ i ] = target_pos;
        _end_position[ i ] = _target[ i ].transform.localPosition;
        _end_position[ i ].y += 0.3f;
    }

    private void playerPostionAdjustment( GameObject target_pos ) {
        if( _players[ 0 ].advance_count + 1 == _players[ 1 ].advance_count
            && getPlayerID( ) == 0 && _limit_value > 1 ) {
            if( _advance_flag ) {
                _adjustment_flag = true;
                _limit_value_adjustment = 1;
                setAdjustmentTargetPos( 1, 0, ref target_pos );
                _end_position[ 1 ].z += ADJUST_FIRST_PLAYER_Z_POS;
            }
        } else if( _players[ 1 ].advance_count + 1 == _players[ 0 ].advance_count
            && getPlayerID( ) == 1 && _limit_value > 1 ) {
            if( _advance_flag ) {
                _adjustment_flag = true;
                _limit_value_adjustment = 1;
                setAdjustmentTargetPos( 1, 1, ref target_pos );
                _end_position[ 1 ].z += ADJUST_FIRST_PLAYER_Z_POS;
            }
        } else if(  _players[ 1 ].advance_count - 1 == _players[ 0 ].advance_count || _players[ 1 ].advance_count == _players[ 0 ].advance_count - 1 ) {
            if( _limit_value == 1 && _players[ 1 ].advance_count > 0 && _players[ 0 ].advance_count > 0 ) {
                _adjustment_flag = true;
                _limit_value_adjustment = 1;
                if ( _advance_flag ) {
                    if( _player_id == 0 )
                        setAdjustmentTargetPos( 1, 1, ref target_pos );
                    else
                        setAdjustmentTargetPos( 1, 0, ref target_pos );
                }
                _end_position[ 0 ].z += ADJUST_FIRST_PLAYER_Z_POS;
                _end_position[ 1 ].z -= ADJUST_FIRST_PLAYER_Z_POS;
            } 
        } else {
            _adjustment_flag = false;
            _limit_value_adjustment = 0;
        }
    }

    /*private void playerAdjustment( ) {
        if( _players[ 0 ].advance_count + 1 == _players[ 1 ].advance_count ) {
            if( _advance_flag ) {
                if( getPlayerID( ) == 0 && _limit_value > 1 ) {
                    playerAdjustmentMove( 1, 1 );
                } else if( _limit_value == 1 && _players[ 1 ].advance_count > 0 && _players[ 0 ].advance_count > 0 ) {
                    playerAdjustmentMove( 1, 0 );
                }
            }
        } else if( _players[ 1 ].advance_count + 1 == _players[ 0 ].advance_count ) {
            if( _advance_flag ) {
                if( getPlayerID( ) == 1 && _limit_value > 1 ) {
                    playerAdjustmentMove( 1, 0 );
                } else if( _limit_value == 1 && _players[ 1 ].advance_count > 0 && _players[ 0 ].advance_count > 0 ) {
                    playerAdjustmentMove( 1, 1 );
                }
            }
        } /*else if(  _players[ 1 ].advance_count - 1 == _players[ 0 ].advance_count ) {
            if( _limit_value == 1 && _players[ 1 ].advance_count > 0 && _players[ 0 ].advance_count > 0 ) {
                if( _advance_flag ) {
                    if( _player_id == 0 ) {
                        playerAdjustmentMove( 1, 1 );
                    }
                } else {
                    if( _players[ 1 ].advance_count == _players[ 0 ].advance_count - 1 ) {
                        if( _player_id == 1 ) {
                            playerAdjustmentMove( 1, 0 );
                        }
                    }
                }
            }
        }
    }

    public void playerAdjustmentMove( int i, int id ) {
        var diff = Time.timeSinceLevelLoad - _startTime[ i ];
		if ( diff > _time ) {
			_players[ id ].obj.transform.position = _end_position[ i ];
            _limit_value_adjustment--;
        }

		var rate = diff / _time;

		_players[ id ].obj.transform.position = Vector3.Lerp ( _start_position[ i ], _end_position[ i ], rate );
    }*/
    /// <summary>
	/// ターゲットの設定
	/// </summary>
	/// <param name="count">Count.</param>
	/// <param name="advance_pos">Advance position.</param>
	/// <param name="back_pos">Back position.</param>
	private void setTargetPos(int i, int id, ref GameObject target_pos)
    {
        if (_time <= 0)
        {
            _players[id].obj.transform.position = _end_position[i];
            _player_id = -1;
            _target[0] = null;
            return;
        }

        if(_current_flag) _time = 1.0f;
        else _time = 0.5f;

        _startTime[i] = Time.timeSinceLevelLoad;
        _start_position[i] = _players[id].obj.transform.position;
        _target[i] = target_pos;
        _end_position[i] = _target[i].transform.localPosition;

        Vector3 direc = Vector3.Cross(_start_position[i], _end_position[i]).normalized;
        switch (_player_id)
        {
        case 0:
                _end_position[i].x -= ADJUST_PLAYER_POS;
                _end_position[i].z += ADJUST_PLAYER_POS;
            break;
        case 1:
                _end_position[i].x += ADJUST_PLAYER_POS;
                _end_position[i].z -= ADJUST_PLAYER_POS;
                break;
        }
        _end_position[i].y += 0.3f;
        _move_flag = true;
    }
    /// <summary>
    /// プレイヤーを動かす処理
    /// </summary>
    private void playerMove( int i, int id ) {
        var diff = Time.timeSinceLevelLoad - _startTime[ i ];
        float distance = Vector3.Distance( _players[ id ].obj.transform.position, _end_position[ i ] );
		if ( diff > _time * 3.5f ) {
			_players[ id ].obj.transform.position = _end_position[ i ];
            _accel_init = false;
            
            if( _current_flag ) {
			    if ( _advance_flag ) {
				    _players[ id ].advance_count += _limit_value;
			    } else {
				    _players[ id ].advance_count -= _limit_value;
			    }
                _limit_value = 0;
            } else {
                if ( _advance_flag ) {
				    _players[ id ].advance_count++;
			    } else {
				    _players[ id ].advance_count--;
			    }
                _limit_value--;
            }
            _current_flag = false;
            _move_flag = false;
            return;
        }
        _players[ id ].obj.transform.position = Vector3.SmoothDamp(_players[id].obj.transform.position, _end_position[i], ref _velocity , _time);//Vector3.Lerp ( _start_position[ i ], _end_position[ i ], rate );
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
                if( _current_flag ){
				    return getPlayerCount( getPlayerID( ), length ) + _limit_value;
                } else {
                    return getPlayerCount( getPlayerID( ), length ) + 1;
                }
			} else {
                _limit_value = 0;
				return getPlayerCount( getPlayerID( ), length );
			}
		} else {
            if( _current_flag ){
                return getPlayerCount( getPlayerID( ), length ) - _limit_value;
            } else {
			    return getPlayerCount( getPlayerID( ), length ) - 1;
            }
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

    public bool isAdjsutmentStart( ) {
        return _adjustment_flag;
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

    public void setCurrentFlag( bool flag ){
        _current_flag = flag;
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

     /// <summary>
     /// 特定のプレイヤーをボーナスマップへ移動させる
     /// プレイヤーID ボーナス適応中かどうか　
     /// </summary>
     /// <param name="id">Identifier.</param>
     /// <param name="pos">Position.</param>
     /// <param name="bonus">If set to <c>true</c> bonus.</param>
     public void startBonusMode( int id, GAME_STAGE stage ) {
         _players[ id ].obj.transform.position = new Vector3( 7, 1, 3 );
         _players[ id ].stage = stage;
     }
     /// <summary>
     /// 
     /// </summary>
     /// <param name="id">Identifier.</param>
     /// <param name="bonus">If set to <c>true</c> bonus.</param>
     public void endBonusMode( int id, GAME_STAGE stage ) {
         _players[ id ].obj.transform.position = new Vector3( 25, 0, 0);
         _players[ id ].stage = stage;
     }
}