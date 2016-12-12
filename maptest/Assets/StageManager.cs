﻿using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {


    public FileManager _file_manager;
    public GameObject _mass_prefab;

    int _create_count_main       = 0;

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

        massUpdate( );

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
        //マスの生成
        if( _create_count_main < _file_manager.getMassCount( ) ) {
            massCreate( _create_count_main );
			_create_count_main++;
        }

    }

    public void massCreate( int count ) {

        GameObject obj = 
            ( GameObject )Instantiate( _mass_prefab, 
            new Vector3( _file_manager.getMapData().ma[ count ].x, _file_manager.getMapData().ma[ count ].y, _file_manager.getMapData().ma[ count ].z ), 
            Quaternion.identity );

        // マネージャーの配下に設定
        obj.transform.parent = transform;

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
