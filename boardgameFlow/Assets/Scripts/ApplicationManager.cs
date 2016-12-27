﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class ApplicationManager : MonoBehaviour {

	[ SerializeField ]
	private SCENE _scene = SCENE.SCENE_CONNECT;
	[ SerializeField ]
	private NetworkMNG _network_manager;
	[ SerializeField ]
	private PhaseManager _phase_manager;
	[ SerializeField ]
	private CardManager _card_manager;
    [ SerializeField ]
    private NetworkGUIControll _network_gui_controll;
    [ SerializeField ]
    private HostData _host_data;
    [ SerializeField ]
    private ClientData _client_data;

	public Text _scene_text;

    void Awake( ) {
        DontDestroyOnLoad( this.gameObject );
	}

	// Use this for initialization
	void Start( ) {
		try {
			if ( _network_manager == null ) {
				_network_manager = GameObject.Find( "NetworkManager" ).GetComponent< NetworkMNG >( );
			}
			if ( _phase_manager == null ) {
				_phase_manager = GameObject.Find( "PhaseManager" ).GetComponent< PhaseManager >( );
			}
			if ( _card_manager == null ) {
				_card_manager = GameObject.Find( "CardManager" ).GetComponent< CardManager >( );
			}
			_network_gui_controll = GameObject.Find( "NetworkManager" ).GetComponent< NetworkGUIControll >( );
		}
		catch {
			Debug.Log( "参照に失敗しました。" );
		}
	}
	
	// Update is called once per frame
	void Update( ) {
		if ( _host_data == null && _network_manager.getHostObj( ) != null ) {
			_host_data = _network_manager.getHostObj( ).GetComponent< HostData >( );
		}
        
		if ( _client_data == null && _network_manager.getClientObj( 0 ) != null ) {
			_client_data = _network_manager.getClientObj( 0 ).GetComponent< ClientData >( );
		}

		// デバッグ
        /*
		if ( _network_data != null && !_network_data.isLocal( ) ) {
			_scene = _network_data.getRecvData( ).scene;
			_phase_manager.setPhase(_network_data.getRecvData ().main_game_phase);
		}
        */

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

		if ( _host_data != null && _client_data != null ) {
			if ( _client_data.getRecvData( ).changed_scene == true ) {
				_host_data.setSendChangeFieldScene( false );
				Debug.Log( "ok" );
			}
 		}
	}

	/// <summary>
	/// ConnectSceneの更新
	/// </summary>
	private void updateConnectScene( ) {
		if ( _network_manager.isConnected( ) ||  Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_TITLE;
			_scene_text.text = "SCENE_TITLE";
			_host_data.setSendScene( _scene );
            _host_data.setSendChangeFieldScene( true );
		}
	}

	/// <summary>
	/// TitleSceneの更新
	/// </summary>
	private void updateTitleScene( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_GAME;
			_scene_text.text = "SCENE_GAME";
			_host_data.setSendScene( _scene );
            _host_data.setSendChangeFieldScene( true );
			_network_gui_controll.setShowGUI( false );
		}
	}

	/// <summary>
	/// FinishSceneの更新
	/// </summary>
	private void updateFinishScene( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_TITLE;
			_scene_text.text = "SCENE_TITLE";
			_host_data.setSendScene( _scene );
            _host_data.setSendChangeFieldScene( true );
		}
	}

	/// <summary>
	/// GameSceneの更新
	/// </summary>
	private void updateGameScene( ) {
		// フェイズチェンジ
		_phase_manager.changeMainGamePhase( );
		// 通信データのセット
		if ( _phase_manager.isPhaseChanged( ) ) {
			_host_data.setSendGamePhase( _phase_manager.getMainGamePhase( ) );
		}

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
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_FINISH;
			_scene_text.text = "SCENEFINISH";
			_host_data.setSendScene( _scene );
            _host_data.setSendChangeFieldScene( true );
		}
	}

	public void OnGUI( ) {
		if ( _host_data != null && _scene == SCENE.SCENE_CONNECT ) {
			drawConnectScene( );
		}
	}

	/// <summary>
	/// ConnectSceneの描画
	/// </summary>
	private void drawConnectScene( ) {
        
		if( !_network_manager.isConnected( ) && _host_data.getServerState( ) != SERVER_STATE.STATE_HOST ) {
			//_network_manager.noConnectDraw( );
		}

		if ( _host_data.getServerState( ) == SERVER_STATE.STATE_HOST ) {
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
