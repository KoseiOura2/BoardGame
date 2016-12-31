using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common;

public class PlayerManager : MonoBehaviour {
    
    public float ADJUST_FIRST_PLAYER_Y_POS = 0.3f;          // プレイヤー初期生成時の修正Y座標
    public float ADJUST_FIRST_PLAYER_Z_POS = 0.1f;          // プレイヤー初期生成時の修正Z座標

    [ SerializeField ]
    private PLAYER_ORDER _player_order;           // どのプレイヤーが行動中か

    private Vector3 _start_position;        //現在位置を設定
    private Vector3 _end_position;          //到達位置を設定
    private GameObject _target;             //進む先のターゲットを設定
    private GameObject[ ] _players = new GameObject[ 2 ];    //プレイヤーを設定
    private int[ ] _player_advance_count = new int[ 2 ];    //プレイヤーの進んでいる回数
    private int _set_player_id = -1;    //動かすプレイヤーID設定
    private int _limit_value = -1;      //進むマス数設定
    private float _time = 1;
    private float _startTime;
    private bool _move_flag = false;        //動かす時のフラグが立っているか
    private bool _advance_flag = true;   //前に進むか後ろに戻るか


    public Text[ ] _count_text  = new Text[ 2 ];    //Text用変数
    public Text[ ] _environment = new Text[ 2 ];    //Text用変数
    private GameObject[ ] _player_pref = new GameObject[ 2 ];    //プレイヤーのモデルをロード
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="first_pos"></param>
    public void init( Vector3 first_pos ) {

        createObj( first_pos );
        // playerオブジェクトの色替え
        
        _players[ 0 ].GetComponent< Renderer>( ).material.color = Color.magenta;
        _players[ 1 ].GetComponent< Renderer>( ).material.color = Color.green;

    }
    
    /// <summary>
    /// ゲーム開始時プレイヤーを生成
    /// </summary>
    public void createObj( Vector3 first_pos ) {
        _player_order = PLAYER_ORDER.PLAYER_ONE;

        for( int i = 0; i < _player_pref.Length; i++ ) {
            first_pos.y = ADJUST_FIRST_PLAYER_Y_POS;
            switch ( _player_order ) {
                case PLAYER_ORDER.PLAYER_ONE:
                    first_pos.z += ADJUST_FIRST_PLAYER_Z_POS;
                    _player_order = PLAYER_ORDER.PLAYER_TWO;
                    break;
                case PLAYER_ORDER.PLAYER_TWO:
                    first_pos.z -= ADJUST_FIRST_PLAYER_Z_POS;
                    _player_order = PLAYER_ORDER.NO_PLAYER;
                    break;
            }

            _players[ i ] = ( GameObject )Instantiate( _player_pref[ i ], first_pos, Quaternion.identity );
            _players[ i ].transform.parent = transform;
            _players[ i ].name = "Player" + i;
        }
    }

	// Use this for initialization
	void Start( ) {
	
	}

	// Update is called once per frame
	void Update( ) {
	
	}
	
    /// <summary>
    /// MovePhaseの更新
    /// </summary>
    /// <param name="count"></param>
    public void movePhaseUpdate( int count ) {
		if ( _limit_value > 0 && _set_player_id > -1 ) {
			playerUpdate( count );
		} else if ( _limit_value == 0 ) {
			_limit_value--;
            // 残りマスを表示
            resideCount( _set_player_id, count );
		} else {
			_set_player_id = -1;
			_target = null;
		}
    }

    void playerUpdate( int count ) {
        if ( !_move_flag ) {
            // ターゲットの設定
            if ( _time <= 0 ) {
                _players[ _set_player_id ].transform.position = _end_position;
                _set_player_id = -1;
                _target = null;
                return;
            }

            _startTime = Time.timeSinceLevelLoad;
            _start_position = _players[ _set_player_id ].transform.position;
            if( _advance_flag ) {
                _target = _stage_manager.getTargetMass ( getPlayerCount( _set_player_id ) + 1 );
            } else { 
                _target = _stage_manager.getTargetMass ( getPlayerCount( _set_player_id ) - 1 );
            }
            _end_position = _target.transform.localPosition;
            _end_position.y += 0.3f;
            _move_flag = true;
        } else {
           playerMove( count );
        }
    }
    //プレイヤーを動かす処理
    void playerMove( int count ) {
        for ( int i = 0; i < _players.Length; i++ ) {
            resideCount( i, count );
        }
        var diff = Time.timeSinceLevelLoad - _startTime;
		if ( diff > _time ) {
			_players[ _set_player_id ].transform.position = _end_position;
            _limit_value--;
            if( _advance_flag ) _player_advance_count[ _set_player_id ]++;
            else _player_advance_count[ _set_player_id ]--;
            _move_flag = false;
            _target = null;
        }

		var rate = diff / _time;
		
		_players[ _set_player_id ].transform.position = Vector3.Lerp ( _start_position, _end_position, rate );
    }

    /// <summary>
    /// プレイヤーの現在位置（環境）
    /// </summary>
    /// <param name="environment"></param>
    /// <param name="num"></param>
    public void playerEnvironment( string environment, int num ) {
		_environment[ num ].text = "プレイヤー" + ( num + 1 ) + ":" + environment;
    }

    /// <summary>
    /// ゴールまでの残りマスを表示
    /// </summary>
    /// <param name="num"></param>
    /// <param name="count"></param>
    public void resideCount( int num ,int count ) {
        _count_text[ num ].text = "プレイヤー" + num.ToString( ) + "：残り" + count.ToString( ) + "マス";
    }

    //プレイヤーがどれくらい進んでいるかを取得
    public int getPlayerCount( int i ) {
        return _player_advance_count[ i ];
    }

    /// <summary>
    /// プレイヤーの人数を取得
    /// </summary>
    /// <returns></returns>
    public int getPlayerNum( ) {
        return _players.Length;
    }

    /// <summary>
    /// playeridを取得
    /// </summary>
    /// <returns></returns>
    public int getPlayerID( ) {
        return _set_player_id;
    }

}
