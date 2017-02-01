using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Common;

public class ClientPlayerManager : MonoBehaviour {

	private const float MAX_DICE_VALUE   = 3.9f;
	private const float MIN_DICE_VALUE   = 1.0f;
	private const int INIT_PLAYER_POWER  = 10;
    private const int MAX_PLAYER_CARD_NUM = 6;
	private const int MAX_SEND_CARD_NUM = 4;
    private const float EXPANTION_MAGNIFICATION = 2.0f;

    public enum DRAW_CARD_ACTION {
        ACTION_NONE,
        MOVE_FOR_GET_ACTION,
        ROTATE_ACTION,
        MOVE_FOR_HAND_ACTION,
    }

	/// <summary>
	/// プレイヤーの持つカードのデータ
	/// </summary>
	private struct PLAYER_CARD_DATA {
		public List< CARD_DATA >  hand_list;
		public List< GameObject > hand_obj_list;
		public List< Vector3 >    select_position;
		public List< CARD_DATA >  select_list;
	}

    private struct DRAW_CARD_DATA {
        public CARD_DATA card_data;
        public GameObject obj;
        public Vector3 pos;
        public float angle;
        public bool move;
        public bool rotate;

        public DRAW_CARD_DATA( CARD_DATA card_data, GameObject obj, Vector3 pos,
                               float angle, bool move, bool rotate ) {
            this.card_data = card_data;
            this.obj       = obj;
            this.pos       = pos;
            this.angle     = angle;
            this.move      = move;
            this.rotate    = rotate;
        }
    };
    
    private struct THROW_CARD_DATA {
        public CARD_DATA card_data;
        public GameObject obj;
        public Vector3 pos;
        public bool move;

        public THROW_CARD_DATA( CARD_DATA card_data, GameObject obj, Vector3 pos, bool move ) {
            this.card_data = card_data;
            this.obj       = obj;
            this.pos       = pos;
            this.move      = move;
        }
    };

	[ SerializeField ]
	private CardManager _card_manager;
	[ SerializeField ]
	private PLAYER_CARD_DATA _player_card = new PLAYER_CARD_DATA( );
	private PLAYER_DATA _player_data;
    private List< DRAW_CARD_DATA > _draw_card_list = new List< DRAW_CARD_DATA >( );
    private List< THROW_CARD_DATA > _throw_card_list = new List< THROW_CARD_DATA >( );
    
	private GameObject _profile_card_pref;
	private GameObject _profile_card_obj;
	private GameObject _profile_card_area;
    private GameObject _expantion_card;
    private GameObject _expantion_card_area;
    [ SerializeField ]
    private GameObject _create_draw_card_pos;
    private GameObject _draw_card_area;
	private GameObject _player_card_area_base;
	private GameObject _select_area_base;
    private Vector3[ ] _select_area = new Vector3[ MAX_PLAYER_CARD_NUM ];
	private GameObject[ ] _throw_player_card_area_base = new GameObject[ 2 ];
	private GameObject _select_throw_area_base;
    private Vector3[ ] _select_throw_area = new Vector3[ MAX_PLAYER_CARD_NUM ];
	private GameObject _throw_area;

    private GAME_PLAY_MODE _play_mode = GAME_PLAY_MODE.MODE_NORMAL_PLAY;
    [ SerializeField ]
    private DRAW_CARD_ACTION _draw_card_action = DRAW_CARD_ACTION.ACTION_NONE;

    private List< bool > _arrived_list = new List< bool >( );
    private List< bool > _rotate_list  = new List< bool >( );
    private int _power = 0;
	private int _dice_value = 0;
    private int _expantion_num = -1;
    private float _card_width = 3.1f;
    private float _draw_card_pos_x_adjust = 0.5f;
    private float _draw_card_move_speed = 0.5f;
    [ SerializeField ]
    private bool _draw_card  = false;
    private bool _throw_card = false;
	private bool _dice_roll = false;
    private bool _select_throw_complete = false;
    private bool _expansion = false;

	[ SerializeField ]
	private int _hand_num = 0;
	[ SerializeField ]
	private int _hand_obj_num = 0;

	#if UNITY_EDITOR
	private bool _debug_inst_flag;
	[ SerializeField ]
	private bool _auto_inst_flag = false;	// オートで生成したくない場合はfalseに
	#endif

	public GameObject _card_obj;

