using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class LongPressScroll : ScrollRect{

	/// ドラッグ状態
	public bool isDrag
	{
		get;
		private set;
	}

	//初期位置
	private Vector2 _init_Position;

	//ドラッグ限界
	private Vector2 _End_Position;

	[SerializeField]
	private LongPressButton _pressed_Button;

	private Map2d _map_2D;

	new void Awake () {
		// 子のLongPressButtonに自身の参照を持たせる
		LongPressButton[] buttons = GetComponentsInChildren<LongPressButton> ();
		foreach (LongPressButton item in buttons) {
			item.scroll = this;
		}

		_map_2D = GetComponent<Map2d> ();

		//自身の初期位置を持つ
		_init_Position = content.anchoredPosition;

		_End_Position = _map_2D.getEndPosition();

	}

	void Update(){
		//最初の位置より左に行かないように
		if (content.anchoredPosition.x > 0) {
			content.anchoredPosition = _init_Position;
		}
		/*
		if (content.anchoredPosition.x < _End_Position.x) {
			content.anchoredPosition = _End_Position;
		}
		*/
	}
		
	// 現在も押下状態であるかの判定を返す
	public bool CheckPressedStill(LongPressButton button){
		_pressed_Button = button;
		if (isDrag)
			return true;
		return false;
	}

	public override void OnBeginDrag(PointerEventData eventData){
		base.OnBeginDrag (eventData);
		isDrag = true;

	}

	public override void OnEndDrag(PointerEventData eventData){
		base.OnEndDrag (eventData);
		isDrag = false;
		if(_pressed_Button)
		_pressed_Button.EndPress ();
		_pressed_Button = null;
	}
}