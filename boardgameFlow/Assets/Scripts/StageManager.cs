using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour {

	[ SerializeField ]
	private List< GameObject > _mass_list = new List< GameObject >( );	//マスデータをリストとして保持しておく
	private GameObject _mass_prefab;	//マスデータロード用関数

    private int _create_mass_count = 0;

	public void init( ) {

		for( int i = 0; i < _mass_list.Count - 1; i++ ) {
			_mass_prefab = ( GameObject )Resources.Load( "Prefabs/masu_mini" );
			Vector3 pos = Vector3.Lerp( _mass_list[ i ].transform.localPosition, _mass_list[ i + 1 ].transform.localPosition, 0.5f );
			GameObject obj = ( GameObject )Instantiate( _mass_prefab, pos, _mass_prefab.transform.localRotation );
			obj.transform.parent = _mass_list[ i ].transform;
		}
        
	}

	// Use this for initialization
	void Start( ) {
	
	}

    void massUpdate( ) {
        
    }

    //ゲーム開始時マスを生成
	public void massCreate( int num, string type, Vector3 pos ) {
		// タイプによるリソース分け
		switch ( type ) {
			case "start":
			case "goal":
				_mass_prefab = ( GameObject )Resources.Load( "Prefabs/masu_yellow" );
                break;
            case "draw":
            case "advance":
            case "selectDraw":
				_mass_prefab = ( GameObject )Resources.Load( "Prefabs/masu_blue" );
                break;
			case "trap1":
			case "trap2":
				_mass_prefab = ( GameObject )Resources.Load( "Prefabs/masu_red" );
				break;
			case "event":
				_mass_prefab = ( GameObject )Resources.Load( "Prefabs/masu_green" );
                break;
        }

		// 生成
		GameObject obj = ( GameObject )Instantiate( _mass_prefab, pos, _mass_prefab.transform.localRotation );
		obj.name = "Mass:ID" + num;

        // マネージャーの配下に設定
        obj.transform.parent = transform;
        _mass_list.Add( obj );
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
	
	// Update is called once per frame
	void Update( ) {
	
	}
}
