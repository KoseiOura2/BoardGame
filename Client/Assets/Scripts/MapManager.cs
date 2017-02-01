using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;

public class MapManager : MonoBehaviour {

    private const int MASS_TYPE_NUM = 5;

    public struct MASS_DATA {
        public MASS_TYPE mass_type;
        public EVENT_TYPE event_type;
        public int normal_value;
        public int trap_value;

        public MASS_DATA( int init ) {
            this.mass_type    = MASS_TYPE.MASS_NONE;
            this.event_type   = EVENT_TYPE.EVENT_NONE;
            this.normal_value = init;
            this.trap_value   = init;
        }
    };
    
    private Sprite _player_icon;
	private Sprite[ ] _small_num = new Sprite[ 10 ];
    private GameObject[ ] _mass_pref = new GameObject[ MASS_TYPE_NUM ];
	private GameObject _base_point;
	private GameObject _player_point;
    private List< GameObject > _mass_obj = new List< GameObject >( );
    private List< MASS_DATA > _mass_list = new List< MASS_DATA >( );
    private List< Vector3 > _mass_pos = new List< Vector3 >( );
    private int _target_mass_id;
    private int _create_mass_count = 0;
	private int _player_pos_num = 0;
	[ SerializeField ]
	private int _sea_deep = 0;
    [ SerializeField ]
    private float _adjust_mass_pos = 3.0f;
    private float _purpose_distance_x = 0.0f;
    private float _purpose_distance_y = 0.0f;
    private float _move_speed = 1.0f;
    private bool _move = false;

	// Use this for initialization
	void Start( ) {
	
	}

    public void init( ) {
        loadMapGraphic( );
		loadNumGraphics( );
		initSeaDeep( );

        if ( _base_point == null ) {
            _base_point = GameObject.Find( "MassBasePoint" );
        }

        if ( _player_point == null ) {
            _player_point = GameObject.Find( "PlayerPoint" );
        }
    }
    

    public void createMassObj( int num, MASS_TYPE mass_type, EVENT_TYPE event_type, Vector3 pos, int value_1, int value_2 ) {
        MASS_DATA data = new MASS_DATA( 0 );
        GameObject pref = null;
		// タイプによるリソース分け
		switch ( mass_type ) {
			case MASS_TYPE.MASS_START:
			case MASS_TYPE.MASS_GOAL:
				pref = _mass_pref[ 4 ];
                break;
			case MASS_TYPE.MASS_NORMAL:
				pref = _mass_pref[ 1 ];
                break;
            case MASS_TYPE.MASS_DENGER:
				pref = _mass_pref[ 3 ];
				break;
			case MASS_TYPE.MASS_EVENT:
				pref = _mass_pref[ 2 ];
                break;
        }

		// 生成
		GameObject obj = ( GameObject )Instantiate( pref );
		obj.name = "Mass:ID" + num;
        obj.transform.SetParent( _base_point.transform );
        obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
        obj.GetComponent< RectTransform >( ).localScale = new Vector3( 0.2f, 0.2f, 1 );
        
        // 座標の設定
        float x = _adjust_mass_pos * pos.x;
        float y = _adjust_mass_pos * pos.z;
        Vector3 adjust_pos = new Vector3( x, y, pref.transform.position.z );
        _mass_pos.Add( adjust_pos );
        obj.GetComponent< RectTransform >( ).localPosition = adjust_pos;
        // ボタンにイベントを追加
        obj.GetComponent< Button >( ).onClick.AddListener( obj.GetComponent< Mass >( ).selectedOnClick );
        _mass_obj.Add( obj );

        data.mass_type  = mass_type;
        data.event_type = event_type;

        // 値の設定
        data.normal_value = value_1;
        data.trap_value   = value_2;

        _mass_list.Add( data );
    }
    
	public void createMiniMass( ) {
		for( int i = 0; i < _mass_list.Count - 1; i++ ) {

			GameObject obj = ( GameObject )Instantiate( _mass_pref[ 0 ] );
            
            obj.transform.SetParent( _base_point.transform );
            obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
			Vector3 pos = Vector3.Lerp( _mass_obj[ i ].GetComponent< RectTransform >( ).localPosition, _mass_obj[ i + 1 ].GetComponent< RectTransform >( ).localPosition, 0.5f );
            pos = new Vector3( pos.x, pos.y, _mass_pref[ 0 ].transform.position.z );
            obj.GetComponent< RectTransform >( ).localPosition = pos;

            obj.transform.SetParent( _mass_obj[ i ].transform );
            obj.GetComponent< RectTransform >( ).localScale = new Vector3( 0.3f, 0.3f, 1 );
            
		}
	}

    private void loadMapGraphic( ) {
        for ( int i = 0; i < MASS_TYPE_NUM; i++ ) {
            _mass_pref[ i ] = Resources.Load< GameObject >( "Prefabs/UI/Mass/ui_map_mass" + i );
        }
    }

