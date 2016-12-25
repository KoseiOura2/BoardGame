using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class Move : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    // 位置座標
    private Vector3 position;
    // スクリーン座標をワールド座標に変換した位置座標
    private Vector3 screenToWorldPointPosition;

    RectTransform rectTransform = null;

    [SerializeField]
    Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponent<Graphic>().canvas;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Vector3でマウス位置座標を取得する
        position = Input.mousePosition;
        var pos = Vector2.zero;
        var uiCamera = Camera.main;
        var worldCamera = Camera.main;
        var canvasRect = canvas.GetComponent<RectTransform>();
        var screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out pos);
        rectTransform.localPosition = pos;
        //GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y, 0.0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}