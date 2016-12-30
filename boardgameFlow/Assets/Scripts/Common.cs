using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace Common {
    /// <summary>
    /// メインゲームのフロー
    /// </summary>
    enum MAIN_GAME_PHASE {
        GAME_PHASE_NO_PLAY,
        GAME_PHASE_THROW_DICE,
        GAME_PHASE_ASSIGNMENT_BUFF,
        GAME_PHASE_RESULT_BATTLE,
        GAME_PHASE_MOVE_CHARACTER,
        GAME_PHASE_FIELD_GIMMICK,
        GAME_PHASE_FINISH,
    };
}
