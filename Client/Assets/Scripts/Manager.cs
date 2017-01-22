using UnityEngine;
using System.Collections;

public abstract class Manager< T > : MonoBehaviour {

	static private T _instance;

	// Use this for initialization
	private void Awake( ) {
		if ( _instance == null ) {
			_instance = gameObject.GetComponent< T >( );
			DontDestroyOnLoad( gameObject );
		} else {
			Destroy( gameObject );
		}

		initialize( ); 
	}
	
	/// <summary>
	/// インスタンス取得
	/// </summary>
	/// <returns></returns>
	static public T getInstance( ) {
		return _instance;
	}

	// 継承先のAwake関数の代わり
	protected abstract void initialize( );
}