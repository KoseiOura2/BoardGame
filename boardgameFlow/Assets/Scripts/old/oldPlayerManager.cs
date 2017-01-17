/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;
using PlayerData;
public class oldPlayerManager : MonoBehaviour {




		public GameObject _card_Template_Prefab;    //2Dカードプレハブ
		public GameObject _player_Baloon_Prefab;    //プレイヤー枠
		public GameObject _enemy_Baloon_Prefab;     //エネミー枠
		private GameObject _canvas_Root;            //キャンバスを取得
		private GameObject _content_Root;           //コンテンツを取得
		private GameObject _hand_Area;              //手札エリアを取得

		public FileManager _file_manager;           //ファイルマネージャー

		private PLAYER_ORDER _is_player;            //プレイヤーが1Pか2Pかを取得
		private BATTLE_RESULT _is_result;           //プレイヤーが勝っているか負けているかを取得

		private int _player_here;                   //プレイヤーの現在地
		private int _enemy_here;                    //敵プレイヤーの現在地
		private int _goal_point;                    //ゴールを取得
		private int _hand_max               = 6;    //手札限界数
		private int _select_area_max        = 4;    //セレクトエリアの最大数

		private float _mass_while_x         = 186;  //マス間の間
		private float _baloon_width         = -276; //吹き出しの横幅
		private float _player_baloon_height = 50;   //プレイヤーの吹き出しの高さ
		private float _enemy_baloon_height  = -49;  //相手プレイヤーの吹き出しの高さ
		private float _select_area_start_x  = -81;  //セレクトエリアの開始位置
		private float _select_area_width    = 139;  //セレクトエリアの横幅
		private float _select_area_height   = 45;   //セレクトエリアの高さ

		//手札データ　選んだカードのリスト、手札のリスト、オブジェクト情報といった形で整理をしています
		[SerializeField]
		private struct HAND_DATA {
			public List< CARD_DATA >  select_list;
			public List< CARD_DATA >  hand_list;
			public List< GameObject > hand_obj_list;
			public List< GameObject > select_obj_list;
			public List< Vector3 >    select_position;
		}

		private FILE_DATA _file_Data = new FILE_DATA( );             //マップデータを設定
		private HAND_DATA _hand_Data = new HAND_DATA( );             //プレイヤーの手札を設定
		private List< bool > _select_position_use;                   //セレクトエリアの使用フラグ
		private List< OBJECT_DATA > _playerManager_Objects;          //プレイヤーフェイズオブジェクトをまとめる
		private PLAYER_DATA _player_data;

		// Awake関数の代わり
		protected override void initialize( ) {
			PlayerInitialize( );
		}

		void PlayerInitialize( ) {
			//各種リストを初期化
			_hand_Data.hand_list              = new List< CARD_DATA >( );
			_hand_Data.select_list            = new List< CARD_DATA >( );
			_hand_Data.hand_obj_list          = new List< GameObject >( );
			_hand_Data.select_obj_list        = new List< GameObject >( );
			_hand_Data.select_position        = new List< Vector3 >( );
			_select_position_use              = new List< bool >( );
		}

		void Start( ) {

			//ゴールエリアを取得
			//自身がmapのどの場所にいるかを設定（初期はスタート地点にいるのでStartを探します）
			for ( int i = 0; i < _file_manager.getMassCount( ); i++ ) {
				_file_Data = _file_manager.getMapData( );
				if ( _file_Data.mass[ i ].type == "start" ) {
					_player_here = i + 1;
					_enemy_here  = i;
				}
				if ( _file_Data.mass[ i ].type == "goal" ) {
					_goal_point = i;
				}
			}

			//セレクトエリアのポジションとフラグを設定
			for ( int i = 0; i < _select_area_max; i++ ) {
				//セレクトエリアのフラグをセレクトエリアの数分作成
				_select_position_use.Add( false );

			}

			//カードを生成（デバッグ用）
			for ( int i = 0; i < 6; i++ ) {
				deckCardList( i );
			}

		}

		//プレイヤーの現在地を取得する関数の生成
		public int getPlayerHere( ) {
			return _player_here;
		}

		//ゴール地点を取得する関数
		public int getGoalPoint( ) {
			return _goal_point;
		}

