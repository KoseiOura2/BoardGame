using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class BattleManager : MonoBehaviour {
    
	private const int MAX_SEND_CARD_NUM = 4;
    private const float BATTLE_TIME = 60;

    private float _battle_time = BATTLE_TIME;
    private bool _complete = false;

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {
	}

    public int[ ] resultSelectCard( int[ ] select_card_list ) {
        List< int > card_list = new List< int >( );

        for ( int i = 0; i < select_card_list.Length; i++ ) {
            card_list.Add( select_card_list[ i ] );
        }

        int[ ] return_card_list = new int[ card_list.Count ];

        for ( int i = 0; i < card_list.Count; i++ ) {
            return_card_list[ i ] = card_list[ i ];
        }

        return return_card_list;

    }

    public void readyComplete( ) {
        _complete = true;
    }

    public void battleTimeCount( ) {
        if ( !_complete ) {
            _battle_time -= Time.deltaTime;
            Debug.Log( _battle_time );

            if ( _battle_time < 0 ) {
                _battle_time = 0;
                _complete = true;
            }
        }
    }

    public void refreshBattleTime( ) {
        _battle_time = BATTLE_TIME;
    }

    public bool isComplete( ) {
        if ( _complete ) {
            _complete = false;
            return true;
        }

        return false;
    }
}
