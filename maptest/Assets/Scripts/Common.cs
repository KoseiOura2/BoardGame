using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace Common {

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

// ファイルデータ
	public struct FILE_DATA {
		public POSS_DATA[ ] mass; // マス配列
	}

	public enum PHASE {
		DICE_PHASE,
		MOVE_PHASE,
		DRAW_PHASE,
		BATTLE_PHASE,
		RESULT_PHASE,
		EVENT_PHASE,
		PHASE_NUM
	}
}
