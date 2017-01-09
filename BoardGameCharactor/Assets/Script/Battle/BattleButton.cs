using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class BattleButton : MonoBehaviour {

	private BattlePhaseManager _battle_Phase_Manager;

	private string Yes_Text = "YES";

	// Use this for initialization
	void Awake () {
		//バトルマネージャーの取得、存在しなければ取得しない
		if (_battle_Phase_Manager == null) {
			GameObject _battle_Manager_Obj = GameObject.Find ("BattlePhaseManager");
			if (_battle_Manager_Obj != null) {
				_battle_Phase_Manager = _battle_Manager_Obj.GetComponent<BattlePhaseManager> ();
			}
		}
	}

	public void PushButton(){
		//自身がYESかNOかによってシーンで返答を変える
		Text judghText = GetComponentInChildren<Text>();
		//テキストがYESであるかどうか
		if (judghText.text == Yes_Text) {
			//シーンがドローフェイズであるなら
			if (_battle_Phase_Manager.GetMainGamePhase () == MAIN_GAME_PHASE.GAME_PHASE_DROW) {
				//ドローカードを使用することを選択したことを送信
				_battle_Phase_Manager.DrowCardUse (true);
			}
		} else {
			//テキストがYESでないならNOである
			//シーンがドローフェイズであるなら
			if (_battle_Phase_Manager.GetMainGamePhase () == MAIN_GAME_PHASE.GAME_PHASE_DROW) {
				//ドローカードを使用しないことを選択したことを送信
				_battle_Phase_Manager.DrowCardUse (false);
			}
		}
	}

}
