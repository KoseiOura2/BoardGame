using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map2d : MonoBehaviour {

	//スタートマスの初期位置X
	[SerializeField]
	private float _start_Mass_X = -282;
	[SerializeField]
	private float _start_Mass_Y = 0;
	//マスとの距離
	[SerializeField]
	private float _mini_Mass_While_X = 93.0f;
	[SerializeField]
	private float _mass_While_X = 186;
	//壁の橋とマスの間
	private float _mass_While = -16;

	public FileManager _file_manager;
	public GameObject _mass_Prefab;
	public GameObject _mini_Mass_Prefab;

	public Transform _Contect;

	private int _create_mass_count = 0;

	private Vector2 end_Position;

	// Use this for initialization
	void Awake () {
		if ( !_mass_Prefab ) {
			_mass_Prefab = (GameObject)Resources.Load ("Prefab/masu");
		}
		if ( !_mini_Mass_Prefab ) {
			_mini_Mass_Prefab = (GameObject)Resources.Load ("Prefab/masu_mini");
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

		int max_Mass = _file_manager.getMassCount ();
		end_Position = new Vector2( ( -_mass_While_X - _mass_While ) * (float)max_Mass , _start_Mass_Y ) ;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool isError() {
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

	void massCreate( int count ){
		//マスのプレハブを生成
		GameObject obj = ( GameObject )Instantiate( _mass_Prefab );
		obj.transform.SetParent (_Contect);
		obj.GetComponent<RectTransform> ().anchoredPosition3D
		= new Vector3 ( _start_Mass_X + (count * _mass_While_X), _start_Mass_Y, 0 );
		obj.GetComponent<massData>().SetMassData( _file_manager.getMapData().mass[ count ].type );
		obj.GetComponent<massData> ().SetBaloonPosition (count);
		obj.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		obj.name = "Mass:ID" + count;
	}

	void miniMassCreate( int count ) {
		GameObject obj = ( GameObject )Instantiate( _mini_Mass_Prefab );
		obj.transform.SetParent (_Contect);
		obj.GetComponent<RectTransform> ().anchoredPosition3D 
		= new Vector3 ( _start_Mass_X + (count * _mass_While_X + _mini_Mass_While_X ) , _start_Mass_Y, 0 );
		obj.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		obj.name = "mini_Mass:ID" + count;
	}

	public Vector2 getEndPosition( ){
		return end_Position;
	}
}
