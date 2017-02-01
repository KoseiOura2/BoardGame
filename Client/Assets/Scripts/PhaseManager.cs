using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class PhaseManager : MonoBehaviour {

    [ SerializeField ]
    private MAIN_GAME_PHASE _main_game_phase;   // メインゲームのフロー
	private bool _phase_changed = false;

	// Use this for initialization
	void Start( ) {
        _main_game_phase = MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY;
	}
	
	// Update is called once per frame
	void Update( ) {
		
	}

    /// <summary>
    /// MainGamePhaseが移行可能かどうか確認する
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="log_text"></param>
    public void changeMainGamePhase( MAIN_GAME_PHASE phase, string log_text ) {
        try {
            _main_game_phase = phase;
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
