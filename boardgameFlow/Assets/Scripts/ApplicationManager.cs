using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class ApplicationManager : Manager< ApplicationManager > {

	[ SerializeField ]
	private NetworkMNG _network_manager;
	[ SerializeField ]
	private PhaseManager _phase_manager;
    [ SerializeField ]
    private FileManager _file_manager;
	[ SerializeField ]
	private CardManager _card_manager;
    [ SerializeField ]
    private PlayerManager _player_manager;
    [ SerializeField ]
    private StageManager _stage_manager;
	[ SerializeField ]
	private CameraManager _camera_manager;

    [ SerializeField ]
    private NetworkGUIControll _network_gui_controll;
    [ SerializeField ]
    private HostData _host_data;
    [ SerializeField ]
    private ClientData _client_data;
    
	[ SerializeField ]
	private SCENE _scene = SCENE.SCENE_CONNECT;

	public GameObject[ ] debug_objs = new GameObject[ 2 ];
	public Text _scene_text;
	public Text[ ] _reside_text = new Text[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //残りマス用テキスト
	public Text[ ] _environment = new Text[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //環境情報用テキスト
    private int _event_count = 0;        //イベントを起こす回数   

	// Awake関数の代わり
	protected override void initialize( ) {
		init( );
	}

    void init( ) {
        if ( isError( ) ) {
            return;
        }

		referManager( );
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

	// Use this for initialization
	void Start( ) {

		referManager( );

		_card_manager.init( );
	}

	void referManager( ) {
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
			if ( _player_manager == null ) {
				_player_manager = GameObject.Find( "PlayerManager" ).GetComponent< PlayerManager >( );
			}
			if ( _stage_manager == null ) {
				_stage_manager = GameObject.Find( "StageManager" ).GetComponent< StageManager >( );
			}
			if ( _camera_manager == null ) {
				_camera_manager = Camera.main.GetComponent< CameraManager >( );
			}
			_network_gui_controll = GameObject.Find( "NetworkManager" ).GetComponent< NetworkGUIControll >( );
		}
		catch {
			Debug.Log( "参照に失敗しました。" );
		}
	}
	
	// Update is called once per frame
	void FixedUpdate( ) {
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
			_network_gui_controll.setShowGUI( false );
			try {
				_host_data.setSendScene( _scene );
            	_host_data.setSendChangeFieldScene( true );
			}
			catch {
				Debug.Log( "通信に失敗しまいました" );
			}
		}
	}

	/// <summary>
	/// TitleSceneの更新
	/// </summary>
	private void updateTitleScene( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_GAME;
			_scene_text.text = "SCENE_GAME";

			_player_manager.init( _file_manager.getMassCoordinate( 0 ) );

			//マスの生成
			for( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
				int num = _stage_manager.getMassCount( );
				_stage_manager.massCreate( num, _file_manager.getFileData( ).mass[ num ].type, _file_manager.getMassCoordinate( num ) );
				_stage_manager.increaseMassCount( );
			}
			_stage_manager.init( );

			try {
				_host_data.setSendScene( _scene );
				_host_data.setSendChangeFieldScene( true );
			}
			catch {
				Debug.Log( "通信に失敗しまいました" );
			}
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
			try {
				_host_data.setSendScene( _scene );
				_host_data.setSendChangeFieldScene( true );
			}
			catch {
				Debug.Log( "通信に失敗しまいました" );
			}
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
		case MAIN_GAME_PHASE.GAME_PHASE_DICE:
			updateDicePhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER:
			updateMovePhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_DRAW_CARD:
			updateDrawPhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_BATTLE:
			updateButtlePhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_RESULT:
			updateResultPhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_EVENT:
			updateEventPhase( );
			break;
		case MAIN_GAME_PHASE.GAME_PHASE_FINISH:
			updateFinishPhase( );
			break;
		}

        // playerの環境情報を更新
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			if ( _file_manager.getEnvironment( _player_manager.getPlayerCount( i ) ) != "" ) {
				string environment = _file_manager.getEnvironment ( _player_manager.getPlayerCount( i ) );
				playerEnvironment( environment, i );
			}
		}

		_camera_manager.moveCameraPos( debug_objs[ 0 ], debug_objs[ 1 ] );
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
	/// MovePhaseの更新
	/// </summary>
	private void updateMovePhase( ) {
		_player_manager.movePhaseUpdate( getResideCount( _player_manager.getPlayerID( ) ),
			_stage_manager.getTargetMass( _player_manager.getPlayerCount( _player_manager.getPlayerID( ) ) + 1 ),
			_stage_manager.getTargetMass( _player_manager.getPlayerCount( _player_manager.getPlayerID( ) ) - 1 )  );
		// ゴールまでの残りマスを表示
		resideCount( );
	}

	/// <summary>
	/// BuffPhaseの更新
	/// </summary>
	private void updateDrawPhase( ) {
		
	}

	/// <summary>
	/// GimmickPhaseの更新
	/// </summary>
	private void updateButtlePhase( ) {
		
	}

	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
		
	}

	/// <summary>
	/// EventPhaseの更新
	/// </summary>
	private void updateEventPhase( ) {
		eventPhaseUpdate( );
	}
    
    public void eventPhaseUpdate( ) {
		Debug.Log( "マスイベント！" );
		if ( _event_count < 2 ) {
			massEvent( _player_manager.getPlayerCount( _player_manager.getPlayerID( ) ) );
			_event_count++;
		}
    }

	/// <summary>
	/// マスイベントの処理
	/// </summary>
	/// <param name="i">The index.</param>
	public void massEvent( int i ) {
		switch ( _file_manager.getFileData( ).mass[ i ].type ) {
		case "draw":
			int value = _file_manager.getMassValue( i )[ 0 ];
			Debug.Log( "カード" + value + "ドロー" );
			for ( int j = 0; j < value; j++ ) {
				_card_manager.distributeCard( );
			}
			//_file_manager.getMassValue( i );
			break;
		case "trap1":
			Debug.Log( "トラップ発動" );
			Debug.Log( "カード" + _file_manager.getMassValue( i )[ 1 ] + "捨てる" );
			Debug.Log( _file_manager.getMassValue( i )[ 0 ] + "マス進む" );
			_player_manager.setAdvanceFlag( true );
			_player_manager.setLimitValue( _file_manager.getMassValue( i )[ 0 ] );
			_file_manager.getMassValue( i );
			break;
		case "trap2":
			Debug.Log( "トラップ発動");
			Debug.Log( "カード"+_file_manager.getMassValue( i )[ 0 ] + "ドロー");
			Debug.Log( _file_manager.getMassValue( i )[ 1 ] + "マス戻る" );
			_player_manager.setAdvanceFlag( false );
			_player_manager.setLimitValue( _file_manager.getMassValue( i )[ 1 ] );
			_file_manager.getMassValue( i );
			break;
		case "advance":
			Debug.Log(_file_manager.getMassValue( i )[ 0 ] + "マス進む" );
			_player_manager.setAdvanceFlag( true );
			_player_manager.setLimitValue( _file_manager.getMassValue( i )[ 0 ] );
			_file_manager.getMassValue( i );
			break;
		case "event":
			Debug.Log( "イベント発生!!" );
			break;
		case "goal":
			Debug.Log( "Goal!!" );
			break;
		}       
	}

	/// <summary>
	/// FinishPhaseの更新
	/// </summary>
	private void updateFinishPhase( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			_scene = SCENE.SCENE_FINISH;
			_scene_text.text = "SCENEFINISH";
			try {
				_host_data.setSendScene( _scene );
				_host_data.setSendChangeFieldScene( true );
			}
			catch {
				Debug.Log( "通信に失敗しまいました" );
			}
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


	/// <summary>
	/// プレイヤーの現在位置（環境）
	/// </summary>
	/// <param name="environment"></param>
	/// <param name="num"></param>
	public void playerEnvironment( string environment, int num ) {
		_environment[ num ].text = "プレイヤー" + ( num + 1 ) + ":" + environment;
	}

	/// <summary>
	/// ゴールまでの残りマスを表示
	/// </summary>
	public void resideCount( ) {
		for ( int i = 0; i < ( int )PLAYER_ORDER.MAX_PLAYER_NUM; i++ ) {
			_reside_text[ i ].text = "プレイヤー" + i.ToString( ) + "：残り" + getResideCount( i ).ToString( ) + "マス";
		}
	}

    /// <summary>
    /// ゴールまでどれくらい残っているか取得
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public int getResideCount( int i ) {
        return _file_manager.getMassCount( ) - 1 - _player_manager.getPlayerCount( i );
    }

	public void setEventCount( int count ) {
		_event_count = count;
	}

}
