using UnityEngine;
using System.Collections;
using Common;

public class ApplicationManager : MonoBehaviour {



	[ SerializeField ]
	private SCENE _scene = SCENE.SCENE_CONNECT;
	private NetWorkManager _network_manager;

	void Awake( ) {
		try {
			_network_manager = GameObject.Find( "NetWorkManager" ).GetComponent< NetWorkManager >( );
		}
		catch {
			Debug.Log( "参照に失敗しました。" );
		}
	}

	// Use this for initialization
	void Start( ) {
	}
	
	// Update is called once per frame
	void Update( ) {
		switch( _scene ) {
		case SCENE.SCENE_CONNECT:
			updateConnectScene( );
			break;
		case SCENE.SCENE_TITLE:
			updateTitleScene( );
			break;
		case SCENE.SCENE_GAME:
			updateGameScene( );
			break;
		case SCENE.SCENE_FINISH:
			updateFinishScene( );
			break;
		}
	}

	/// <summary>
	/// ConnectSceneの更新
	/// </summary>
	private void updateConnectScene( ) {
		if ( _network_manager.isConnected( ) ) {
			_scene = SCENE.SCENE_TITLE;
		}
	}

	/// <summary>
	/// TitleSceneの更新
	/// </summary>
	private void updateTitleScene( ) {
		if ( Input.GetMouseButtonDown( 0 ) ) {
			_scene = SCENE.SCENE_GAME;
		}
	}

	/// <summary>
	/// GameSceneの更新
	/// </summary>
	private void updateGameScene( ) {
		if ( Input.GetMouseButtonDown( 0 ) ) {
			_scene = SCENE.SCENE_FINISH;
		}
	}

	/// <summary>
	/// FinishSceneの更新
	/// </summary>
	private void updateFinishScene( ) {
		if ( Input.GetMouseButtonDown( 0 ) ) {
			_scene = SCENE.SCENE_TITLE;
		}
	}

	/// <summary>
	/// シーン情報を返す
	/// </summary>
	/// <returns>The scene.</returns>
	public SCENE getScene( ) {
		return _scene;
	}

}
