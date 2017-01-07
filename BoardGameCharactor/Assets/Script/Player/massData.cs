using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class massData : MonoBehaviour {

	//吹き出しの取得
	private GameObject _Mass_Baloon;

	//吹き出しの場所を取得
	private Vector3 _baloon_Position;

	//_Content_Rootを取得
	private RectTransform _Content;

	//Canvasを取得
	private GameObject _Canvas_Root;

	//吹き出しの場所のXとYを設定
	private float _baloon_X = 185;
	private float _baloon_Y = 114;

	private float _start_Content_Postion;

	//自身のイメージ
	private Image img_Source;

	//書き換えスプライト
	private Sprite img_Sprite;

	//書き換えテキスト
	private string _baloon_Text;

	void Awake () {
		img_Source = GetComponent<Image> ();
		//吹き出しを保存
		//_Mass_Baloon = Resources.Load ("Prefab/Mass_Baloon");
		//キャンバスを取得
		if (_Canvas_Root == null) {
			_Canvas_Root = GameObject.Find ("Canvas");
		}
		//Scrollのコンテンツを取得
		if (_Content == null) {
			GameObject _Content_Obj = GameObject.Find ("Content");
			_Content = _Content_Obj.GetComponent<RectTransform> ();
		}
		//コンテンツポジションの開始時の位置を取得
		_start_Content_Postion = _Content.GetComponent<RectTransform> ().anchoredPosition.x;
	}
	public void SetMassData( string SetData ){
		//マスのスプライトを取得,設定をします。吹き出しを取得、テキストを設定をします
		switch (SetData)
		{
		case "start":
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_yellow");
			_baloon_Text = "開始マス";
			break;
		case "goal":
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_yellow");
			_baloon_Text = "ゴールマス";
			break;
		case "draw":
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_blue");
			_baloon_Text = "ドローマス";
			break;
		case "advance":
			_baloon_Text = "アドバンスマス";
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_blue");
			break;
		case "trap1":
		case "trap2":
			_baloon_Text = "トラップマス";
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_red");
			break;
		case "event":
			_baloon_Text = "イベントマス";
			img_Sprite = Resources.Load<Sprite> ("Graphec/Sprite/masu/masu_green");
			break;
		}
		img_Source.sprite = img_Sprite;
	}

	public void SetBaloonPosition( int SetID ){
		//IDをもらってその場所のIDに吹き出しが来るように座標を設定
		_baloon_Position = new Vector3 ( -_baloon_X + (_baloon_X * SetID), _baloon_Y, 0 );
	}

	public void BaloonDrow(){
		//コンテンツPosの修正をする
		float _content_Pos = (-_start_Content_Postion + _Content.GetComponent<RectTransform> ().anchoredPosition.x);
		//コンテンツの座標を加えた座標を取得
		Vector3 revisionPos = new Vector3 (_baloon_Position.x + _content_Pos, _baloon_Y, 0);

		//吹き出しのプレハブから生成
		_Mass_Baloon = ( GameObject )Instantiate( Resources.Load ("Prefab/Mass_Baloon") );
		//看板のテキストを取得
		Text _mass_Text = _Mass_Baloon.GetComponentInChildren<Text>();
		//テキストを変更
		_mass_Text.text = _baloon_Text;
		//キャンバス直下に配置
		_Mass_Baloon.transform.SetParent (_Canvas_Root.transform);
		//修正された座標を吹き出しの座標にする
		_Mass_Baloon.GetComponent<RectTransform> ().anchoredPosition3D = revisionPos;
		//スケールがめちゃくちゃになっているので修正
		_Mass_Baloon.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		//名前を設定
		_Mass_Baloon.name = "MassBaloon";
	}
		
	public void BaloonDelete(){
		//吹き出しの削除
		Destroy (_Mass_Baloon);
	}
		
}