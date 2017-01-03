using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class massData : MonoBehaviour {

	public FileManager _file_manager;

	//自身のイメージの場所
	private Image img_Source;

	private Sprite img_Sprite;

	//自身がどの場所のマスなのか取得


	//
	void Awake () {
		img_Source = GetComponent<Image> ();
	}

	void Update () {
	}

	public void SetMassData( string SetData ){
		
		switch (SetData)
		{
		case "start":
		case "goal":
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_yellow");
			break;
		case "draw":
		case "advance":
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_blue");
			break;
		case "trap1":
		case "trap2":
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_red");
			break;
		case "event":
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_green");
			break;
		}
		img_Source.sprite = img_Sprite;
	}

}