		//プレイヤーの勝敗を取得する関数
		public BATTLE_RESULT getBattleResult( ) {
			return _is_result;
		}

		//バトルリザルトを設定する関数
		public void SetBattleResult( BATTLE_RESULT setResult ) {
			_is_result = setResult;
		}

		//現在のの手札の生成を行う
		public void AllHandCreate( CARD_DATA setCard ) {
			//キャンバスを取得
			if ( _canvas_Root == null ) {
				_canvas_Root = GameObject.Find( "Canvas" );
			}
			//ハンドエリアを取得
			if ( _hand_Area == null ) {
				_hand_Area = GameObject.Find( "HandArea" );
			}

			//カードを手札に追加
			_hand_Data.hand_list.Add( setCard );

			//現在のカードを削除
			for ( int i = _hand_Data.hand_obj_list.Count - 1; i >= 0; i-- ) {
				Destroy( _hand_Data.hand_obj_list[ i ] );
				_hand_Data.hand_obj_list.RemoveAt( i );
			}

			//手札の最大値6よりも大きいなら手札の最大値を更新
			if ( _hand_max < _hand_Data.hand_list.Count ) {
				_hand_max = _hand_Data.hand_list.Count;
			} else {
				_hand_max = 6;
			}

			for ( int i = 0; i < _hand_Data.hand_list.Count; i++ ) {
				float card_X;

				//プレハブを生成してリストのオブジェクトに入れる
				_hand_Data.hand_obj_list.Add( ( GameObject )Instantiate( _card_Template_Prefab ) );
				//カードデータ設定
				_hand_Data.hand_obj_list[ i ].GetComponent< Card >( ).setCardData( _hand_Data.hand_list[ i ] );
				//ハンドエリアの大きさを取得
				float handArea_Width_Size = _hand_Area.GetComponent< Transform >( ).localScale.x;
				//ハンドエリアの高さを取得
				float handArea_postion_y  = _hand_Area.GetComponent< Transform >( ).position.y;
				//スタート地点を取得
				float start_Card_Point = ( handArea_Width_Size / 2 ) - _hand_Data.hand_obj_list[ i ].transform.localScale.x;
				//手札が6枚以下なら
				//カード間に現在の生成中の手札の順番を掛ける
				card_X = -start_Card_Point + ( handArea_Width_Size / _hand_Data.hand_list.Count ) * i;
				//位置を設定する
				_hand_Data.hand_obj_list[ i ].GetComponent< Transform >( ).position = new Vector3( card_X, handArea_postion_y, 3 );
			}
		}

		//最新のカードを生成を行う(ドロー関数)
		public void setHandLatestCardCreate( CARD_DATA setCard ) {
			float card_X;

			//キャンバスを取得
			if ( _canvas_Root == null ) {
				_canvas_Root = GameObject.Find( "Canvas" );
			}
			//ハンドエリアを取得
			if ( _hand_Area == null ) {
				_hand_Area = GameObject.Find( "HandArea" );
			}

			//カードを手札に追加
			_hand_Data.hand_list.Add( setCard );

			//手札の最大値6よりも大きいなら
			if ( _hand_max < _hand_Data.hand_list.Count ) {
				_hand_max = _hand_Data.hand_list.Count;
			} else {
				_hand_max = 6;
			}

			//プレハブを生成してリストのオブジェクトに入れる
			_hand_Data.hand_obj_list.Add( ( GameObject )Instantiate( _card_Template_Prefab ) );
			//最新の手札の配列値を取得
			int HandDataLatest = _hand_Data.hand_list.Count - 1;
			//カードデータ設定
			_hand_Data.hand_obj_list[ HandDataLatest ].GetComponent< Card >( ).setCardData( _hand_Data.hand_list[ HandDataLatest ] );
			//ハンドエリアの大きさを取得
			float handArea_Width_Size = _hand_Area.GetComponent< Transform >( ).localScale.x;
			//ハンドエリアの高さを取得
			float handArea_postion_y  = _hand_Area.GetComponent< Transform >( ).position.y;
			//スタート地点を取得
			float start_Card_Point    = ( handArea_Width_Size / 2 ) - _hand_Data.hand_obj_list[ HandDataLatest ].transform.localScale.x;
			//カード間に現在の生成中の手札の順番を掛ける
			card_X = -start_Card_Point + ( handArea_Width_Size / _hand_Data.hand_list.Count ) * HandDataLatest;

			//位置を設定する
			_hand_Data.hand_obj_list[ HandDataLatest ].GetComponent< Transform >( ).position = new Vector3( card_X, handArea_postion_y, 3 );

		}

