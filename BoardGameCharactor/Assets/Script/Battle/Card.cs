﻿using UnityEngine;
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

	private PlayerManager _player_Manager;

	[SerializeField]
	//自身のカードデータを取得
	private CARD_DATA ownCardData;

	//自身のイメージを取得
	private Image _cardImage;

	//カードの画像を取得
	private Sprite _cardSprite;

	//自身がセレクトエリアに入っているか
	private bool InSelectArea = false;

    // Use this for initialization
    void Awake () {
        //キャンバスのRectTransformの取得
        GameObject canvasObj = GameObject.Find("Canvas");
        CanvasRect = canvasObj.GetComponent<RectTransform>();
        //自身のRectTransfromの取得
		uI_Element = GetComponent<RectTransform>();

		//バトルマネージャーの取得
		if (_battle_Phase_Manager == null) {
			GameObject _battle_Manager_Obj = GameObject.Find ("BattlePhaseManager");
			if (_battle_Manager_Obj != null) {
				_battle_Phase_Manager = _battle_Manager_Obj.GetComponent<BattlePhaseManager> ();
			}
		}
		//プレイヤーマネージャーの取得
		if (_player_Manager == null) {
			GameObject _player_Manager_Obj = GameObject.Find ("PlayerManager");
			_player_Manager = _player_Manager_Obj.GetComponent<PlayerManager> ();
		}
		//自身のImageを読み込む
		_cardImage = GetComponent<Image>();

    }

	void Start(){
		//初期位置を取得
		initCardPosition = uI_Element.anchoredPosition;
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

	public void SetCardData( CARD_DATA setCard ) {
		ownCardData = setCard;
	}

    public void drag(){
		//バトルフェイズマネージャーでカードセレクトが始まっているなら動くように、存在しなければ動かさない
		if (_battle_Phase_Manager != null) {
			if (_battle_Phase_Manager.getCardSelectStart ()) {
				//マウスの位置へカードが移動
				uI_Element.anchoredPosition = ScreenPosition;
			}
		}
    }
    public void onPointUp(){
		//バトルフェイズマネージャーでカードセレクトが始まっているなら動くように、存在しなければ動かさない
		if (_battle_Phase_Manager != null) {
			if (_battle_Phase_Manager.getCardSelectStart ()) {
				//現在のマウスカーソルの場所を取得
				Vector3 mousePos = Input.mousePosition;

				//マウスカーソルの場所へ飛ばすRayの生成
				Ray ray = Camera.main.ScreenPointToRay (mousePos);
				RaycastHit hit = new RaycastHit ();

				//ヒットしたなら
				if (Physics.Raycast (ray, out hit)) { 
					//セレクトエリアに当たったなら特定の位置へ移動
					if (hit.collider.tag == "SelectArea") {
						//セレクトエリアに入ったかのフラグを取得
						InSelectArea = true;
						//セレクトエリアのポジションに移動できたかどうかを取得
						bool isSelectAreaCheck = _player_Manager.SetSelectAreaPosition( ownCardData );
						if (!isSelectAreaCheck) {
							//セレクトエリアに入れられなかったら初期位置へ
							uI_Element.anchoredPosition = initCardPosition;
							InSelectArea = false;
						}
					} else {
						//セレクトエリアから出たので自身を選択エリアから解除する
						_player_Manager.SetSelectAreaOut( ownCardData);
						//特定の場所以外で離した場合は初期位置へ
						uI_Element.anchoredPosition = initCardPosition;
					}
				} else {
					//セレクトエリアから出たので自身を選択エリアから解除する
					_player_Manager.SetSelectAreaOut( ownCardData);
					//特定の場所以外で離した場合は初期位置へ
					uI_Element.anchoredPosition = initCardPosition;
				}
			}
		}
    }

	//セレクトエリアに入ってるかどうかを取得
	public bool getInSelectArea(){
		return InSelectArea;
	}
}
