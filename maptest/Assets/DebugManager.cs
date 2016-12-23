using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class DebugManager : MonoBehaviour {

	InputField inputField;
    public PlayerManager _player_manager;
 
 
    /// <summary>
    /// Startメソッド
    /// InputFieldコンポーネントの取得および初期化メソッドの実行
    /// </summary>
 
    void Start() {
 
        inputField = GetComponent<InputField>();
 
        InitInputField();
    }
 
 
 
    /// <summary>
    /// Log出力用メソッド
    /// 入力値を取得してLogに出力し、初期化
    /// </summary>
    public void SetPlayerID() {
        if( inputField.text != "" ){
            _player_manager._set_player_id = int.Parse( inputField.text );
            InitInputField();
        }
    }

    public void SetLimitValue() {
        if( inputField.text != "" ){
            _player_manager._limit_value = int.Parse( inputField.text );
            _player_manager._advance_flag = true;
            _player_manager._event_count = 0;
            InitInputField();
        }
    }
 
 
 
    /// <summary>
    /// InputFieldの初期化用メソッド
    /// 入力値をリセットして、フィールドにフォーカスする
    /// </summary>
 
    void InitInputField() {
 
        // 値をリセット
        inputField.text = "";
 
        // フォーカス
        inputField.ActivateInputField();
    }

    
}