		//対応したプレイヤーによってオブジェクトを変更
		public void setPlayerObject( ) {
			//タグでプレイヤーによって変わる部分を取得し色とテキストを変えます
			GameObject[ ] PlayerChanges = GameObject.FindGameObjectsWithTag( "PlayerChange" );
			for(int i = 0; i < PlayerChanges.Length; i++ ) {
				switch ( PlayerChanges[ i ].name ) {
				case "EnemyLabel":
					//テキストを取得
					Text _enemyText = PlayerChanges[ i ].GetComponentInChildren< Text >( );
					//対応したプレイヤーによって色とテキストを変える
					if ( _is_player == PLAYER_ORDER.PLAYER_ONE ) {
						_enemyText.text = "2P";
						PlayerChanges[ i ].GetComponent< Image >( ).color = new Color( 0, 1, 0 );
					}  else {
						_enemyText.text = "1P";
						PlayerChanges[ i ].GetComponent< Image >( ).color = new Color( 1, 0, 0 );
					}
					break;

				case "PlayerLabel":
					//テキストを取得
					Text _playerText = PlayerChanges[ i ].GetComponentInChildren< Text >( );
					//対応したプレイヤーによって色とテキストを変える
					if (_is_player == PLAYER_ORDER.PLAYER_ONE) {
						_playerText.text = "1P";
						PlayerChanges[ i ].GetComponent< Image >( ).color = new Color( 1, 0, 0 );
					}  else {
						_playerText.text = "2P";
						PlayerChanges[ i ].GetComponent< Image >( ).color = new Color( 0, 1, 0 );
					}
					break;
				}
			}
		}

		//相手の状態によって変わるオブジェクトをセット
		public void setEnemyObject( ) {
			//敵のオブジェクトをセット
			//テキストと手札を取得
			GameObject[ ] EnemyChanges = GameObject.FindGameObjectsWithTag( "EnemyChange" );

			for(int i = 0; i < EnemyChanges.Length; i++){
				switch ( EnemyChanges[ i ].name ) {

				case "EnemyHand":
					//テキストを取得
					Text _enemy_hand_text = EnemyChanges[ i ].GetComponentInChildren< Text >( );
					//相手の手札を取得
					//テキストを設定
					_enemy_hand_text.text = "相手の手札　" + "枚";
					break;

				case "EnemyStates":
					//テキストを取得
					Text _enemy_status_text = EnemyChanges[ i ].GetComponentInChildren< Text >( );
					//相手のステータスを取る
					//テキストを設定
					_enemy_status_text.text = "ステータス ";
					break;
				}
			}
		}

		//プレイヤーの吹き出しの変更を2D簡易マップで行います。0で行う事でプレイヤーの現在地のオブジェクトを生成する
		public void setPlayerPos( int setMoveNumber = 0) {
			//キャンバスを取得
			if ( _canvas_Root == null ) {
				_canvas_Root = GameObject.Find("Canvas");
			}
			//コンテンツを取得
			if ( _content_Root == null ) {
				_content_Root = GameObject.Find( "Content" );
			}

			//現在生成されている吹き出しを削除

			//移動数分をプレイヤーの現在地に
			_player_here += setMoveNumber;

			if ( _player_here >= _goal_point ) {
				//ゴール地点より先に行ってしまったらゴール地点で止める
				_player_here = _goal_point;
			}

			//プレイヤー1とプレイヤー2の吹き出しを設定
			GameObject player_baloon_obj = ( GameObject )Instantiate( _player_Baloon_Prefab );
			GameObject enemy_baloon_obj  = ( GameObject )Instantiate( _enemy_Baloon_Prefab );

			//キャンバスのContentに入れる
			player_baloon_obj.transform.SetParent( _content_Root.transform, false );
			enemy_baloon_obj.transform.SetParent( _content_Root.transform, false );

			//プレイヤーオブジェクトを対応したマスのところに移動(数値分Xをずらす)
			player_baloon_obj.GetComponent< RectTransform >( ).anchoredPosition3D = new Vector3( _baloon_width + ( _player_here * _mass_while_x ),
				_player_baloon_height,
				0 );
			enemy_baloon_obj.GetComponent< RectTransform >( ).anchoredPosition3D = new Vector3( _baloon_width + ( _enemy_here * _mass_while_x ),
				_enemy_baloon_height,
				0 );

			//フィールドの最大値から現在の値を
			int FiledPos = _file_manager.getMassCount( ) - _player_here;

			//ゴールナビオブジェクトの取得
			GameObject GoalNavi = GameObject.FindWithTag( "GoalNavi" );

			//子のゴールテキストの取得
			Text GoalText = GoalNavi.GetComponentInChildren< Text >( );

			//テキストの設定
			GoalText.text = "宝まで " + FiledPos + "マス";
		}

