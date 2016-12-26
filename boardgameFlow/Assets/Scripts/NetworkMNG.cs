﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;//必要です。
using System.Net;//これもいるかもしれない
using Common;

public class NetworkMNG : MonoBehaviour {

	//ファイヤーウォールを無効化してテスト
	//ファイヤーウォールの接続を許可すること.
	[ SerializeField ]
	private static IPAddress _ip_address;
	[ SerializeField ]
	private SERVER_STATE _server_state = SERVER_STATE.STATE_NONE;
	private GameObject _object_prefab;
	private GameObject _object_prefab_2;
	private GameObject _host_obj   = null;
	private List< GameObject > _client_obj = new List< GameObject >( );
	private string _ip   = "localhost";
	private string _port = "5037";
	private bool _connected = false;
    private int _player_num = 0;
	void Awake( ) {
		try {
			_object_prefab   = ( GameObject )Resources.Load( "Prefabs/Player1" );
			_object_prefab_2 = ( GameObject )Resources.Load( "Prefabs/Player2" );
		}
		catch {
			Debug.Log( "resourceのロードに失敗しました。" );
		}
	}

	// Use this for initialization
	void Start( ) {
		try {
			// IPアドレスの取得
			_ip_address = IPAddress.Parse( Network.player.ipAddress );
            GetComponent< NetworkManager >( ).networkAddress = _ip_address.ToString( );
		}
		catch {
			Debug.Log( "IPの取得に失敗しまいました" );
		}
	}

	// Update is called once per frame
	void Update( ) {
        if ( _host_obj == null ) {
            _host_obj = GameObject.FindWithTag( "HostObj" );
        }

        
	    // IPアドレスの取得
        GetComponent< NetworkManager >( ).networkAddress = _ip_address.ToString( );
	}
    /*
	//サーバ立ち上げ時に呼ばれるメソッド
	public void OnServerInitialized( ) {
		try {
			//ネットワーク内のすべてのPCでインスタンス化が行われるメソッド
			_host_obj = ( GameObject )Network.Instantiate( _object_prefab, _object_prefab.transform.position, _object_prefab.transform.rotation, 1 );
		}
		catch {
			Debug.Log( "サーバーの初期化に失敗しました" );
		}
	}
    */


    /// <summary>
    /// サーバー時新しいクライアント接続で呼ばれる
    /// </summary>
    void OnPlayerConnected( ) {
        _player_num++;
        foreach( GameObject obj in GameObject.FindGameObjectsWithTag( "ClientObj" ) ) {
            if ( !_client_obj.Contains( obj ) ) {
                _client_obj.Add( obj );
            }
        }
        if ( _player_num >= 1 ) {
            _connected = true;
        }
    }

	/// <summary>
	/// 未接続時の描画
	/// </summary>
	public void noConnectDraw( ) {
		GUI.Label( new Rect( 40, 250, 100, 30 ), "HOST IP" );
		Rect rect1 = new Rect( 100, 250, 250, 30 );
		_ip = GUI.TextField( rect1, _ip, 32 );

		if( GUI.Button( new Rect( 10, 10, 90, 90 ), "Client" ) ) {    
			//( hostのIPアドレス,hostが接続を受け入れているポート番号 )
			Network.Connect( _ip, int.Parse( _port ) ); 
			_server_state = SERVER_STATE.STATE_CLIANT;
		}
		if( GUI.Button( new Rect( 10, 110, 90, 90 ), "Server" ) ) {    
			//(接続可能人数,接続を受け入れるポート番号,NATのパンチスルー機能の設定 )
			Network.InitializeServer( 10, int.Parse( _port ), false ) ;
			_ip = _ip_address.ToString( );
			_server_state = SERVER_STATE.STATE_HOST;
		}
	}

	/// <summary>
	/// ホスト側の描画
	/// </summary>
	public void hostStateDraw( ) {
		// 文字の設定
		string text = _ip_address.ToString( );
		int width   = 500;
		int height  = 30;
		GUIStyle style = new GUIStyle( );
		style.fontSize = 50;
		GUIStyleState style_state = new GUIStyleState( );
		style_state.textColor = Color.black;
		style.normal = style_state;

		// IPアドレスの表示
		GUI.Label( new Rect( Screen.width / 2 - text.Length / 2, Screen.height / 2, width, height ), text, style );
	}

	/// <summary>
	/// 接続されたかどうか返す
	/// </summary>
	/// <returns><c>true</c>, if connected was ised, <c>false</c> otherwise.</returns>
	public bool isConnected( ) {
		return _connected;
	}

	public SERVER_STATE getServerState( ) {
		return _server_state;
	}

	public GameObject getHostObj( ) {
		return _host_obj;
	}
    
	public GameObject getClientObj( int num ) {
        if ( num >= _client_obj.Count ) {
            return null;
        }

		return _client_obj[ num ];
	}

}

