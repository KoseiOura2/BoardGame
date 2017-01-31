using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common;

public class PlayerManager : MonoBehaviour {
    
    public float FINISH_MOVE_TIME_MAGNIFICANT = 3.5f;          // 移動を終了させる時間倍率

    // どのプレイヤーが行動中か
    [ SerializeField ]
    private PLAYER_ORDER _player_order = PLAYER_ORDER.NO_PLAYER;
	private Player[ ] _players = new Player[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
    // プレイヤーのモデルをロード
	private GameObject[ ] _player_pref = new GameObject[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];
	private GameObject _firstest_player;
	private GameObject _latest_player;
	private GameObject _winner_player;
	private GameObject _loser_player;
    
    // 進むマス数設定
    [ SerializeField ]
	private int _limit_value = 0;    
    private float _time = 0.3f;
    // プレイヤーの順序が変わったかどうか
    private bool _change_player_order = false;
    // 動かす時のフラグが立っているか
	[ SerializeField ]
    private bool _move_flag           = false;
    // 前に進むか後ろに戻るか
    private bool _advance_flag        = true;   
	// 海流時
    private bool _current_flag        = false;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="first_pos"></param>
    public void init( ref Vector3 first_pos ) {
        for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
            // プレイヤープレハブのロード
            _player_pref[ i ] = ( GameObject )Resources.Load( "Prefabs/Player/Player" + i );
            // 出力したデータを元にプレイヤーを初期化
            _players[ i ].init( i, ref first_pos, ref _player_pref[ i ] );
        }
    }

	// Use this for initialization
	void Start( ) {
	
	}

    #if UNITY_EDITOR
    /// <summary>
    /// Unityエディタ上でのみデバッグ機能を有効
    /// </summary>
    void Update( ) {
    
        /*
        if ( Input.GetKeyDown( KeyCode.P ) ) {
            startBonusMode( 1, GAME_STAGE.BONUS );
        }
        if ( Input.GetKeyDown( KeyCode.Q ) ) {
            endBonusMode( 1, GAME_STAGE.NORMAL );
        }
         */
    }
    #endif
    
    /// <summary>
    /// プレイヤーの処理順を更新
    /// </summary>
    public void updatePlayerOrder( ) {
        if ( _change_player_order ) {
            // プレイヤーの順番を取得
            int num = ( int )_player_order;
            num += ( int )_player_order + 1;

            // プレイヤーの最大数を超えたらNoneに戻す
            if ( num >= ( int )PLAYER_ORDER.MAX_PLAYER_NUM ) {
                num = -1;
            }

            // 順番を更新
            _player_order = ( PLAYER_ORDER )num;

            _change_player_order = false;
        }
    }

    // MovePhaseの更新
    public void movePhaseUpdate( ref int[ ] count, GameObject target_pos ) {
        // プレイヤーの順位を設定
        dicisionTopAndLowestPlayer( ref count );

        if ( _player_order != PLAYER_ORDER.NO_PLAYER ) {
            _players[ ( int )_player_order ].startMove( );
            if ( _limit_value > 0 ) {
                if ( !_move_flag ) {
                    if ( _current_flag ) {
                        _time = 1.0f;
                    } else { 
                        _time = 0.5f;
                    }
                    // ターゲットのマスを設定
                    _players[ ( int )_player_order ].setTargetPos( _time, ref target_pos );
                    _move_flag = true;
                } else {
                    // 強制的に目的地へ移動
                    forceDistination( );
                    // ターゲットに向かって移動
                    _players[ ( int )_player_order ].move( _time );
                }
            } else if ( _limit_value == 0 ) {
                _players[ ( int )_player_order ].finishMove( );
                _limit_value--;
            }
        } else {
			_players[ ( int )_player_order ].deleteTargetMass( );
		}
	}

