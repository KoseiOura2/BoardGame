using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;

public class PhaseManager : MonoBehaviour {

    [ SerializeField ]
    private MAIN_GAME_PHASE _main_game_phase;   // メインゲームのフロー
	private bool _phase_changed = false;

    private Sprite _dice_phase_image;
    private GameObject _phase_image_obj;
	public Text _phase_text;

	// Use this for initialization
	void Start( ) {
        _main_game_phase = MAIN_GAME_PHASE.GAME_PHASE_NO_PLAY;
		_phase_text.text = "NoPlay";

        _dice_phase_image = Resources.Load< Sprite >( "Graphics/UI/ui_phase_dice" );
	}
	
	// Update is called once per frame
	void Update( ) {
		
	}

    /// <summary>
    /// MainGamePhaseが移行可能かどうか確認する
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="log_text"></param>
    public void changeMainGamePhase( MAIN_GAME_PHASE phase, string log_text ) {
        try {
            _main_game_phase = phase;
			_phase_text.text = log_text;
			_phase_changed = true;
        }
        catch {
            Debug.Log( log_text + "へ移行できませんでした。" );
        }
    }

    public void createPhaseText( MAIN_GAME_PHASE phase ) {
        _phase_image_obj = new GameObject( "PhaseImage" );
        _phase_image_obj.transform.parent = GameObject.Find( "Canvas" ).transform;
        _phase_image_obj.AddComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, 0, 0 );
        _phase_image_obj.GetComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );
        switch ( phase ) {
            case MAIN_GAME_PHASE.GAME_PHASE_DICE:
                _phase_image_obj.AddComponent< Image >( ).sprite = _dice_phase_image;
            break;
        }
        _phase_image_obj.GetComponent< Image >( ).preserveAspect = true;
        _phase_image_obj.GetComponent< Image >( ).SetNativeSize( );
    }

	/// <summary>
	/// MainGamePhaseの取得
	/// </summary>
	/// <returns>The main game phase.</returns>
	public MAIN_GAME_PHASE getMainGamePhase( ) {
		return _main_game_phase;
	}

	/// <summary>
	/// phaseが変わったかどうか
	/// </summary>
	/// <returns><c>true</c>, if phase changed was ised, <c>false</c> otherwise.</returns>
	public bool isPhaseChanged( ) {
		bool flag = false;

		if ( _phase_changed == true ) {
			_phase_changed = false;
			flag = true;
		}

		return flag;
	}

	public void setPhase( MAIN_GAME_PHASE phase ) {
		_main_game_phase = phase;
	}
}