		//セレクトエリアに入ってるカードを手札からセレクトエリアのカードリストに移動します
		public void SetSelectAreaCard( ) {
			//セレクトエリアに入っているなら手札リストからセレクトカードリストに移動する
			//自身の現在の手札数を行う
			for ( int i = 0; i < _hand_Data.hand_list.Count; i++ ) {
				bool SelectAreaCheck = _hand_Data.hand_obj_list[ i ].GetComponent< Card >( ).getInSelectArea( );
				//セレクトエリアに入っているか
				if ( SelectAreaCheck ) {
					//オブジェクトをセレクトオブジェクトリストに追加
					_hand_Data.select_obj_list.Add( _hand_Data.hand_obj_list[ i ] );
				}
			}
		}

		//セレクトエリアにカードをドロップした際にセレクトエリアの空いてる場所に入れるようにする
		public bool setSelectAreaPosition( CARD_DATA setCard ) {

			//セレクトエリアのポジションに開いてる順番に入れるようにします
			for ( int i = 0; i < _hand_Data.hand_list.Count; i++ ) {
				//手札とセットカードのIDが一致した場合
				if (_hand_Data.hand_list[ i ].id == setCard.id ) {
					//セレクトエリアのポジションの0番目から～3番目までを順番に確認して使われているかどうかを見る
					//また、セレクトリストのカードがドロップされた場合は位置を戻す
					//そのカードのセレクトエリアIDを取得
					int UseSelectAreaID = _hand_Data.hand_obj_list[ i ].GetComponent< Card >( ).getSelectUseId( );

					//-1で振り分けなしで対応したユーザーIDで使われていないなら設定をする。使われていてもその場所の使用IDを持っていたら通る
					if ( UseSelectAreaID == -1 && !_select_position_use[ 0 ] ) {
						//0ポイントを設定
						setSelectAreaPoint( i, 0 );
						return true;
					}  else if ( UseSelectAreaID == 0 ) {
						//0ポイントを設定
						setSelectAreaPoint( i, 0 );
						return true;
					}

					//-1で振り分けなしか対応したユーザーIDなら設定をする
					if ( UseSelectAreaID == -1 && !_select_position_use[ 1 ] ) {
						//1ポイントを設定
						setSelectAreaPoint( i, 1 );
						return true;
					}  else if ( UseSelectAreaID == 1 ) {
						//1ポイントを設定
						setSelectAreaPoint( i, 1 );
						return true;
					}

					//-1で振り分けなしか対応したユーザーIDなら設定をする
					if ( UseSelectAreaID == -1 && !_select_position_use[ 2 ] ) {
						//2ポイントを設定
						setSelectAreaPoint( i, 2 );
						return true;
					}  else if ( UseSelectAreaID == 2 ) {
						//2ポイントを設定
						setSelectAreaPoint( i, 2 );
						return true;
					}

					//-1で振り分けなしか対応したユーザーIDなら設定をする
					if ( UseSelectAreaID == -1 && !_select_position_use[ 3 ] ) {
						//3ポイントを設定
						setSelectAreaPoint( i, 3 );
						return true;
					}  else if ( UseSelectAreaID == 3 ) {
						//3ポイントを設定
						setSelectAreaPoint( i, 3 );
						return true;
					}
				}
			}
			return false;
		}

