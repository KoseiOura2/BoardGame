using UnityEngine;
using System.Collections;
using Common;

public class PlayerNetWorkManager : Manager<PlayerNetWorkManager> {

	public PlayerManager _player_manager;

    //自身がどっちのプレイヤーか
	private PLAYER_ORDER _currentPlayer;

    //敵の手札数
	private int _enemyHandNumber;

    //敵のステータス
	private int _enemyStatus;

	//敵の現在地
	private int _EnemyHere;

	//ドローが完了したか
	private bool DrowCompletion;

	// Awake関数の代わり
	protected override void initialize( ) {
		cheackNetWork( );
	}

	//ここでネットとの受信が出来ているか確認、出来ていなければエラーを送る、自身がプレイヤー1かプレイヤー2か取得
	void cheackNetWork(){

		//相手の手札数を取得 今回は仮で6枚固定
		_enemyHandNumber = 6;

		//相手のステータスを取得 仮で10枚固定
		_enemyStatus = 10;

		//相手の現在地を取得　仮で0固定
		_EnemyHere = 0;

		//プレイヤーが1か2か取得今回は仮でプレイヤー1にする
		_currentPlayer = PLAYER_ORDER.PLAYER_ONE;

		//今回は仮で勝利固定
		networkResultReceipt(BATTLE_RESULT.WIN);
		//ドローが終了したかどうか
		DrowCompletion = false;
	}

	//通信から次のフェイズに進む要求が出た場合に受信フラグをONにする
	public bool networkPhaseChangeReceipt( ) {
        //通信に成功したかどうか
        bool isNetworkReceipt = false;

		//通信を飛ばしてフェイズ受信命令がないかを確認をする。受信フラグが来ていたらtrue
        if ( isNetworkReceipt ) {
            return true;
        } else {
            return false;
        }
	}

	//通信から勝敗をセット
	public bool networkResultReceipt( BATTLE_RESULT _SetBattleResult ){
		//通信に成功したかどうか
		bool isNetworkReceipt = false;

		//勝敗をセット
		_player_manager.SetBattleResult(_SetBattleResult);

		//通信に成功したらtrue失敗したらfalse
		if ( isNetworkReceipt ) {
			return true;
		} else {
			return false;
		}
	}

	//通信からカードIDをセット
    public bool networkCardIdReceipt( CARD_DATA SetCardID ) {
        //通信に成功したかどうか
        bool isNetworkReceipt = false;
        //貰うデータはドローカードのＩＤ
        //プレイヤーマネージャーでカードデータを生成して手札に加える
		_player_manager.DeckCardList( SetCardID );
        //通信に成功したらtrue失敗したらfalse
        if ( isNetworkReceipt ) {
            return true;
        } else {
            return false;
        }
    }

	//通信から敵プレイヤーのデータをセット
	public bool networkEnemyDataReceipt( int EnemyHand, int EnemyStatus, int EnemyHere ) {
        //通信に成功したかどうか
        bool isNetworkReceipt = false;
        //敵の手札数とステータスと現在地を保存
        _enemyHandNumber = EnemyHand;
        _enemyStatus = EnemyStatus;
		_EnemyHere = EnemyHere;
        //通信に成功したらtrue失敗したらfalse
        if ( isNetworkReceipt ) {
            return true;
        } else {
            return false;
        }
    }

	//通信からドローが終了したかどうかをセット
	public bool isnetworkDrowDataLimit( bool isCardLimit ){
		//ドローが終了したかどうかを取得
		DrowCompletion = isCardLimit;
		return DrowCompletion;
	}

	//デバッグ用でIキーを押したらドローが完了する
	public void DebugDrowCompletion(){
		if (Input.GetKeyDown (KeyCode.I)) {
			DrowCompletion = true;
		}
	}

	public bool networkDataAcross( MAIN_GAME_PHASE SetPhase, int RandomData = 0, bool isGoal = false ,  int Playertrout = 0, CARD_DATA DrowCardID = new CARD_DATA() ) {
		//ここでネットワークデータの送信を行います
        //送信が成功すればtrueに
		bool isNetworkAcross = false;
		//対応したシーンによって渡すデータを決める
		switch (SetPhase) {

		case MAIN_GAME_PHASE.GAME_PHASE_DICE:
			//ボタンを押した際に出たランダムデータを渡す
            //RandomData;
            isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_DRAW_CARD:
            //使用したドローカードのＩＤを渡す
			//カードIDが０ならドローカードを使用していない
			//DrowCardID
			isNetworkAcross = true;
			break;
		
		case MAIN_GAME_PHASE.GAME_PHASE_BATTLE:
			//プレイヤーマネージャーからSelectlistのカードIDを渡す

			isNetworkAcross = true;
			//渡した後カードを削除
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER:
			//isGoalゴールしたかどうかを渡します
			//プレイヤーの現在地からどのマスに調整したかを渡します
            //PlayerTrout
			isNetworkAcross = true;
			break;

		default:
			Debug.LogError ("フェイズが読み込めませんでした");
			break;
		}
		//ネットのデータに送信が成功フラグがあればtrue失敗の場合はfalse
		if (isNetworkAcross) {
			return true; 
		} else {
			return false;
		}
	}

	//自分のプレイヤーがどっちなのかを取得する関数
	public PLAYER_ORDER getPlayer( ){
		return _currentPlayer;
	}

	//相手の手札数を取得する関数
	public int getEnemyHand(){
		//手札数
		return _enemyHandNumber;
	}

	//敵のステータスを取得する関数
	public int getEnemyStatus(){
		//敵の現在のステータス
		return _enemyStatus;
	}

	//敵の現在地を取得する関数
	public int getEnemyHere(){
		//敵の現在地
		return _EnemyHere;
	}

	//ドローが終了したかどうか
	public bool getDrowCompletion(){
		return DrowCompletion;
	}

}
