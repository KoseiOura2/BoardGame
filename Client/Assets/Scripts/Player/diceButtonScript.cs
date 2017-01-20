using UnityEngine;
using System.Collections;

public class diceButtonScript : MonoBehaviour {

	private GameObject _player_phase_manager;

	// Use this for initialization
	void Awake ( ) {
		if ( _player_phase_manager == null ) {
            _player_phase_manager = GameObject.Find( "PlayerPhaseManager" );
		}
	}

    public void onDiceButtonPush ( ) {
        //1から3までをランダムで取得
        int DiceNumber = Random.Range( 1, 3 );
        _player_phase_manager.GetComponent< PlayerPhaseManager > ( ).setDiceData( DiceNumber );
    }
}
