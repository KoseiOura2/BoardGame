using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {


    public FileManager _file_manager;
    public GameObject _mass_prefab;

    void Awake( ) {
		if ( !_mass_prefab ) {
			_mass_prefab = ( GameObject )Resources.Load( "Prefab/Mass" );
		}
	}

	// Use this for initialization
	void Start () {
	
	}

    void FixedUpdate( ) {
        if ( isError( ) ) {
            return;
        }
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

    public void massCreate( int count ) {

        GameObject obj = 
            ( GameObject )Instantiate( _mass_prefab, 
            new Vector3( _file_manager.getMapData.ma.x, _file_manager.getMapData.ma.y, _file_manager.getMapData.ma.z ), 
            Quaternion.identity );

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
