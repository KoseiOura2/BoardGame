using UnityEngine;
using System.Collections;

public class SelectArea : MonoBehaviour {

    public bool SelectAreaFlag;

	// Use this for initialization
	void Awake () {
        SelectAreaFlag = false;
    }

    public void selectAreaIn()
    {
        SelectAreaFlag = true;
        Debug.Log("ok");
    }

    public void selectAreaOut()
    {
        SelectAreaFlag = false;
    }

    public bool getSelectAreaFlag()
    {
        //セレクトエリアフラグの状態を返す
        return SelectAreaFlag;
    }

}