    void Awake( ) {
		// プレイヤーの初期化
		_player_card.hand_list       = new List< CARD_DATA >( );
		_player_card.hand_obj_list   = new List< GameObject >( );
		_player_card.select_position = new List< Vector3 >( );
		_player_card.select_list     = new List< CARD_DATA >( );
    }

	// Use this for initialization
	void Start( ) {
		if ( _profile_card_area == null ) {
			_profile_card_area = GameObject.Find( "PlayerCardArea" );
		}
		if ( _create_draw_card_pos == null ) {
			_create_draw_card_pos = GameObject.Find( "CreateDrawCardPos" );
		}
		if ( _draw_card_area == null ) {
			_draw_card_area = GameObject.Find( "DrawCardArea" );
		}
		if ( _player_card_area_base == null ) {
			_player_card_area_base = GameObject.Find( "HandArea" );
		}
		if ( _select_area_base == null ) {
			_select_area_base = GameObject.Find( "SelectHandArea" );
		}
		if ( _throw_area == null ) {
			_throw_area = GameObject.Find( "ThrowArea" );
		}
		if ( _expantion_card_area == null ) {
			_expantion_card_area = GameObject.Find( "ExpantionArea" );
		}
        
        // 選択エリアの設定
        for ( int i = 0; i < _select_area.Length; i++ ) {
            float start_pos = _select_area_base.transform.position.x - _select_area_base.transform.localScale.x / 2;
            float adjust = 0.3f;

            float x = start_pos + ( _card_width + adjust ) * i;
            float y = _select_area_base.transform.position.y;
            float z = _select_area_base.transform.position.z;

            _select_area[ i ] = new Vector3( x, y, z );
        }

        for ( int i = 0; i < 2; i++ ) {
            if ( _throw_player_card_area_base[ i ] == null ) {
                _throw_player_card_area_base[ i ] = GameObject.Find( "ThrowHandArea_" + i );
            }
        }

		if ( _select_throw_area_base == null ) {
			_select_throw_area_base = GameObject.Find( "ThrowSelectArea" );
		}
        
        // 選択エリアの設定
        for ( int i = 0; i < _select_throw_area.Length; i++ ) {
            float start_pos = _select_throw_area_base.transform.position.x - _select_throw_area_base.transform.localScale.x / 2;
            float adjust = 0.4f;

            float x = start_pos + ( _card_width + adjust ) * i;
            float y = _select_throw_area_base.transform.position.y;
            float z = _select_throw_area_base.transform.position.z;

            _select_throw_area[ i ] = new Vector3( x, y, z );
        }

		if ( _card_obj == null ) {
			_card_obj = ( GameObject )Resources.Load( "Prefabs/Card" );
		}
		if ( _card_manager == null ) {
			_card_manager = GameObject.Find( "CardManager" ).GetComponent< CardManager >( );
		}

		_player_data.power = INIT_PLAYER_POWER;
	}
	
	/// <summary>
    /// エディタ上でのみデバッグ機能が実行される
    /// </summary>
	#if UNITY_EDITOR
	void Update( ) {
        /*
        // カードデータの追加
		if ( Input.GetKeyDown( KeyCode.X ) || _auto_inst_flag ) {
			//適当に追加　ToDoランダムに手札を追加する機能
			addPlayerCard( 1 );
			addPlayerCard( 2 );
			addPlayerCard( 3 );
			addPlayerCard( 4 );
			addPlayerCard( 1 );
			addPlayerCard( 1 );
			addPlayerCard( 1 );
			_debug_inst_flag = true;
		}
        */

		if ( Input.GetKeyDown( KeyCode.U ) && _debug_inst_flag || _auto_inst_flag && _debug_inst_flag ) {
			// カードオブジェクトの更新処理
			updateAllPlayerCard( );
			_debug_inst_flag = false;
			if ( _auto_inst_flag ) {
				_auto_inst_flag = false;
			}
		}


	}
	#endif

    public void createProfileCard( int player_num ) {
        _profile_card_pref = Resources.Load< GameObject >( "Prefabs/PlayerCard" );
        Vector3 pos = _profile_card_pref.GetComponent< RectTransform >( ).localPosition;
            
        _profile_card_obj = ( GameObject )Instantiate( _profile_card_pref );
        _profile_card_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
        _profile_card_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
        _profile_card_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
        _profile_card_obj.GetComponent< RectTransform >( ).localPosition = pos;
        _profile_card_obj.GetComponent< Image >( ).sprite = Resources.Load< Sprite >( "Graphics/PlayerCard/PlayerCard" + player_num.ToString( ) );
    }

