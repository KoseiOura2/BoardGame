using UnityEngine;
using System.Collections;
using Common;
using UnityEngine.EventSystems;

public class Mass : MonoBehaviour {
    
    [ SerializeField ]
    private bool _selected     = false;
    private bool _reject       = false;
    [ SerializeField ]
    private bool _mouse_overed = false;

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    public void selectedOnClick( ) {
        if ( !_reject ) {
            _selected = true;
        }
    }

    public bool isSelected( ) {
        if ( _selected ) {
            _selected = false;
            return true;
        }

        return false;
    }

    public void changeReject( bool flag ) {
        _reject = flag;
    }

    public bool isMouseOvered( ) {
        return _mouse_overed;
    }

    public void OnMouseEnter( ) {
        _mouse_overed = true;
    }

    // オブジェクトの範囲内からマウスポインタが出た際に呼び出されます。
    // 
    public void OnMouseExit( ) {
        _mouse_overed = false;
    }
}
