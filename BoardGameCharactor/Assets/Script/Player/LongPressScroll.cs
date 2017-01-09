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
	private Vector2 _end_Position;

	private float _init_Content_Position = -280;

	[SerializeField]
	private PressButton _pressed_Button;

	private Map2d _map_2D;

	new void Awake () {
		
		// 子のLongPressButtonに自身の参照を持たせる
		PressButton[] buttons = GetComponentsInChildren<PressButton> ();
		foreach (PressButton item in buttons) {
			item.scroll = this;
		}

		_map_2D = GetComponent<Map2d> ();

		//コンテンツが-280になっていなければ
		if (content.anchoredPosition.x != _init_Content_Position) {
			
			content.anchoredPosition = new Vector2 (_init_Content_Position, content.anchoredPosition.y);
		}
		//コンテンツの初期位置を持つ
		_init_Position = content.anchoredPosition;
	}

	new void Start(){
		//マップから最終マスまでを取得（隙間分まで）
		_end_Position = _map_2D.getEndPosition();
	}

	void Update(){
		//最初の位置より左に行かないように
		if (content.anchoredPosition.x > _init_Position.x) {
			content.anchoredPosition = _init_Position;
		}
		//最後のマスより右に行かないように
		if (content.anchoredPosition.x < _end_Position.x) {
			content.anchoredPosition = _end_Position;
		}
	}
		
	// 現在も押下状態であるかの判定を返す
	public bool CheckPressedStill(PressButton button){
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
		//_pressed_Button.EndPress ();
		_pressed_Button = null;
	}
}