    public void destroyProfileCard( ) {
        Destroy( _profile_card_obj );
        _profile_card_obj  = null;
        _profile_card_pref = null;
    }
    
    /// <summary>
    /// ドローしたカードを保持
    /// </summary>
    /// <param name="get_card_id"></param>
    /// <param name="num"></param>
	public void addDrawCard( int get_card_id, int num, int length ) {
        _draw_card_action = DRAW_CARD_ACTION.MOVE_FOR_GET_ACTION;

		//IDのカードデータを取得
		CARD_DATA card_data = _card_manager.getCardData( get_card_id );
        // オブジェクトの生成
        GameObject obj = Instantiate( _card_obj,
                                      _create_draw_card_pos.transform.position,
                                      _card_obj.transform.rotation ) as GameObject;
        obj.GetComponent< Card >( ).setCardData( card_data );

        // ポジションの決定
        Vector3 pos = _draw_card_area.transform.position;
        float pos_x = _draw_card_area.transform.position.x;
        if ( length % 2 == 0 ) {
            float count = ( ( float  )length + 1 ) / 2 - ( num + 1 );

            if ( count > 0 ) {
                count += 0.5f;
            } else if ( count < 0 ) {
                count -= 0.5f;
            }
            pos_x = _draw_card_area.transform.position.x - count * ( _card_width + _draw_card_pos_x_adjust );
        } else {
            int count = ( length + 1 ) / 2 - ( num + 1 );
            if ( count == 0 ) {
                pos_x = _draw_card_area.transform.position.x;
            } else {
                pos_x = _draw_card_area.transform.position.x - count * ( _card_width + _draw_card_pos_x_adjust );
            }
        }
        pos = new Vector3( pos_x, pos.y, pos.z );

        float angle = -180.0f;

        // フラグの初期化
        bool move = true;
        //bool move = false;
        bool rotate = false;
        
		DRAW_CARD_DATA card = new DRAW_CARD_DATA( card_data, obj, pos, angle, move, rotate );
		//カードを追加
        _draw_card_list.Add( card );
    }

    public void moveStartDrawCard( int id ) {
        bool move = true;
        

        _draw_card_list[ id ] = new DRAW_CARD_DATA( _draw_card_list[ id ].card_data, _draw_card_list[ id ].obj,
                                                    _draw_card_list[ id ].pos, _draw_card_list[ id ].angle, 
                                                    move, _draw_card_list[ id ].rotate );
    }

    public void setDrawCardMoveTarget( int id, Vector3 pos ) {
        _draw_card_list[ id ] = new DRAW_CARD_DATA( _draw_card_list[ id ].card_data, _draw_card_list[ id ].obj,
                                                    pos, _draw_card_list[ id ].angle, 
                                                    _draw_card_list[ id ].move, _draw_card_list[ id ].rotate );
    }

    public void moveDrawCard( int id ) {
        if ( _draw_card_list[ id ].move ) {
            // 目的地までのベクトルを出す
            Vector3 velocity = _draw_card_list[ id ].pos - _draw_card_list[ id ].obj.transform.position;
            // ベクトルを単位化
            velocity = velocity.normalized;
            // カードの移動
            _draw_card_list[ id ].obj.transform.position += velocity * _draw_card_move_speed;

            float distance = Vector3.Distance( _draw_card_list[ id ].obj.transform.position, _draw_card_list[ id ].pos );
            // 目的地までの距離が微小になったら
            if ( distance < _draw_card_move_speed ) {
                bool move   = false;
                // 座標の修正
                _draw_card_list[ id ].obj.transform.position = _draw_card_list[ id ].pos;

                _draw_card_list[ id ] = new DRAW_CARD_DATA( _draw_card_list[ id ].card_data, _draw_card_list[ id ].obj,
                                                            _draw_card_list[ id ].pos, _draw_card_list[ id ].angle,
                                                            move, _draw_card_list[ id ].rotate );
                _arrived_list.Add( true );
            }
        }
    }

