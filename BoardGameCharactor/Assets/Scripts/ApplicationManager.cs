using UnityEngine;
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
	private FadeManager _fade_manager;
	[ SerializeField ]
	private BattlePhaseManager _battle_phase_manager;
	[ SerializeField ]
	private PlayerPhaseManager _player_phase_manager;
    [ SerializeField ]
    private NetworkGUIControll _network_gui_controll;
    [ SerializeField ]
    private HostData _host_data;
    [ SerializeField ]
    private ClientData _client_data;

    [ SerializeField ]
    private int _player_num = 0;

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
			if ( _fade_manager == null ) {
				_fade_manager = GameObject.Find( "FadeManager" ).GetComponent< FadeManager >( );
			}
            if ( _battle_phase_manager == null ) {
                _battle_phase_manager = GameObject.Find ( "BattlePhaseManager" ).GetComponent<BattlePhaseManager> ( );
            }
            if ( _player_phase_manager == null ) {
                _player_phase_manager = GameObject.Find ( "PlayerPhaseManager" ).GetComponent<PlayerPhaseManager> ( );
                _player_phase_manager.awake ( );
            }
            //_network_gui_controll = GameObject.Find( "NetworkManager" ).GetComponent< NetworkGUIControll >( );
        }
		catch {
			Debug.Log( "参照に失敗しました。" );
		}
	}

	private void referBattlePhaseManager( ) {
		try {
			if ( _battle_phase_manager == null ) {
				_battle_phase_manager = GameObject.Find( "BattlePhaseManager" ).GetComponent< BattlePhaseManager >( );
			}
		}
		catch {
			Debug.Log( "参照に失敗しました。" );
		}
	}

	private void referPlayerPhaseManager( ) {
		try {
			if ( _player_phase_manager == null ) {
				_player_phase_manager = GameObject.Find( "PlayerPhaseManager" ).GetComponent< PlayerPhaseManager >( );
				_player_phase_manager.awake( );
			}
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

        if ( _client_data == null && _network_manager.getClientObj( ) != null ) {
            _client_data = _network_manager.getClientObj( ).GetComponent< ClientData >( );
        }
        
        if ( _host_data != null && _client_data != null ) {
            // シーンの切り替え
            sceneChange( );
            // 切り替え完了を送る
            if ( _client_data.getRecvData( ).changed_scene == true && _host_data.getRecvData( ).change_scene == false ) {
                _client_data.CmdSetSendChangedScene( false );
                _client_data.setChangedScene( false );
            }
        }

		//フェード中なら動かさない
		if ( _fade_manager.fadeCheck( ) == false ) {
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
        //デバッグ用でキーを押したらフェイズを変える
        if ( Input.GetKeyDown( KeyCode.F1 )) {
            _phase_manager.setPhase ( MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY );
        }
        if ( Input.GetKeyDown ( KeyCode.F2 ) ) {
            _phase_manager.setPhase ( MAIN_GAME_PHASE.GAME_PHASE_DICE );
        }
        if ( Input.GetKeyDown ( KeyCode.F3 ) ) {
            _phase_manager.setPhase ( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER );
        }
        if ( Input.GetKeyDown ( KeyCode.F4 ) ) {
            _phase_manager.setPhase ( MAIN_GAME_PHASE.GAME_PHASE_DRAW_CARD );
        }
        if ( Input.GetKeyDown ( KeyCode.F5 ) ) {
            _phase_manager.setPhase ( MAIN_GAME_PHASE.GAME_PHASE_BATTLE );
        }
        if ( Input.GetKeyDown ( KeyCode.F6 ) ) {
            _phase_manager.setPhase ( MAIN_GAME_PHASE.GAME_PHASE_RESULT );
        }
        if ( Input.GetKeyDown ( KeyCode.F7 ) ) {
            _phase_manager.setPhase ( MAIN_GAME_PHASE.GAME_PHASE_EVENT );
        }
    }
    
    /// <summary>
    /// シーンの切り替え
    /// </summary>
    private void sceneChange( ) { 
        if ( _host_data.isChangeFieldScene( ) ) {
            _scene = _host_data.getRecvData( ).scene;
            _scene_text.text = _scene.ToString( );
            _client_data.CmdSetSendChangedScene( true );
            _client_data.setChangedScene( true );
        }
    }

	/// <summary>
	/// ConnectSceneの更新
	/// </summary>
	private void updateConnectScene( ) {

	}

	/// <summary>
	/// TitleSceneの更新
	/// </summary>
	private void updateTitleScene( ) {

	}

	/// <summary>
	/// FinishSceneの更新
	/// </summary>
	private void updateFinishScene( ) {

	}

	/// <summary>
	/// GameSceneの更新
	/// </summary>
	private void updateGameScene( ) {
		// フェイズごとの更新
		switch( _phase_manager.getMainGamePhase( ) ) {
            case MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY:
                updateNoPlayPhase( );
                break;
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

        if ( Input.GetKeyDown ( KeyCode.P ) ) {
            Debug.Log ( "test" );
            //切り替え時リセット
            _player_phase_manager.phaseReset ( );
            _battle_phase_manager.phaseReset ( );
        }

        if ( _host_data != null && _client_data != null ) {
            // フェイズの切り替え
            phaseChange( );

            // 切り替え完了を送る
            if ( _client_data.getRecvData( ).changed_phase == true && _host_data.getRecvData( ).change_phase == false ) {
                _client_data.CmdSetSendChangedPhase( false );
                _client_data.setChangedPhase( false );
            }
        }
	}

    /// <summary>
    /// フェイズの切り替え
    /// </summary>
    private void phaseChange( ) { 
        if ( _host_data.isChangeFieldPhase( ) ) {

            //プレイヤー＆バトルフェイズ切り替え時リセット
            _player_phase_manager.phaseReset ( );
            _battle_phase_manager.phaseReset ( );

            _phase_manager.setPhase( _host_data.getRecvData( ).main_game_phase );
            _client_data.CmdSetSendChangedPhase( true );
            _client_data.setChangedPhase( true );
        }
    }

	/// <summary>
	/// NoPlayPhaseの更新
	/// </summary>
	private void updateNoPlayPhase( ) {
        //待機中に初期設定を行う
        _player_phase_manager.waitPhase ( );
    }

	/// <summary>
	/// DicePhaseの更新
	/// </summary>
	private void updateDicePhase( ) {
        int value = 0;

        //ダイスフェイズの処理
        _player_phase_manager.dicePhase ( );

        //賽の目を入れる(無い場合は0）
        value = _player_phase_manager.getDiceData ( );

        //賽の目に1以上のダイス目が入ったら
        if( value > 1 ) {
            if ( _client_data != null ) {
                _client_data.CmdSetSendDiceValue ( value );
                _client_data.setDiceValue ( value );
            }
        }
        /*
        if ( Input.GetKeyDown( KeyCode.A ) ) {
            _client_data.CmdSetSendDiceValue( value );
            _client_data.setDiceValue( value );
        }
        */
	}
    
	/// <summary>
	/// MovePhaseの更新
	/// </summary>
	private void updateMovePhase( ) {
        //上画面に誘導をするメッセージの処理
        _player_phase_manager.inductionPhase ( );
        if ( _client_data != null ) {
            if ( _client_data.getRecvData ( ).dice_value > 0 ) {
                // さいの目を―1に初期化
                _client_data.CmdSetSendDiceValue ( -1 );
                _client_data.setDiceValue ( -1 );
            }
        }
	}
    
	/// <summary>
	/// DrawPhaseの更新
	/// </summary>
	private void updateDrawPhase( ) {
        //前回のプレイヤーフェイズのオブジェクトを削除
        _player_phase_manager.objectDelete ( );

        _battle_phase_manager.drawPhase ( );
        if ( _player_num == 0 ) {
            if ( _client_data != null ) {
                if ( _host_data.getRecvData ( ).card_list_one.Length > 0 ) {
                    // 手札にカードを加える処理
                    //_battle_phase_manager.getCardId ( );
                }
            }
        } else if ( _player_num == 1 ) {
		    if ( _host_data.getRecvData( ).card_list_two.Length > 0 ) {
                // 手札にカードを加える処理
                //_battle_phase_manager.getCardId ( );
            }
        }
        //ドローが終了したかどうかを取得
        //_battle_phase_manager.drawEnd ( true );

        if ( Input.GetKeyDown( KeyCode.A ) ) {
            // 準備完了を送る
            _client_data.CmdSetSendReady( true );
            _client_data.setReady( true );
        }
	}
    
	/// <summary>
	/// ButtlePhaseの更新
	/// </summary>
	private void updateButtlePhase( ) {
        //battleフェイズがプレイヤー待機状態になったか否か
        bool _battle_phase_wait = _battle_phase_manager.getPhaseWait ( );

        if ( _client_data != null ) {
            if ( _client_data.getRecvData ( ).ready == true ) {
                // 準備完了を初期化
                _client_data.CmdSetSendReady ( false );
                _client_data.setReady ( false );
            }
        }
        _battle_phase_manager.cardPhase( );

        if( _battle_phase_wait ) {
            int player_status = 10;
            int[ ] card_list = new int[ ] { 0, 1, 2 };

            if ( _client_data != null ) {
                _client_data.CmdSetSendBattleData ( true, player_status, card_list );
                _client_data.setBattleData ( true, player_status, card_list );
            }
        }

        if ( Input.GetKeyDown( KeyCode.A ) ) {
            // 選択結果を送る
            int player_status = 10;
            int[ ] card_list = new int[ ] { 0, 1, 2 };

            _client_data.CmdSetSendBattleData( true, player_status, card_list );
            _client_data.setBattleData( true, player_status, card_list );
        }
	}
    
	/// <summary>
	/// ResultPhaseの更新
	/// </summary>
	private void updateResultPhase( ) {
        //battleフェイズがプレイヤー待機状態になったか否か
        bool _battle_phase_wait = _battle_phase_manager.getPhaseWait ( );

        if ( _player_num == 0 ) {
            if ( _host_data != null ) {
                if ( _host_data.getBattleResultOne ( ) == ( int )BATTLE_RESULT.WIN ||
                     _host_data.getBattleResultOne ( ) == ( int )BATTLE_RESULT.DRAW ) {
                }
            }
            // マス調整の処理
            MASS_ADJUST adjust = MASS_ADJUST.ADVANCE;
            if ( !_battle_phase_wait ) {
                //一定秒数表示したら読み込みを変える
                _battle_phase_manager.resultPhase ( BATTLE_RESULT.WIN );
            } else {
                //バトルフェイズのオブジェクトを削除
                _battle_phase_manager.objectDelete ( );
                //切り替えフラグを見たらプレイヤーフェイズのマス調整フェイズに向かう
                _player_phase_manager.movePhase ( BATTLE_RESULT.WIN );
            }
            if ( _client_data != null ) {
            _client_data.CmdSetSendMassAdjust ( true, adjust );
            _client_data.setMassAdjust ( true, adjust );
            }
        }
        if ( _host_data != null ) {
            if ( _host_data.getBattleResultOne ( ) == ( int )BATTLE_RESULT.LOSE ) {
                // マス調整の処理
                MASS_ADJUST adjust = MASS_ADJUST.NO_ADJUST;

                if ( !_battle_phase_wait ) {
                    //一定秒数表示したらシーンを変える…ここでの操作はしてはいけない…
                    _battle_phase_manager.resultPhase ( BATTLE_RESULT.LOSE );
                } else {
                    //切り替えフラグを見たらプレイヤーフェイズのマス調整フェイズに向かう
                    _player_phase_manager.movePhase ( BATTLE_RESULT.LOSE );
                }

                _battle_phase_manager.resultPhase ( BATTLE_RESULT.LOSE );
                _client_data.CmdSetSendMassAdjust ( true, adjust );
                _client_data.setMassAdjust ( true, adjust );
            } else if ( _player_num == 1 ) {
                if ( _host_data.getBattleResultTwo ( ) == ( int )BATTLE_RESULT.WIN ||
                     _host_data.getBattleResultTwo ( ) == ( int )BATTLE_RESULT.DRAW ) {
                    // マス調整の処理
                    MASS_ADJUST adjust = MASS_ADJUST.ADVANCE;

                    _client_data.CmdSetSendMassAdjust ( true, adjust );
                    _client_data.setMassAdjust ( true, adjust );
                } else if ( _host_data.getBattleResultTwo ( ) == ( int )BATTLE_RESULT.LOSE ) {
                    // マス調整の処理
                    MASS_ADJUST adjust = MASS_ADJUST.NO_ADJUST;

                    _client_data.CmdSetSendMassAdjust ( true, adjust );
                    _client_data.setMassAdjust ( true, adjust );
                }
            }
        }
	}
    
	/// <summary>
	/// EventPhaseの更新
	/// </summary>
	private void updateEventPhase( ) {
		if ( _client_data.getRecvData( ).ready == true ) {
            // 準備完了を初期化
            _client_data.CmdSetSendMassAdjust( false, MASS_ADJUST.NO_ADJUST );
            _client_data.setMassAdjust( true, MASS_ADJUST.NO_ADJUST );
        }
	}

	/// <summary>
	/// FinishPhaseの更新
	/// </summary>
	private void updateFinishPhase( ) {

	}

	public void OnGUI( ) {
		if ( _scene == SCENE.SCENE_CONNECT && _host_data != null ) {
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
/*
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class ApplicationManager : MonoBehaviour {
	private PlayerManager _player_manager;
	private CardManager _card_Manager;
	private int _debug_turn = 0;
	public enum CARD_BATTLE_PHASE {
		STAND_BY,
		SELECT,
		BATTLE,
		END
	}
	public CARD_BATTLE_PHASE _phase = CARD_BATTLE_PHASE.STAND_BY;
	void Start( ) {
		if ( _player_manager == null ) {
			_player_manager = GameObject.Find( "PlayerManager" ).GetComponent<PlayerManager>( );
		}
		if ( _card_Manager == null ) {
			_card_Manager = GameObject.Find( "CardManager" ).GetComponent<CardManager>( );
		}
	}
	void Update( ) {
		cardSettingDebug( );
	}
	/// <summary>
	/// カード使用、ステータス変更のデバッグ キーボードのAで実行
	/// </summary>
	private void cardSettingDebug( ) {
		if ( Input.GetKeyDown( KeyCode.A ) ) {
			switch( _phase ) {
			case CARD_BATTLE_PHASE.STAND_BY:
				_debug_turn++;
				Debug.Log( _debug_turn + "ターン目" );
				//カードゲーム開始処理 
				_phase = CARD_BATTLE_PHASE.SELECT;
				//持続エンハンス効果をカウントダウン
				_player_manager.turnTypeCardCountDown( );
				break;
			case CARD_BATTLE_PHASE.SELECT:
				_player_manager.debugPlayTurnCard( );
				_phase = CARD_BATTLE_PHASE.BATTLE;
				break;
			case CARD_BATTLE_PHASE.BATTLE:
				//ステータス計算
				if ( _player_manager.getHandRange( ) > 0 ) {
					_player_manager.debugPlayCard( );
				}
				_phase = CARD_BATTLE_PHASE.STAND_BY;
				break;
			case CARD_BATTLE_PHASE.END:
				//終了処理
				_phase = CARD_BATTLE_PHASE.STAND_BY;
				break;
			}
		}
	}
}
*/
