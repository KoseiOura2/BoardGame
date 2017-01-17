using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class BattleButton : MonoBehaviour {

	private BattlePhaseManager _battle_phase_manager;

	private string yes_text = "YES";

	// Use this for initialization
	void Awake( ) {
		//バトルマネージャーの取得
		if ( _battle_phase_manager == null ) {
			_battle_phase_manager = GameObject.Find ( "BattlePhaseManager" ).GetComponent< BattlePhaseManager > ( );
		}
	}

	public void pushButton( ) {
		//自身がYESかNOかによってシーンで返答を変える
		Text judghText = GetComponentInChildren< Text >( );
		//テキストがYESであるかどうか
		if ( judghText.text == yes_text ) {
			//ドローカードを使用することを選択したことを送信
			_battle_phase_manager.drowCardUse ( true );
		} else {
			//テキストがYESでないならNO
			_battle_phase_manager.drowCardUse ( false );
		}
	}

    public void cardSelectButton ( ) {
        _battle_phase_manager.select_push ( );
    }

}