    public bool isArrivedAllDrawCard( ) {
        // 全てのカードが到着したら
        if ( _arrived_list.Count == _draw_card_list.Count ) {
            if ( _draw_card_action == DRAW_CARD_ACTION.MOVE_FOR_GET_ACTION ) {
                _draw_card_action = DRAW_CARD_ACTION.ROTATE_ACTION;
                // 回転スタート
                for ( int i = 0; i < _draw_card_list.Count; i++ ) {
                    rotateStartDrawCard( i );
                }
            } else if ( _draw_card_action == DRAW_CARD_ACTION.MOVE_FOR_HAND_ACTION ) {
                _draw_card_action = DRAW_CARD_ACTION.ACTION_NONE;
                
                for ( int i = 0; i < _draw_card_list.Count; i++ ) {
                    // ハンドに追加
                    addPlayerCard( _draw_card_list[ i ].card_data );
                    // オブジェクトの削除
                    Destroy( _draw_card_list[ i ].obj );
                }
                
                // リフレッシュ
                _draw_card_list.Clear( );
            }

            _arrived_list.Clear( );

            return true;
        }

        return false;
    }
    
    public void rotateStartDrawCard( int id ) {
        bool rotate = true;

        _draw_card_list[ id ] = new DRAW_CARD_DATA( _draw_card_list[ id ].card_data, _draw_card_list[ id ].obj,
                                                    _draw_card_list[ id ].pos, _draw_card_list[ id ].angle, 
                                                    _draw_card_list[ id ].move, rotate );
    }

    public void rotateDrawCard( int id ) {
        if ( _draw_card_list[ id ].rotate ) {
            float angle = _draw_card_list[ id ].angle;
            angle += 10f;

            if ( angle >= 0 ) {
                // 誤差修正
                _draw_card_list[ id ].obj.transform.rotation = Quaternion.Euler( 90, 0, 0 );
                
                bool rotate = false;
                _draw_card_list[ id ] = new DRAW_CARD_DATA( _draw_card_list[ id ].card_data, _draw_card_list[ id ].obj,
                                                            _draw_card_list[ id ].pos, _draw_card_list[ id ].angle, 
                                                            _draw_card_list[ id ].move, rotate );
                _rotate_list.Add( true );

                return;
            }

            // 回転
            _draw_card_list[ id ].obj.transform.rotation = Quaternion.Euler( 90, angle, 0 );

            _draw_card_list[ id ] = new DRAW_CARD_DATA( _draw_card_list[ id ].card_data, _draw_card_list[ id ].obj,
                                                        _draw_card_list[ id ].pos, angle, 
                                                        _draw_card_list[ id ].move, _draw_card_list[ id ].rotate );
        }
    }
    
    public bool isFinishRotateAllDrawCard( ) {
        // 全てのカードが回転したら
        if ( _rotate_list.Count == _draw_card_list.Count ) {
            _draw_card_action = DRAW_CARD_ACTION.MOVE_FOR_HAND_ACTION;
            // 移動スタート
            for ( int i = 0; i < _draw_card_list.Count; i++ ) {
                moveStartDrawCard( i );
            }
            _rotate_list.Clear( );
            return true;
        }

        return false;
    }
    public void addThrowCard( int id ) {
        CARD_DATA card_data = _player_card.hand_list[ id ];

        // オブジェクトの生成
        GameObject obj = Instantiate( _card_obj,
                                      _player_card.hand_obj_list[ id ].transform.position,
                                      _player_card.hand_obj_list[ id ].transform.rotation ) as GameObject;
        obj.GetComponent< Card >( ).setCardData( card_data );

        // ポジションの決定
        Vector3 pos = _player_card.hand_obj_list[ id ].transform.position;
        float pos_y = _player_card.hand_obj_list[ id ].transform.position.y;
        pos_y = _throw_area.transform.position.y;

        pos = new Vector3( pos.x, pos_y, pos.z );

        // フラグの初期化
        bool move = true;

        // 手札から削除
        deletePlayerCardObject( id );
        deletePlayerCardData( id );
        
        THROW_CARD_DATA data = new THROW_CARD_DATA( card_data, obj, pos, move );
		//カードを追加
        _throw_card_list.Add( data );
    }
    public void moveThrowCard( int id ) {
        if ( _throw_card_list[ id ].move ) {
            // 目的地までのベクトルを出す
            Vector3 velocity = _throw_card_list[ id ].pos - _throw_card_list[ id ].obj.transform.position;
            // ベクトルを単位化
            velocity = velocity.normalized;
            // カードの移動
            _throw_card_list[ id ].obj.transform.position += velocity * _draw_card_move_speed;

            float distance = Vector3.Distance( _throw_card_list[ id ].obj.transform.position, _throw_card_list[ id ].pos );
            // 目的地までの距離が微小になったら
            if ( distance < _draw_card_move_speed ) {
                bool move   = false;
                // 座標の修正
                _throw_card_list[ id ].obj.transform.position = _throw_card_list[ id ].pos;

                _throw_card_list[ id ] = new THROW_CARD_DATA( _throw_card_list[ id ].card_data, _throw_card_list[ id ].obj,
                                                              _throw_card_list[ id ].pos, move );
                _arrived_list.Add( true );
            }
        }
    }

