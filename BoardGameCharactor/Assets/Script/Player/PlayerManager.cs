using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;

public class PlayerManager : Manager<PlayerManager> {

	//2Dカードプレハブ
	public GameObject _card_Template_Prefab;

	//プレイヤー位置枠
	public GameObject _player_Baloon_Prefab;

	//キャンバスを取得
	public GameObject _canvas_Root;

	//待ち時間を設定
	public float intervalTime = 3.0f;

	//ファイルマネージャー
	public FileManager _file_manager;

	//プレイヤーネットワークマネージャー
	public PlayerNetWorkManager _player_NetWork_Manager;

	//経過時間を取得
	private float nowTime;

	//プレイヤーの現在地
	private int _playerHere;

	//1Pか2Pか
	private PLAYER isPlayer; 

	//手札データ　選んだカードのリスト、手札のリスト、オブジェクト情報といった形で整理をしています
	private struct HAND_DATA
	{
		public List< CARD_DATA > select_List; 
		public List< CARD_DATA > hand_List;
		public List < GameObject > hand_Obj_List;
		public List < GameObject > select_Obj_List;
	}

	//セレクトエリアのポジションをリストにして4つ分用意する。

	//マップデータを設定
	private FILE_DATA _file_data = new FILE_DATA ();

	//プレイヤーの手札を設定
	private HAND_DATA _hand_data = new HAND_DATA( );

	// Awake関数の代わり
	protected override void initialize( ) {
		PlayerInitialize( );
	}

	void PlayerInitialize(){
		//リストを初期化
		_hand_data.hand_List = new List< CARD_DATA >( );
		_hand_data.hand_Obj_List = new List< GameObject >( );

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
		if (_hand_data.hand_List == null) {
			_hand_data.hand_List = new List< CARD_DATA > ();
		}
		if (_hand_data.hand_Obj_List == null) {
			_hand_data.hand_Obj_List = new List< GameObject > ();
		}
		//自身の初期手札数文を生成
		for (int i = 0; i < _hand_data.hand_List.Count; i++) {
			//プレハブを生成してリストのオブジェクトに入れる
			_hand_data.hand_Obj_List.Add ((GameObject)Instantiate (_card_Template_Prefab));
			//カード画像設定
			_hand_data.hand_Obj_List [i].GetComponent<Card> ().SetCardImage ( _hand_data.hand_List[i].type );
			//初期位置を設定する
			//_hand_data.hand_Obj_List [i].GetComponent<RectTransform> ().anchoredPosition();
			//手札の枚数によって表示位置をずらしていく
		}
	}

	public void setPlayerObject( ){
		//タグでプレイヤーによって変わる部分を取得し色とテキストを変えます
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
		//敵のオブジェクトをセット
		//テキストと手札を取得
		GameObject[] EnemyChanges = GameObject.FindGameObjectsWithTag("EnemyChange");

		for(int i = 0; i < EnemyChanges.Length; i++){
			switch (EnemyChanges [i].name) {

			case "EnemyHand":
				//テキストを取得
				Text _enemyHandText = EnemyChanges [i].GetComponentInChildren<Text> ();
				//相手の手札を取得
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
		//プレイヤーの位置を参照してマスに向けて吹き出しを作ります

		//プレイヤーの吹き出しを設定

		//移動数分をプレイヤーの現在地に
		_playerHere += SetMoveNumber;

		//プレイヤーオブジェクトを対応したマスのところに移動(数値分Xをずらす)
		//_start_Mass_X + (count * _mass_While_X), _start_Mass_Y, 0 );

		//フィールドの最大値から現在の値を
		int FiledPos =_playerHere - _file_manager.getMassCount();
		//ゴールナビオブジェクトの取得
		GameObject GoalNavi = GameObject.FindWithTag ("GoalNavi");
		//子のゴールテキストの取得
		Text GoalText = GoalNavi.GetComponentInChildren<Text> ();
		//テキストの設定
		GoalText.text = "宝まで " + FiledPos + "マス";
	}

	public void SetSelectAreaCard( ){
		//セレクトエリアに入っているなら手札リストからセレクトカードリストに移動して手札リストとオブジェクトリストから削除をする
		//自身の現在の手札数を行う
		for (int i = 0; i < _hand_data.hand_List.Count; i++) {
			bool SelectAreaCheck = _hand_data.hand_Obj_List [i].GetComponent<Card> ().getInSelectArea();
			//セレクトエリアに入っているか
			if (SelectAreaCheck) {
				//セレクトカードリストに追加
				_hand_data.select_List.Add (_hand_data.hand_List [i]);
				//オブジェクトをセレクトオブジェクトリストに追加
				_hand_data.select_Obj_List.Add (_hand_data.hand_Obj_List [i]);
				//手札リストとオブジェクトリストから削除
				_hand_data.hand_List.RemoveAt (i);
				_hand_data.hand_Obj_List.RemoveAt (i);
			}
		}
	}

	//カード生成を行う
	public void DeckCardList(){
		//カードを特定の回数回して生成するように？
		_hand_data.hand_List.Add(_player_NetWork_Manager.cardDataReceipt () );
	}
	//持続効果をセットする関数
}
