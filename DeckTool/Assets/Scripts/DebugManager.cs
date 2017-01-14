﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class DebugManager : MonoBehaviour {

	private InputField _input_field;
	[ SerializeField ]
    CardManager _card_manager;
 
    /// <summary>
    /// Startメソッド
    /// InputFieldコンポーネントの取得および初期化メソッドの実行
    /// </summary>
    void Start( ) {
 
        _input_field = GetComponent< InputField >( );
 
        InitInputField( );
    }

    public void AddToCard( ) {
        if( _input_field.text != "" ) {

            InitInputField( );
        }
    }

    public void SetLimitValue( ) {
        if( _input_field.text != "" ) {

            InitInputField( );
        }
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
