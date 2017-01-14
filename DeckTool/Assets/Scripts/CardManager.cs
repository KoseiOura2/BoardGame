using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CardManager : MonoBehaviour {

    enum MODE {
        CREATE_MODE,
        DECK_MODE
    };

	/// <summary>
	/// デッキクラス
	/// </summary>
	public class Deck {
		private int _max_card_num;
		private int _card_num;
        [ SerializeField ]
		private List< string > _cards_list;

        public Deck( ) {
            _max_card_num = 0;
            _card_num     = 0;
            _cards_list   = new List< string >( );
        }
        
        public void addToCard( string card ) {
            _cards_list.Add( card );
            _card_num++;
            _max_card_num = _card_num;
        }

        public string drawCard( int id ) {
            string card = _cards_list[ id ];
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

	private List< string > _card_datas = new List< string >( );
    [ SerializeField ]
	private Deck _deck = new Deck( );
	private TextAsset _csv_file;
	private string _name;
	private List< string[ ] > _csv_datas = new List< string[ ] >( );

    private List< int > _card_num_for_name = new List< int >( );
    private MODE _mode = MODE.CREATE_MODE;

    private string _draw_card = "";

	void Awake( ) {
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

    void OnGUI( ) {
        if ( _mode == MODE.CREATE_MODE ) {
            drawCreateMode( );
        } else if ( _mode == MODE.DECK_MODE ) {
            drawDeckMode( );
        }
    }

    void drawCreateMode( ) {
        GUI.Label( new Rect( 10, 0, 400, 100 ), "カードの種類" );
        GUI.Label( new Rect( 100, 0, 400, 100 ), "デッキ内訳" );

        int deck_card_num = 0;

        // 各カードの内訳数などを変えるテキストフィールドを表示
        for ( int i = 0; i < _card_datas.Count; i++ ) {
            GUI.Label( new Rect( 10, 10 + 20 * ( i + 1 ), 400, 100 ), _card_datas[ i ] );

            string num;
            num = GUI.TextField( new Rect( 100, 10 + 20 * ( i + 1 ), 30, 20 ), _card_num_for_name[ i ].ToString( ) );
            if ( num != "" ) {
                _card_num_for_name[ i ] = int.Parse( num );
            }

            deck_card_num += _card_num_for_name[ i ];
        }

        GUI.Label( new Rect( 180, 0, 400, 100 ), "デッキ総数：" + deck_card_num.ToString( ) );

        if ( GUI.Button( new Rect( 150, 30, 150, 100 ), "デッキを生成！" ) ) {
            _deck.init( );
            createDeck( );
            saveDeckFile( );
        }

        if ( GUI.Button( new Rect( Screen.width - 160, Screen.height - 110, 150, 100 ), "ドロー画面へ" ) ) {
            _mode = MODE.DECK_MODE;
        }
    }

    void drawDeckMode( ) {
        GUI.Label( new Rect( 10, 0, 400, 100 ), "デッキ残数：" + _deck.getCardNum( ).ToString( ) );
        
        if ( GUI.Button( new Rect( 10, 20, 150, 100 ), "カードドロー" ) ) {
            _draw_card = distributeCard( );
            if ( _draw_card == "" ) {
                _draw_card = "デッキにカードがありません！";
            }
        }

        GUI.Label( new Rect( 10, 120, 400, 100 ), "入手カードは：" + _draw_card );

        if ( GUI.Button( new Rect( 10, 150, 150, 100 ), "デッキのロード" ) ) {
            _deck.init( );
            loadDeckFile( );
            createDeck( );
        }

        if ( GUI.Button( new Rect( Screen.width - 160, Screen.height - 110, 150, 100 ), "デッキ生成画面へ" ) ) {
            _mode = MODE.CREATE_MODE;
        }
    }

    void saveDeckFile( ) {
        StreamWriter sw = new StreamWriter( Application.dataPath + "/Resources/CSV/DeckData.csv", false );

        /*
        // 種類数を登録
        sw.Write( _csv_datas.Count );
        sw.WriteLine( );
        */

		for ( int i = 0; i < _card_datas.Count; i++ ) {
            // カード名
            sw.Write( _card_datas[ i ] );
			sw.Write( "," );
			// 内訳数
			sw.Write( _card_num_for_name[ i ] );
			sw.Write( "," );
			sw.WriteLine( );
		}
		sw.Close( );
    }

    void loadDeckFile( ) {
        StreamReader sr = new StreamReader( Application.dataPath + "/Resources/CSV/DeckData.csv", false );
        
        for ( int i = 0; i < _card_datas.Count; i++ ) {
		    string str = sr.ReadLine( );
		    string[ ] values = str.Split( ',' );

            for ( int j = 0; j < _card_datas.Count; j++ ) {
                if ( _card_datas[ j ] == values[ 0 ] ) {
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

            for ( int i = 0; i < length; i++ ) {
		        string str_1 = sr.ReadLine( );
		        string[ ] values_1 = str_1.Split( ',' );

				_card_datas.Add( values_1[ 0 ] );
                _card_num_for_name.Add( 0 );
            }
            sr.Close( );

            /*
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
						string data = _csv_datas[ i ][ 0 ];
						_card_datas.Add( data );
                        _card_num_for_name.Add( 0 );
					}
				} catch {
					Debug.Log( "変換エラー" );
				}
			}
            */
		} catch {
			Debug.Log( "カードデータロードエラー" );
		}
	}

	/// <summary>
	/// デッキ生成
	/// </summary>
	private void createDeck( ) {
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
	public string distributeCard( ) {
		string card = "";
        if ( _deck.getCardNum( ) > 0 ) {
		    int num = ( int )Random.Range( 0, ( float )_deck.getCardNum( ) );
		    card = _deck.drawCard( num );
        }

		return card;
	}
}
