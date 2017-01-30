using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;

public class CardManager : Manager< CardManager > {
    /// <summary>
	/// デッキクラス
	/// </summary>
	public class Deck {
		private int _max_card_num;
		private int _card_num;
        [ SerializeField ]
		private List< CARD_DATA > _cards_list;

        public Deck( ) {
            _max_card_num = 0;
            _card_num     = 0;
            _cards_list   = new List< CARD_DATA >( );
        }
        
        public void addToCard( CARD_DATA card ) {
            _cards_list.Add( card );
            _card_num++;
            _max_card_num = _card_num;
        }

        public CARD_DATA drawCard( int id ) {
            CARD_DATA card = _cards_list[ id ];
            _cards_list.RemoveAt( id );
            _card_num--;

            return card;
        }

        public void init( ) {
            _max_card_num = 0;
            _card_num     = 0;
            _cards_list.Clear( );
        }

        public int getMaxCardNum( ) {
            return _max_card_num;
        }

        public int getCardNum( ) {
            return _card_num;
        }
	};

	private List< CARD_DATA > _card_datas = new List< CARD_DATA >( );
	private Deck _deck = new Deck( );
	//private int _height = 0;
    private List< int > _card_num_for_name = new List< int >( );

	// Awake関数の代わり
	protected override void initialize( ) {
		
	}

	public void init( ) {
		loadCardDataFile( );
        loadDeckFile( );
        createDeck( );
	}

	// Use this for initialization
	void Start( ) {
		
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}
    
    /// <summary>
    /// デッキの読み込み
    /// </summary>
    void loadDeckFile( ) {
        StreamReader sr = new StreamReader( Application.dataPath + "/Resources/CSV/DeckData.csv", false );
        
        for ( int i = 0; i < _card_datas.Count; i++ ) {
		    string str = sr.ReadLine( );
		    string[ ] values = str.Split( ',' );

            for ( int j = 0; j < _card_datas.Count; j++ ) {
                if ( _card_datas[ j ].name == values[ 0 ] ) {
                    _card_num_for_name[ j ] = int.Parse( values[ 1 ] );
                    continue;
                }
            }
        }
        sr.Close( );
    }

	/// <summary>
	/// CSVを読み込み　文字列から数値などへ変換
	/// </summary>
	public void loadCardDataFile( ) {
		try {
            StreamReader sr = new StreamReader( Application.dataPath + "/Resources/CSV/data.csv", false );
        
		    string str_0 = sr.ReadLine( );
		    string[ ] values_0 = str_0.Split( ',' );

            int length = int.Parse( values_0[ 0 ] );
            
			{
				//変換
				try {
                    for ( int i = 0; i < length; i++ ) {
		                string str_1 = sr.ReadLine( );
		                string[ ] values_1 = str_1.Split( ',' );

                        CARD_DATA data = new CARD_DATA( int.Parse( values_1[ 0 ] ), values_1[ 1 ], ( CARD_TYPE )int.Parse( values_1[ 2 ] ),
                                                        int.Parse( values_1[ 3 ] ), int.Parse( values_1[ 4 ] ), int.Parse( values_1[ 5 ] ) );
				        _card_datas.Add( data );
                        _card_num_for_name.Add( 0 );
                    }
				} catch {
					Debug.Log( "変換エラー" );
				}
		    }
            sr.Close( );
		} catch {
			Debug.Log( "カードデータロードエラー" );
		}
	}

	/// <summary>
	/// デッキ生成
	/// </summary>
	public void createDeck( ) {
		for ( int i = 0; i < _card_num_for_name.Count; i++ ) {
            for ( int j = 0; j < _card_num_for_name[ i ]; j++ ) {
                _deck.addToCard( _card_datas[ i ] );
            }
        }
	}

	/// <summary>
	/// カード配布
	/// </summary>
	/// <returns>The card.</returns>
	public CARD_DATA distributeCard( ) {
		CARD_DATA card = new CARD_DATA( );

        if ( _deck.getCardNum( ) > 0 ) {
		    int num = ( int )Random.Range( 0, ( float )_deck.getCardNum( ) );
		    card = _deck.drawCard( num );
        }

		return card;
	}

	public int getDeckCardNum( ) {
		return _deck.getCardNum( );
	}

	/// <summary>
	/// 第一引数ID 返り値カードデータ　失敗した場合ダミーデータ
	/// </summary>
	public CARD_DATA getCardData( int id ) {
		try {
			if ( id == 5 ) {
				Debug.Log( "特定種類のカードの取得に成功しました" );
			}
			return _card_datas[ id ];
		} catch {
			Debug.Log("カードデータ取得エラー");
			return _card_datas[ 0 ];
		}
	}
}
