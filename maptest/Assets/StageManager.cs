using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {


    public FileManager _file_manager;
    public GameObject _mass_prefab;

    int _create_mass_count = 0;

    void Awake( ) {
		if ( !_mass_prefab ) {
			_mass_prefab = ( GameObject )Resources.Load( "Prefab/Mass" );
		}

        if ( isError( ) ) {
            return;
        }
        //マスの生成
        for( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
            massUpdate( );
            _create_mass_count++;
        }
        
	}

	// Use this for initialization
	void Start () {
	
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
        massCreate( _create_mass_count );
    }

    public void massCreate( int count ) {

        GameObject obj = 
            ( GameObject )Instantiate( _mass_prefab, 
            _file_manager.getMassCoordinate( count ), 
            Quaternion.identity );

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

    }
    public Vector3 getmassPosition( int i ) {
        return _file_manager.getMassCoordinate( i );

    }

    public void massEvent( int i ) {
        _file_manager.getMassValue( i );
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
