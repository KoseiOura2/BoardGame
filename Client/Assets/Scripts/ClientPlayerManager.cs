using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Common;

public class ClientPlayerManager : MonoBehaviour {

	private const float MAX_DICE_VALUE = 3.9f;
	private const float MIN_DICE_VALUE = 1.0f;
	private const int INIT_PLAYER_POWER = 10;

	/// <summary>
	/// プレイヤーの持つカードのデータ
	/// </summary>
	private struct PLAYER_CARD_DATA {
		public List< CARD_DATA >  hand_list;
		public List< GameObject > hand_obj_list;
		public List< Vector3 >    select_position;
		public List< CARD_DATA >  select_list;
	}

	[ SerializeField ]
	private CardManager _card_manager;
	[ SerializeField ]
	private PLAYER_CARD_DATA _player_card = new PLAYER_CARD_DATA( );
	private PLAYER_DATA _player_data;
	[ SerializeField ]
	private GameObject _hand_area;
	private GameObject _select_area;
	private float _hand_area_width;
	private float _no_select_position_y;
	private float _selected_position_y;
	private int _dice_value = 0;
	private bool _dice_roll = false;

	#if UNITY_EDITOR
	private bool _debug_inst_flag;
	[ SerializeField ]
	private bool _auto_inst_flag = false;	// オートで生成したくない場合はfalseに
	#endif

	public GameObject _card_obj;

	// Use this for initialization
	void Start( ) {
		if ( _hand_area == null ) {
			_hand_area = GameObject.Find( "HandArea" );
		}
		if ( _select_area == null ) {
			_select_area = GameObject.Find( "SelectHandArea" );
		}
		if ( _card_obj == null ) {
			_card_obj = ( GameObject )Resources.Load( "Prefabs/Card" );
		}
		if ( _card_manager == null ) {
			_card_manager = GameObject.Find( "CardManager" ).GetComponent< CardManager >( );
		}

		// プレイヤーの初期化
		_player_card.hand_list       = new List< CARD_DATA >( );
		_player_card.hand_obj_list   = new List< GameObject >( );
		_player_card.select_position = new List< Vector3 >( );
		_player_card.select_list     = new List< CARD_DATA >( );

		_hand_area_width      = _hand_area.GetComponent< Transform >( ).localScale.x;
		_no_select_position_y = _hand_area.GetComponent< Transform >( ).position.y;
		_selected_position_y  = _select_area.GetComponent< Transform >( ).position.y;

		_player_data.power = INIT_PLAYER_POWER;
	}
	
	/// <summary>
    /// エディタ上でのみデバッグ機能が実行される
    /// </summary>
	#if UNITY_EDITOR
	void Update( ) {
        // カードデータの追加
		if ( Input.GetKeyDown( KeyCode.X ) || _auto_inst_flag ) {
			//適当に追加　ToDoランダムに手札を追加する機能
			addPlayerCard( 1 );
			addPlayerCard( 2 );
			addPlayerCard( 3 );
			addPlayerCard( 4 );
			addPlayerCard( 1 );
			addPlayerCard( 1 );
			addPlayerCard( 1 );
			_debug_inst_flag = true;
		}

		if ( Input.GetKeyDown( KeyCode.U ) && _debug_inst_flag || _auto_inst_flag && _debug_inst_flag ) {
			// カードオブジェクトの更新処理
			updateAllPlayerCard( );
			_debug_inst_flag = false;
			if ( _auto_inst_flag ) {
				_auto_inst_flag = false;
			}
		}

		if ( mouseClick( ) ) {
			//GUIにカード情報表示用
			Debug.Log( getSelectCard( ).name );
		}

	}
	#endif

    /// <summary>
	/// 手札にカードを追加する処理(追加用のカードIDを登録)
    /// </summary>
    /// <param name="get_card_id"></param>
	public void addPlayerCard( int get_card_id ) {
		CARD_DATA card;

		//IDのカードデータを取得
		card = _card_manager.getCardData( get_card_id );
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
		allDeletePlayerCard( );
		for ( int i = 0; i < _player_card.hand_list.Count; i++ ) {
			//プレハブを生成してリストのオブジェクトに入れる
			_player_card.hand_obj_list.Add( ( GameObject )Instantiate( _card_obj ) );
			//カードデータ設定
			_player_card.hand_obj_list[ i ].GetComponent< Card >( ).setCardData( _player_card.hand_list[ i ] );
			_player_card.hand_obj_list[ i ].GetComponent< Card >( ).changeHandNum( i );
			playerCardPositionSetting( i, false );
		}
	}

