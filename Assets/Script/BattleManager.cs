using UnityEngine;
using System.Collections;
using System;

public class BattleManager : MonoBehaviour {

	enum BATTLE_PHASE{
		DICE_PHASE,
		DROW_PHASE,
		CARD_PHASE,
		RESULT_PHASE
	}

	private BATTLE_PHASE currentPhase;

	// Use this for initialization
	void Start () {
		currentPhase = BATTLE_PHASE.DICE_PHASE;
	}
	
	// Update is called once per frame
	void Update () {
		//フェイズ読み込み
		phaseLoad ();

		//Zキーを押したら次のEnumの値に
		if (Input.GetKeyDown(KeyCode.Z) ) {
			if (currentPhase != BATTLE_PHASE.RESULT_PHASE) {
				currentPhase++;
			} else {
				currentPhase = BATTLE_PHASE.DICE_PHASE;
			}
		}

	}

	void phaseLoad (){
		switch (currentPhase) {

		case BATTLE_PHASE.DICE_PHASE:
			dicePhase ();
			break;
		
		case BATTLE_PHASE.DROW_PHASE:
			drowPhase ();
			break;

		case BATTLE_PHASE.CARD_PHASE:
			cardPhase ();
			break;

		case BATTLE_PHASE.RESULT_PHASE:
			result_Phase ();
			break;

		default:
			Debug.LogError ("errorCase:" + currentPhase);
			break;
		}
	}

	void dicePhase (){
		Debug.Log ("ダイスフェイズです");
	}

	void drowPhase (){
		Debug.Log ("ドローフェイズです");
	}

	void cardPhase (){
		Debug.Log ("カードフェイズです");
	}

	void result_Phase (){
		Debug.Log ("リザルトフェイズです");
	}

}
