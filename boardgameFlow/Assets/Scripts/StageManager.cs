using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class StageManager : MonoBehaviour {

	private Color SHOAL_COLOR       = new Color( 171.0f / 255.0f, 219.0f / 255.0f, 227.0f / 255.0f );		// 浅瀬の色
	private Color MESOPELAGIC_COLOR = new Color( 117.0f / 255.0f, 198.0f / 255.0f, 195.0f / 255.0f );		// 中間の色
	private Color DEEP_SEA_COLOR    = new Color(   0.0f / 255.0f,  72.0f / 255.0f, 103.0f / 255.0f );		// 深海の色
	private Color GOAL_COLOR        = new Color(  17.0f / 255.0f,  43.0f / 255.0f,  76.0f / 255.0f );		// ｺﾞｰﾙの色

	private const float SHOAL_SEA_DEPTH   =    0.0f;
	private const float MESOPELAGIC_DEPTH =  200.0f;
	private const float DEEP_SEA_DEPTH    = 1000.0f;
	private const float GOAL_DEPTH        = 1200.0f;

	[ SerializeField ]
	private List< GameObject > _mass_list = new List< GameObject >( );	//マスデータをリストとして保持しておく
	private GameObject _mass_prefab;	//マスデータロード用関数
    private GameObject _arrangement_prefab;
	[ SerializeField ]
	private Light _main_light;
    private int _create_mass_count = 0;
	private Color[ ] _sea_color = new Color[ 4 ];
	private int[ ] _sea_environment_id = new int[ ( int )FIELD_ENVIRONMENT.FIELD_ENVIRONMENT_NUM ]{ 0, 0, 0 };
	private FIELD_ENVIRONMENT _environment = FIELD_ENVIRONMENT.NO_FIELD;

	public void init( ) {
		_sea_color = new Color[ ]{ SHOAL_COLOR, MESOPELAGIC_COLOR, DEEP_SEA_COLOR, GOAL_COLOR };

		for( int i = 0; i < _mass_list.Count - 1; i++ ) {
			_mass_prefab = ( GameObject )Resources.Load( "Prefabs/Mass/mass_mini" );
			Vector3 pos = Vector3.Lerp( _mass_list[ i ].transform.localPosition, _mass_list[ i + 1 ].transform.localPosition, 0.5f );
			GameObject obj = ( GameObject )Instantiate( _mass_prefab, pos, _mass_prefab.transform.localRotation );
			obj.transform.parent = transform;
		}

		GameObject light = GameObject.Find( "MainLight" ); 
		_main_light = light.GetComponent< Light >( );

		_main_light.color = SHOAL_COLOR;
	}

	// Use this for initialization
	void Start( ) {
	
	}

    void massUpdate( ) {
        
    }

    //ゲーム開始時マスを生成
	public void massCreate( int num, MASS_TYPE type, EVENT_TYPE event_type, Vector3 pos ) {
		// タイプによるリソース分け
		switch ( type ) {
			case MASS_TYPE.MASS_START:
			case MASS_TYPE.MASS_GOAL:
				_mass_prefab = ( GameObject )Resources.Load( "Prefabs/Mass/mass_yellow" );
                break;
            case MASS_TYPE.MASS_NORMAL:
				_mass_prefab = ( GameObject )Resources.Load( "Prefabs/Mass/mass_blue" );
                break;
			case MASS_TYPE.MASS_DENGER:
				_mass_prefab = ( GameObject )Resources.Load( "Prefabs/Mass/mass_red" );
				break;
			case MASS_TYPE.MASS_EVENT:
				_mass_prefab = ( GameObject )Resources.Load( "Prefabs/Mass/mass_green" );
                break;
        }

		// 生成
		GameObject obj = ( GameObject )Instantiate( _mass_prefab, pos, _mass_prefab.transform.localRotation );
		obj.name = "Mass:ID" + num;

        switch( event_type ) {
            case EVENT_TYPE.EVENT_DRAW:
                _arrangement_prefab = ( GameObject )Resources.Load( "Prefabs/BackGroundObj/object_chest" );
                GameObject obj_arrangement = ( GameObject )Instantiate( _arrangement_prefab, new Vector3(obj.transform.localPosition.x,obj.transform.localPosition.y,obj.transform.localPosition.z + 2), _arrangement_prefab.transform.rotation );
                break; 
        }

        // マネージャーの配下に設定
        obj.transform.parent = transform;
        _mass_list.Add( obj );
    }

	public void updateLightColor( FIELD_ENVIRONMENT environment, int mass_id ) {
		int mass_num = 0;
		int player_pos = 0;
		float r = 0.0f;
		float g = 0.0f;
		float b = 0.0f;

		// 区間のマスの数と現在地を測定
		if ( environment == FIELD_ENVIRONMENT.DEEP_SEA_FIELD ) {
			mass_num   = getMassCount( ) - _sea_environment_id[ ( int )environment ];
			player_pos = getMassCount( ) - mass_id;
			r = ( _sea_color[ _sea_color.Length - 1 ].r - _sea_color[ ( int )environment ].r ) / mass_num;
			g = ( _sea_color[ _sea_color.Length - 1 ].g - _sea_color[ ( int )environment ].g ) / mass_num;
			b = ( _sea_color[ _sea_color.Length - 1 ].b - _sea_color[ ( int )environment ].b ) / mass_num;
		} else {
			mass_num   = _sea_environment_id[ ( int )environment + 1 ] - _sea_environment_id[ ( int )environment ] + 1;
			player_pos = _sea_environment_id[ ( int )environment + 1 ] - mass_id;
			r = ( _sea_color[ ( int )environment + 1 ].r - _sea_color[ ( int )environment ].r ) / mass_num;
			g = ( _sea_color[ ( int )environment + 1 ].g - _sea_color[ ( int )environment ].g ) / mass_num;
			b = ( _sea_color[ ( int )environment + 1 ].b - _sea_color[ ( int )environment ].b ) / mass_num;
		}

		// カラーの設定
		_main_light.color = new Color( _sea_color[ ( int )environment + 1 ].r - r * player_pos,
									   _sea_color[ ( int )environment + 1 ].g - g * player_pos,
									   _sea_color[ ( int )environment + 1 ].b - b * player_pos );

	}

	public void setEnvironmentID( int id, FIELD_ENVIRONMENT environment ) {
		_sea_environment_id[ ( int )environment ] = id;
	}

    //進む先のマスを取得
    public GameObject getTargetMass( int i ) {
        return _mass_list[ i ];
    }

	public void increaseMassCount( ) {
		_create_mass_count++;
	}

	public int getMassCount( ) {
		return _create_mass_count;
	}

	public void setEnvironment( FIELD_ENVIRONMENT environment ) {
		_environment = environment;
	}

	public FIELD_ENVIRONMENT getEnvironment( ) {
		return _environment;
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    public void resetMassColor ( int i, ref bool flag ) {
        if ( getTargetMass ( i ).transform.localScale.x > 0.3f || getTargetMass ( i ).transform.localScale.z > 0.3f ) {
            getTargetMass ( i ).GetComponent<Renderer> ( ).material.SetColor ( "_Color", new Color ( 0.4f, 0.4f, 0.4f, 1f ) );
            getTargetMass ( i ).transform.localScale =
            new Vector3 ( getTargetMass ( i ).transform.localScale.x - 0.05f,
            getTargetMass ( i ).transform.localScale.y,
            getTargetMass ( i ).transform.localScale.z - 0.05f );
        } else {
            getTargetMass ( i ).transform.localScale =
            new Vector3 ( 0.3f, getTargetMass ( i ).transform.localScale.y, 0.3f );
            flag = false;
        }
    }
}