    public bool isArrivedAllThrowCard( ) {
        // 全てのカードが到着したら
        if ( _arrived_list.Count == _throw_card_list.Count ) {
            for ( int i = 0; i < _throw_card_list.Count; i++ ) {
                // オブジェクトの削除
                Destroy( _throw_card_list[ i ].obj );
            }
                
            // リフレッシュ
            _throw_card_list.Clear( );

            _arrived_list.Clear( );

            return true;
        }

        return false;
    }

    public int getThrowCardNum( ) {
        return _throw_card_list.Count;
    }
    /// <summary>
	/// 手札にカードを追加する処理
    /// </summary>
    /// <param name="card"></param>
	public void addPlayerCard( CARD_DATA card ) {
		//カードを手札に追加
		_player_card.hand_list.Add( card );
    }

    /// <summary>
    /// 任意の持ち札を手札データから削除する
    /// </summary>
    /// <param name="id"></param>
    private void deletePlayerCardData( int id ) {
		_player_card.hand_list.RemoveAt( id );
    }

    /// <summary>
    /// 手札の更新を行う
    /// </summary>
	public void initAllPlayerCard( ) {
		allDeletePlayerCard( );
		for ( int i = 0; i < _player_card.hand_list.Count; i++ ) {
			//プレハブを生成してリストのオブジェクトに入れる
			_player_card.hand_obj_list.Add( ( GameObject )Instantiate( _card_obj ) );
            _player_card.hand_obj_list[ i ].transform.rotation = Quaternion.Euler( 90,
                                                                                   0,
                                                                                   _player_card.hand_obj_list[ i ].transform.rotation.z );
			//カードデータ設定
			_player_card.hand_obj_list[ i ].GetComponent< Card >( ).setCardData( _player_card.hand_list[ i ] );
			_player_card.hand_obj_list[ i ].GetComponent< Card >( ).changeHandNum( i );
            if ( _play_mode == GAME_PLAY_MODE.MODE_NORMAL_PLAY ) {
			    playerCardPositionSetting( i, false );
            } else if ( _play_mode == GAME_PLAY_MODE.MODE_PLAYER_SELECT ) {
                overHandPlayerCardPositionSetting( i, false );
            }
		}
        _hand_num = _player_card.hand_list.Count;
        _hand_obj_num = _player_card.hand_obj_list.Count;
	}
    
    /// <summary>
    /// 手札の更新を行う
    /// </summary>
	public void updateAllPlayerCard( ) {
		for ( int i = 0; i < _player_card.hand_list.Count; i++ ) {
			//カードデータ設定
            Card card = _player_card.hand_obj_list[ i ].GetComponent< Card >( );
            if ( _play_mode == GAME_PLAY_MODE.MODE_NORMAL_PLAY ) {
			    playerCardPositionSetting( i, card.getSelectFlag( ) );
            } else if ( _play_mode == GAME_PLAY_MODE.MODE_PLAYER_SELECT ) {
                overHandPlayerCardPositionSetting( i, card.getSelectFlag( ) );
            }
		}
	}

