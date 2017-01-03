using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : Manager<FadeManager> {

	private float alfa;
	private float alfaMax = 1;
	private float alfaMin = 0;
	public float speed = 0.01f;
	private float red, green, blue;

	private bool fadeInFlag;

	private string _onScene;
	public GameObject FadePrefab;

	private GameObject FadeInObject;
	private GameObject FadeOutObject;

	private bool FadeInCheck;

	// Awake関数の代わり
	protected override void initialize( ) {
		FadeInit( );
	}

	void Update(){
		FadeStart ();
	}

	void  FadeInit (){
		fadeInFlag = false;
		FadeInCheck = false;
		red = FadePrefab.GetComponent<Image> ().color.r;
		green = FadePrefab.GetComponent<Image> ().color.g;
		blue = FadePrefab.GetComponent<Image> ().color.b;
	}

	public void FadeStart( string _SetScene ){
		alfa = alfaMin;
		_onScene = _SetScene;
		fadeInFlag = true;
	}

	void FadeStart(){
		if (fadeInFlag) {
			if (!FadeInCheck) { 
				//フェードインのオブジェクトがない場合生成
				if (FadeInObject == null) {
					FadeInObject = canvasFadeSet (Vector3.zero, alfaMax);
				}
				FadeInObject.GetComponent<Image> ().color = new Color (red, green, blue, alfa);
				//画面のα値を上げていく
				alfa += speed;
				if (alfa > alfaMax) {
					//フェードインが終わったのでチェックを入れる
					FadeInCheck = true;
					//ここで設定したシーンに移動
					SceneManager.LoadScene (_onScene);
				}
			}

			if( FadeInCheck ){
				//フェードアウトオブジェクトがない場合は生成
				if (FadeOutObject == null) {
					FadeOutObject = canvasFadeSet (Vector3.zero, alfaMin);
				}
				//フェードアウトを行います
				FadeOutObject.GetComponent<Image> ().color = new Color (red, green, blue, alfa);
				//画面のα値を下げていく
				alfa -= speed;
				if (alfa < alfaMin) {
					//フェードアウトオブジェクトを削除、各種フラグをリセット
					Destroy (FadeOutObject);
					FadeInCheck = false;
					fadeInFlag = false;
				}
			}
		}
	}

	GameObject canvasFadeSet( Vector3 _setPosition, float _setAlfa ){
		//キャンバスを取得
		GameObject	_canvas_Root = GameObject.Find ("Canvas");
		//セットされたプレハブの生成、座標の修正、キャンパスの中に生成します
		GameObject _Setobj = ( GameObject )Instantiate( FadePrefab );
		_Setobj.transform.SetParent (_canvas_Root.transform);
		_Setobj.GetComponent<RectTransform> ().anchoredPosition3D = _setPosition;
		_Setobj.GetComponent<RectTransform> ().sizeDelta = Vector2.zero;
		_Setobj.GetComponent<RectTransform> ().localScale = Vector3.one;
		_Setobj.GetComponent<Image> ().color = new Color (red, green, blue, _setAlfa);

		return _Setobj;
	}

}
