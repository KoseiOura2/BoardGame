using UnityEngine;
using System.Collections;
using Common;

public class PlayerNetWorkManager : Manager<PlayerNetWorkManager> {

    //各種マネージャー
	public CardManager _card_manager;

	public PlayerManager _player_manager;

    //自身がどっちのプレイヤーか
	private PLAYER _currentPlayer;

    //敵の手札数
	private int _enemyHandNumber;

    //敵のステータス
	private int _enemyStatus;

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

		//今回は仮でプレイヤー1にする
		_currentPlayer = PLAYER.PLAYER_1;
	}

	//通信から次のフェイズに進む要求が出た場合に受信フラグをONにする
	public bool networkPhaseChangeReceipt( ) {
        //通信に成功したかどうか
        bool isNetworkReceipt = false;

		//通信を飛ばしてフェイズ受信命令がないかを確認をする。受信フラグが来ていたらtrue
        /*          
            通信お願いします
        */
        if ( isNetworkReceipt ) {
            return true;
        } else {
            return false;
        }
	}

    public bool networkCardIdReceipt( CARD_DATA SetCardID ) {
        //通信に成功したかどうか
        bool isNetworkReceipt = false;
        //貰うデータはドローカードのＩＤ
        //IDの数だけ回す
        int DrowCardMax = 0;
        //今回は仮で直接カードマネージャーから生成してもらいます
        for ( int i = 0; i < DrowCardMax; i++ ) {
            //カードデータを生成して手札に加える
            cardDataReceipt( SetCardID );
        }
        //通信に成功したらtrue失敗したらfalse
        if ( isNetworkReceipt ) {
            return true;
        } else {
            return false;
        }
    }

    public bool networkEnemyDataReceipt( int EnemyHand, int EnemyStatus ) {
        //通信に成功したかどうか
        bool isNetworkReceipt = false;
        //敵の手札数とステータスを保存
        _enemyHandNumber = EnemyHand;
        _enemyStatus = EnemyStatus;
        //ステータスがちゃんとあるなら通信成功フラグをtrue
        if ( _enemyHandNumber != null && _enemyStatus != null ) {
            isNetworkReceipt = true;
        }
        //通信に成功したらtrue失敗したらfalse
        if ( isNetworkReceipt ) {
            return true;
        } else {
            return false;
        }
    }

    public bool networkDataAcross( MAIN_GAME_PHASE SetPhase, int RandomData = 0, bool UseDrow = false, int Playertrout = 0 ) {
		//ここでネットワークデータの送信を行います
        //送信が成功すればtrueに
		bool isNetworkAcross = false;
		//対応したシーンによって渡すデータを決める
		switch (SetPhase) {

		case MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE:
			//ボタンを押した際に出たランダムデータを渡す
            //RandomData;
            /*
                通信お願いします
            */
            isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_DROW:
			//ドローカードを使用したかどうか
            //UseDrowCard;
            //使用したカードのＩＤを渡す
            /*          
                通信お願いします
            */
			isNetworkAcross = true;
			break;
		
		case MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF:
			//プレイヤーマネージャーから選択リストのカードIDを渡す
			isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_GIMMICK:
			//どのマスに止まったかを渡します
            //PlayerTrout
			//プレイヤーデータ
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
	public PLAYER getPlayer( ){
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

	//通信からカードデータを貰い生成
    void cardDataReceipt( CARD_DATA SetCardId ){
        //DeckCardList()を使用して回す予定
        _player_manager.DeckCardList( SetCardId );
	}

}
