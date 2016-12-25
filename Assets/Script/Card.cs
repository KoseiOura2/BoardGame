using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    //初期位置座標
    private Vector2 initCardPosition;
    // マウス位置座標
    private Vector3 Mouseposition;
    // ビューポイント座標をキャンバスの座標に変換した位置座標
    private Vector3 ScreenPosition;
    //マウス位置座標をキャンバスの座標に変換した位置座標
    private Vector3 ViewportPosition;

    private SelectArea selectArea;

    //this is the ui element
    public RectTransform UI_Element;

    //first you need the RectTransform component of your canvas
    private RectTransform CanvasRect;

    // Use this for initialization
    void Awake () {
        //セレクトエリアの取得
        GameObject selectAreaObject = GameObject.FindGameObjectWithTag("SelectArea");
        selectArea = selectAreaObject.GetComponent<SelectArea>();
        //キャンバスのRectTransformの取得
        GameObject canvasObj = GameObject.Find("Canvas");
        CanvasRect = canvasObj.GetComponent<RectTransform>();
        //自身のRectTransfromの取得
        UI_Element = GetComponent<RectTransform>();
        //初期位置を取得
        initCardPosition = UI_Element.anchoredPosition;
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
    public void drag()
    {
        //マウスの位置へカードが移動
        UI_Element.anchoredPosition = ScreenPosition;
    }
    public void onPointUp()
    {
        bool Areain = selectArea.getSelectAreaFlag();
        if (Areain)
        {
            Debug.Log("エリア内にいます");
        }
        else {
            //特定の場所以外で離した場合は初期位置へ
            UI_Element.anchoredPosition = initCardPosition;
        }
    }
}