    public void forceDistination( ) {
        var diff = Time.timeSinceLevelLoad - _players[ ( int )_player_order ].getStartTime( );
        // 一定時間移動したら強制的に到着させる
		if ( diff > _time * FINISH_MOVE_TIME_MAGNIFICANT ) {
			_players[ ( int )_player_order ].setObjPosForceDistination( );

            if( _current_flag ) {
			    if ( _advance_flag ) {
				    _players[ ( int )_player_order ].updateAdvanceCount( _limit_value );
			    } else {
				    _players[ ( int )_player_order ].updateAdvanceCount( -_limit_value );
			    }
                _limit_value = 0;
            } else {
                if ( _advance_flag ) {
				    _players[ ( int )_player_order ].updateAdvanceCount( 1 );
			    } else {
				    _players[ ( int )_player_order ].updateAdvanceCount( -1 );
			    }
                _limit_value--;
            }

            _players[ ( int )_player_order ].changeMassCountFlag( true );
            _current_flag = false;
            _move_flag    = false;
            return;
        }
    }

    /// <summary>
    /// プレイヤーの順番を1Pからにする
    /// </summary>
    public void startPlayerOrder( ) {
        _player_order = PLAYER_ORDER.PLAYER_ONE;
    }
    
    /// <summary>
    /// ランク付け関数
    /// </summary>
    /// <param name="count" "ゴールまでの残りマス"></param>
	public void dicisionTopAndLowestPlayer( ref int[ ] count ) {
        // プレイヤー同士のマスを比較する
		if( count[ ( int )PLAYER_ORDER.PLAYER_ONE ] != count[ ( int )PLAYER_ORDER.PLAYER_TWO ] ) {
            // カウントを比較しランクを設定する
			if ( count[ ( int )PLAYER_ORDER.PLAYER_ONE ] > count[ ( int )PLAYER_ORDER.PLAYER_TWO ] ) {
                _latest_player   = _players[ ( int )PLAYER_ORDER.PLAYER_ONE ].getData( ).obj;
				_firstest_player = _players[ ( int )PLAYER_ORDER.PLAYER_TWO ].getData( ).obj;
				_players[ ( int )PLAYER_ORDER.PLAYER_ONE ].setPlayerRank( PLAYER_RANK.RANK_SECOND );
				_players[ ( int )PLAYER_ORDER.PLAYER_TWO ].setPlayerRank( PLAYER_RANK.RANK_FIRST );
            } else {
                _latest_player   = _players[ ( int )PLAYER_ORDER.PLAYER_TWO ].getData( ).obj;
				_firstest_player = _players[ ( int )PLAYER_ORDER.PLAYER_ONE ].getData( ).obj;
				_players[ ( int )PLAYER_ORDER.PLAYER_TWO ].setPlayerRank( PLAYER_RANK.RANK_SECOND );
				_players[ ( int )PLAYER_ORDER.PLAYER_ONE ].setPlayerRank( PLAYER_RANK.RANK_FIRST );
            }
		} else {
            // 同じ値の場合
            _latest_player   = _players[ ( int )PLAYER_ORDER.PLAYER_TWO ].getData( ).obj;
			_firstest_player = _players[ ( int )PLAYER_ORDER.PLAYER_ONE ].getData( ).obj;
			_players[ ( int )PLAYER_ORDER.PLAYER_TWO ].setPlayerRank( PLAYER_RANK.RANK_SECOND );
			_players[ ( int )PLAYER_ORDER.PLAYER_ONE ].setPlayerRank( PLAYER_RANK.RANK_FIRST );
		}
	}
    
