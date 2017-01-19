using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public float MASS_SIZE     = 0.5f;
	public float INTERVAL_SIZE = 0.5f;
	public float CAMERA_ANGLE  = 30.0f;

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

	/// <summary>
	/// Moves the camera position.
	/// </summary>
	/// <param name="first_player">First player.</param>
	/// <param name="last_player">Last player.</param>
	public void moveCameraPos( GameObject first_player, GameObject last_player ) {
		int mass_num     = 6;
		int interval_num = mass_num + 1;

		// カメラの横幅を出す
		float camera_width = ( MASS_SIZE * mass_num ) + ( INTERVAL_SIZE * interval_num ) + ( first_player.transform.position.x - last_player.transform.position.x );
		// カメラの注視点を計算
		float view_x = last_player.transform.position.x + ( camera_width / 2 );
		Vector3 view_point = new Vector3( view_x, first_player.transform.position.y, first_player.transform.position.z );

		// カメラから注視点までの距離を計算
		float camera_field = Camera.main.fieldOfView;
		float distance = camera_width / 2 /  Mathf.Tan( camera_field / 4 );

		// カメラの座標を計算
		float y = distance * Mathf.Sin( CAMERA_ANGLE );
		float z = distance * Mathf.Cos( CAMERA_ANGLE );
        float adjust = -5.0f;
		Camera.main.transform.position = new Vector3( view_x, y, z + adjust );

		// カメラの回転を計算
		Camera.main.transform.LookAt( view_point );
	}
}
