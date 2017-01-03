using UnityEngine;
using System.Collections;
using Common;

public class PlayerNetWorkManager : Manager<PlayerNetWorkManager> {

	// Awake関数の代わり
	protected override void initialize( ) {
		cheackNetWork( );
	}

	//ここでネットとの受信が出来ているか確認、出来ていなければエラーを送る
	void cheackNetWork(){
	}

	//通信から次のフェイズに進む要求が出た場合に受信フラグをONにする
	public bool netDataReceipt( ) {
		//通信を飛ばして受信命令がないかを確認をする。受信フラグが来ていたらtrue
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
			isNetworkAcross = true;
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_MOVE_CHARACTER:
			//特に送信するものはありません
			isNetworkAcross = true;
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
}
