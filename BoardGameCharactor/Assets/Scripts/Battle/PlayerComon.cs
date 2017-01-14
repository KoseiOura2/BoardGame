using UnityEngine;
using System.Collections;
using System;

namespace PlayerComon {

	/// <summary>
	/// プレイヤーで使用するオブジェクトのリスト
	/// </summary>
	public enum PLAYER_OBJECT_LIST{
		BLACKOUT_PANEL,
		TEXT_WINDOW,
		YES_BUTTON,
		NO_BUTTON,
		DISCARD_AREA,
		DISCARD_BUTTON
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
