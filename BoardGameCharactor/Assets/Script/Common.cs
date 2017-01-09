using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace Common {
	/// <summary>
	/// ホストかクライアントか
	/// </summary>
	public enum SERVER_STATE {
		STATE_NONE,
		STATE_HOST,
		STATE_CLIANT,
	};

    /// <summary>
    /// メインゲームのフロー
    /// </summary>
    public enum MAIN_GAME_PHASE {
        GAME_PHASE_NO_PLAY,
        GAME_PHASE_THROW_DICE,
        GAME_PHASE_ASSIGNMENT_BUFF,
        GAME_PHASE_RESULT_BATTLE,
        GAME_PHASE_MOVE_CHARACTER,
        GAME_PHASE_FIELD_GIMMICK,
        GAME_PHASE_FINISH,
		GAME_PHASE_DROW,
		GAME_PHASE_FIELD_INDUCTION,
		GAME_PHASE_DIS_CARD,
    };

	public enum CARD_TYPE {
		CARD_TYPE_NONE_TYPE,
		CARD_TYPE_ONCE_ENHANCE,
		CARD_TYPE_ONCE_WEAKEN,
		CARD_TYPE_CONTUNU_ENHANCE,
		CARD_TYPE_INSURANCE,
		CARD_TYPE_UNAVAILABLE,
	};

	/// <summary>
	/// カードの構造体データ
	/// </summary>
	public struct CARD_DATA {
		public int id;
		public string name;
		public CARD_TYPE type;
	}

	/// <summary>
	/// シーン
	/// </summary>
	public enum SCENE {
		SCENE_CONNECT,
		SCENE_TITLE,
		SCENE_GAME,
		SCENE_FINISH,
	};

	/// <summary>
	/// フィールドデータファイル
	/// </summary>
	public struct FILE_DATA {
		public POSS_DATA[ ] mass; // マス配列
	}

	// 座標データ
	public struct POSS_DATA {
		public int index;	// インデックス
		public uint x;	// X座標
		public uint y;	// Y座標
		public uint z;	// Z座標
		public string type; //マスタイプ
		public int nomalValue; //値１
		public int trapValue; //値２
		public string environment; //環境情報
	}

	//プレイヤー1かプレイヤー2か
	public enum PLAYER {
		PLAYER_1,
		PLAYER_2,
	};

	public enum RESULT{
		WINNER,
		DROW,
		LOSE,
	};
}
