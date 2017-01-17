using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : Manager< FadeManager > {

	private float _alfa;
	private float _alfa_max = 1;
	private float _alfa_min = 0;
	public float _speed = 0.01f;
	private float _red, _green, _blue;

	private bool _fade_in_flag;

	private string _onScene;
	public GameObject _fade_prefab;

	private GameObject _fade_in_object;
	private GameObject _fade_out_object;

	private bool _fade_in_check;

	// Awake関数の代わり
	protected override void initialize( ) {
		fadeInit( );
	}

	void Update( ){
		fadeStart ( );
	}

	void  fadeInit ( ){
		_fade_in_flag     = false;
		_fade_in_check    = false;
		_red   = _fade_prefab.GetComponent <Image > ( ).color.r;
		_green = _fade_prefab.GetComponent< Image > ( ).color.g;
		_blue  = _fade_prefab.GetComponent< Image > ( ).color.b;
	}

    //フェードを開始させる。セットシーンにシーンを移動
	public void fadeStart( string _SetScene ){
		_alfa      = _alfa_min;
		_onScene   = _SetScene;
		_fade_in_flag = true;
	}

	void fadeStart( ){
		if ( _fade_in_flag ) {
			if ( !_fade_in_check ) { 
				//フェードインのオブジェクトがない場合生成
				if ( _fade_in_object == null ) {
					_fade_in_object = canvasFadeSet ( Vector3.zero, _alfa_max );
				}
				_fade_in_object.GetComponent< Image > ( ).color = new Color ( _red, _green, _blue, _alfa );
				//画面のα値を上げていく
				_alfa += _speed;
				if ( _alfa > _alfa_max ) {
					//フェードインが終わったのでチェックを入れる
					_fade_in_check = true;
					//ここで設定したシーンに移動
					SceneManager.LoadScene (_onScene);
				}
			}

			if( _fade_in_check ){
				//フェードアウトオブジェクトがない場合は生成
				if ( _fade_out_object == null ) {
					_fade_out_object = canvasFadeSet ( Vector3.zero, _alfa_min );
				}
				//フェードアウトを行います
				_fade_out_object.GetComponent< Image >( ).color = new Color( _red, _green, _blue, _alfa );
				//画面のα値を下げていく
				_alfa -= _speed;
				if ( _alfa < _alfa_min ) {
					//フェードアウトオブジェクトを削除、各種フラグをリセット
					Destroy ( _fade_out_object );
					_fade_in_check = false;
					_fade_in_flag = false;
				}
			}
		}
	}

	GameObject canvasFadeSet( Vector3 set_position, float set_alfa ){
		//キャンバスを取得
		GameObject	_canvas_Root = GameObject.Find ( "Canvas" );
		//セットされたプレハブの生成、座標の修正、キャンパスの中に生成します
		GameObject _Setobj = ( GameObject )Instantiate( _fade_prefab );
		_Setobj.transform.SetParent ( _canvas_Root.transform );
		_Setobj.GetComponent< RectTransform >( ).anchoredPosition3D = set_position;
		_Setobj.GetComponent< RectTransform >( ).sizeDelta = Vector2.zero;
		_Setobj.GetComponent< RectTransform >( ).localScale = Vector3.one;
		_Setobj.GetComponent< Image >( ).color = new Color ( _red, _green, _blue, set_alfa );

		return _Setobj;
	}

	public bool fadeCheck( ) {
		return _fade_in_flag;
	}

}
