using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class CardManager : MonoBehaviour {

	/// <summary>
	/// デッキデータ
	/// </summary>
	private struct DECK_DATA {
		public int max_card_num;
		public int card_num;
		public List< CARD_DATA > cards_list;
	};

	private List< CARD_DATA > _card_datas = new List< CARD_DATA >( );
	private DECK_DATA _deck_data = new DECK_DATA( );

	void Awake( ) {
		_deck_data.max_card_num = 30;
		_deck_data.card_num     = 0;
		_deck_data.cards_list   = new List< CARD_DATA >( );
	}

	// Use this for initialization
	void Start( ) {
		loadCardDataFile( );
		createDeck ();
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

	public void loadCardDataFile( ) {
		
	}

	/// <summary>
	/// デッキ生成
	/// </summary>
	public void createDeck( ) {
		for ( int i = 0; i < _deck_data.max_card_num; i++ ) {
			try {
				// ランダムで選び出す
				int card_id = ( int )Random.Range( 0, ( float )_card_datas.Count );
				CARD_DATA card = _card_datas[ card_id ];
				_deck_data.cards_list.Add( card );
			}
			catch {
				Debug.Log( "デッキの生成に失敗しました" );
			}
		}
		_deck_data.card_num = _deck_data.max_card_num;
	}

	/// <summary>
	/// カード配布
	/// </summary>
	/// <returns>The card.</returns>
	public CARD_DATA distributeCard( ) {
		CARD_DATA card = new CARD_DATA( );
		int num = ( int )Random.Range( 0, ( float )_deck_data.card_num );
		card = _deck_data.cards_list[ num ];
		_deck_data.cards_list.RemoveAt( num );
		_deck_data.card_num--;

		return card;
	}

	public int getDeckCardNum( ) {
		return _deck_data.card_num;
	}
}
/*
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
	public GET_CARD_DATA _get_data_type;
	[SerializeField]
	private int _enhance;		//合計の強化情報格納
	private PlayerManager _player_manager;
	private List<CARD_DATA> _cardData = new List<CARD_DATA>( );
    private string _name;
    private TextAsset _csvFile;
    private List<string[ ]> _csvDatas = new List<string[ ]>( );
    private int _height = 0;
	void Awake( ) {
		if ( _player_manager == null ) {
			_player_manager = GameObject.Find( "PlayerManager" ).GetComponent<PlayerManager>( );
		}
		cardDataLoad( );
	}
	/// <summary>
	/// 第一引数ID　返り値カードデータ　失敗した場合ダミーデータ
	/// </summary>
	public CARD_DATA getCardData( int id ) {
		try {
			return _cardData[ id ];
		} catch {
			Debug.Log("カードデータ取得エラー");
			return _cardData[ 0 ];
		}
	}
	/// <summary>
	/// CSVを読み込み　文字列から数値などへ変換
	/// </summary>
    private void cardDataLoad( ) {
        try {
			//よみこみ
            _name = "data";
            _csvFile = Resources.Load( "CSV/" + _name ) as TextAsset; // Resouces/CSV下のCSV読み込み 
StringReader reader = new StringReader( _csvFile.text );
while ( reader.Peek( ) > -1 ) {
	string line = reader.ReadLine( );
	_csvDatas.Add( line.Split( ',' ) );
	_height++;
}
{
	//変換
	try {
		for ( int i = 0; i < _csvDatas.Count; i++ ) {
			CARD_DATA data = new CARD_DATA( _csvDatas[ i ][ 0 ], _csvDatas[ i ][ 1 ], int.Parse( _csvDatas[ i ][ 2 ] ),
				int.Parse( _csvDatas[ i ][ 3 ]), int.Parse( _csvDatas[ i ][ 4 ] ) );
			_cardData.Add( data );
		}
	} catch {
		Debug.Log( "変換エラー" );
	}
}
} catch {
	Debug.Log( "カードデータロードエラー" );
}
}
}

*/