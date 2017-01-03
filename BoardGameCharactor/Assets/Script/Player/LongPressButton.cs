

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressButton : 
MonoBehaviour,
IPointerDownHandler,
IPointerUpHandler
{
	// マウスオーバー時に呼び出すイベント
	public UnityEvent onMouseOver = new UnityEvent ();

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

	public void OnPointerDown (PointerEventData eventData)
	{                               
		onMouseOver.Invoke ();
		isMouseOver = true;
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		// マウスオーバー状態が終了した場合に実行
		if (!isMouseOver) {
			EndPress ();
			return;
		}
		// 押下状態が続いている(isDrag:true)なら何もしない
		if ((scroll??(scroll = FindObjectOfType<LongPressScroll>())).CheckPressedStill (this))
			return;
		EndPress ();
	}

	/// <summary>
	/// マウスオーバー状態終了時に呼ぶメソッド
	public void EndPress(){
		isMouseOver = false;
	}

}