	private void loadNumGraphics( ) {
		for ( int i = 0; i < _small_num.Length; i++ ) {
			_small_num[ i ] = Resources.Load< Sprite >( "Graphics/Number/number_status_" + i );
		}
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    /// <summary>
    /// ターゲットの設定
    /// </summary>
    /// <param name="player_pos"></param>
    public void dicisionMoveTarget( int player_pos ) {
        _purpose_distance_x = _mass_obj[ _player_pos_num ].transform.position.x - _mass_obj[ player_pos ].transform.position.x;
        _purpose_distance_y = _mass_obj[ _player_pos_num ].transform.position.y - _mass_obj[ player_pos ].transform.position.y;
        _player_pos_num = player_pos;
        _move = true;
    }

    public void massMove( ) {
        bool x_finish = false;
        bool y_finish = false;
        float[ ] x = new float[ _mass_list.Count ];
        float[ ] y = new float[ _mass_list.Count ];

        // x座標を動かす
        if ( _purpose_distance_x > 0 ) {
            if ( _mass_obj[ _player_pos_num ].transform.position.x >= _player_point.transform.position.x ) {
                for ( int i = 0; i < _mass_list.Count; i++ ) {
                    x[ i ] = _mass_obj[ i ].transform.position.x;
                }
                x_finish = true;
            } else {
                for ( int i = 0; i < _mass_list.Count; i++ ) {
                    x[ i ] = _mass_obj[ i ].transform.position.x;
                    x[ i ] += _move_speed;
                }
            }
        } else if ( _purpose_distance_x < 0 ) {
            if ( _mass_obj[ _player_pos_num ].transform.position.x <= _player_point.transform.position.x ) {
                for ( int i = 0; i < _mass_list.Count; i++ ) {
                    x[ i ] = _mass_obj[ i ].transform.position.x;
                }
                x_finish = true;
            } else {
                for ( int i = 0; i < _mass_list.Count; i++ ) {
                    x[ i ] = _mass_obj[ i ].transform.position.x;
                    x[ i ] -= _move_speed;
                }
            }
        } else if ( _purpose_distance_x == 0 ) {
            for ( int i = 0; i < _mass_list.Count; i++ ) {
                x[ i ] = _mass_obj[ i ].transform.position.x;
            }
            x_finish = true;
        }
        // y座標を動かす
        if ( _purpose_distance_y > 0 ) {
            if ( _mass_obj[ _player_pos_num ].transform.position.y >= _player_point.transform.position.y ) {
                for ( int i = 0; i < _mass_list.Count; i++ ) {
                    y[ i ] = _mass_obj[ i ].transform.position.y;
                }
                y_finish = true;
            } else {
                for ( int i = 0; i < _mass_list.Count; i++ ) {
                    y[ i ] = _mass_obj[ i ].transform.position.y;
                    y[ i ] += _move_speed;
                }
            }
        } else if ( _purpose_distance_y < 0 ) {
            if ( _mass_obj[ _player_pos_num ].transform.position.y <= _player_point.transform.position.y ) {
                for ( int i = 0; i < _mass_list.Count; i++ ) {
                    y[ i ] = _mass_obj[ i ].transform.position.y;
                }
                y_finish = true;
            } else {
                for ( int i = 0; i < _mass_list.Count; i++ ) {
                    y[ i ] = _mass_obj[ i ].transform.position.y;
                    y[ i ] -= _move_speed;
                }
            }
        } else if ( _purpose_distance_y == 0 ) {
            for ( int i = 0; i < _mass_list.Count; i++ ) {
                y[ i ] = _mass_obj[ i ].transform.position.y;
            }
            y_finish = true;
        }

        // 座標の設定
        for ( int i = 0; i < _mass_list.Count; i++ ) {
            _mass_obj[ i ].transform.position = new Vector3( x[ i ], y[ i ], _mass_obj[ i ].transform.position.z );
        }

        if ( x_finish && y_finish ) {
            _move = false;
            massPosAdustBasePos( );
        }
    }

    public void massPosAdustBasePos( ) {
        // 移動量を計算
        float distance_x = _player_point.transform.position.x - _mass_pos[ _player_pos_num ].x;
        float distance_y = _player_point.transform.position.y - _mass_pos[ _player_pos_num ].y;

        // 座標を修正
        for ( int i = 0; i < _mass_list.Count; i++ ) {
            _mass_obj[ i ].transform.position = new Vector3( _mass_pos[ i ].x + distance_x,
                                                             _mass_pos[ i ].y + distance_y,
                                                             _mass_obj[ i ].transform.position.z );
        }
    }

	public void initSeaDeep( ) {
		_sea_deep = 0;
	}

	public void adbanceSea( ) {
		_sea_deep = _player_pos_num * 10;
	}

    public void allMassReject( ) {
        for ( int i = 0; i < _mass_list.Count; i++ ) {
            _mass_obj[ i ].GetComponent< Mass >( ).changeReject( true );
        }
    }

    public void setMassNotReject( int id ) {
        _mass_obj[ id ].GetComponent< Mass >( ).changeReject( false );
    }

    public void setMassColor( int id, Color color ) {
        _mass_obj[ id ].GetComponent< Image >( ).color = color;
    }

    public void allMassVisible( bool flag ) {
        for ( int i = 0; i < _mass_list.Count; i++ ) {
            _mass_obj[ i ].GetComponent< Image >( ).enabled  = flag;
            _mass_obj[ i ].GetComponent< Button >( ).enabled = flag;
            try {
                /*
                _mass_obj[ i ].GetComponentInChildren< Image >( false ).enabled = flag;
                bool a = _mass_obj[ i ].GetComponentInChildren< Image >( ).enabled;
                string b = _mass_obj[ i ].GetComponentInChildren< Image >( ).gameObject.name;
                */
                if ( i != _mass_list.Count - 1 ) {
                    _mass_obj[ i ].transform.GetChild( 0 ).gameObject.GetComponent< Image >( ).enabled = flag;
                }
            }
            catch {
                Debug.Log( "Missing Image.." );
            }
        }
    }

    public void bindSprite( PLAYER_ORDER player_num ) {
        if ( player_num == PLAYER_ORDER.PLAYER_ONE ) {
            _player_icon = Resources.Load< Sprite >( "Graphics/UI/Player/ui_location_player_1P" );
        } else if ( player_num == PLAYER_ORDER.PLAYER_TWO ) {
            _player_icon = Resources.Load< Sprite >( "Graphics/UI/Player/ui_location_player_2P" );
        }

        _player_point.transform.GetChild( 0 ).gameObject.GetComponent< Image >( ).sprite = _player_icon;

        _player_point.transform.GetChild( 0 ).gameObject.GetComponent< Image >( ).SetNativeSize( );
        _player_point.transform.GetChild( 0 ).gameObject.GetComponent< Image >( ).color = new Color( 1, 1, 1, 1 );
    }

    public void freeSprite( ) {
        _player_point.transform.GetChild( 0 ).gameObject.GetComponent< Image >( ).sprite = null;
    }

	public void changeGoalImageNum( GameObject ten_digit, GameObject digit ) {
		// ゴールまでの残りマス
		int num = _create_mass_count - _player_pos_num;

		// 10の位を求める
		int ten = ( int )( num / 10 );
		ten = ten % 10;
		// 1の位を求める
		int one = num % 10;

		// イメージのSpriteを変える
		ten_digit.GetComponent< Image >( ).sprite = _small_num[ ten ];
		digit.GetComponent< Image >( ).sprite     = _small_num[ one ];
	}

	public void changeSeaDeepNum( GameObject hundred_digit, GameObject ten_digit, GameObject digit ) {
		// 100の位を求める
		int hundred = ( int )( _sea_deep / 100 );
		hundred = hundred % 10;
		// 10の位を求める
		int ten = ( int )( _sea_deep / 10 );
		ten = ten % 10;
		// 1の位を求める
		int one = _sea_deep % 10;

		// イメージのSpriteを変える
		hundred_digit.GetComponent< Image >( ).sprite = _small_num[ hundred ];
		ten_digit.GetComponent< Image >( ).sprite     = _small_num[ ten ];
		digit.GetComponent< Image >( ).sprite         = _small_num[ one ];
	}

    public void setVisibleSprite( bool flag ) {
        _player_point.transform.GetChild( 0 ).gameObject.GetComponent< Image >( ).enabled = flag;
    }

	public void increaseMassCount( ) {
		_create_mass_count++;
	}

	public int getMassCount( ) {
		return _create_mass_count;
	}

    public int getPlayerPosNum( ) {
        return _player_pos_num;
    }

    public MASS_DATA getOveredMassData( ) {
        MASS_DATA data = new MASS_DATA( 0 );

        for ( int i = 0; i < _mass_list.Count; i++ ) {
            if ( _mass_obj[ i ].GetComponent< Mass >( ).isMouseOvered( ) ) {
                data = _mass_list[ i ];
                return data;
            }
        }

        return data;
    }

    public bool isMassMove( ) {
        return _move;
    }

    public int isSelect( ) {
        int num = -1;

        for ( int i = 0; i < _mass_list.Count; i++ ) {
            if ( _mass_obj[ i ].GetComponent< Mass >( ).isSelected( ) ) {
                num = i;
                return num;
            }
        } 

        return num;
    }

    public void destroyObj( ) {
        for ( int i = 0; i < _mass_obj.Count; i++ ) {
            Destroy( _mass_obj[ i ] );
        }
        freeSprite( );

        _mass_list.Clear( );
        _mass_obj.Clear( );
        _mass_pos.Clear( );
        _target_mass_id = 0;
        _create_mass_count = 0;
        _player_pos_num = 0;
        _sea_deep = 0;
    }
}
