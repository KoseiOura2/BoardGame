using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

    private const int MASS_TYPE_NUM = 5;

    private GameObject[ ] _mass_pref = new GameObject[ MASS_TYPE_NUM ];
    private GameObject _base_point;
    private List< GameObject > _mass_obj = new List< GameObject >( );
    //private List< Vector3 > _mass_pos = new List< Vector3 >( );
    private int _create_mass_count = 0;
    [ SerializeField ]
    private float _adjust_mass_pos = 3.0f;

	// Use this for initialization
	void Start( ) {
	
	}

    public void init( ) {
        loadMapGraphic( );

        if ( _base_point == null ) {
            _base_point = GameObject.Find( "MassBasePoint" );
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
        float y = _adjust_mass_pos * pos.y;
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

	public void increaseMassCount( ) {
		_create_mass_count++;
	}

	public int getMassCount( ) {
		return _create_mass_count;
	}
}
