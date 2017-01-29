using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Common;

public class Card : MonoBehaviour {

    const float MOUSE_OVER_TIME = 2.0f;

    [ SerializeField ]
	private GameObject _front_object;
	private Material _front_material;
	private CARD_DATA _card_data;
	[ SerializeField ]
	private int _hand_num = -1;
	[ SerializeField ]
    private int _select_area_num = -1;
	[ SerializeField ]
	private bool _selected;
	[ SerializeField ]
	private int _id = -1;
    [ SerializeField ]
    private float _over_time = 0.0f;
    private bool _mouse_overed = false;

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

    public void changeSelectAreaNum( int id ) {
        _select_area_num = id;
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

    public int getSelectAreaNum( ) {
        return _select_area_num;
    }

	public bool getSelectFlag( ) {
		return _selected;
	}

    public bool isMouseOvered( ) {
        return _mouse_overed;
    }

    public void mouseOverFinish( ) {
        _mouse_overed = false;
    }

    public void OnMouseOver( ) {
        if ( !_mouse_overed ) {
            _over_time += Time.deltaTime;
            if ( _over_time >= MOUSE_OVER_TIME ) {
                _mouse_overed = true;
                _over_time = 0.0f;
            }
        }
    }

    // オブジェクトの範囲内からマウスポインタが出た際に呼び出されます。
    // 
    public void OnMouseExit( ) {
        _over_time = 0.0f;
    }
}
