using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common;

public class BattleManager : MonoBehaviour {
    
	private const int MAX_SEND_CARD_NUM = 4;
    private const float BATTLE_TIME = 60;

	private Sprite[ ] _small_num = new Sprite[ 10 ];
	private GameObject _result_pref;
	private GameObject _result_obj;
    private float _battle_time = BATTLE_TIME;
	private bool _result_open = false;
	private bool _complete = false;

	// Use this for initialization
	void Start( ) {
	
	}

	public void init( ) {
		loadNumGraphics( );
	}
	
	// Update is called once per frame
	void Update( ) {
	}

    public int[ ] resultSelectCard( int[ ] select_card_list ) {
        List< int > card_list = new List< int >( );

        for ( int i = 0; i < select_card_list.Length; i++ ) {
            card_list.Add( select_card_list[ i ] );
        }

        int[ ] return_card_list = new int[ card_list.Count ];

        for ( int i = 0; i < card_list.Count; i++ ) {
            return_card_list[ i ] = card_list[ i ];
        }

        return return_card_list;

	}

	private void loadNumGraphics( ) {
		for ( int i = 0; i < _small_num.Length; i++ ) {
			_small_num[ i ] = Resources.Load< Sprite >( "Graphics/Number/number_status_" + i );
		}
	}

    public void readyComplete( ) {
        _complete = true;
    }

    public void battleTimeCount( ) {
        if ( !_complete ) {
            _battle_time -= Time.deltaTime;
            if ( _battle_time < 0 ) {
                _battle_time = 0;
                _complete = true;
            }
        }
    }

    public void refreshBattleTime( ) {
        _battle_time = BATTLE_TIME;
    }

	public void changeBattleTimeImageNum( GameObject ten_digit, GameObject digit ) {
		// 10の位を求める
		int ten = ( int )( _battle_time / 10 );
		ten = ten % 10;
		// 1の位を求める
		int one = ( int )_battle_time % 10;

		// イメージのSpriteを変える
		ten_digit.GetComponent< Image >( ).sprite = _small_num[ ten ];
		digit.GetComponent< Image >( ).sprite     = _small_num[ one ];
	}

	/// <summary>
	/// リザルトの表示
	/// </summary>
	/// <param name="result">Result.</param>
	public void createResultImage( BATTLE_RESULT result ) {
		int num = ( int )result;

		// リソースの読み込み
		_result_pref = Resources.Load< GameObject >( "Prefabs/UI/Result/Result_" + num );

		Vector3 pos = _result_pref.GetComponent< RectTransform >( ).localPosition;

		_result_obj = ( GameObject )Instantiate( _result_pref );
		_result_obj.transform.SetParent( GameObject.Find( "Canvas" ).transform );
		_result_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
		_result_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 0.15f, 0.15f, 0.15f );
		_result_obj.GetComponent< RectTransform >( ).localPosition = new Vector3( 0, 10, -550 );

		_result_open = true;
	}

	public void deleteResultImage( ) {
		Destroy( _result_obj );
		_result_obj  = null;
		_result_pref = null;
	}

	public void clearResult( ) {
		_result_open = false;
	}

    public bool isComplete( ) {
        if ( _complete ) {
            _complete = false;
            return true;
        }

        return false;
    }

	public bool isResultOpen( ) {
		return _result_open;
	}
}
