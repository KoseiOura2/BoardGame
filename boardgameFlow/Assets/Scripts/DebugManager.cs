using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class DebugManager : MonoBehaviour {

	private InputField _input_field;
 
    /// <summary>
    /// Startメソッド
    /// InputFieldコンポーネントの取得および初期化メソッドの実行
    /// </summary>
    void Start( ) {
 
        _input_field = GetComponent< InputField >( );
 
        InitInputField( );
    }
 
    /// <summary>
    /// Log出力用メソッド
    /// 入力値を取得してLogに出力し、初期化
    /// </summary>
    public void SetPlayerID( ) {
        if( _input_field.text != "" ) {
            /*
            _player_manager._set_player_id = int.Parse( _input_field.text );
            InitInputField( );
            */
        }
    }

    public void SetLimitValue( ) {
        /*
        if( _input_field.text != "" ) {
            _player_manager._limit_value = int.Parse( _input_field.text );
            _player_manager._advance_flag = true;
            _player_manager._event_count = 0;
            InitInputField( );
        }
        */ 
    }
 
    /// <summary>
    /// InputFieldの初期化用メソッド
    /// 入力値をリセットして、フィールドにフォーカスする
    /// </summary>
    void InitInputField( ) {
 
        // 値をリセット
        _input_field.text = "";
 
        // フォーカス
        _input_field.ActivateInputField( );
    }

    
}
