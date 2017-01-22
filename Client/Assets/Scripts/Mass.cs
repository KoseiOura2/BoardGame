using UnityEngine;
using System.Collections;

public class Mass : MonoBehaviour {
    
    [ SerializeField ]
    private bool _selected = false;

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    public void selectedOnClick( ) {
        _selected = true;
    }

    public bool isSelected( ) {
        if ( _selected ) {
            _selected = false;
            return true;
        }

        return false;
    }
}
