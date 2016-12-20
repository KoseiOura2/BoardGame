using UnityEngine;
using System.Collections;
using Common;

public class ApplicationManager : MonoBehaviour {

	[ SerializeField ]
	private SCENE _scene = SCENE.SCENE_CONNECT;
	[ SerializeField ]
	private NetWorkManager _network_manager;
	[ SerializeField ]
	private PhaseManager _phase_manager;
	[ SerializeField ]
	private CardManager _card_manager;


    void Awake( ) {
        DontDestroyOnLoad( this.gameObject );
		try {
			_network_manager = GameObject.Find( "NetWorkManager" ).GetComponent< NetWorkManager >( );
			_phase_manager = GameObject.Find( "PhaseManager" ).GetComponent< PhaseManager >( );
			_card_manager = GameObject.Find( "CardManager" ).GetComponent< CardManager >( );
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
		if ( _network_manager.isConnected( ) || Input.GetMouseButtonDown( 0 ) ) {
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
	/// FinishSceneの更新
	/// </summary>
	private void updateFinishScene( ) {
		if ( Input.GetMouseButtonDown( 0 ) ) {
			_scene = SCENE.SCENE_TITLE;
		}
	}

	/// <summary>
	/// GameSceneの更新
	/// </summary>
	private void updateGameScene( ) {
		// フェイズチェンジ
		_phase_manager.changeMainGamePhase( );

		// フェイズごとの更新
		switch( _phase_manager.getMainGamePhase( ) ) {
		case MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE:
			updateDicePhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF:
			updateBuffPhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_RESULT_BATTLE:
			updateResultPhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER:
			updateMovePhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_GIMMICK:
			updateGimmickPhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_FINISH:
			updateFinishPhase( );
			break;
		}
	}

	/// <summary>
	/// DicePhaseの更新
	/// </summary>
	private void updateDicePhase( ) {
		// ダイスを振ったら(通信)
		if ( Input.GetMouseButtonDown( 0 ) ) {
			int sai = 3;	// 送られてきた賽の目の数
			for ( int i = 0; i < sai; i++ ) {
				// デッキのカード数が０になったらリフレッシュ
				if ( _card_manager.getDeckCardNum( ) <= 0 ) {
					_card_manager.createDeck( );
				}
				Debug.Log( _card_manager.distributeCard( ) );
			}
		}
	}

	/// <summary>
	/// BuffPhaseの更新
	/// </summary>
	private void updateBuffPhase( ) {
		
	}

	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
		
	}

	/// <summary>
	/// MovePhaseの更新
	/// </summary>
	private void updateMovePhase( ) {
		
	}

	/// <summary>
	/// GimmickPhaseの更新
	/// </summary>
	private void updateGimmickPhase( ) {
		
	}

	/// <summary>
	/// FinishPhaseの更新
	/// </summary>
	private void updateFinishPhase( ) {
		if ( Input.GetMouseButtonDown( 0 ) ) {
			_scene = SCENE.SCENE_FINISH;
		}
	}

	public void OnGUI( ) {
		if ( _scene == SCENE.SCENE_CONNECT ) {
			drawConnectScene( );
		}
	}

	/// <summary>
	/// ConnectSceneの描画
	/// </summary>
	private void drawConnectScene( ) {
		if( !_network_manager.isConnected( ) ) {
			_network_manager.noConnectDraw( );
		}

		if ( _network_manager.getServerState( ) == SERVER_STATE.STATE_HOST ) {
			_network_manager.hostStateDraw( );
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