	/// <summary>
	/// カードの表示場所を設定
	/// </summary>
	/// <param name="list_id"> 手札ID </param>
	/// <param name="selected"> ture=選択状態 false=！選択状態 </param>
	private void playerCardPositionSetting( int list_id, bool selected ) {
		float hand_area_postion_y = 0.0f;
        
		float start_card_point = _player_card_area_base.transform.position.x - _player_card_area_base.transform.localScale.x / 2;
		float card_potision_x = 0.0f;

		if ( !selected ) {
            card_potision_x = start_card_point + _card_width * list_id;
			hand_area_postion_y = _player_card_area_base.transform.position.y;//位置を設定する
		    _player_card.hand_obj_list[ list_id ].GetComponent< Transform >( ).position = new Vector3( card_potision_x,
                                                                                                       hand_area_postion_y,
                                                                                                       _player_card_area_base.transform.position.z );
		} else {
            //位置を設定する
            int num = _player_card.hand_obj_list[ list_id ].GetComponent< Card >( ).getSelectAreaNum( );
		    _player_card.hand_obj_list[ list_id ].GetComponent< Transform >( ).position = _select_area[ num ];
        }
	}
    
	private void overHandPlayerCardPositionSetting( int list_id, bool selected ) {
		float hand_area_postion_y = 0.0f;
		float card_potision_x = 0.0f;

		if ( !selected ) {
            if ( list_id >= MAX_PLAYER_CARD_NUM ) {
                float start_card_point = _throw_player_card_area_base[ 1 ].transform.position.x - _throw_player_card_area_base[ 1 ].transform.localScale.x / 2;
                card_potision_x = start_card_point + _card_width * ( list_id - MAX_PLAYER_CARD_NUM + 1);
			    hand_area_postion_y = _throw_player_card_area_base[ 1 ].transform.position.y;//位置を設定する
		        _player_card.hand_obj_list[ list_id ].GetComponent< Transform >( ).position = new Vector3( card_potision_x,
                                                                                                           hand_area_postion_y,
                                                                                                           _throw_player_card_area_base[ 1 ].transform.position.z );
            } else {
                float start_card_point = _throw_player_card_area_base[ 0 ].transform.position.x - _throw_player_card_area_base[ 0 ].transform.localScale.x / 2;
                card_potision_x = start_card_point + _card_width * list_id;
			    hand_area_postion_y = _throw_player_card_area_base[ 0 ].transform.position.y;//位置を設定する
		        _player_card.hand_obj_list[ list_id ].GetComponent< Transform >( ).position = new Vector3( card_potision_x,
                                                                                                           hand_area_postion_y,
                                                                                                           _throw_player_card_area_base[ 0 ].transform.position.z );
            }
		} else {
            //位置を設定する
            int num = _player_card.hand_obj_list[ list_id ].GetComponent< Card >( ).getSelectAreaNum( );
		    _player_card.hand_obj_list[ list_id ].GetComponent< Transform >( ).position = _select_throw_area[ num ];
        }
	}
		
    public void allSelectInit( ) {
        for ( int i = 0; i < _player_card.hand_obj_list.Count; i++ ) {
            _player_card.hand_obj_list[ i ].GetComponent< Card >( ).setSelectFlag( false );
        }
    }

	/// <summary>
	/// 手札を全て削除
	/// </summary>
	public void allDeletePlayerCard( ) {
		for ( int i = 0; i < _player_card.hand_obj_list.Count; i++ ) {
			Destroy( _player_card.hand_obj_list[ i ] );
		}
        _player_card.hand_obj_list.Clear( );
	}

    /// <summary>
    /// 任意の持ち札オブジェクトを削除する
    /// </summary>
    /// <param name="id"></param>
    private void deletePlayerCardObject( int id ) {
		Destroy( _player_card.hand_obj_list[ id ] );
		_player_card.hand_obj_list.RemoveAt( id );
	}

