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
	public UnityEvent _on_mouse_over = new UnityEvent ( );

	//マウスオーバー終了時に呼び出すイベント
	public UnityEvent _on_mouse_over_exit = new UnityEvent( );

	public UnityEvent _on_click = new UnityEvent ( );

	// マウスオーバー状態
	public bool _is_mouse_over
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
		if ( ( scroll ?? ( scroll = FindObjectOfType< LongPressScroll > ( ) ) ).CheckPressedStill ( this ) ) {
			endMouseOver ( );
		}
	}

	//マウスカーソルが入った場合
	public void OnPointerEnter ( PointerEventData eventData )
	{
		if ( ( scroll ?? ( scroll = FindObjectOfType< LongPressScroll > ( ) ) ).CheckPressedStill ( this ) ) {
			return;
		} else {
			_on_mouse_over.Invoke ( );
			_is_mouse_over = true;
		}
	}

	//マウスカーソルが出た場合
	public void OnPointerExit ( PointerEventData eventData )
	{
		endMouseOver ( );
	}

	//クリックされた場合
	public void OnPointerClick( PointerEventData eventData ){
		_on_click.Invoke ( );
	}
		
	/// マウスオーバー状態終了時に呼ぶメソッド
	public void endMouseOver( ){
		_on_mouse_over_exit.Invoke ( );
		_is_mouse_over = false;
	}

}

