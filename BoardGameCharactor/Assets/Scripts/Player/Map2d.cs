using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map2d : MonoBehaviour {

	//スタートマスの初期位置X
	[SerializeField]
	private float _start_masu_x = -282;
	[SerializeField]
	private float _start_masu_y = 0;
	//マスとの距離
	[SerializeField]
	private float _mini_masu_while_x = 93.0f;
	[SerializeField]
	private float _mass_while_x = 186;
	//壁の橋とマスの間
	private float _mass_while = -16;

	private FileManager _file_manager;
	private GameObject _mass_prefab;
	private GameObject _mini_mass_prefab;

	public Transform _contect;

	private int _create_mass_count = 0;

	private Vector2 _end_position;

	// Use this for initialization
	void Awake () {
		if ( !_mass_prefab ) {
			_mass_prefab = (GameObject)Resources.Load ("Prefab/masu");
		}
		if ( !_mini_mass_prefab ) {
			_mini_mass_prefab = (GameObject)Resources.Load ("Prefab/masu_mini");
		}
        if( !_file_manager ) {
            _file_manager = GameObject.Find ( "FileManager" ).GetComponent<FileManager> ( );
        }

        if ( isError( ) ) {
			return;
		}

		//マスの生成
		for (int i = 0; i < _file_manager.getMassCount (); i++) {
			massCreate (_create_mass_count );
			_create_mass_count++;
		}

		//ミニマスの生成
		for (int i = 0; i < _file_manager.getMassCount () - 1 ; i++) {
			miniMassCreate (i);
		}
        //最大マスを取得
		int max_Mass = _file_manager.getMassCount ();

        //最後のマスのポジションを取得
		_end_position = new Vector2( ( -_mass_while_x - _mass_while ) * (float)max_Mass , _start_masu_y ) ;
	}
	
	bool isError( ) {
		bool error = false;

		if ( !_file_manager ) {
			try {
				error = true;
				_file_manager = FileManager.getInstance( );
			} catch {
				Debug.LogError( "ファイルマネージャーのインスタンスが取得できませんでした。" );
			}
		}

		return error;
	}

	void massCreate( int count ) {
		//マスのプレハブを生成
		GameObject obj = ( GameObject )Instantiate( _mass_prefab );
		obj.transform.SetParent( _contect );
		obj.GetComponent< RectTransform >( ).anchoredPosition3D = new Vector3( _start_masu_x + ( count * _mass_while_x ), _start_masu_y, 0 );
		obj.GetComponent< massData >( ).setMassData( _file_manager.getMapData( ).mass[ count ].type, count );
		obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
		obj.name = "Mass:ID" + count;
	}

	void miniMassCreate( int count ) {
		GameObject obj = ( GameObject )Instantiate( _mini_mass_prefab );
		obj.transform.SetParent( _contect );
		obj.GetComponent< RectTransform >( ).anchoredPosition3D = new Vector3( _start_masu_x + ( count * _mass_while_x + _mini_masu_while_x ) , _start_masu_y, 0 );
		obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
		obj.name = "mini_Mass:ID" + count;
	}

	public Vector2 getEndPosition( ) {
		return _end_position;
	}
}
