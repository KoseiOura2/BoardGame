using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace Common {
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
	};
}
