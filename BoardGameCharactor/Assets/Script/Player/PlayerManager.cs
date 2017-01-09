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

	//エネミー位置枠
	public GameObject _enemy_Baloon_Prefab;

	//キャンバスを取得
	private GameObject _canvas_Root;

	private GameObject _content_Root;

	private GameObject _hand_Area;

	//ファイルマネージャー
	public FileManager _file_manager;

	//プレイヤーネットワークマネージャー
	public PlayerNetWorkManager _player_NetWork_Manager;

	//testでカードマネージャーを使用
	private CardManager _card_Manager;

	//プレイヤーの現在地
	private int _playerHere;

	//ゴールを取得
	private int _goalPoint;

	//1Pか2Pか
	private PLAYER _isPlayer;

	//プレイヤーの勝敗
	private RESULT _isResult;

	//マスとマス間の間
	private float _mass_While_X = 186;

	//吹き出しの横幅
	private float _BaloonWidth = -276;

	//プレイヤーとエネミーの吹き出しの高さ
	private float playerBaloonHeight = 50;
	private float EnemyBaloonHeight = -49;

	//手札限界数
	private int _hand_Max = 6;

	//手札生成数（デバッグ用）
	public int _debug_Hand = 6;

	//セレクトエリアの最大数
	private int _selectArea_Max = 4;

	//セレクトエリアの開始位置X
	private float _select_Area_Start_X = -81;

	//セレクトエリアの横幅
	private float _selectArea_Width = 141;

	//セレクトエリアの高さ
	private float _selectArea_Height = 45;

	//マスアクションフェイズに行っていいか
	private bool _isActionPhase = false;

	//手札データ　選んだカードのリスト、手札のリスト、オブジェクト情報といった形で整理をしています
	private struct HAND_DATA
	{
		public List< CARD_DATA > select_List; 
		public List< CARD_DATA > hand_List;
		public List < GameObject > hand_Obj_List;
		public List < GameObject > select_Obj_List;
		public List < Vector3 > Select_Position;
	}

	private struct PLAYER_FLAG
	{
		public List < bool > select_Position_Use;
	}

	//マップデータを設定
	private FILE_DATA _file_Data = new FILE_DATA ();

	//プレイヤーの手札を設定
	private HAND_DATA _hand_Data = new HAND_DATA( );

	//プレイヤーのフラグを管理
	private PLAYER_FLAG _player_Flags = new PLAYER_FLAG( );

	// Awake関数の代わり
	protected override void initialize( ) {
		PlayerInitialize( );
	}

	void PlayerInitialize(){
		//各種リストを初期化
		_hand_Data.hand_List = new List< CARD_DATA >( );
		_hand_Data.select_List = new List< CARD_DATA >( );
		_hand_Data.hand_Obj_List = new List< GameObject >( );
		_hand_Data.select_Obj_List = new List < GameObject >( );
		_hand_Data.Select_Position = new List < Vector3 > ( );
		_player_Flags.select_Position_Use = new List < bool > ();
	}

    void Start( ) {
		//テストでカードマネージャーを取得
		GameObject cardObj = GameObject.Find("CardManager");
		_card_Manager = cardObj.GetComponent<CardManager> ();

        //自身がプレイヤー1か2か取得
        _isPlayer = _player_NetWork_Manager.getPlayer( );

		//ゴールエリアを取得
        //自身がmapのどの場所にいるかを設定（初期はスタート地点にいるのでStartを探します）
        for ( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
			_file_Data = _file_manager.getMapData( );
			if ( _file_Data.mass[ i ].type == "start" ) {
                _playerHere = i;
            }
			if (_file_Data.mass [i].type == "goal") {
				_goalPoint = i;
			}
        }

		//テストで手札データを生成中
		for (int i = 0; i < _debug_Hand; i++) {
			_player_NetWork_Manager.networkCardIdReceipt (_card_Manager.distributeCard ());
		}

		//セレクトエリアのポジションとフラグを設定
		for (int i = 0; i < _selectArea_Max; i++) {
			//セレクトエリアのフラグをセレクトエリアの数分作成
			_player_Flags.select_Position_Use.Add (false);

			//セレクトエリアのポジションを設定
			Vector3 _position = new Vector3 ( _select_Area_Start_X + _selectArea_Width * i, _selectArea_Height, 0 );

			//セレクトエリアのポジションをセレクトエリアの数分作成
			_hand_Data.Select_Position.Add( _position );
		}

    }

	public void DebugCardDrow(){
		if (Input.GetKeyDown (KeyCode.O)) {
			//キー入力をしたらカードをドロー
			_player_NetWork_Manager.networkCardIdReceipt (_card_Manager.distributeCard ());
		}
	}

	//プレイヤーの現在地を取得する関数の生成
	public int getPlayerHere(){
		return _playerHere;
	}

	//ゴール地点を取得する関数
	public int getGoalPoint(){
		return _goalPoint;
	}

	//プレイヤーの勝敗を取得する関数
	public RESULT getBattleResult(){
		return _isResult;
	}

	public bool getIsActionPhase(){
		return _isActionPhase;
	}

	//バトルリザルトを設定する関数
	public void SetBattleResult( RESULT setResult ){
		_isResult = setResult;
	}

	public void SetIsActionPhase( ){
		_isActionPhase = true;
	}

	//現在の手札の生成を行う
	public void AllHandCreate( ){

		//キャンバスを取得
		if (_canvas_Root == null) {
			_canvas_Root = GameObject.Find("Canvas");
		}

		//ハンドエリアを取得
		if (_hand_Area == null) {
			_hand_Area = GameObject.Find ("HandArea");
		}

		//リストが出来ていなければ生成をする
		if (_hand_Data.hand_List == null) {
			_hand_Data.hand_List = new List< CARD_DATA > ();
		}
		if (_hand_Data.hand_Obj_List == null) {
			_hand_Data.hand_Obj_List = new List< GameObject > ();
		}

		for (int i = 0; i < _hand_Data.hand_List.Count; i++) {
			//プレハブを生成してリストのオブジェクトに入れる
			_hand_Data.hand_Obj_List[i] = ((GameObject)Instantiate (_card_Template_Prefab));

			//カード画像設定
			_hand_Data.hand_Obj_List [i].GetComponent<Card> ().SetCardImage (_hand_Data.hand_List [i].type);

			//カードデータ設定
			_hand_Data.hand_Obj_List [i].GetComponent<Card> ().SetCardData (_hand_Data.hand_List [i]);

			//キャンバスの直下に入れる
			_hand_Data.hand_Obj_List [i].transform.SetParent (_canvas_Root.transform);

			//サイズの修正
			_hand_Data.hand_Obj_List [i].GetComponent<RectTransform> ().localScale = Vector3.one;

			//ハンドエリアの大きさを取得
			float HandArea_Width_Size = _hand_Area.GetComponent<RectTransform> ().sizeDelta.x;
			float HandArea_Postion_Size = _hand_Area.GetComponent<RectTransform> ().anchoredPosition.y;

			float HandCard_Width_Size = _hand_Data.hand_Obj_List [i].GetComponent<RectTransform> ().sizeDelta.x;
			//ハンドエリアの大きさを反転して半分にした数値とカードの大きさのサイズの半分を足した数値をスタート地点に
			float Start_Card_Point = ((-HandArea_Width_Size / 2) + (HandCard_Width_Size / 2)) + HandCard_Width_Size;

			//手札エリアの大きさ/手札限界数で割った後に現在の生成中の手札の順番を掛ける
			float Card_While_X = (HandArea_Width_Size / _hand_Max) * i;

			//位置を設定する
			_hand_Data.hand_Obj_List [i].GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (Start_Card_Point + Card_While_X, HandArea_Postion_Size, 0);
		}
	}

	//並び替え

	//最新のカードを選択して生成を行う
	public void setHandLatestCardCreate( CARD_DATA setCard ){

		//キャンバスを取得
		if (_canvas_Root == null) {
			_canvas_Root = GameObject.Find("Canvas");
		}

		//ハンドエリアを取得
		if (_hand_Area == null) {
			_hand_Area = GameObject.Find ("HandArea");
		}

		//リストが出来ていなければ生成をする
		if (_hand_Data.hand_List == null) {
			_hand_Data.hand_List = new List< CARD_DATA > ();
		}
		if (_hand_Data.hand_Obj_List == null) {
			_hand_Data.hand_Obj_List = new List< GameObject > ();
		}
			
		//カードを手札に追加
		_hand_Data.hand_List.Add( setCard );

		//手札の最大値6よりも大きいなら
		if (_hand_Max < _hand_Data.hand_List.Count) {
			_hand_Max = _hand_Data.hand_List.Count;
		} else {
			_hand_Max = 6;
		}

		//プレハブを生成してリストのオブジェクトに入れる
		_hand_Data.hand_Obj_List.Add ((GameObject)Instantiate (_card_Template_Prefab));

		//最新の手札の配列値を取得
		int HandDataLatest =_hand_Data.hand_List.Count - 1;

		//カード画像設定
		_hand_Data.hand_Obj_List [HandDataLatest].GetComponent<Card> ().SetCardImage ( _hand_Data.hand_List[HandDataLatest].type );

		//カードデータ設定
		_hand_Data.hand_Obj_List [HandDataLatest].GetComponent<Card> ().SetCardData ( _hand_Data.hand_List[HandDataLatest]);

		//キャンバスの直下に入れる
		_hand_Data.hand_Obj_List [HandDataLatest].transform.SetParent(_canvas_Root.transform);

		//サイズの修正
		_hand_Data.hand_Obj_List [HandDataLatest].GetComponent<RectTransform> ().localScale = Vector3.one;

		//ハンドエリアの大きさを取得
		float HandArea_Width_Size = _hand_Area.GetComponent<RectTransform>().sizeDelta.x;
		float HandArea_Postion_Size = _hand_Area.GetComponent<RectTransform>().anchoredPosition.y;

		float HandCard_Width_Size = _hand_Data.hand_Obj_List [HandDataLatest].GetComponent<RectTransform> ().sizeDelta.x;
		//ハンドエリアの大きさを反転して半分にした数値とカードの大きさのサイズの半分を足した数値をスタート地点に
		float Start_Card_Point = ((-HandArea_Width_Size / 2) + ( HandCard_Width_Size / 2)) + HandCard_Width_Size ;

		//手札エリアの大きさ/手札限界数で割った後に現在の生成中の手札の順番を掛ける
		float Card_While_X = ( HandArea_Width_Size / _hand_Max ) * HandDataLatest;

		//位置を設定する
		_hand_Data.hand_Obj_List[HandDataLatest].GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 ( Start_Card_Point + Card_While_X, HandArea_Postion_Size, 0 ) ;

	}

	public void setPlayerObject( ){
		//タグでプレイヤーによって変わる部分を取得し色とテキストを変えます
		GameObject[] PlayerChanges = GameObject.FindGameObjectsWithTag("PlayerChange");
		for(int i = 0; i < PlayerChanges.Length; i++){
			switch (PlayerChanges [i].name) {

			case "EnemyLabel":
				//テキストを取得
				Text _enemyText = PlayerChanges [i].GetComponentInChildren<Text> ();
				Debug.Log (_isPlayer);
				//対応したプレイヤーによって色とテキストを変える
				if (_isPlayer == PLAYER.PLAYER_1) {
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
				if (_isPlayer == PLAYER.PLAYER_1) {
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

	public void SetPlayerMove( int SetMoveNumber = 0, bool Setback = false ){
		
		//キャンバスを取得
		if (_canvas_Root == null) {
			_canvas_Root = GameObject.Find("Canvas");
		}
		//コンテンツを取得
		if (_content_Root == null) {
			_content_Root = GameObject.Find ("Content");
		}

		//プレイヤーの位置を参照してマスに向けて吹き出しを作ります
		//移動数分をプレイヤーの現在地に
		_playerHere += SetMoveNumber;
		if (_playerHere >= _goalPoint) {
			//ゴール地点より先に行ってしまったらゴール地点で止める
			_playerHere = _goalPoint;
		}

		//エネミーの現在地をネットワークの持っているデータで取得
		int _enemyHere = _player_NetWork_Manager.getEnemyHere();
		//セットバックがtrueならここを実行しない
		if (!Setback) {
			//プレイヤー1とプレイヤー2の吹き出しを設定
			GameObject Player_Baloon_Obj = (GameObject)Instantiate (_player_Baloon_Prefab);
			GameObject Enemy_Baloon_Obj = (GameObject)Instantiate (_enemy_Baloon_Prefab);

			//キャンバスのContentに入れる
			Player_Baloon_Obj.transform.SetParent (_content_Root.transform);
			Enemy_Baloon_Obj.transform.SetParent (_content_Root.transform);

			//サイズの修正
			Player_Baloon_Obj.GetComponent<RectTransform> ().localScale = Vector3.one;
			Enemy_Baloon_Obj.GetComponent<RectTransform> ().localScale = Vector3.one;

			//プレイヤーオブジェクトを対応したマスのところに移動(数値分Xをずらす)
			Player_Baloon_Obj.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (_BaloonWidth + (_playerHere * _mass_While_X), playerBaloonHeight, 0);
			Enemy_Baloon_Obj.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (_BaloonWidth + (_enemyHere * _mass_While_X), EnemyBaloonHeight, 0);

			//フィールドの最大値から現在の値を
			int FiledPos = _file_manager.getMassCount () - _playerHere;

			//ゴールナビオブジェクトの取得
			GameObject GoalNavi = GameObject.FindWithTag ("GoalNavi");

			//子のゴールテキストの取得
			Text GoalText = GoalNavi.GetComponentInChildren<Text> ();

			//テキストの設定
			GoalText.text = "宝まで " + FiledPos + "マス";
		}
	}

	public void SetSelectAreaCard( ){

		//セレクトエリアに入っているなら手札リストからセレクトカードリストに移動する
		//自身の現在の手札数を行う
		for (int i = 0; i < _hand_Data.hand_List.Count; i++) {
			bool SelectAreaCheck = _hand_Data.hand_Obj_List [i].GetComponent<Card> ().getInSelectArea ();
			//セレクトエリアに入っているか
			if (SelectAreaCheck) {

				//オブジェクトをセレクトオブジェクトリストに追加
				_hand_Data.select_Obj_List.Add (_hand_Data.hand_Obj_List [i]);
			}
		}
	}

	public bool SetSelectAreaPosition( CARD_DATA setCard ){

		//セレクトエリアのポジションに開いてる順番に入れるようにします
		for (int i = 0; i < _hand_Data.hand_List.Count; i++) {
			//手札とセットカードのIDが一致した場合
			if (_hand_Data.hand_List [i].id == setCard.id) {
				//セレクトエリアのポジションの0番目から～3番目までを順番に確認して使われているかどうかを見る
				if (!_player_Flags.select_Position_Use [0] || _hand_Data.select_List[0].id == setCard.id ) {
					//カードを保存しておく
					_hand_Data.select_List.Add( _hand_Data.hand_List [i] );
					_hand_Data.select_Obj_List.Add (_hand_Data.hand_Obj_List [i] );
					//カードのポジションを変更
					_hand_Data.hand_Obj_List [i].GetComponent<RectTransform>().anchoredPosition3D = _hand_Data.Select_Position[0];
					_player_Flags.select_Position_Use[0] = true;
					return true;
				}

				if (!_player_Flags.select_Position_Use [1] || _hand_Data.select_List[1].id == setCard.id ) {
					//カードを保存しておく
					_hand_Data.select_List.Add( _hand_Data.hand_List [i] );
					_hand_Data.select_Obj_List.Add (_hand_Data.hand_Obj_List [i] );
					//カードのポジションを変更
					_hand_Data.hand_Obj_List [i].GetComponent<RectTransform>().anchoredPosition3D = _hand_Data.Select_Position[1];
					_player_Flags.select_Position_Use[1] = true;
					return true;
				}

				if (!_player_Flags.select_Position_Use [2] || _hand_Data.select_List[2].id == setCard.id ) {
					//カードを保存しておく
					_hand_Data.select_List.Add(_hand_Data.hand_List [i] );
					_hand_Data.select_Obj_List.Add (_hand_Data.hand_Obj_List [i] );
					//カードのポジションを変更
					_hand_Data.hand_Obj_List [i].GetComponent<RectTransform>().anchoredPosition3D = _hand_Data.Select_Position[2];
					_player_Flags.select_Position_Use[2] = true;
					return true;
				}
				if (!_player_Flags.select_Position_Use [3] || _hand_Data.select_List[3].id == setCard.id ) {
					//カードをセレクトエリアに
					_hand_Data.select_List.Add( _hand_Data.hand_List [i] );
					_hand_Data.select_Obj_List.Add (_hand_Data.hand_Obj_List [i] );
					//カードのポジションを変更
					_hand_Data.hand_Obj_List [i].GetComponent<RectTransform>().anchoredPosition3D = _hand_Data.Select_Position[3];
					_player_Flags.select_Position_Use[3] = true;
					return true;
				}
			}
		}
		return false;
	}

	public void SetSelectAreaOut( CARD_DATA setCard ){
		//選択カードから手札に戻す場合にフラグをアウトにする
		for (int i = 0; i < _hand_Data.select_List.Count; i++) {
			//セレクトカードと消すカードのIDが一致した場合
			if (_hand_Data.select_List [i].id == setCard.id) {
				//その選択カードの情報を消してその場所のフラグをoffにする
				_hand_Data.hand_List.Add( _hand_Data.select_List[i]);
				_hand_Data.hand_Obj_List.Add (_hand_Data.select_Obj_List [i] );
				_player_Flags.select_Position_Use [i] = false;
			}
		}
	}

	//カード生成を行う
	public void DeckCardList( CARD_DATA GetCard ){
		//最新のカードを生成
		setHandLatestCardCreate ( GetCard );
	}

	//現在の手札枚数を取得する
	public int getHandListNumber(){
		return _hand_Data.hand_List.Count;
	}

	//確定カードの枚数を取得する
	public int getSelectListNumber(){
		return _hand_Data.hand_List.Count;
	}

	//現在の手札をサーチしてあるかどうかを取得
	public bool getHandSerach( CARD_TYPE cardType ){
		//現在の手札をサーチして選んだカードタイプを取得
		for (int i = 0; i < _hand_Data.hand_List.Count; i++) {
			if ( _hand_Data.hand_List [i].type == cardType ) {
				return true;
			}
		}
		return false;
	}

	//現在の手札をサーチして取得
	public CARD_DATA getHandData( CARD_TYPE CardType ){
		CARD_DATA card = new CARD_DATA();
		//現在の手札をサーチして選んだカードタイプのカードデータを取得
		for (int i = 0; i < _hand_Data.hand_List.Count; i++) {
			if ( _hand_Data.hand_List [i].type == CardType ) {
				card = _hand_Data.hand_List [i];
				return card;
			}
		}
		return card;
	}

	//設定したカードが手札にあれば削除を行う
	public bool CardDelete( CARD_DATA Card ){
		//現在の手札をサーチして選んだカードタイプのカードデータを取得
		for (int i = 0; i < _hand_Data.hand_List.Count; i++) {
			//手札に同一のカードID
			if ( _hand_Data.hand_List [i].id == Card.id ) {

				//設定した手札を削除
				Destroy (_hand_Data.hand_Obj_List [i]);
				//手札リストとオブジェクトリストから削除
				_hand_Data.hand_List.RemoveAt (i);
				_hand_Data.hand_Obj_List.RemoveAt (i);
				return true;
			}
		}
		return false;
	}

	//セレクトエリアのカードを全て削除する
	public void SelectAreaDelete(){
		for (int i = _hand_Data.select_List.Count - 1; i >= 0; i--) {
			//セレクトリストとセレクトオブジェクトリストから削除
			_hand_Data.hand_List.RemoveAt (i);
			_hand_Data.hand_Obj_List.RemoveAt (i);
			_player_Flags.select_Position_Use [i] = false;
		}
	}

	//セレクトエリアのカードを全て戻す
	public void SelectAreaReturn(){
		for (int i = _hand_Data.select_List.Count - 1; i >= 0; i--) {
			//セレクトリストとセレクトオブジェクトリストから削除
			_hand_Data.select_List.RemoveAt (i);
			_hand_Data.select_Obj_List.RemoveAt (i);
			_player_Flags.select_Position_Use [i] = false;
		}
	}
}
