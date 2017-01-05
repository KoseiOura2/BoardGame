using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common;

public class Card : MonoBehaviour {
    //初期位置座標
    private Vector2 initCardPosition;

    // マウス位置座標
    private Vector3 Mouseposition;

    // ビューポイント座標をキャンバスの座標に変換した位置座標
    private Vector3 ScreenPosition;

    //マウス位置座標をキャンバスの座標に変換した位置座標
    private Vector3 ViewportPosition;

    //RectTransfromの取得
    private RectTransform uI_Element;

    //キャンバスのRectTransfromを取得
    private RectTransform CanvasRect;

	//BattleManagerの取得
	private BattlePhaseManager _battle_Phase_Manager;

	//自身のイメージを取得
	private Image _cardImage;
	//カードの画像を取得
	private Sprite _cardSprite;

    // Use this for initialization
    void Awake () {
        //キャンバスのRectTransformの取得
        GameObject canvasObj = GameObject.Find("Canvas");
        CanvasRect = canvasObj.GetComponent<RectTransform>();
        //自身のRectTransfromの取得
		uI_Element = GetComponent<RectTransform>();

		//バトルマネージャーの取得
		GameObject _battle_Manager_Obj = GameObject.Find("BattlePhaseManager");
		_battle_Phase_Manager = _battle_Manager_Obj.GetComponent<BattlePhaseManager>();

        //初期位置を取得
		initCardPosition = uI_Element.anchoredPosition;
		//自身のImageを読み込む
		_cardImage = GetComponent<Image>();

    }
	
	// Update is called once per frame
	void Update () {
        // Vector3でマウス位置座標を取得する
        Mouseposition = Input.mousePosition;
        // マウス位置座標をスクリーン座標からワールド座標に変換する
        ViewportPosition = Camera.main.ScreenToViewportPoint(Mouseposition);
        //　ビューポイント座標をキャンバス座標に変換を行う
        ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

    }

	public void SetCardImage( CARD_TYPE cardType ) {
		switch( cardType ){
		case CARD_TYPE.CARD_TYPE_ONCE_ENHANCE:
			_cardSprite = Resources.Load<Sprite> ("Graphec/Sprite/Cards/card_dagger");
			break;
		case CARD_TYPE.CARD_TYPE_ONCE_WEAKEN:
			_cardSprite = Resources.Load<Sprite> ("Graphec/Sprite/Cards/card_drug");
			break;
		case CARD_TYPE.CARD_TYPE_CONTUNU_ENHANCE:
			_cardSprite = Resources.Load<Sprite> ("Graphec/Sprite/Cards/card_hat");
			break;
		case CARD_TYPE.CARD_TYPE_UNAVAILABLE:
			_cardSprite = Resources.Load<Sprite> ("Graphec/Sprite/Cards/card_sword");
			break;
		case CARD_TYPE.CARD_TYPE_INSURANCE:
			_cardSprite = Resources.Load<Sprite> ("Graphec/Sprite/Cards/card_Boots");
			break;
		}
		_cardImage.sprite = _cardSprite;
	}

    public void drag()
    {
		//バトルフェイズマネージャーが効果選択フェイズなら動くように、存在しなければ動かさない
		if (_battle_Phase_Manager != null) {
			if (_battle_Phase_Manager.GetMainGamePhase () == MAIN_GAME_PHASE.GAME_PHASE_ASSIGNMENT_BUFF) {
				//マウスの位置へカードが移動
				uI_Element.anchoredPosition = ScreenPosition;
			}
		}
    }
    public void onPointUp(){
        //現在のマウスカーソルの場所を取得
        Vector3 mousePos = Input.mousePosition;

        //マウスカーソルの場所へ飛ばすRayの生成
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit = new RaycastHit();

        //ヒットしたなら
        if (Physics.Raycast(ray, out hit)) { 
            //セレクトエリアに当たったなら特定の位置へ移動
            if (hit.collider.tag == "SelectArea"){
                Debug.Log("エリア内にいます");
            } else {
                //特定の場所以外で離した場合は初期位置へ
                uI_Element.anchoredPosition = initCardPosition;
            }
        } else {
            //特定の場所以外で離した場合は初期位置へ
            uI_Element.anchoredPosition = initCardPosition;
        }
    }
}