		void setSelectAreaPoint( int setID, int setFlagPoint ) {
			//カードのポジションを変更
			_hand_Data.hand_obj_list[ setID ].GetComponent< RectTransform >( ).anchoredPosition3D = _hand_Data.select_position[ setFlagPoint ];
			//カードにどこの選択エリアを使用しているかをセット
			_hand_Data.hand_obj_list[ setID ].GetComponent< Card >( ).setSelectAreaUseId( setFlagPoint );
			//その選択エリアの使用フラグをON 
			_select_position_use[ setFlagPoint ] = true;
		}

		//選択カード以外にドロップされた
		public void setSelectAreaOut( CARD_DATA setCard ) {
			//選択カードから手札に戻す場合にフラグをアウトにする
			for ( int i = 0; i < _hand_Data.hand_list.Count; i++ ) {
				if ( _hand_Data.hand_list[ i ].id == setCard.id ) {
					//セレクトエリアに入っているか確認
					bool _select_area_check = _hand_Data.hand_obj_list[ i ].GetComponent< Card >( ).getInSelectArea( );
					if ( _select_area_check ) {
						//カードにどこの選択エリアを使用しているかをセット
						int _selectarea_id = _hand_Data.hand_obj_list[ i ].GetComponent< Card >( ).getSelectUseId( );
						//その場所のフラグをoffにする
						_select_position_use[ _selectarea_id ] = false;
						//選択エリアの使用情報をリセット
						_hand_Data.hand_obj_list[ i ].GetComponent< Card >( ).setSelectAreaUseId( -1 );
					}
				}
			}
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

		//現在の手札枚数を取得する
		public int getHandListNumber( ) {
			return _hand_Data.hand_list.Count;
		}

		//確定カードの枚数を取得する
		public int getSelectListNumber( ) {
			return _hand_Data.hand_list.Count;
		}

		//現在の手札をサーチしてあるかどうかを取得
		public bool getHandSerach( string cardType ) {
			//現在の手札をサーチして選んだカードタイプを取得
			for ( int i = 0; i < _hand_Data.hand_list.Count; i++ ) {
				if ( _hand_Data.hand_list[ i ].enchant_type == cardType ) {
					return true;
				}
			}
			return false;
		}

		//現在の手札をサーチして取得
		public CARD_DATA getHandData( string CardType ) {
			CARD_DATA card = new CARD_DATA();
			//現在の手札をサーチして選んだカードタイプのカードデータを取得
			for (int i = 0; i < _hand_Data.hand_list.Count; i++) {
				if ( _hand_Data.hand_list[ i ].enchant_type == CardType ) {
					card = _hand_Data.hand_list[ i ];
					return card;
				}
			}
			return card;
		}

		//設定したカードが手札にあれば削除を行う
		public bool cardDelete( int _card_id ) {
			//現在の手札をサーチして選んだカードタイプのカードデータを取得
			for ( int i = 0; i < _hand_Data.hand_list.Count; i++ ) {
				//手札に同一のカードID
				if ( _hand_Data.hand_list[ i ].id == _card_id ) {
					//設定した手札を削除
					Destroy ( _hand_Data.hand_obj_list[ i ] );
					//手札リストとオブジェクトリストから削除
					_hand_Data.hand_list.RemoveAt( i );
					_hand_Data.hand_obj_list.RemoveAt( i );
					return true;
				}
			}
			return false;
		}

		//セレクトエリアのカードを全て削除する
		public void selectAreaDelete( ) {
			for ( int i = _hand_Data.select_list.Count - 1; i >= 0; i-- ) {
				//セレクトリストとセレクトオブジェクトリストから削除
				_hand_Data.hand_list.RemoveAt(i);
				_hand_Data.hand_obj_list.RemoveAt( i );
				_select_position_use[ i ] = false;
			}
		}

		//セレクトエリアのカードを全て戻す
		public void selectAreaReturn(){
			for ( int i = _hand_Data.select_list.Count - 1; i >= 0; i-- ) {
				//セレクトリストとセレクトオブジェクトリストから削除
				_hand_Data.select_list.RemoveAt( i );
				_hand_Data.select_obj_list.RemoveAt( i );
				_select_position_use[ i ] = false;
			}
		}
	}
*/