using UnityEngine;
using System.Collections;

public class Mass : MonoBehaviour {
    
    [ SerializeField ]
    private bool _selected = false;
    private bool _reject   = false;

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
}