	/// <summary>
	/// マウスから飛ばしたレイでカード情報を拾う カードを選択した時の処理
	/// 要マウスクリック判定と併用
	/// </summary>
	/// <returns>The select card.</returns>
	public CARD_DATA getSelectCard( ) {
		CARD_DATA card_data = _card_manager.getCardData( 0 );		//念のためダミーデータを挿入
		/*http://qiita.com/valbeat/items/799a18da3174a6af0b89*/
		float distance = 100f;

		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		// Rayの当たったオブジェクトの情報を格納する
		RaycastHit hit = new RaycastHit( );
		// オブジェクトにrayが当たった時
		if ( Physics.Raycast( ray, out hit, distance ) ) {
			// rayが当たったオブジェクトの名前を取得
			if ( hit.collider.gameObject.name == "Card(Clone)" ) {
				Card card = hit.collider.gameObject.GetComponent< Card >( );
				card_data = card.getCardData( );

                if ( _player_card.select_list.Count >= MAX_PLAYER_CARD_NUM &&
                     !card.getSelectFlag( ) ) {
				    return card_data;
                }
                card.setSelectFlag( !card.getSelectFlag( ) );

                if ( card.getSelectFlag( ) ) {
                    _player_card.select_list.Add( _player_card.hand_list[ card.getHandNum( ) ] );
                    card.changeSelectAreaNum( _player_card.select_list.Count - 1 );
                } else {
                    _player_card.select_list.RemoveAt( card.getSelectAreaNum( ) );
                    
                    int count = card.getSelectAreaNum( ) + 1;
                    for ( int i = 0; i < _player_card.hand_obj_list.Count; i++ ) {
                        for ( int j = count; j < _player_card.select_list.Count + 1; j++ ) {
                            if ( _player_card.hand_obj_list[ i ].GetComponent< Card >( ).getSelectAreaNum( ) == j ) {
                                _player_card.hand_obj_list[ i ].GetComponent< Card >( ).changeSelectAreaNum( j - 1 );
                            }
                        }
                    }
                    card.changeSelectAreaNum( -1 );
                    updateAllPlayerCard( );
                }

				int id = card.getHandNum( );
                if ( _play_mode == GAME_PLAY_MODE.MODE_NORMAL_PLAY ) {
			        playerCardPositionSetting( id, card.getSelectFlag( ) );
                } else if ( _play_mode == GAME_PLAY_MODE.MODE_PLAYER_SELECT ) {
                    overHandPlayerCardPositionSetting( id, card.getSelectFlag( ) );
                }
			}
		}
		return card_data;
	}

	/// <summary>
	/// ダイスの目を決定
	/// </summary>
	public void dicisionDiceValue( ) {
		_dice_value = ( int )Random.Range( MIN_DICE_VALUE, MAX_DICE_VALUE );

		_dice_roll = true;
	}

	/// <summary>
	/// 選択したカードを決定する
	/// </summary>
	/// <returns>The select card.</returns>
	public int[ ] dicisionSelectCard( ) {
        List< int > card_num = new List< int >( );
		for ( int i = 0; i < _player_card.hand_list.Count; i++ ) {
			if ( _player_card.hand_obj_list[ i ].GetComponent< Card >( ).getSelectFlag( ) ) {
                card_num.Add( i );
			}
		}
        
        if ( _player_card.select_list.Count <= MAX_PLAYER_CARD_NUM ) {
            int count = 0;
		    for ( int i = 0; i < card_num.Count; i++ ) {
			    // 選択したカードを削除
		        deletePlayerCardData( card_num[ i ] - count );
		        deletePlayerCardObject( card_num[ i ] - count );
                count++;
		    }
        }
		

		// 選択カードのIDを返す
		int[ ] card_list = new int[ _player_card.select_list.Count ];
		for ( int i = 0; i < _player_card.select_list.Count; i++ ) {
			card_list[ i ] = _player_card.select_list[ i ].id;
		}

		return card_list;
	}
    
	public void dicisionSelectThrowCard( ) {
        List< int > card_num = new List< int >( );

		for ( int i = 0; i < _player_card.hand_list.Count; i++ ) {
			if ( !_player_card.hand_obj_list[ i ].GetComponent< Card >( ).getSelectFlag( ) ) {
                card_num.Add( i );
			}
		}

        // プレイヤーカード数が最大所時数以内だったら
        if ( _player_card.hand_list.Count - card_num.Count <= MAX_PLAYER_CARD_NUM ) {
            int count = 0;
		    for ( int i = 0; i < card_num.Count; i++ ) {
			    // 選択したカードを削除
		        deletePlayerCardData( card_num[ i ] - count );
		        deletePlayerCardObject( card_num[ i ] - count );
                count++;
		    }

            _select_throw_complete = true;
        }
	}

