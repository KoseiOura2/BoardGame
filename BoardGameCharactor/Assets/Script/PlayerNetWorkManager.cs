using UnityEngine;
using System.Collections;
using Common;

public class PlayerNetWorkManager : Manager<PlayerNetWorkManager> {

	public CardManager _card_manager;

	public PlayerManager _player_manager;

	private PLAYER _currentPlayer;

	private struct EnemyStetas{
	}

	private int _enemyHandNumber;

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
	public bool netDataReceipt( ) {
		//通信を飛ばして受信命令がないかを確認をする。受信フラグが来ていたらtrue
		//受信データをもらう際に敵の手札数とステータスを取得
		_enemyHandNumber = 6;
		_enemyStatus = 10;
		//return true;
		return false;
	}

	public bool netDataAcross( MAIN_GAME_PHASE SetPhase ){

		bool isNetworkAcross = false;
		//対応したシーンによって渡すデータを決める
		switch (SetPhase) {

		case MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY:
			//特に送信するものはありません
			isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_THROW_DICE:
			//ボタンを押した際に出たランダムデータを渡す
			//エネミーに
			isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER:
			//特に送信するものはありません
			isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_DROW:
			//ドローカードを使用したかどうか
			isNetworkAcross = true;
			break;
		
		case MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF:
			//ドローカードを使用したかどうか
			isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_GIMMICK:
			//ドローカードを使用したかどうか
			isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_INDUCTION:
			//フィールドに誘導をさせる
			break;
		default:
			Debug.LogError ("フェイズが読み込めませんでした");
			break;
		}
		//ネットのデータに送信が成功すればtrue失敗の場合はfalse
		if (isNetworkAcross) {
			return true; 
		} else {
			return false;
		}
	}

	//プレイヤーを取得する関数
	public PLAYER getPlayer( ){
		return _currentPlayer;
	}

	//手札数を取得する関数
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
	public CARD_DATA cardDataReceipt(){
		//今回は仮で直接カードマネージャーから生成してもらいます
		CARD_DATA _DROW_CARD = new CARD_DATA( );
		_DROW_CARD =  _card_manager.distributeCard ();
		return _DROW_CARD;
	}

}
