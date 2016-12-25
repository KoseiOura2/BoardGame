using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common;

public class PlayerManager : MonoBehaviour {

    public GameObject _target;
    public FileManager _file_manager;
    public StageManager _stage_manager;
	public PhaseManager _phase_manager;
    public Text[] _count_text = new Text[2]; //Text用変数
    public Text[] _environment = new Text[2];

    public GameObject[] _player_load_data = new GameObject[ 2 ];
    private GameObject[] _player = new GameObject[ 2 ];
    private int[] _player_advance_count = new int[ 2 ];

    public int _set_player_id = -1;
    public int _limit_value = -1;

    private bool _first = true;
    private bool _move_flag = false;
    public bool _advance_flag = true;
    public int _event_count = 0;

    private float _time = 1;
    private float _startTime;
	private Vector3 _start_position;
    private Vector3 _end_position;

    void Awake() {
        if ( isError( ) ) {
            return;
        }

        playerCreate ();

        _player[ 0 ].GetComponent<Renderer>().material.color = Color.magenta;
        _player[ 1 ].GetComponent<Renderer>().material.color = Color.green;

        resideCount();
		_phase_manager.setPhase(PHASE.MOVE_PHASE);

    }

    bool isError( ) {
        bool error = false;

        if ( !_file_manager ) {
            try {
                error = true;
                _file_manager = FileManager.getInstance( );
            } catch {
                Debug.LogError( "ファイルマネージャーのインスタンスが取得できませんでした。" );
            }
        }

        return error;
    }

	// Use this for initialization
	void Start() {
	
	}
	
    void FixedUpdate() {
        
		switch (_phase_manager.getPhase()) {
			case PHASE.DICE_PHASE:
				_phase_manager.setPhase(PHASE.MOVE_PHASE);
				break;
			case PHASE.DRAW_PHASE:
				break;
			case PHASE.MOVE_PHASE:
				if (_limit_value > 0 && _set_player_id > -1) {
					playerUpdate ();
				} else if (_limit_value == 0) {
					_limit_value--;
					_phase_manager.setPhase(PHASE.EVENT_PHASE);
					resideCount ();
				} else {
					_set_player_id = -1;
					_target = null;
				}
				break;
			case PHASE.BATTLE_PHASE:
				break;
			case PHASE.RESULT_PHASE:
				break;
			case PHASE.EVENT_PHASE:
				Debug.Log ("マスイベント！");
				if (_event_count < 2) {
					_stage_manager.massEvent (_player_advance_count [_set_player_id]);
					_event_count++;
				}
				_phase_manager.setPhase(PHASE.DICE_PHASE);
				break;
		}
		playerEnvironment ();
    }

    void playerCreate() {
        for( int i = 0; i < _player_load_data.Length; i++ ){
            Vector3 first_position = _file_manager.getMassCoordinate( 0 );
            first_position.y = 0.3f;
            if( _first )
            {
                first_position.z += 0.1f;
            } else {
                first_position.z -= 0.1f;
            }
            _player[ i ] = 
            ( GameObject )Instantiate( _player_load_data[ i ], first_position, Quaternion.identity );
            _first = !_first;
            _player[ i ].transform.parent = transform;
            _player[ i ].name = "Player" + i;
        }
    }

    void playerUpdate( ) {
        if ( !_move_flag )
        {
            if ( _time <= 0 )
            {
                _player[_set_player_id].transform.position = _end_position;
                _set_player_id = -1;
                _target = null;
                return;
            }

            _startTime = Time.timeSinceLevelLoad;
            _start_position = _player[ _set_player_id ].transform.position;
            if( _advance_flag ) _target = _stage_manager.getTargetMass ( getPlayerCount( _set_player_id ) + 1 );
            else _target = _stage_manager.getTargetMass ( getPlayerCount( _set_player_id ) - 1 );
            _end_position = _target.transform.localPosition;
            _end_position.y += 0.3f;
            _move_flag = true;
        } else {
           playerMove();
        }
    }

    void playerMove() {
        resideCount();
        var diff = Time.timeSinceLevelLoad - _startTime;
		if ( diff > _time ) {
			_player[ _set_player_id ].transform.position = _end_position;
            _limit_value--;
            if( _advance_flag ) _player_advance_count[ _set_player_id ]++;
            else _player_advance_count[ _set_player_id ]--;
            _move_flag = false;
            _target = null;
        }

		var rate = diff / _time;
		
		_player[ _set_player_id ].transform.position = Vector3.Lerp ( _start_position, _end_position, rate );
    }

    void playerEnvironment( ) {
		for (int i = 0; i < _player.Length; i++) {
			if (_file_manager.getEnvironment (getPlayerCount (i)) != "") {
				string evironment = _file_manager.getEnvironment ( getPlayerCount( i ) );
				_environment[ i ].text = "プレイヤー" + (i + 1) + ":" + evironment;
			}
		}
    }

    public int getPlayerCount( int i ) {
        return _player_advance_count[ i ];
    }

    public int getResideCount( int i ) {
        return _file_manager.getMassCount() - 1 - getPlayerCount(i);
    }

    void resideCount() {
        _count_text[ 0 ].text = "プレイヤー1：残り" + getResideCount( 0 ) + "マス";
        _count_text[ 1 ].text = "プレイヤー2：残り" + getResideCount( 1 ) + "マス";
    }

	// Update is called once per frame
	void Update() {
	
	}
}
