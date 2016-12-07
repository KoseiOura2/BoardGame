using UnityEngine;
using System.Collections;
using Common;

public class PhaseManager : MonoBehaviour {

    [ SerializeField ]
    MAIN_GAME_PHASE _main_game_phase;   // メインゲームのフロー

	// Use this for initialization
	void Start( ) {
        _main_game_phase = MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY;
	}
	
	// Update is called once per frame
	void Update( ) {
	    changeMainGamePhase( );
	}

    /// <summary>
    /// 受け取ったデータによってMainGamePhaseを切り替える
    /// </summary>
    void changeMainGamePhase( ) {
        // ThrowDicePhaseへ移行
        if ( Input.GetKeyDown( KeyCode.F1 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE, "ThrowDicePhase" );
        }
        // Assignmentへ移行
        if ( Input.GetKeyDown( KeyCode.F2 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF, "Assignment" );
        }
        // ResultBattleへ移行
        if ( Input.GetKeyDown( KeyCode.F3 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_RESULT_BATTLE, "ResultBattle" );
        }
        // MoveCharacterへ移行
        if ( Input.GetKeyDown( KeyCode.F4 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER, "MoveCharacter" );
        }
        // FieldGimmickへ移行
        if ( Input.GetKeyDown( KeyCode.F5 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_FIELD_GIMMICK, "FieldGimmick" );
        }
        // Finishへ移行
        if ( Input.GetKeyDown( KeyCode.F6 ) ) {
            changeMainGamePhase( MAIN_GAME_PHASE.GAME_PHASE_FINISH, "Finish" );
        }
    }

    /// <summary>
    /// MainGamePhaseが移行可能かどうか確認する
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="log_text"></param>
    void changeMainGamePhase( MAIN_GAME_PHASE phase, string log_text ) {
        try {
            _main_game_phase = phase;
        }
        catch {
            Debug.Log( log_text + "へ移行できませんでした。" );
        }
    }
}
