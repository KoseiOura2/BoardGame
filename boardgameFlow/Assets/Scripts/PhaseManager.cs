using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class PhaseManager : MonoBehaviour {

    [ SerializeField ]
    private MAIN_GAME_PHASE _main_game_phase;   // メインゲームのフロー
	private bool _phase_changed = false;

	public Text _phase_text;

	// Use this for initialization
	void Start( ) {
        _main_game_phase = MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY;
		_phase_text.text = "NoPlay";
	}
	
	// Update is called once per frame
	void Update( ) {
		
	}

    /// <summary>
    /// 受け取ったデータによってMainGamePhaseを切り替える
    /// </summary>
    public void changeMainGamePhase( ) {
        // DicePhaseへ移行
        if ( Input.GetKeyDown( KeyCode.F1 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_DICE, "DicePhase" );
        }
        // Moveへ移行
		if ( Input.GetKeyDown( KeyCode.F2 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER, "MovePhase" );
        }
        // DrawCardへ移行
        if ( Input.GetKeyDown( KeyCode.F3 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_DRAW_CARD, "DrawCardPhase" );
        }
        // Battleへ移行
        if ( Input.GetKeyDown( KeyCode.F4 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_BATTLE, "BattlePhase" );
        }
        // Resultへ移行
        if ( Input.GetKeyDown( KeyCode.F5 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_RESULT, "ResultPhase" );
        }
        // Eventへ移行
        if ( Input.GetKeyDown( KeyCode.F6 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_EVENT, "EventPhase" );
        }
        // Finishへ移行
        if ( Input.GetKeyDown( KeyCode.F7 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_FINISH, "FinishPhase" );
        }
    }

    /// <summary>
    /// MainGamePhaseが移行可能かどうか確認する
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="log_text"></param>
    private void changeMainGamePhase( MAIN_GAME_PHASE phase, string log_text ) {
        try {
            _main_game_phase = phase;
			_phase_text.text = log_text;
			_phase_changed = true;
        }
        catch {
            Debug.Log( log_text + "へ移行できませんでした。" );
        }
    }

	/// <summary>
	/// MainGamePhaseの取得
	/// </summary>
	/// <returns>The main game phase.</returns>
	public MAIN_GAME_PHASE getMainGamePhase( ) {
		return _main_game_phase;
	}

	/// <summary>
	/// phaseが変わったかどうか
	/// </summary>
	/// <returns><c>true</c>, if phase changed was ised, <c>false</c> otherwise.</returns>
	public bool isPhaseChanged( ) {
		bool flag = false;

		if ( _phase_changed == true ) {
			_phase_changed = false;
			flag = true;
		}

		return flag;
	}

	public void setPhase( MAIN_GAME_PHASE phase ) {
		_main_game_phase = phase;
	}
}
