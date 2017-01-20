using UnityEngine;
using System.Collections;
using Common;

public class Card : MonoBehaviour {
    [ SerializeField ]
	private GameObject _front_object;
	private Material _front_material;
	private CARD_DATA _card_data;
	private int _hand_num = -1;
	[ SerializeField ]
	private bool _selected;
	[ SerializeField ]
	private int _id = -1;

	void Awake( ) {
		if ( _front_material == null ) {
			_front_object = gameObject.transform.FindChild( "Front" ).gameObject;
		}
	}

	// <summary>
	/// CSVからカードを判別してマテリアルを張り替える
	/// </summary>
	/// <param name="card_data">Card data.</param>
	public void setCardData( CARD_DATA card_data ) {
		_front_material = Resources.Load< Material >( "Materials/Cards/" + card_data.name );
		_front_object.GetComponent< Renderer >( ).material = _front_material;
		_card_data = card_data;
		_id = card_data.id;
	}

	public void changeHandNum( int id ) {
		_hand_num = id;
	}

	public void setSelectFlag( bool selectFlag ) {
		_selected = selectFlag;
	}

	public CARD_DATA getCardData( ) {
		return _card_data;
	}

	public int getHandNum( ) {
		return _hand_num;
	}

	public bool getSelectFlag( ) {
		return _selected;
	}
}
