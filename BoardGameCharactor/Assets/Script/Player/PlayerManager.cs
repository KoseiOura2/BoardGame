using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;

public class PlayerManager : Manager<PlayerManager> {

	private GameObject _card_Template_Prefab;

	public GameObject _player_Baloon_Prefab;

	//キャンバスを取得
	public GameObject _canvas_Root;

	//経過時間を取得
	private float nowTime;

	//プレイヤーの現在地
	private int _playerHere;

	//待ち時間を設定
	public float intervalTime = 3.0f;

	public FileManager _file_manager;

	public PlayerNetWorkManager _player_NetWork_Manager;

	//1Pか2Pか
	private PLAYER isPlayer; 

	//手札情報
	private struct HAND_DATA
	{
		public List< CARD_DATA > select_list; 
		public List< CARD_DATA > hand_list;
		public List < GameObject > hand_Obj;
	}

	//プレイヤーオブジェクト情報
	private enum PlayerObjects {
		EnemyLabel,
		EnemyHand,
		EnemyStates,
		GoalNavi,
		PlayerLabel
	}

	private FILE_DATA _file_data = new FILE_DATA ();
	private HAND_DATA _hand_data = new HAND_DATA( );

	// Awake関数の代わり
	protected override void initialize( ) {
		PlayerInitialize( );
	}

	void PlayerInitialize(){
		//リストを初期化
		_hand_data.hand_list = new List< CARD_DATA >( );
		_hand_data.hand_Obj = new List< GameObject >( );
		if (_card_Template_Prefab == null) {
			_card_Template_Prefab = (GameObject)Resources.Load ("Resources/Prefab/Card");
		}

		//自身がプレイヤー1か2か取得
		isPlayer = _player_NetWork_Manager.getPlayer();

		//自身がmapのどの場所にいるかを設定（初期はスタート地点にいるのでStartを探します）
		for (int i = 0; i < _file_manager.getMassCount (); i++) {
			_file_data = _file_manager.getMapData ();
			if (_file_data.mass [i].type == "start") {
				_playerHere = i;
			}
		}

	}

	//プレイヤーの現在地を取得する関数の生成
	public int getPlayerHere(){
		return _playerHere;
	}

	//自身の手札リストの中のカードを生成して表示を行う
	public void setHandCardCreate(){
		//生成出来ていなければ生成をする
		if (_hand_data.hand_list == null) {
			_hand_data.hand_list = new List< CARD_DATA > ();
		}
		if (_hand_data.hand_Obj == null) {
			_hand_data.hand_Obj = new List< GameObject > ();
		}

		//自身の初期手札数文を生成
		for (int i = 0; i < _hand_data.hand_list.Count; i++) {
			//プレハブを生成してリストのオブジェクトに入れる
			_hand_data.hand_Obj.Add ((GameObject)Instantiate (_card_Template_Prefab));
			//カード画像設定
			_hand_data.hand_Obj [i].GetComponent<Card> ().SetCardImage ( _hand_data.hand_list[i].type );
			//手札の枚数によって表示位置をずらしていく
		}
	}

	public void setPlayerObject( ){
		//タグでプレイヤーによって変わる部分を取得しfor文で処理を行う
		//もう一つ忘れない内に相手の手札やステータスに変動がおきた場合変更を加えるシステムを作ります（ここでUpdate）で関数でうまくやりましょう？
		GameObject[] PlayerChanges = GameObject.FindGameObjectsWithTag("PlayerChange");
		for(int i = 0; i < PlayerChanges.Length; i++){
			switch (PlayerChanges [i].name) {
			case "EnemyLabel":
				//テキストを取得
				Text _enemyText = PlayerChanges [i].GetComponentInChildren<Text> ();
				Debug.Log (isPlayer);
				//対応したプレイヤーによって色とテキストを変える
				if (isPlayer == PLAYER.PLAYER_1) {
					_enemyText.text = "2P";
					PlayerChanges [i].GetComponent<Image> ().color = new Color (0, 1, 0);
				} else {
					_enemyText.text = "1P";
					PlayerChanges [i].GetComponent<Image> ().color = new Color (1, 0, 0);
				}
				break;
			case "PlayerLabel":
				//テキストを取得
				Text _playerText = PlayerChanges [i].GetComponentInChildren<Text> ();
				//対応したプレイヤーによって色とテキストを変える
				if (isPlayer == PLAYER.PLAYER_1) {
					_playerText.text = "1P";
					PlayerChanges [i].GetComponent<Image> ().color = new Color (1, 0, 0);
				} else {
					_playerText.text = "2P";
					PlayerChanges [i].GetComponent<Image> ().color = new Color (0, 1, 0);
				}
				break;
			}
		}
	}

	public void SetEnemyObject(){
		//テキストと手札を取得
		GameObject[] EnemyChanges = GameObject.FindGameObjectsWithTag("EnemyChange");
		for(int i = 0; i < EnemyChanges.Length; i++){
			switch (EnemyChanges [i].name) {
			case "EnemyHand":
				//テキストを取得
				Text _enemyHandText = EnemyChanges [i].GetComponentInChildren<Text> ();
				//相手の手札を取得
				Debug.Log(_player_NetWork_Manager.getEnemyHand());
				int _enemyHand = _player_NetWork_Manager.getEnemyHand();
				_enemyHandText.text = "相手の手札　" + _enemyHand + "枚";
				break;
			case "EnemyStates":
				//テキストを取得
				Text _enemyStatusText = EnemyChanges [i].GetComponentInChildren<Text> ();
				//相手のステータスを取得
				int _enemyStatus = _player_NetWork_Manager.getEnemyStatus();
				_enemyStatusText.text = "ステータス " + _enemyStatus;
				break;
			}
		}
	}

	public void SetPlayerMove( int SetMoveNumber ){

		//プレイヤーの吹き出しを設定

		//移動数分をプレイヤーの現在地に
		_playerHere += SetMoveNumber;

		//プレイヤーオブジェクトを対応したマスのところに移動(数値分Xをずらす)
		//_start_Mass_X + (count * _mass_While_X), _start_Mass_Y, 0 );

		//フィールドの最大値から現在の値を
		int FiledPos =_playerHere - _file_manager.getMassCount();
		GameObject GoalNavi = GameObject.FindWithTag ("GoalNavi");
		Text GoalText = GoalNavi.GetComponentInChildren<Text> ();
		GoalText.text = "宝まで " + FiledPos + "マス";
	}

	//指定されたカードをリストに入れ生成を行う

	//持続効果をセットする関数
}
