using UnityEngine;
using System.Collections;
using System;
using System.Collections.ObjectModel;

namespace PlayerData {

	/*
	//ここで待機メッセージなどのリストを作る
	public class NAME{
		public static readonly ReadOnlyCollection<string> TEXT_LIST = "ダイスを振ります\n(今回はダイスのアニメーションはありません)";
	}
	*/

	//ResourceロードのURLなどもここでリストを作る

	/// <summary>
	/// プレイヤーで使用するオブジェクトのリスト
	/// </summary>
	public enum PLAYER_OBJECT_LIST{
		NONE_OBJECT,
		BLACKOUT_PANEL,
		TEXT_WINDOW,
		YES_BUTTON,
		NO_BUTTON,
		DISCARD_AREA,
		DISCARD_BUTTON,
		DICE_BUTTON,
        MAINGAME_SELECT_AREA,
        BATTLE_SELECT_AREA
	}

	/// <summary>
	/// オブジェクト管理用の構造体
	/// </summary>
	public struct OBJECT_DATA {
		public GameObject obj;          //オブジェクト
		public GameObject resource;     //画像データなどのリソース
		public int id;                  //オブジェクトID
		public PLAYER_OBJECT_LIST type;        //オブジェクトの種類
	};

}