    public bool createExpantionCard( ) {
        bool flag = false;

        if ( !_expansion ) {
            for ( int i = 0; i < _player_card.hand_obj_list.Count; i++ ) {
                if ( _player_card.hand_obj_list[ i ].GetComponent< Card >( ).isMouseOvered( ) ) {
                    // カード生成
			        _expantion_card = ( GameObject )Instantiate( _card_obj );
                    _expantion_card.transform.position = _expantion_card_area.transform.position;
                    _expantion_card.transform.localScale = new Vector3( _card_obj.transform.localScale.x * EXPANTION_MAGNIFICATION,
                                                                        1, _card_obj.transform.localScale.z * EXPANTION_MAGNIFICATION );
                    _expantion_card.transform.rotation = Quaternion.Euler( 90,  0, _player_card.hand_obj_list[ i ].transform.rotation.z );
			        //カードデータ設定
			        _expantion_card.GetComponent< Card >( ).setCardData( _player_card.hand_list[ i ] );
			        _expantion_card.GetComponent< Card >( ).changeHandNum( i );

                    _expantion_num = i;
                    _expansion = true;
                    flag = true;
                    continue;
                }
            }
        }

        return flag;
    }

    public void destroyExpantionCard( ) {
        Destroy( _expantion_card );
        _player_card.hand_obj_list[ _expantion_num ].GetComponent< Card >( ).mouseOverFinish( );

        _expantion_num = -1;
        _expansion = false;
    }

    public bool isExpantion( ) {
        return _expansion;
    }

    public int getPower( ) {
        return _power;
    } 

    public void setPower( int power ) {
        _player_data.power = power;
    }

    public int getHandNum( ) {
        return _hand_num;
    }

	public void refreshSelectCard( ) {
		_player_card.select_list.Clear( );
	}

    public void playerCardEnable( bool flag ) { 
        _profile_card_obj.SetActive( flag );
    }

    public bool isDrawCard( ) {
        return _draw_card;
    }

    public int getDrawCardNum( ) {
        return _draw_card_list.Count;
    }

    public GameObject getPlayerCardArea( ) {
        return _player_card_area_base;
    }

    public float getCardWidth( ) {
        return _card_width;
    }

    public void setDrawCardFlag( bool flag ) {
        _draw_card = flag;
    }

    public bool isThrowCard( ) {
        return _throw_card;
    }

    public void setThrowCard( bool flag ) {
        _throw_card = flag;
    }

	/// <summary>
	/// ダイスの目を返す
	/// </summary>
	/// <returns>The dice value.</returns>
	public int getDiceValue( ) {
		return _dice_value;
	}

	/// <summary>
	/// ダイスの目を初期化
	/// </summary>
	public void initDiceValue( ) {
		_dice_value = 0;
	}

    public void cardListVisible( bool flag ) {
        for ( int i = 0; i < _player_card.hand_obj_list.Count; i++ ) {
            _player_card.hand_obj_list[ i ].SetActive( flag );
        }
    }

    public void setPlayMode( GAME_PLAY_MODE mode ) {
        _play_mode = mode;
    }

    public DRAW_CARD_ACTION getDrawCardAction( ) {
        return _draw_card_action;
    }

	public PLAYER_DATA getPlayerData( ) {
		return _player_data;
	}

	/// <summary>
	/// プレイヤーの手札の枚数を返す
	/// </summary>
	/// <returns>The player card number.</returns>
	public int getPlayerCardNum( ) {
		return _player_card.hand_list.Count;
	}

    public int getMaxPlayerCardNum( ) {
        return MAX_PLAYER_CARD_NUM;
    }

	/// <summary>
	/// ダイスをふったかどうかを返す
	/// </summary>
	/// <returns><c>true</c>, if dice roll was ised, <c>false</c> otherwise.</returns>
	public bool isDiceRoll( ) {
		if ( _dice_roll == true ) {
			_dice_roll = false;
			return true;
		}

		return false;
	}

	/// <summary>
	/// マウスの左クリックの状態を取得
	/// </summary>
	/// <returns><c>true</c>, if click was moused, <c>false</c> otherwise.</returns>
	public bool mouseClick( ) {
		bool flag = false;

		if ( Input.GetMouseButtonDown( 0 ) ) {
			flag = true;
		}

		return flag;
	}

    public bool isSelectThrowComplete( ) {
        if ( _select_throw_complete ) {
            _select_throw_complete = false;
            return true;
        }
        
        return false;
    }

    public void destroyObj( ) {
        allDeletePlayerCard( );
        Destroy( _profile_card_obj );

        _player_card.hand_obj_list.Clear( );
        _player_card.hand_list.Clear( );
        _player_card.select_list.Clear( );
        _player_card.select_position.Clear( );
    }
}
