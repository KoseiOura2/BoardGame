using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Common;

public class ClientPlayerManager : MonoBehaviour {
	
	public GameObject _card_obj;
	private struct PLAYER_CARD_DATA {
		public List<CARD_DATA>  hand_list;
		public List<GameObject> hand_obj_list;
		public List<Vector3>    select_position;
	}

	private PLAYER_CARD_DATA _player_card = new PLAYER_CARD_DATA( );
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//カード生成を行う
	public void deckCardList( int get_card_id ) {
		CARD_DATA card;
		CardManager card_manager = GameObject.Find ( "CardManager" ).GetComponent< CardManager >( );

		//IDのカードデータを取得
		card = card_manager.getCardData( get_card_id );

		//最新のカードを生成
		AllHandCreate( card );
	}
	//現在のの手札の生成を行う
	public void AllHandCreate( CARD_DATA setCard ) {

		//カードを手札に追加
		_player_card.hand_list.Add( setCard );

		//現在のカードを削除
		for ( int i = _player_card.hand_obj_list.Count - 1; i >= 0; i-- ) {
			Destroy( _player_card.hand_obj_list[ i ] );
			_player_card.hand_obj_list.RemoveAt( i );
		}

		for ( int i = 0; i < _player_card.hand_list.Count; i++ ) {
			float card_X;

			//プレハブを生成してリストのオブジェクトに入れる
			_player_card.hand_obj_list.Add( ( GameObject )Instantiate( _card_obj ) );
			//カードデータ設定
			//_player_card.hand_obj_list[ i ].GetComponent< Card >( ).setCardData( _player_card.hand_list[ i ] );
			//スタート地点を取得
		//	float start_Card_Point = ( handArea_Width_Size / 2 ) - _player_card.hand_obj_list[ i ].transform.localScale.x;
			//手札が6枚以下なら
			//カード間に現在の生成中の手札の順番を掛ける
		//	card_X = -start_Card_Point + ( handArea_Width_Size / _player_card.hand_list.Count ) * i;
			//位置を設定する
		//	_player_card.hand_obj_list[ i ].GetComponent< Transform >( ).position = new Vector3( card_X, handArea_postion_y, 3 );
		}
	}

}
