using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkGUIControll : MonoBehaviour {

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {
		
	}

    /// <summary>
    /// ネットワークGUIを表示するか決める
    /// </summary>
    /// <param name="flag"></param>
    public void setShowGUI( bool flag ) {
        GetComponent< NetworkManagerHUD >( ).showGUI = flag;
    }
}
