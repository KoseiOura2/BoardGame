using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    private Camera _main_camera;

	// Use this for initialization
	void Start( ) {
        //対象のカメラが指定されない場合にはMainCameraを対象とします。
        if ( _main_camera == null) {
            _main_camera = Camera.main;
        }
	}
	
	// Update is called once per frame
	void Update( ) {
        //カメラの方向を向くようにする。
        this.transform.LookAt( _main_camera.transform.position);      
	}
}
