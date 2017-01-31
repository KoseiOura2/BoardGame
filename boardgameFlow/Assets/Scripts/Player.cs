using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class Player : MonoBehaviour {
    
    public float ADJUST_FIRST_PLAYER_Y_POS    = 0.3f;          // プレイヤー初期生成時の修正Y座標
    public float ADJUST_PLAYER_POS            = 1.0f;          // プレイヤー初期生成時の修正Z座標

    private PLAYER_DATA _data;

    // プレイヤーのモデルデータ
	private GameObject _player_pref = null;
    // 進む先のターゲットを設定
    [ SerializeField ]
    private GameObject _target_mass;
    // マス移動時の開始位置
    private Vector3 _start_position;   
    // マス移動時の到達位値   
    [ SerializeField ]
    private Vector3 _end_position;          
    // ドローしたカードのリスト
    [ SerializeField ]
    private List< int > _draw_card = new List< int >( );
    
    // プレイヤーに付与するステータス値
	private int _plus_draw;
	private int _plus_power;
    // デフォルトのステータス値
	private int _defalut_draw  = 0;
	private int _defalut_power = 10;

    private float _start_time  = 0.0f;
    // 移動開始したかどうか
	[ SerializeField ]
    private bool _move_start   = false;
    // 移動終了したかどうか
	[ SerializeField ]
    private bool _move_finish  = false;
    // イベント開始したかどうか
	[ SerializeField ]
	private bool _event_start  = false;
    // イベント開始したかどうか
	[ SerializeField ]
	private bool _event_finish = false;
    // カウント変更したかどうか
	[ SerializeField ]
	private bool _change_count = false;
    // 加速の初期化したかどうか
    private bool _accel_init   = false;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="id" "プレイヤーのＩＤ"></param>
    /// <param name="first_pos" "初期配置位置"></param>
    /// <param name="pref" "オブジェクトのプレハブ"></param>
    public void init( int id, ref Vector3 first_pos, ref GameObject pref ) {
        // 初期化
        _data.id          = id;
        _data.event_type  = EVENT_TYPE.EVENT_NONE;
		_data.on_move     = true;

		_player_pref = pref;

        createObj( ref first_pos );

		// ステータス値の初期化
		setDefalutStatus( );
		plusValueInit( );

    }

    /// <summary>
    /// ゲーム開始時プレイヤーを生成
    /// </summary>
    /// <param name="first_pos" "初期配置位置"></param>
    public void createObj( ref Vector3 first_pos ) {
        // オブジェクト生成
		_data.obj = ( GameObject )Instantiate( _player_pref, adjustPos( ref first_pos ), _player_pref.transform.rotation );
        _data.obj.transform.parent = transform;
        _data.obj.name   = "Player" + _data.id;
    }
    
    /// <summary>
    /// 座標の微調整
    /// </summary>
    /// <param name="pos" "修正するポジション"></param>
    /// <returns></returns>
    public Vector3 adjustPos( ref Vector3 pos ) {
        Vector3 adjust_pos = pos;
        // プレイヤーＩＤによって座標を修正
        adjust_pos.x += ( _data.id % 2 == 0 ) ? -ADJUST_PLAYER_POS : ADJUST_PLAYER_POS;
        adjust_pos.z += ( _data.id % 2 == 0 ) ? ADJUST_PLAYER_POS : -ADJUST_PLAYER_POS;

        return pos;
    }

	/// <summary>
	/// プレイヤーのステータスを初期値へ戻す
	/// </summary>
	public void setDefalutStatus( ) {
		_data.draw  = _defalut_draw;
		_data.power = _defalut_power;
	}

	/// <summary>
	/// 強化値を初期化
	/// </summary>
	public void plusValueInit( ) {
		_plus_power = 0;
		_plus_draw  = 0;
	}

	// Use this for initialization
	void Start( ) {
	
	}
	
	// Update is called once per frame
	void Update( ) {
	
	}

    /// <summary>
	/// ターゲットの設定
	/// </summary>
	/// <param name="target_pos"></param>
	public void setTargetPos( float time, ref GameObject target_pos ) {
        if ( time <= 0 ) {
            _data.obj.transform.position = _end_position;
            _target_mass = null;
            return;
        }

        _start_time     = Time.timeSinceLevelLoad;
        _start_position = _data.obj.transform.position;
        _target_mass    = target_pos;
        _end_position   = _target_mass.transform.localPosition;

        // 座標の修正
        _end_position = adjustPos( ref _end_position );
    }

    /// <summary>
	/// プレイヤーを動かす処理
	/// </summary>
     public void move( float time ) {
        
        // 方向を変える
		Quaternion dir = Quaternion.LookRotation( _end_position - _data.obj.transform.position );
		_data.obj.transform.rotation = Quaternion.SlerpUnclamped( _data.obj.transform.rotation, dir, time );
         
        Vector3 velocity = Vector3.zero;
        _data.obj.transform.position = Vector3.SmoothDamp( _data.obj.transform.position, _end_position, ref velocity , time );
		//Vector3.Lerp ( _start_position[ i ], _end_position[ i ], rate );
	}

    public void addDrawCard( int num ) {
        _draw_card.Add( num );
    }

    public List< int > getDrawCard( ) {
        int count = _draw_card.Count;
        
        Debug.Log( "ドローカードの数" + count );
        List< int > card = new List< int >( );

        for ( int i = 0; i < count; i++ ) {
            card.Add( _draw_card[ i ] );
            Debug.Log( "kari:" + card[ i ] );
            Debug.Log( "honmei:" + _draw_card[ i ] );
        }
        _draw_card.Clear( );


        return card;
    }

    public int getDrawCardNum( ) {
        return _draw_card.Count;
    }
    
    // プレイヤーがどれくらい進んでいるかを更新する
    public void updateAdvanceCount( int num ) {
        _data.advance_count += num;
    }

    public void setAdvanceCount( int num ) {
        _data.advance_count = num;
    }

    public void setPlayerRank( PLAYER_RANK rank ) {
        _data.rank = rank;
    }

    public void setEventType( EVENT_TYPE type ) {
        _data.event_type = type;
    }

    public void changeMassCountFlag( bool flag ) {
        _change_count = flag;
    }

    public void setPower( int power ) { 
        _data.power = power;
    }

    public void setBattleResult( BATTLE_RESULT result ) {
        _data.battle_result = result;
    }

    public void addPower( int power ) {
        _plus_power += power;
    }

    public void setOnMove( bool flag ) {
        _data.on_move = flag;
    }
    
    public void endStatus( ) {
        _data.power += _plus_power;
    }

    public void startMove( ) {
        _move_start = true;
    }

    public void finishMove( ) {
        _move_finish = true;
    }

    public void refreshMoveFlag( ) {
        _move_start  = false;
        _move_finish = false;
    }

    public void setEventStart( bool flag ) {
        _event_start = flag;
    }

    public void setEventFinish( bool flag ) {
        _event_finish = flag;
    }

    public void refreshEventFlag( ) {
        _event_start  = false;
        _event_finish = false;
    }

    public void setObjPosForceDistination( ) {
        _data.obj.transform.position = _end_position;
    }

    public void deleteTargetMass( ) {
        _target_mass = null;
    }

    public PLAYER_DATA getData( ) {
        return _data;
    }

    public float getStartTime( ) {
        return _start_time;
    }

    public bool isMoveStart( ) {
        return _move_start;
    }

    public bool isMoveFinish( ) {
        return _move_finish;
    }

    public bool isEventStart( ) {
        return _event_start;
    }

    public bool isEventFinish( ) {
        return _event_finish;
    }
    
    public bool isChangeCount( ) {
        if ( _change_count ) {
            _change_count  = false;
            return true;
        }
        return false;
    }
}
