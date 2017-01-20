using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Common;

public class ClientPlayerManager : MonoBehaviour {
	
	private struct PLAYER_CARD_DATA {
		public List< CARD_DATA >  hand_list;
		public List< GameObject > hand_obj_list;
		public List< Vector3 >    select_position;
	}

	[ SerializeField ]
	private CardManager _card_manager;
	[ SerializeField ]
	private PLAYER_CARD_DATA _player_card = new PLAYER_CARD_DATA( );
	[ SerializeField ]
	private GameObject _hand_Area;
	private GameObject _hand_Area2;
	float handArea_Width_Size;
	private float _no_select_position_y;
	private float _selected_position_y;
	public GameObject _card_obj;
	#if UNITY_EDITOR
	private bool _debug_inst_flag;
	/// <summary>
	/// オートで生成したくない場合はfalseに
	/// </summary>
	[SerializeField]
	private bool _auto_inst_flag = false;
	#endif

	// Use this for initialization
	void Start( ) {
		if ( _hand_Area == null ) {
			_hand_Area = GameObject.Find( "HandArea" );
		}
		if ( _hand_Area2 == null ) {
			_hand_Area2 = GameObject.Find( "HandArea" );
		}
		if ( _card_obj == null ) {
			_card_obj = ( GameObject )Resources.Load( "Prefabs/Card" );
		}
		if ( _card_manager == null ) {
			_card_manager = GameObject.Find( "CardManager" ).GetComponent< CardManager >( );
		}
		_player_card.hand_list       = new List< CARD_DATA >( );
		_player_card.hand_obj_list   = new List< GameObject >( );
		_player_card.select_position = new List< Vector3 >( );
		handArea_Width_Size = _hand_Area.GetComponent< Transform >( ).localScale.x;
		_no_select_position_y = _hand_Area.GetComponent< Transform > ().position.y;
		_selected_position_y = _hand_Area2.GetComponent< Transform > ().position.y;
	}
	
	/// <summary>
    /// エディタ上でのみデバッグ機能が実行される
    /// </summary>
	#if UNITY_EDITOR
	void Update( ) {
        // カードデータの追加
		if ( Input.GetKeyDown( KeyCode.X ) || _auto_inst_flag) {
			//適当に追加　ToDoランダムに手札を追加する機能
			addPlayerCard( 1 );
			addPlayerCard( 2 );
			addPlayerCard( 3 );
			addPlayerCard( 4 );
			addPlayerCard( 1 );
			addPlayerCard( 1 );
			_debug_inst_flag = true;
		}

		if (Input.GetKeyDown (KeyCode.U) && _debug_inst_flag || _auto_inst_flag && _debug_inst_flag) {
			// カードオブジェクトの更新処理
			updateAllPlayerCard ();
			_debug_inst_flag = false;
			if (_auto_inst_flag) {
				_auto_inst_flag = false;
			}
		}
		if (mouseClick ()) {
			//GUIにカード情報表示用
			Debug.Log (getSelectCard().name);
		}

	}
	#endif

	//現在の手札の生成を行う
    /// <summary>
	/// 手札にカードを追加する処理(追加用のカードIDを登録)
    /// </summary>
    /// <param name="get_card_id"></param>
	private void addPlayerCard( int get_card_id ) {
		CARD_DATA card;

		//IDのカードデータを取得
		card = _card_manager.getCardData( get_card_id );

		Debug.Log( card.name );
		//カードを手札に追加
		_player_card.hand_list.Add( card );
    }

    /// <summary>
    /// 任意の持ち札を手札データから削除する
    /// </summary>
    /// <param name="id"></param>
    private void deletePlayerCardData( int id ) {
		_player_card.hand_list.RemoveAt( id );
    }

    /// <summary>
    /// 手札の更新を行う
    /// </summary>
	public void updateAllPlayerCard( ) {
		//allDeletePlayerCard ();
		for ( int i = 0; i < _player_card.hand_list.Count; i++ ) {
			//プレハブを生成してリストのオブジェクトに入れる
			_player_card.hand_obj_list.Add( ( GameObject )Instantiate( _card_obj ) );
			//カードデータ設定
			_player_card.hand_obj_list[ i ].GetComponent< Card >( ).setCardData( _player_card.hand_list[ i ] );
			playerCardPositionSetting(i, false);
		}
	}

	/// <summary>
	/// カードの表示場所を設定　第一引数手札ID 第二引数ture=選択状態 false=！選択状態
	/// </summary>
	/// <param name="list_id">List identifier.</param>
	/// <param name="selected">If set to <c>true</c> selected.</param>
	private void playerCardPositionSetting(int list_id, bool selected ){
		float handArea_postion_y;
		if (selected) {
			handArea_postion_y = _selected_position_y;
		} else {
			handArea_postion_y = _no_select_position_y;
		}
		float start_Card_Point = ( handArea_Width_Size / 2 ) - _player_card.hand_obj_list[ list_id ].transform.localScale.x;
		float card_potision_x;
		//手札が6枚以下なら
		//カード間に現在の生成中の手札の順番を掛ける
		card_potision_x = -start_Card_Point + ( handArea_Width_Size / _player_card.hand_list.Count ) * list_id;
		//位置を設定する
		_player_card.hand_obj_list[ list_id ].GetComponent< Transform >( ).position = new Vector3( card_potision_x, handArea_postion_y, 3 );
	}
		
	/// <summary>
	/// 手札を全て削除
	/// </summary>
	public void allDeletePlayerCard(){
		for (int i = 0; i < _player_card.hand_obj_list.Count + 1; i++){
			deletePlayerCardObject( i );
		}
	}
    /// <summary>
    /// 任意の持ち札オブジェクトを削除する
    /// </summary>
    /// <param name="id"></param>
    private void deletePlayerCardObject( int id ) {
		Destroy( _player_card.hand_obj_list[ id ] );
		_player_card.hand_obj_list.RemoveAt( id );
	}

	/// <summary>
	/// マウスから飛ばしたレイでカード情報を拾う カードを選択した時の処理
	/// 要マウスクリック判定と併用
	/// </summary>
	/// <returns>The select card.</returns>
	public CARD_DATA getSelectCard( ) {
		CARD_DATA card_data = _card_manager.getCardData(0);		//念のためダミーデータを挿入
		/*http://qiita.com/valbeat/items/799a18da3174a6af0b89*/
		float distance = 100f;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Rayの当たったオブジェクトの情報を格納する
		RaycastHit hit = new RaycastHit();
		// オブジェクトにrayが当たった時
		if (Physics.Raycast(ray, out hit, distance)) {
			// rayが当たったオブジェクトの名前を取得
			if (hit.collider.gameObject.name == "Card(Clone)") {
				Card card = hit.collider.gameObject.GetComponent<Card>();
				card_data = card.getCardData ();
				card.setSelectFlag(!card.getSelectFlag());
				//以下未実装項目
				#if false
				//getSelectCardでオブジェクト番号（何番目の手札か？）をlist検索などで判別

				//playerCardPositionSettingで選択状態を判別して表示位置を更新
				playerCardPositionSetting(playerCardID, card.getSelectFlag());
				#endif 
			}
		}
		return card_data;
	}

	/// <summary>
	/// マウスの左クリックの状態を取得
	/// </summary>
	/// <returns><c>true</c>, if click was moused, <c>false</c> otherwise.</returns>
	public bool mouseClick(){
		if (Input.GetMouseButtonDown (0)) {
			return true;
		} else {
			return false;
		}
	}
}
