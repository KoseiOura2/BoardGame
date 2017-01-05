using UnityEngine;
using System.Collections;

public class diceButtonScript : MonoBehaviour {

	private GameObject _playerPhaseManager;

	// Use this for initialization
	void Awake () {
		if (_playerPhaseManager == null) {
			_playerPhaseManager = GameObject.Find ("PlayerPhaseManager");
		}
	}

	public void onDiceButtonPush(){
		//1から3までをランダムで取得
		int DiceNumber = Random.Range (1, 6);
		_playerPhaseManager.GetComponent<PlayerPhaseManager> ().SetDiceData (DiceNumber);
	}
}