	/// <summary>
	/// カードの表示場所を設定
	/// </summary>
	/// <param name="list_id"> 手札ID </param>
	/// <param name="selected"> ture=選択状態 false=！選択状態 </param>
	private void playerCardPositionSetting( int list_id, bool selected ) {
		float hand_area_postion_y;

		if ( selected ) {
			hand_area_postion_y = _selected_position_y;
			Debug.Log ( "card position y = " + hand_area_postion_y );
		} else {
			hand_area_postion_y = _no_select_position_y;
		}

		float start_card_point = ( _hand_area_width / 2 ) - _player_card.hand_obj_list[ list_id ].transform.localScale.x;
		float card_potision_x = 0.0f;

		//手札が6枚以下なら
		//カード間に現在の生成中の手札の順番を掛ける
		card_potision_x = -start_card_point + ( _hand_area_width / _player_card.hand_list.Count ) * list_id;
		//位置を設定する
		_player_card.hand_obj_list[ list_id ].GetComponent< Transform >( ).position = new Vector3( card_potision_x, hand_area_postion_y, 3 );
	}
		
	/// <summary>
	/// 手札を全て削除
	/// </summary>
	public void allDeletePlayerCard( ) {
		for ( int i = 0; i < _player_card.hand_obj_list.Count; i++ ) {
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
		CARD_DATA card_data = _card_manager.getCardData( 0 );		//念のためダミーデータを挿入
		/*http://qiita.com/valbeat/items/799a18da3174a6af0b89*/
		float distance = 100f;

		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		// Rayの当たったオブジェクトの情報を格納する
		RaycastHit hit = new RaycastHit( );
		// オブジェクトにrayが当たった時
		if ( Physics.Raycast( ray, out hit, distance ) ) {
			// rayが当たったオブジェクトの名前を取得
			if ( hit.collider.gameObject.name == "Card(Clone)" ) {
				Card card = hit.collider.gameObject.GetComponent< Card >( );
				card_data = card.getCardData( );
				card.setSelectFlag( !card.getSelectFlag( ) );
				int id = card.getHandNum( );
				playerCardPositionSetting( id, card.getSelectFlag( ) );
			}
		}
		return card_data;
	}

	/// <summary>
	/// ダイスの目を決定
	/// </summary>
	public void dicisionDiceValue( ) {
		_dice_value = ( int )Random.Range( MIN_DICE_VALUE, MAX_DICE_VALUE );

		_dice_roll = true;
	}

	/// <summary>
	/// 選択したカードを決定する
	/// </summary>
	/// <returns>The select card.</returns>
	public int[ ] dicisionSelectCard( ) {
        List< int > card_num = new List< int >( );
		for ( int i = 0; i < _player_card.hand_list.Count; i++ ) {
			if ( _player_card.hand_obj_list[ i ].GetComponent< Card >( ).getSelectFlag( ) ) {
				// 選択カードに登録
				_player_card.select_list.Add( _player_card.hand_list[ i ] );

                card_num.Add( i );
			}
		}
        
        int count = 0;
		for ( int i = 0; i < card_num.Count; i++ ) {
			// 選択したカードを削除
		    deletePlayerCardData( card_num[ i ] - count );
		    deletePlayerCardObject( card_num[ i ] - count );
            count++;
		}
		

		// 選択カードのIDを返す
		int[ ] card_list = new int[ _player_card.select_list.Count ];
		for ( int i = 0; i < _player_card.select_list.Count; i++ ) {
			card_list[ i ] = _player_card.select_list[ i ].id;
		}

		return card_list;
	}

	public void refreshSelectCard( ) {
		_player_card.select_list.Clear( );
	}

	/// <summary>
	/// ダイスの目を返す
	/// </summary>
	/// <returns>The dice value.</returns>
	public int getDiceValue( ) {
		return _dice_value;
	}

	/// <summary>
	/// ダイスの目を初期化
	/// </summary>
	public void initDiceValue( ) {
		_dice_value = 0;
	}

	public PLAYER_DATA getPlayerData( ) {
		return _player_data;
	}

	/// <summary>
	/// プレイヤーの手札の枚数を返す
	/// </summary>
	/// <returns>The player card number.</returns>
	public int getPlayerCardNum( ) {
		return _player_card.hand_list.Count;
	}

	/// <summary>
	/// ダイスをふったかどうかを返す
	/// </summary>
	/// <returns><c>true</c>, if dice roll was ised, <c>false</c> otherwise.</returns>
	public bool isDiceRoll( ) {
		if ( _dice_roll == true ) {
			_dice_roll = false;
			return true;
		}

		return false;
	}

	/// <summary>
	/// マウスの左クリックの状態を取得
	/// </summary>
	/// <returns><c>true</c>, if click was moused, <c>false</c> otherwise.</returns>
	public bool mouseClick( ) {
		bool flag = false;

		if ( Input.GetMouseButtonDown( 0 ) ) {
			flag = true;
		}

		return flag;
	}
}
