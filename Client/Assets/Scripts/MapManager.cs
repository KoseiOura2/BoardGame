using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

    private const int MASS_TYPE_NUM = 5;

    private GameObject[ ] _mass_pref = new GameObject[ MASS_TYPE_NUM ];
    private GameObject _base_point;
    private List< GameObject > _mass_obj = new List< GameObject >( );
    private Vector3 _player_point;
    private int _target_mass_id;
    //private List< Vector3 > _mass_pos = new List< Vector3 >( );
    private int _create_mass_count = 0;
    private int _player_pos_num = 0;
    [ SerializeField ]
    private float _adjust_mass_pos = 3.0f;
    private float _purpose_distance_x = 0.0f;
    private float _purpose_distance_y = 0.0f;
    private float _move_speed = 0.3f;
    private bool _move = false;

	// Use this for initialization
	void Start( ) {
	
	}

    public void init( ) {
        loadMapGraphic( );

        if ( _base_point == null ) {
            _base_point = GameObject.Find( "MassBasePoint" );
        }

        if ( _player_point == null ) {
            _player_point = GameObject.Find( "PlayerPoint" ).transform.position;
        }
    }
    

    public void createMassObj( int num, string type, Vector3 pos ) {
        GameObject pref = null;

		// タイプによるリソース分け
		switch ( type ) {
			case "start":
			case "goal":
				pref = _mass_pref[ 4 ];
                break;
            case "draw":
            case "advance":
				pref = _mass_pref[ 1 ];
                break;
			case "trap1":
			case "trap2":
				pref = _mass_pref[ 3 ];
				break;
			case "event":
				pref = _mass_pref[ 2 ];
                break;
        }

		// 生成
		GameObject obj = ( GameObject )Instantiate( pref );
		obj.name = "Mass:ID" + num;
        
        obj.transform.SetParent( _base_point.transform );
        obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
        obj.GetComponent< RectTransform >( ).localScale = new Vector3( 0.2f, 0.2f, 1 );

        float x = _adjust_mass_pos * pos.x;
        float y = _adjust_mass_pos * pos.z;
        Vector3 adjust_pos = new Vector3( x, y, pref.transform.position.z );
        obj.GetComponent< RectTransform >( ).localPosition = adjust_pos;
        
        obj.GetComponent< Button >( ).onClick.AddListener( obj.GetComponent< Mass >( ).selectedOnClick );

        _mass_obj.Add( obj );
    }
    
	public void createMiniMass( ) {
		for( int i = 0; i < _mass_obj.Count - 1; i++ ) {

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
        float[ ] x = new float[ _mass_obj.Count ];
        float[ ] y = new float[ _mass_obj.Count ];

        // x座標を動かす
        if ( _purpose_distance_x < 0 ) {
            if ( _mass_obj[ _player_pos_num ].transform.position.x >= _player_point.x ) {
                for ( int i = 0; i < _mass_obj.Count; i++ ) {
                    x[ i ] = _mass_obj[ i ].transform.position.x;
                }
                x_finish = true;
            } else {
                for ( int i = 0; i < _mass_obj.Count; i++ ) {
                    x[ i ] = _mass_obj[ i ].transform.position.x;
                    x[ i ] += _move_speed;
                }
            }
        } else {
            if ( _mass_obj[ _player_pos_num ].transform.position.x <= _player_point.x ) {
                for ( int i = 0; i < _mass_obj.Count; i++ ) {
                    x[ i ] = _mass_obj[ i ].transform.position.x;
                }
                x_finish = true;
            } else {
                for ( int i = 0; i < _mass_obj.Count; i++ ) {
                    x[ i ] = _mass_obj[ i ].transform.position.x;
                    x[ i ] -= _move_speed;
                }
            }
        }
        // y座標を動かす
        if ( _purpose_distance_y < 0 ) {
            if ( _mass_obj[ _player_pos_num ].transform.position.y >= _player_point.y ) {
                for ( int i = 0; i < _mass_obj.Count; i++ ) {
                    y[ i ] = _mass_obj[ i ].transform.position.y;
                }
                y_finish = true;
            } else {
                for ( int i = 0; i < _mass_obj.Count; i++ ) {
                    y[ i ] = _mass_obj[ i ].transform.position.y;
                    y[ i ] += _move_speed;
                }
            }
        } else {
            if ( _mass_obj[ _player_pos_num ].transform.position.y <= _player_point.y ) {
                for ( int i = 0; i < _mass_obj.Count; i++ ) {
                    y[ i ] = _mass_obj[ i ].transform.position.y;
                }
                y_finish = true;
            } else {
                for ( int i = 0; i < _mass_obj.Count; i++ ) {
                    y[ i ] = _mass_obj[ i ].transform.position.y;
                    y[ i ] -= _move_speed;
                }
            }
        }

        // 座標の設定
        for ( int i = 0; i < _mass_obj.Count; i++ ) {
            _mass_obj[ i ].transform.position = new Vector3( x[ i ], y[ i ], _mass_obj[ i ].transform.position.z );
        }

        if ( x_finish && y_finish ) {
            _move = false;
        }
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

    public bool isMassMove( ) {
        return _move;
    }
}
