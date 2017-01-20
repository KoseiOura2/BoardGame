using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class LongPressScroll : ScrollRect{

	/// ドラッグ状態
	public bool _is_drag
	{
		get;
		private set;
	}

	private Vector2 _init_position;	//初期位置

	private Vector2 _end_position;	//ドラッグ限界

	private float _init_content_position = -280;

	[SerializeField]
	private PressButton _pressed_button;

	private Map2d _map_2d;

	new void Awake ( ) {
		
		// 子のLongPressButtonに自身の参照を持たせる
		PressButton[ ] buttons = GetComponentsInChildren< PressButton > ( );
		foreach ( PressButton item in buttons ) {
			item.scroll = this;
		}

		_map_2d = GetComponent< Map2d > ( );

		//コンテンツが-280になっていなければ
		if ( content.anchoredPosition.x != _init_content_position ) {
			
			content.anchoredPosition = new Vector2 ( _init_content_position, content.anchoredPosition.y );
		}
		//コンテンツの初期位置を持つ
		_init_position = content.anchoredPosition;
	}

	new void Start( ){
		//マップから最終マスまでを取得（隙間分まで）
		_end_position = _map_2d.getEndPosition( );
	}

	void Update(){
		//最初の位置より左に行かないように
		if ( content.anchoredPosition.x > _init_position.x ) {
			content.anchoredPosition = _init_position;
		}
		//最後のマスより右に行かないように
		if ( content.anchoredPosition.x < _end_position.x ) {
			content.anchoredPosition = _end_position;
		}
	}
		
	// 現在も押下状態であるかの判定を返す
	public bool CheckPressedStill( PressButton button ){
		_pressed_button = button;
		if ( _is_drag )
			return true;
		return false;
	}

	public override void OnBeginDrag( PointerEventData eventData ){
		base.OnBeginDrag ( eventData );
		_is_drag = true;

	}

	public override void OnEndDrag( PointerEventData eventData ){
		base.OnEndDrag ( eventData );
		_is_drag = false;
		if( _pressed_button )
		_pressed_button = null;
	}
}