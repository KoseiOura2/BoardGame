using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Common;

public class ClientPlayerManager : MonoBehaviour {

	private const float MAX_DICE_VALUE   = 3.9f;
	private const float MIN_DICE_VALUE   = 1.0f;
	private const int INIT_PLAYER_POWER  = 10;
    private const int MAX_PLAYER_CARD_NUM = 6;

	/// <summary>
	/// プレイヤーの持つカードのデータ
	/// </summary>
	private struct PLAYER_CARD_DATA {
		public List< CARD_DATA >  hand_list;
		public List< GameObject > hand_obj_list;
		public List< Vector3 >    select_position;
		public List< CARD_DATA >  select_list;
	}

	[ SerializeField ]
	private CardManager _card_manager;
	[ SerializeField ]
	private PLAYER_CARD_DATA _player_card = new PLAYER_CARD_DATA( );
	private PLAYER_DATA _player_data;

	private GameObject _player_card_area_base;
	private GameObject _select_area_base;
    private Vector3[ ] _select_area = new Vector3[ MAX_PLAYER_CARD_NUM ];
	private GameObject[ ] _throw_player_card_area_base = new GameObject[ 2 ];
	private GameObject _select_throw_area_base;
    private Vector3[ ] _select_throw_area = new Vector3[ MAX_PLAYER_CARD_NUM ];

    private GAME_PLAY_MODE _play_mode = GAME_PLAY_MODE.MODE_NORMAL_PLAY;

    private float _card_width = 3.0f;

	private int _dice_value = 0;
	private bool _dice_roll = false;
    private bool _select_throw_complete = false;

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

	// Use this for initialization
	void Start( ) {
		if ( _player_card_area_base == null ) {
			_player_card_area_base = GameObject.Find( "HandArea" );
		}
		if ( _select_area_base == null ) {
			_select_area_base = GameObject.Find( "SelectHandArea" );
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


		// プレイヤーの初期化
		_player_card.hand_list       = new List< CARD_DATA >( );
		_player_card.hand_obj_list   = new List< GameObject >( );
		_player_card.select_position = new List< Vector3 >( );
		_player_card.select_list     = new List< CARD_DATA >( );

		_player_data.power = INIT_PLAYER_POWER;
	}
	
	/// <summary>
    /// エディタ上でのみデバッグ機能が実行される
    /// </summary>
	#if UNITY_EDITOR
	void Update( ) {
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

    /// <summary>
	/// 手札にカードを追加する処理(追加用のカードIDを登録)
    /// </summary>
    /// <param name="get_card_id"></param>
	public void addPlayerCard( int get_card_id ) {
		CARD_DATA card;

		//IDのカードデータを取得
		card = _card_manager.getCardData( get_card_id );
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
			//カードデータ設定
            if ( _player_card.hand_list[ i ].id < 0 ) {
                Debug.Log( "korosu" );
            }
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
        
        if ( _player_card.select_list.Count <= 4 ) {
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

	public void refreshSelectCard( ) {
		_player_card.select_list.Clear( );
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

    public void setPlayMode( GAME_PLAY_MODE mode ) {
        _play_mode = mode;
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
}
