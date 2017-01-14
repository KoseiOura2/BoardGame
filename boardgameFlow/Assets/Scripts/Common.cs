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

	public enum GAME_STAGE {
		NORMAL,
		BONUS
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
	/// プレイヤーの勝敗結果
	/// </summary>
	public enum BATTLE_RESULT {
		BATTLE_RESULT_NONE,
		WIN,
		LOSE,
		DRAW,
	};

	/// <summary>
	/// マス調整
	/// </summary>
	public enum MASS_ADJUST {
		NO_ADJUST,
		ADVANCE,
		BACK,
	};

	/// <summary>
	/// スペシャルカード　効果
	/// </summary>
	public enum SPECIAL_LIST {
		ENHANCE_TYPE_DRAW,
		NO_DATA
	}

    public enum EVENT_TYPE {
        EVENT_NONE,
        EVENT_DRAW,
        EVENT_MOVE,
        EVENT_ACTION,
        EVENT_GOAL
    }

	/// <summary>
	/// 通信で送受信するフィールド側のデータ
	/// </summary>
	public struct NETWORK_FIELD_DATA {
        public int player_num;
		public SCENE scene;
		public MAIN_GAME_PHASE main_game_phase;
		public bool change_scene;
		public bool change_phase;
		public int[ ] card_list_one;
		public int[ ] card_list_two;
		public BATTLE_RESULT result_player_one;
		public BATTLE_RESULT result_player_two;
		public bool send_result;
	};

	/// <summary>
	/// 通信で送受信するプレイヤー側のデータ
	/// </summary>
	public struct NETWORK_PLAYER_DATA {
		public bool changed_scene;
		public bool changed_phase;
		public int dice_value;
		public bool ready;
		public int player_status;
		public int[ ] used_card_list;
		public bool battle_ready;
		public MASS_ADJUST mass_adjust;
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
		public int cardId;
	}

	/// <summary>
	/// ファイルデータ
	/// </summary>
	public struct FILE_DATA {
		public POSS_DATA[ ] mass; // マス配列
	}

	/// <summary>
	/// プレイヤーのデータ
	/// </summary>
	public struct PLAYER_DATA {
		public GameObject obj;
		public PLAYER_RANK rank;
		public int advance_count;	//プレイヤーの進んでいる回数
		public int attack;			//プレイヤーの攻撃力
		public BATTLE_RESULT battle_result;
		public int draw;
		public int power;
        public EVENT_TYPE event_type;
		public bool onMove;
		public GAME_STAGE stage;
	}

	/// <summary>
	/// カードデータ
	/// </summary>
	public struct CARD_DATA {
		public int id;
		public string name;
		public string enchant_type;
		public int enchant_value;
		public int special_value;
		public int rarity;
		public CARD_DATA( int id, string name, string enchant_type, int enchant_value, int special_value, int rarity ) {
			this.id = id;
			this.name = name;
			this.enchant_type = enchant_type;
			this.enchant_value = enchant_value;
			this.special_value = special_value;
			this.rarity = rarity;
		}
	}
}
