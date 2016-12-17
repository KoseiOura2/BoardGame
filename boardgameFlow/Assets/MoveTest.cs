using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MoveTest : NetworkBehaviour {
    [SyncVar]
    bool flag = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


        if ( Input.GetMouseButtonDown( 0 ) ) {
            flag = true;
        }

        if ( !isLocalPlayer ) {
            if ( flag == true ) {
                flag = false;
                GetComponent<MeshRenderer>( ).material.color = Color.green;
            }
            return;
        }
        var x = Input.GetAxis( "Horizontal" ) * 0.1f;
        var z = Input.GetAxis( "Vertical" ) * 0.1f;

        transform.Translate( x, 0, z );

	}

    public override void OnStartLocalPlayer( ) {
        GetComponent<MeshRenderer>( ).material.color = Color.red;
    }
}
