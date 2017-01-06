using UnityEngine;
using System.Collections;
using System;
using Common;

public class BattlePhaseManager : MonoBehaviour {

	//フェイズ毎の初期設定フラグ
	bool initial_setting = false;

	//現在のフェイズ
	private MAIN_GAME_PHASE _current_Phase;

	//画面を暗くするためのプレハブ
	public GameObject _black_Out;

	//シーン読み込み中かどうか
	private bool nowSceneLoad;

	//フェードマネージャーを取得
	private FadeManager _fade_Manager;

	//プレイヤーのネットワークマネージャーを取得
	private PlayerNetWorkManager _player_NetWork_Manager;

	//プレイヤーマネージャーを取得
	private PlayerManager _player_Manager;

	// Use this for initialization
	void Awake () {

		//各種マネージャーのロード
		if (_fade_Manager == null) {
			GameObject _fade_Manager_Obj = GameObject.Find ("FadeManager");
			_fade_Manager = _fade_Manager_Obj.GetComponent<FadeManager> ();
		}
		if ( _player_NetWork_Manager == null){
			GameObject _player_NetWork_Manager_Obj = GameObject.Find ("PlayerNetWorkManager");
			_player_NetWork_Manager = _player_NetWork_Manager_Obj.GetComponent<PlayerNetWorkManager> ();
		}
		if ( _player_Manager == null){
			GameObject _Player_Manager_Obj = GameObject.Find ("PlayerManager");
			_player_Manager = _Player_Manager_Obj.GetComponent<PlayerManager> ();
		}


		//最初のフェイズをロード
		_current_Phase = MAIN_GAME_PHASE.GAME_PHASE_DROW;

		nowSceneLoad = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!nowSceneLoad) {
			//フェイズ読み込み
			phaseLoad ();
		}
	}

	void phaseLoad (){
		//現在のシーンによって読み込みを変える
		switch (_current_Phase) {

		case MAIN_GAME_PHASE.GAME_PHASE_DROW:
			drowPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF:
			cardPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_RESULT_BATTLE:
			resultPhase ();
			break;

		case MAIN_GAME_PHASE.GAME_PHASE_FIELD_GIMMICK:
			adctionPhase ();
			break;

		default:
			Debug.LogError ("errorCase:" + _current_Phase);
			break;
		}
	}

	void drowPhase (){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("ドローフェイズです");
			//ここでプレイヤーの判別を行い、プレイヤーラベルの色とテキスト表示をエネミーの表示とテキストの表示を変更します
			//初期手札を生成
			_player_Manager.setHandCardCreate();
			//エネミーのテキストを設定
			_player_Manager.SetEnemyObject();
			//プレイヤーによって変わる部分を変更
			_player_Manager.setPlayerObject();
			//初期設定完了フラグ
			initial_setting = true;
		} else {
		}
	}

	void cardPhase (){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("カードフェイズです");
			//初期設定完了フラグ
			initial_setting = true;
		} else {
		}
	}

	void resultPhase (){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("リザルトフェイズです");
			//初期設定完了フラグ
			initial_setting = true;
		} else {
		}
	}
	void adctionPhase(){
		//初期設定が済んでなければ行う
		if (!initial_setting) {
			Debug.Log ("アクションフェイズです");
			//初期設定完了フラグ
			initial_setting = true;
		} else {
		}
	}

	public void phaseChange(){
		//渡す数値を保存しておいてその数値を通信に渡せるようにする

		//カードを消し、画面をくらくしてテキストを表示

		//初期設定フラグをoffに
		initial_setting = false;

		//次のフェイズに移行
		_current_Phase++;
	}

	public MAIN_GAME_PHASE GetMainGamePhase(){
		//現在のフェイズを返します
		return _current_Phase;
	}
}
