using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PressButton : 
MonoBehaviour,
IPointerEnterHandler,
IPointerExitHandler,
IPointerClickHandler
{
	// マウスオーバー時に呼び出すイベント
	public UnityEvent onMouseOver = new UnityEvent ();

	//マウスオーバー終了時に呼び出すイベント
	public UnityEvent onMouseOverExit = new UnityEvent();

	public UnityEvent onClick = new UnityEvent ();

	// マウスオーバー状態
	public bool isMouseOver
	{
		get;
		private set;
	}

	// スクローラーの参照
	public LongPressScroll scroll
	{
		get;
		set;
	}

	void Update(){
		if ((scroll ?? (scroll = FindObjectOfType<LongPressScroll> ())).CheckPressedStill (this)) {
			EndMouseOver ();
		}
	}

	//マウスカーソルが入った場合
	public void OnPointerEnter (PointerEventData eventData)
	{
		if ((scroll ?? (scroll = FindObjectOfType<LongPressScroll> ())).CheckPressedStill (this)) {
			return;
		} else {
			onMouseOver.Invoke ();
			isMouseOver = true;
		}
	}

	//マウスカーソルが出た場合
	public void OnPointerExit (PointerEventData eventData)
	{
		EndMouseOver ();
	}

	//クリックされた場合
	public void OnPointerClick(PointerEventData eventData){
		onClick.Invoke ();
	}
		
	/// マウスオーバー状態終了時に呼ぶメソッド
	public void EndMouseOver(){
		onMouseOverExit.Invoke ();
		isMouseOver = false;
	}

}