	/// <summary>
	/// 攻撃力比較用関数
	/// </summary>
	public void attackTopAndLowestPlayer( int[ ] attack ) {
		if( attack[ ( int )PLAYER_ORDER.PLAYER_ONE ] != ( int )PLAYER_ORDER.PLAYER_TWO ) {
			float winner = Mathf.Max( attack[ ( int )PLAYER_ORDER.PLAYER_ONE ], ( int )PLAYER_ORDER.PLAYER_TWO );

			if( winner == attack[ ( int )PLAYER_ORDER.PLAYER_ONE ] ) {
				_winner_player = _players[ ( int )PLAYER_ORDER.PLAYER_ONE ].getData( ).obj;
				_loser_player  = _players[ ( int )PLAYER_ORDER.PLAYER_TWO ].getData( ).obj;
				_players[ ( int )PLAYER_ORDER.PLAYER_ONE ].setBattleResult(  BATTLE_RESULT.WIN );
				_players[ ( int )PLAYER_ORDER.PLAYER_TWO ].setBattleResult(  BATTLE_RESULT.LOSE );
			} else if( winner == attack[ ( int )PLAYER_ORDER.PLAYER_TWO ] ) {
				_winner_player = _players[ ( int )PLAYER_ORDER.PLAYER_TWO ].getData( ).obj;
				_loser_player  = _players[ ( int )PLAYER_ORDER.PLAYER_ONE ].getData( ).obj;
				_players[ ( int )PLAYER_ORDER.PLAYER_TWO ].setBattleResult(  BATTLE_RESULT.WIN );
				_players[ ( int )PLAYER_ORDER.PLAYER_ONE ].setBattleResult(  BATTLE_RESULT.LOSE );
			} 
		} else {
			_players[ ( int )PLAYER_ORDER.PLAYER_TWO ].setBattleResult( BATTLE_RESULT.DRAW );
			_players[ ( int )PLAYER_ORDER.PLAYER_ONE ].setBattleResult( BATTLE_RESULT.DRAW );
		}
	}

	/// <summary>
	/// カード効果適応
	/// </summary>
	/// <param name="card">Card.</param>
	public void adaptaCard( CARD_DATA data ) {
		switch ( data.enchant_type ) {
            case CARD_TYPE.CARD_TYPE_ONCE_ENHANCE:
			    addPower( data.enchant_value );
			    Debug.Log( "強化効果" + data.enchant_value );
			    break;
            case CARD_TYPE.CARD_TYPE_CONTUNU_ENHANCE:
			    addPower( data.enchant_value );
			    Debug.Log( "強化効果" + data.enchant_value );
			    break;
                /*
            case CARD_TYPE.CARD_TYPE_INSURANCE:
			    specialEnhance( data );
			    Debug.Log( "スペシャル効果" );
			    break;
                */
            case CARD_TYPE.CARD_TYPE_UNAVAILABLE:
			    addPower( -data.enchant_value );
			    Debug.Log( "デメリット効果" + data.enchant_value );
			    break;
		}
	}

	/// <summary>
	/// Adds the power.
	/// </summary>
	/// <param name="enhance">Enhance.</param>
	private void addPower( int enhance ) {
		_players[ ( int )_player_order ].addPower( enhance );
	}

    /*
	/// <summary>
	/// スペシャルタイプのカード効果
	/// </summary>
	/// <param name="data">Data.</param>
	private void specialEnhance( CARD_DATA data ) {
		if ( data.special_value == ( int )SPECIAL_LIST.ENHANCE_TYPE_DRAW ) {
			_plus_draw += data.enchant_value;
		}
	}
    */

