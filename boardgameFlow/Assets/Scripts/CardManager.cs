using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;

public class CardManager : Manager< CardManager > {
	/*
	public enum GET_CARD_DATA {
		CARD_NAME,
		ENCHANT_TYPE,
		ENCHANT_VALUE,
		SPESIAL_VALUE,
		RARITY,
		NO_DATA
	}
	public GET_CARD_DATA _get_data_type;
	*/

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

	private TextAsset _csv_file;


	//[ SerializeField ]
	//private int _enhance;		//合計の強化情報格納
	private string _name;
	private List< string[ ] > _csv_datas = new List< string[ ] >( );
	//private int _height = 0;

	// Awake関数の代わり
	protected override void initialize( ) {
		
	}

	public void init( ) {
		_deck_data.max_card_num = 30;
		_deck_data.card_num     = 0;
		_deck_data.cards_list   = new List< CARD_DATA >( );
		loadCardDataFile( );
	}

	// Use this for initialization
	void Start( ) {
		
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

	/// <summary>
	/// CSVを読み込み　文字列から数値などへ変換
	/// </summary>
	public void loadCardDataFile( ) {
		try {
			//よみこみ
			_name = "data";
			_csv_file = Resources.Load( "CSV/" + _name ) as TextAsset; /* Resouces/CSV下のCSV読み込み */
			StringReader reader = new StringReader( _csv_file.text );
			while ( reader.Peek( ) > -1 ) {
				string line = reader.ReadLine( );
				_csv_datas.Add( line.Split( ',' ) );
				//_height++;
			}
			{
				//変換
				try {
					for ( int i = 0; i < _csv_datas.Count; i++ ) {
						CARD_DATA data = new CARD_DATA( int.Parse( _csv_datas[ i ][ 0 ] ), _csv_datas[ i ][ 1 ], _csv_datas[ i ][ 2 ], int.Parse( _csv_datas[ i ][ 3 ] ),
														int.Parse( _csv_datas[ i ][ 4 ] ), int.Parse( _csv_datas[ i ][ 5 ] ) );
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

	/// <summary>
	/// 第一引数ID 返り値カードデータ　失敗した場合ダミーデータ
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
