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
	private float _phase_image_move_speed = 10.0f;
	private float _phase_image_width      = 0.0f;
	private float _phase_image_height     = 0.0f;
	private bool _phase_image_move_finish = false;

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

	/// <summary>
	/// PhaseTextImageの生成
	/// </summary>
	/// <param name="phase">Phase.</param>
	public void createPhaseText( MAIN_GAME_PHASE phase ) {
		// flagを初期化
		_phase_image_move_finish = false;
		// オブジェクトの生成
        _phase_image_obj = new GameObject( "PhaseImage" );
		_phase_image_obj.transform.parent = GameObject.Find( "Canvas" ).transform;

		// 大きさを単位化
		_phase_image_obj.AddComponent< RectTransform >( ).localScale = new Vector3( 1, 1, 1 );

		// フェイズによって画像を切り替え
		switch ( phase ) {
		case MAIN_GAME_PHASE.GAME_PHASE_DICE:
			_phase_image_obj.AddComponent< Image >( ).sprite = _dice_phase_image;
			break;
		}
		// 画像のアクセプト比を維持し、サイズをリサイズ
		_phase_image_obj.GetComponent< Image >( ).preserveAspect = true;
		_phase_image_obj.GetComponent< Image >( ).SetNativeSize( );

		// サイズを画面に合わせて調整し、位置を設定
		_phase_image_width  = Screen.width * 2 / 3;
		_phase_image_height = Screen.height / 3;
		_phase_image_obj.GetComponent< RectTransform >( ).SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, _phase_image_width );
		_phase_image_obj.GetComponent< RectTransform >( ).SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, _phase_image_height );
		_phase_image_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( -( Screen.width / 2 + _phase_image_width / 2 ), 0, 0 );
    }

	/// <summary>
	/// PhaseTextImageの移動
	/// </summary>
	public void movePhaseImage( ) {
		Vector2 pos = _phase_image_obj.GetComponent< RectTransform >( ).anchoredPosition;

		_phase_image_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector2( pos.x + _phase_image_move_speed, pos.y );

		if ( _phase_image_obj.GetComponent< RectTransform >( ).anchoredPosition.x > Screen.width ) {
			_phase_image_move_finish = true;
		}
	}

	public void setPhaseImagePos( ) {
		_phase_image_height = Screen.height / 6;
		_phase_image_obj.GetComponent< RectTransform >( ).SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, _phase_image_height );
		_phase_image_obj.GetComponent< RectTransform >( ).anchoredPosition = new Vector3( 0, ( Screen.height / 2 ) - _phase_image_height / 2, 0 );
	}

	public void deletePhaseImage( ) {
		Destroy( _phase_image_obj );
		_phase_image_move_finish = false;
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

	public bool isFinishMovePhaseImage( ) {
		return _phase_image_move_finish;
	}
}