	/// <summary>
	/// 指定ランクプレイヤーのゲームオブジェクトを返す
	/// </summary>
	/// <param name="player_rank"></param>
	/// <returns></returns>
	public PLAYER_DATA getTopPlayer( PLAYER_RANK player_rank ) {
		PLAYER_DATA data = new PLAYER_DATA( );

		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			if ( player_rank == _players[ i ].getData( ).rank ) {
				return _players[ i ].getData( );
			} 
		}
		return data;
	}

    /// <summary>
    /// プレイヤーのアニメーションを変える
    /// </summary>
	public void setPlayerMotion( ) {
		if ( _player_order != PLAYER_ORDER.NO_PLAYER ) {
			if( _players[ ( int )_player_order ].getData( ).obj != null ) {
			    switch( _players[ ( int )_player_order ].getData( ).event_type ) {
                    // MovePhase時
                    // マス移動時
                    case EVENT_TYPE.EVENT_NONE:
                    case EVENT_TYPE.EVENT_MOVE:
                    case EVENT_TYPE.EVENT_TRAP_ONE:
                    case EVENT_TYPE.EVENT_TRAP_TWO:
					    if( !_players[ ( int )_player_order ].isMoveStart( ) || _players[ ( int )_player_order ].isMoveFinish( ) ) {
						    _players[ ( int )_player_order ].getData( ).obj.GetComponent< Animator >( ).SetInteger( "state", 0 );
					    } else if ( _players[ ( int )_player_order ].isMoveStart( ) && !_players[ ( int )_player_order ].isMoveFinish( ) ) {
                            //歩くアニメーションをセット
                            _players[ ( int )_player_order ].getData( ).obj.GetComponent< Animator >( ).SetInteger( "state", 1 );
					    }
                        break;
                        /*
                    // ワープイベント時
                    case EVENT_TYPE.EVENT_WORP:
                    case EVENT_TYPE.EVENT_CHANGE:
					    _players[ i ].obj.GetComponent< Animator >( ).SetInteger( "state", 1 );
                        break;
                    // カードを捨てるマス発生時
                    case EVENT_TYPE.EVENT_DISCARD:
                        //イベント時転ぶアニメーションをセット
					    _players[ i ].obj.GetComponent< Animator >( ).SetInteger( "state", 1 ); 
					    break;
                         */
			    }
		    }
	    }
	}

	 public bool getAnimationEnd( ) {
         if ( _players[ ( int )_player_order ].getData( ).obj.GetComponent< Animator >( ).GetCurrentAnimatorStateInfo( 0 ).normalizedTime == 1 ) {
             return true;
         } else {
             return false;
         }
	 }
    
	/// <summary>
	/// 最下位プレイヤーのゲームオブジェクトを返す
	/// </summary>
	/// <returns></returns>
	public GameObject getLastPlayer( ) {
		return _latest_player;
	}

	public void setPlayerOnMove( bool on_move ) {
		_players[ ( int )_player_order ].setOnMove( on_move );
	}
   
	public void endStatus( int id ) {
		_players[ id ].endStatus( );
	}

    public PLAYER_ORDER getPlayerOrder( ) {
        return _player_order;
    }

    public void changePlayerOrder( ) {
        _change_player_order = true;
    }

	public bool isMoveStart( ) {
		return _players[ ( int )_player_order ].isMoveStart( );
	}

	public bool isMoveFinish( ) {
		return _players[ ( int )_player_order ].isMoveFinish( );
	}
    
	public bool isEventStart( ) {
		return _players[ ( int )_player_order ].isEventStart( );
	}

	public bool isEventFinish( ) {
		return _players[ ( int )_player_order ].isEventFinish( );
	}

    public void setEventFinish( bool flag ) {
        _players[ ( int )_player_order ].setEventFinish( flag );
    }

    public void setEventStart( bool flag ) {
        _players[ ( int )_player_order ].setEventStart( flag );
	}

	public bool getPlayerOnMove( ) {
		return _players[ ( int )_player_order ].getData( ).on_move;
	}

    /// <summary>
    /// 全プレイヤーの移動が終わったかどうか
    /// </summary>
    /// <returns></returns>
    public bool isAllPlayerMoveFinish( ) {
        bool flag = false;
        int count = 0;

        for ( int i = 0; i < _players.Length; i++ ) {
            if ( _players[ i ].isMoveFinish( ) ) {
                count++;
            }
        }

        if ( count == _players.Length ) {
            flag = true;
        }

        return flag;
    }
    
    /// <summary>
    /// 全プレイヤーの移動が終わったかどうか
    /// </summary>
    /// <returns></returns>
    public bool isAllPlayerEventFinish( ) {
        bool flag = false;
        int count = 0;

        for ( int i = 0; i < _players.Length; i++ ) {
            if ( _players[ i ].isEventFinish( ) ) {
                count++;
            }
        }

        if ( count == _players.Length ) {
            flag = true;
        }

        return flag;
    }

    /// <summary>
    /// プレイヤーの移動をリセット
    /// </summary>
    public void movedRefresh( ) {
        for ( int i = 0; i < _players.Length; i++ ) {
            _players[ i ].refreshMoveFlag( );
        }
    }

    public void allPlusValueInit( ) {
        for ( int i = 0; i < _players.Length; i++ ) {
            _players[ i ].plusValueInit( );
        }
    }

	public void setAdvanceFlag( bool flag ) {
		_advance_flag = flag;
	}

	public void setLimitValue( int value ) {
		_limit_value = value;
	}

	/// <summary>
	/// 各プレイヤーの攻撃力を取得
	/// </summary>
	/// <returns></returns>
	public int[ ] getPlayerPower( ) {
		int[ ] power = new int[ _players.Length ];
		for ( int i = 0; i < power.Length; i++ ) {
			power[ i ] = _players[ i ].getData( ).power;
		}

		return power;
	}

    public void setPlayerPower( int power ) {
        _players[ ( int )_player_order ].setPower( power );
    }

	//ターゲットとなるマスIDを取得
	public int getTargetMassID( int length ) {
        int mass_id = 0;

        if ( _player_order != PLAYER_ORDER.NO_PLAYER ) {
		    if( _advance_flag ) {
			    if( getPlayerCount( ( int )_player_order, length ) < length - 1 ) {
                    if( _current_flag ){
				        return getPlayerCount( ( int )_player_order, length ) + _limit_value;
                    } else {
                        return getPlayerCount( ( int )_player_order, length ) + 1;
                    }
			    } else {
                    _limit_value = 0;
				    return getPlayerCount( ( int )_player_order, length );
			    }
		    } else {
                if( _current_flag ){
                    return getPlayerCount( ( int )_player_order, length ) - _limit_value;
                } else {
			        return getPlayerCount( ( int )_player_order, length ) - 1;
                }
		    }
        }

        return mass_id;
	}

    /// <summary>
    /// プレイヤーがどれくらい進んでいるかを取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
	public int getPlayerCount( int id, int length ) {
		if ( id >= 0 ) {
			if( _players[ id ].getData( ).advance_count < length - 1 ) {
				return _players[ id ].getData( ).advance_count;
			} else {
				return length - 1;
			}
        } else {
            return 0;
        }
    }

    public BATTLE_RESULT getPlayerResult( int id ) {
        return _players[ id ].getData( ).battle_result;
    }

    public void refreshPlayerResult( ) {
        for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
            _players[ i ].setBattleResult( BATTLE_RESULT.BATTLE_RESULT_NONE );
        }
    }
    public void addDrawCard( int num ) {
        _players[ ( int )_player_order ].addDrawCard( num );
    }

    public List< int > getDrawCard( ) {
        List< int > card = _players[ ( int )_player_order ].getDrawCard( );

        return card;
    }

    public int getPlayerOneDrawCardNum( ) {
        return _players[ ( int )_player_order ].getDrawCardNum( );
    }
    
    public EVENT_TYPE getEventType( ) {
        return _players[ ( int )_player_order ].getData( ).event_type;
    }

    public bool isChangeCount( PLAYER_ORDER player_num ) {
        if ( _players[ ( int )_player_order ].isChangeCount( ) ) {
            return true;
        }
        return false;
    }

	public void eventRefresh( ) {
        for ( int i = 0; i < _players.Length; i++ ) {
            _players[ i ].refreshEventFlag( );
        }
	}

    public void setEventType( int id, EVENT_TYPE event_type ) {
        _players[ ( int )_player_order ].setEventType( event_type );
    }

    /*
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
    */

    public void eventRefresh( ) {
        _players[ ( int )_player_order ].setEventType( EVENT_TYPE.EVENT_NONE );
	 }

     public void setPlayerCount( int count ) {
        _players[ ( int )_player_order ].setAdvanceCount( count );
    }

    public void setPlayerPosition( int id, Vector3 position ) {
        _players[ ( int )_player_order ].getData( ).obj.transform.localPosition = _players[ ( int )_player_order ].adjustPos( ref position );
    }

    public Vector3 isPlayerPosition( ) {
        return _players[ ( int )_player_order ].getData( ).obj.transform.localPosition;
    }

    public void setCurrentFlag( bool flag ){
        _current_flag = flag;
    }

    public void destroyObj( ) {
        for ( int i = 0; i < _players.Length; i++ ) {
            Destroy( _players[ i ].getData( ).obj );
        }
    }
}