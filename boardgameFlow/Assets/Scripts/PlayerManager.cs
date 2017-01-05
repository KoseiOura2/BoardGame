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
	private GameObject[ ] _players = new GameObject[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //プレイヤーを設定
	private int[ ] _player_advance_count = new int[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //プレイヤーの進んでいる回数
    private int _set_player_id = -1;    //動かすプレイヤーID設定
    private int _limit_value = -1;      //進むマス数設定
    private float _time = 1;
    private float _startTime;
    private bool _move_flag = false;        //動かす時のフラグが立っているか
    private bool _advance_flag = true;   //前に進むか後ろに戻るか

	private GameObject[ ] _player_pref = new GameObject[ ( int )PLAYER_ORDER.MAX_PLAYER_NUM ];    //プレイヤーのモデルをロード
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="first_pos"></param>
    public void init( Vector3 first_pos ) {

		_player_pref[ 0 ] = ( GameObject )Resources.Load( "Prefabs/Player1" );
		_player_pref[ 1 ] = ( GameObject )Resources.Load( "Prefabs/Player2" );

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
			// 位置の決定
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

    // MovePhaseの更新
	public void movePhaseUpdate( int count, GameObject advance_pos, GameObject back_pos ) {
		if ( _limit_value > 0 && _set_player_id > -1 ) {
			if ( !_move_flag ) {
				setTargetPos( count, ref advance_pos, ref back_pos );
			} else {
				playerMove( count );
			}
		} else if ( _limit_value == 0 ) {
			_limit_value--;
		} else {
			_set_player_id = -1;
			_target = null;
		}
    }

	/// <summary>
	/// ターゲットの設定
	/// </summary>
	/// <param name="count">Count.</param>
	/// <param name="advance_pos">Advance position.</param>
	/// <param name="back_pos">Back position.</param>
	private void setTargetPos( int count, ref GameObject advance_pos, ref GameObject back_pos ) {
        if ( _time <= 0 ) {
            _players[ _set_player_id ].transform.position = _end_position;
            _set_player_id = -1;
            _target = null;
            return;
        }

        _startTime = Time.timeSinceLevelLoad;
        _start_position = _players[ _set_player_id ].transform.position;
        if( _advance_flag ) {
			_target = advance_pos;
        } else { 
			_target = back_pos;
        }
        _end_position = _target.transform.localPosition;
        _end_position.y += 0.3f;
        _move_flag = true;
    }

    //プレイヤーを動かす処理
    private void playerMove( int count ) {
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


    //プレイヤーがどれくらい進んでいるかを取得
    public int getPlayerCount( int i ) {
        return _player_advance_count[ i ];
    }

    /// <summary>
    /// playeridを取得
    /// </summary>
    /// <returns></returns>
    public int getPlayerID( ) {
        return _set_player_id;
    }

	public void setAdvanceFlag( bool flag ) {
		_advance_flag = flag;
	}

	public void setLimitValue( int value ) {
		_limit_value = value;
	}

	public void setPlayerID( int id ) {
		_set_player_id = id;
	}

}
