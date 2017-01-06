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
		// デバッグ
		int card_num = 5;
		for ( int i = 1; i <= card_num; i++ ) {
			CARD_DATA card_data = new CARD_DATA( );
			card_data.id = i;
			_card_datas.Add( card_data );
		}

		CARD_DATA card = new CARD_DATA( );
		card.name = "once_enhance";
		card.type = CARD_TYPE.CARD_TYPE_ONCE_ENHANCE;
		_card_datas[ 0 ] = card;
		card.name = "once_weaken";
		card.type = CARD_TYPE.CARD_TYPE_ONCE_WEAKEN;
		_card_datas[ 1 ] = card;
		card.name = "continu_enhance";
		card.type = CARD_TYPE.CARD_TYPE_CONTUNU_ENHANCE;
		_card_datas[ 2 ] = card;
		card.name = "insurance";
		card.type = CARD_TYPE.CARD_TYPE_INSURANCE;
		_card_datas[ 3 ] = card;
		card.name = "unavailable";
		card.type = CARD_TYPE.CARD_TYPE_UNAVAILABLE;
		_card_datas[ 4 ] = card;
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
		Debug.Log(_deck_data.card_num);
		card = _deck_data.cards_list[ num ];
		_deck_data.cards_list.RemoveAt( num );
		_deck_data.card_num--;

		return card;
	}

	public int getDeckCardNum( ) {
		return _deck_data.card_num;
	}
}
