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
    /// シーン
    /// </summary>
    public enum SCENE {
        SCENE_CONNECT,
        SCENE_TITLE,
        SCENE_GAME,
        SCENE_FINISH,
    };

    /// <summary>
    /// メインゲームのフロー
    /// </summary>
    public enum MAIN_GAME_PHASE {
        GAME_PHASE_NO_PLAY,
        GAME_PHASE_DICE,
        GAME_PHASE_MOVE_CHARACTER,
        GAME_PHASE_DRAW_CARD,
        GAME_PHASE_BATTLE,
        GAME_PHASE_RESULT,
        GAME_PHASE_EVENT,
        GAME_PHASE_FINISH,
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
    /// 通信で送受信するフィールド側のデータ
    /// </summary>
    public struct NETWORK_FIELD_DATA {
        public SCENE scene;
        public MAIN_GAME_PHASE main_game_phase;
        public bool change_scene;
    };

    /// <summary>
    /// 通信で送受信するプレイヤー側のデータ
    /// </summary>
    public struct NETWORK_PLAYER_DATA {
		public bool changed_scene;
    };

    /// <summary>
    /// 座標データ
    /// </summary>
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

    /// <summary>
    /// ファイルデータ
    /// </summary>
	public struct FILE_DATA {
		public POSS_DATA[ ] mass; // マス配列
	}
		
    /// <summary>
    /// 現在のプレイヤーの行動順
    /// </summary>
    public enum PLAYER_ORDER {
        PLAYER_ONE,
        PLAYER_TWO,
        MAX_PLAYER_NUM,
        NO_PLAYER
    }

	/// <summary>
	/// プレイヤーの順位
	/// </summary>
	public enum PLAYER_RANK {
		NO_RANK,
		RANK_FIRST,
		RANK_SECOND,
	}

	/// <summary>
	/// プレイヤーのデータ
	/// </summary>
	public struct PLAYER_DATA {
		public GameObject obj;
		public PLAYER_RANK rank;
		public int advance_count;	//プレイヤーの進んでいる回数
		//////////////////////////
		public int attack;			//プレイヤーの攻撃力
		public bool battle_winner;
		//////////////////////////
	}
}
