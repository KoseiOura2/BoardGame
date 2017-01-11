using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;

public class PlayerManager : MonoBehaviour {
	private PLAYER_STATUS _player_status;
	private int _turn_enchant;
	[SerializeField]
	private int _defalut_draw = 0;
	[SerializeField]
	private int _defalut_power = 0;
	private int _plus_draw;
	private int _plus_power;
	/// <summary>
	/// 手札
	/// </summary>
	private List<CARD_DATA> _hand = new List<CARD_DATA>( );
	/// <summary>
	/// 持続強化カード
	/// </summary>
	private List<CARD_DATA> _turn = new List<CARD_DATA>( );
	[SerializeField]
	private CardManager _card_manager;
    void Awake( ) {
		if ( _card_manager == null ) {
			_card_manager = GameObject.Find( "CardManager" ).GetComponent<CardManager>( );
		}
    }
	void Start( ) {
		setDefalutStatus( );
		//デバッグ用　カードID 1~4を登録
		for ( int i = 1; i < 5; i++ ) {
			CARD_DATA data = _card_manager.getCardData( i );
			setHand( data );
		}
	}
	void Update( ) {
	}
	/// <summary>
	/// 強化値を初期化
	/// </summary>
	private void turnStartInit( ) {
		_plus_power = 0;
		_plus_draw = 0;
	}
	/// <summary>
	/// 手札データ登録
	/// </summary>
	/// <param name="data">Data.</param>
	public void setHand( CARD_DATA data ) {
		_hand.Add( data );
	}
	/// <summary>
	/// カード効果適応
	/// </summary>
	/// <param name="card">Card.</param>
	private void playCard( CARD_DATA data ) {
		switch ( data.enchant_type ) {
		case "enhance":
			addPower( data.enchant_value );
			Debug.Log( "強化効果" + data.enchant_value );
			Debug.Log( "power" + _plus_power );
			break;
		case "turn":
			addPower( data.enchant_value );
			_turn.Add( data );
			Debug.Log( "強化効果" + data.enchant_value );
			Debug.Log( "power" + _plus_power );
			break;
		case "special":
			specialEnhance( data );
			Debug.Log( "スペシャル効果" );
			break;
		case "demerit":
			addPower( -data.enchant_value );
			Debug.Log( "power" + _plus_power );
			Debug.Log( "デメリット効果" + data.enchant_value );
			break;
		}
		_hand.Remove( data );//手札削除
	}
	/// <summary>
	/// ２ターン目以降の持続カード効果適応
	/// </summary>
	/// <param name="card">Card.</param>
	private void addStatusTurnCard( ) {
		for ( int i = 0; i < GetTurnCardCount( ); i++ ) {
			addPower( _turn[ i ].enchant_value );
		}
	}
	public void endStatus( ) {
		_player_status.draw = _plus_draw;
		_player_status.power = _plus_power;
	}
	/// <summary>
	/// スペシャルタイプのカード効果
	/// </summary>
	/// <param name="data">Data.</param>
	private void specialEnhance( CARD_DATA data ) {
		if ( data.special_value == ( int )SPECIAL_LIST.ENHANCE_TYPE_DRAW ) {
			_plus_draw += data.enchant_value;
		}
	}
	/// <summary>
	/// 持続カードリストの長さを取得
	/// </summary>
	/// <returns>The turn card count.</returns>
	public int GetTurnCardCount( ) {
		return _turn.Count;
	}
	/// <summary>
	/// 持続カードのカウントダウン処理
	/// </summary>
	public void turnTypeCardCountDown( ) {
		for ( int i = 0; i < GetTurnCardCount( ); i++ ) {
			Debug.Log( "" + _turn[ i ].name );	
			CARD_DATA data = _turn[ i ];
			data.special_value = _turn[ i ].special_value - 1;
			_turn[ i ] = data;
			//カウントが０になったらリムーブ
			if ( _turn[ i ].special_value == 0 ) {
				_turn.Remove( _turn[ i ] );
			}
		}
	}
	/// <summary>
	/// 手札枚数取得
	/// </summary>
	/// <returns>The range.</returns>
	public int getHandRange( ) {
		return _hand.Count;
	}
	/// <summary>
	/// Adds the power.
	/// </summary>
	/// <param name="enhance">Enhance.</param>
	public void addPower( int enhance ) {
		_plus_power += enhance;
	}
	/// <summary>
	/// プレイヤーのステータス
	/// </summary>
	/// <returns>The status.</returns>
	public PLAYER_STATUS getStatus( ) {
		return _player_status;
	}
	/// <summary>
	/// プレイヤーのステータスを初期値へ戻す
	/// </summary>
	public void setDefalutStatus( ) {
		_player_status.draw = _defalut_draw;
		_player_status.power = _defalut_power;
	}
	/// <summary>
	/// カード効果を適応　デバッグ用
	/// </summary>
	public void debugPlayCard( ) {
		playCard ( _hand[ 0 ] );
	}
	/// <summary>
	/// 持続カード効果を適応　デバッグ用
	/// </summary>
	public void debugPlayTurnCard( ) {
		addStatusTurnCard( );
	}
}