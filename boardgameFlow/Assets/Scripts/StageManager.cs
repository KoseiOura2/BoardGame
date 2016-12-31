using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour {


    [SerializeField]
    private FileManager _file_manager;
    [SerializeField]
    private PlayerManager _player_manager;

	private GameObject _mass_prefab; //マスデータロード用関数

    private int _create_mass_count = 0;

    [SerializeField]
    private List<GameObject> _mass_list = new List<GameObject>(); //生成されたマスをリストに格納

    void Awake( ) {

        if ( isError( ) ) {
            return;
        }
        //マスの生成
        for( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
            massCreate( _create_mass_count );
            _create_mass_count++;
        }
		for(int i = 0; i < _mass_list.Count - 1; i++) {
			_mass_prefab = (GameObject)Resources.Load ("Prefab/masu_mini");
			Vector3 pos = Vector3.Lerp( _mass_list [i].transform.localPosition, _mass_list [i + 1].transform.localPosition, 0.5f);
			GameObject obj = 
				( GameObject )Instantiate( _mass_prefab, 
					pos, 
					_mass_prefab.transform.localRotation );
			obj.transform.parent = _mass_list [i].transform;
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

    //ゲーム開始時マスを生成
    public void massCreate( int count ) {
		
        switch ( _file_manager.getMapData().mass[ count ].type )
        {
			case "start":
			case "goal":
			_mass_prefab = (GameObject)Resources.Load ("Prefab/masu_yellow");
                break;
            case "draw":
            case "advance":
			_mass_prefab = (GameObject)Resources.Load ("Prefab/masu_blue");
                break;
			case "trap1":
			case "trap2":
			_mass_prefab = (GameObject)Resources.Load ("Prefab/masu_red");
				break;
			case "event":
			_mass_prefab = (GameObject)Resources.Load ("Prefab/masu_green");
                break;
        }
		GameObject obj = 
			( GameObject )Instantiate( _mass_prefab, 
				_file_manager.getMassCoordinate( count ), 
				_mass_prefab.transform.localRotation );
		obj.name = "Mass:ID" + count;

        // マネージャーの配下に設定
        obj.transform.parent = transform;
        _mass_list.Add( obj );

    }

    //進む先のマスを取得
    public GameObject getTargetMass( int i ) {
        return _mass_list[ i ];
    }

    //マスイベントの処理
    public void massEvent( int i ) {
        switch ( _file_manager.getMapData().mass[ i ].type )
        {
            case "draw":
                Debug.Log("カード"+_file_manager.getMassValue( i )[0]+"ドロー");
                _file_manager.getMassValue( i );
                break;
            case "trap1":
				Debug.Log("トラップ発動");
                Debug.Log("カード"+_file_manager.getMassValue( i )[1]+"捨てる");
                Debug.Log(_file_manager.getMassValue( i )[0]+"マス進む");
                _player_manager._advance_flag = true;
                _player_manager._limit_value = _file_manager.getMassValue( i )[0];
                _file_manager.getMassValue( i );
                break;
            case "trap2":
				Debug.Log("トラップ発動");
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
			case "event":
				Debug.Log("イベント発生!!");
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
