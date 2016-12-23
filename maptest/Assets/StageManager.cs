using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour {


    public FileManager _file_manager;
    public PlayerManager _player_manager;
    public GameObject _mass_prefab;

    int _create_mass_count = 0;

    [SerializeField]
    private List<GameObject> _mass_list = new List<GameObject>();

    void Awake( ) {
		if ( !_mass_prefab ) {
			_mass_prefab = ( GameObject )Resources.Load( "Prefab/Mass" );
		}

        if ( isError( ) ) {
            return;
        }
        //マスの生成
        for( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
            massCreate( _create_mass_count );
            _create_mass_count++;
        }
        
	}

	// Use this for initialization
	void Start() {
	
	}

    void FixedUpdate( ) {

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

    void massUpdate( ) {
        
    }

    public void massCreate( int count ) {

        GameObject obj = 
            ( GameObject )Instantiate( _mass_prefab, 
            _file_manager.getMassCoordinate( count ), 
            Quaternion.identity );
        obj.name = "Mass:ID" + count;

        switch ( _file_manager.getMapData().mass[ count ].type )
        {
            case "start":
                obj.GetComponent< Renderer >().material.color = Color.yellow;
                break;
            case "draw":
            case "trap2":
                obj.GetComponent< Renderer >().material.color = Color.blue;
                break;
            case "trap1":
            case "advance":
                obj.GetComponent< Renderer >().material.color = Color.green;
                break;
            case "goal":
                obj.GetComponent< Renderer >().material.color = Color.yellow;
                break;

        }

        // マネージャーの配下に設定
        obj.transform.parent = transform;
        _mass_list.Add( obj );

    }
    public GameObject getTargetMass( int i ) {
        return _mass_list[ i ];

    }

    public void massEvent( int i ) {
        switch ( _file_manager.getMapData().mass[ i ].type )
        {
            case "draw":
                Debug.Log("カード"+_file_manager.getMassValue( i )[0]+"ドロー");
                _file_manager.getMassValue( i );
                break;
            case "trap1":
                Debug.Log("カード"+_file_manager.getMassValue( i )[1]+"捨てる");
                Debug.Log(_file_manager.getMassValue( i )[0]+"マス進む");
                _player_manager._advance_flag = true;
                _player_manager._limit_value = _file_manager.getMassValue( i )[0];
                _file_manager.getMassValue( i );
                break;
            case "trap2":
                Debug.Log("カード"+_file_manager.getMassValue( i )[0]+"ドロー");
                Debug.Log(_file_manager.getMassValue( i )[1]+"マス戻る");
                _player_manager._limit_value = _file_manager.getMassValue( i )[1];
                _player_manager._advance_flag = false;
                _file_manager.getMassValue( i );
                break;
            case "advance":
                Debug.Log(_file_manager.getMassValue( i )[0]+"マス進む");
                _player_manager._advance_flag = true;
                _player_manager._limit_value = _file_manager.getMassValue( i )[0];
                _file_manager.getMassValue( i );
                break;
            case "goal":
                Debug.Log("Goal!!");
                break;
        }
        
        
    }
	
	// Update is called once per frame
	void Update() {
	
	}
}
