using UnityEngine;
using System.Collections;
using System;

public class BattleManager : MonoBehaviour {

	//フェイズ毎の初期設定フラグ
	bool initial_setting = false;

	//バトルフェイズ内容
	enum BATTLE_PHASE{
		DICE_PHASE,
		DROW_PHASE,
		CARD_PHASE,
		RESULT_PHASE
	}

	//現在のフェイズ
	private BATTLE_PHASE currentPhase;

	// Use this for initialization
	void Start () {
		//最初のフェイズをロード
		currentPhase = BATTLE_PHASE.DICE_PHASE;
	}
	
	// Update is called once per frame
	void Update () {
		//フェイズ読み込み
		phaseLoad ();

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
			resultPhase ();
			break;

		default:
			Debug.LogError ("errorCase:" + currentPhase);
			break;
		}
	}

	void dicePhase (){
		if (!initial_setting) {
			Debug.Log ("ダイスフェイズです");
			//初期設定完了フラグ
			initial_setting = true;
		} else {
		}
	}

	void drowPhase (){
		if (!initial_setting) {
			Debug.Log ("ドローフェイズです");
			//初期設定完了フラグ
			initial_setting = true;
		} else {
		}
	}

	void cardPhase (){
		if (!initial_setting) {
			Debug.Log ("カードフェイズです");
			//初期設定完了フラグ
			initial_setting = true;
		} else {
		}
	}

	void resultPhase (){
		if (!initial_setting) {
			Debug.Log ("リザルトフェイズです");
			//初期設定完了フラグ
			initial_setting = true;
		} else {
		}
	}

	public void phaseChange(){
		//渡す数値を保存しておいてその数値を通信に渡せるようにする


		//カードを消し、画面をくらくしてテキストを表示

		//初期設定フラグをoffに
		initial_setting = false;

		//フラグが立つと次のフェイズに移行
		if (currentPhase != BATTLE_PHASE.RESULT_PHASE) {
			currentPhase++;
		} else {
			currentPhase = BATTLE_PHASE.DICE_PHASE;
		}
	}

}
