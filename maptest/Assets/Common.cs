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
	}

// ファイルデータ
	public struct FILE_DATA {
		#region リズム 構造体
		public struct MAP {
			public POSS_DATA[ ] ma;	// マップ座標配列
		}
		#endregion

		public MAP map;
	}
}
