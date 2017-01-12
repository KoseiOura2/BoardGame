using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;

public class CardManager : MonoBehaviour {

	public enum GET_CARD_DATA {
		CARD_NAME,
		ENCHANT_TYPE,
		ENCHANT_VALUE,
		SPESIAL_VALUE,
		RARITY,
		NO_DATA
	}

	private List< CARD_DATA > _card_datas = new List< CARD_DATA >( );
    
	public GET_CARD_DATA _get_data_type;
	[ SerializeField ]
	private int _enhance;		//合計の強化情報格納
	private PlayerManager _player_manager;
    private string _name;
    private TextAsset _csv_file;
    private List< string[ ] > _csv_datas = new List< string[ ] >( );
    private int _height = 0;

	void Awake( ) {
		if ( _player_manager == null ) {
			_player_manager = GameObject.Find( "PlayerManager" ).GetComponent<PlayerManager>( );
		}
		loadCardDataFile( );
	}

	// Use this for initialization
	void Start( ) {

	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

	public void loadCardDataFile( ) {
        try {
		    //よみこみ
            _name = "data";
            _csv_file = Resources.Load( "CSV/" + _name ) as TextAsset; // Resouces/CSV下のCSV読み込み 
            StringReader reader = new StringReader( _csv_file.text );
            while ( reader.Peek( ) > -1 ) {
	            string line = reader.ReadLine( );
	            _csv_datas.Add( line.Split( ',' ) );
	            _height++;
            }

            {
	            //変換
	            try {
		            for ( int i = 0; i < _csv_datas.Count; i++ ) {
			            CARD_DATA data = new CARD_DATA( int.Parse( _csv_datas[ i ][ 0 ] ), _csv_datas[ i ][ 1 ], _csv_datas[ i ][ 2 ],
                            int.Parse( _csv_datas[ i ][ 3 ] ), int.Parse( _csv_datas[ i ][ 4 ]), int.Parse( _csv_datas[ i ][ 5 ] ) );
			            _card_datas.Add( data );
		            }
	            } catch {
		            Debug.Log( "変換エラー" );
	            }
            }
        } catch {
	        Debug.Log( "カードデータロードエラー" );
        }
    }
    
	/// <summary>
	/// 第一引数ID　返り値カードデータ　失敗した場合ダミーデータ
	/// </summary>
	public CARD_DATA getCardData( int id ) {
		try {
			return _card_datas[ id ];
		} catch {
			Debug.Log("カードデータ取得エラー");
			return _card_datas[ 0 ];
		}
	}

}