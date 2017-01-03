using UnityEngine;
using System.Collections;

public class diceButtonScript : MonoBehaviour {

	private GameObject _playerManager;

	// Use this for initialization
	void Awake () {
		if (_playerManager == null) {
			_playerManager = GameObject.Find ("PlayerManager");
		}
	}

	public void onDiceButtonPush(){
		//1から3までをランダムで取得
		int DiceNumber = Random.Range (1, 6);
		_playerManager.GetComponent<PlayerManager> ().SetDiceData (DiceNumber);
	}
}
