using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class CellItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    public enum LineType
    {
        None,
        Line1,
        Line2,
        curLine,
    }
    
    [SerializeField] private RectTransform drawObj;
    [SerializeField] private Image defaultBg;
    [SerializeField] private Image firstItem;
    [SerializeField] private Image line1Image;
    [SerializeField] private Image line2Image;
    [SerializeField] private Image curLineImage;
    [SerializeField] private Image curCircleImage;
    [SerializeField] private Image circleImage;
    
    public Vector2 Position => transform.position;
    public Vector2Int CellPos { get; private set; }
    public bool IsPlayer { get; private set; }
    public bool IsCanUse { get; private set; }

    private bool isDrawing;
    public bool IsDrawing
    {
        get => !IsCanUse || isDrawing;
        private set => isDrawing = value;
    }
    
    private Action<PointerEventData, CellItem> OnBeginDragAction;
    private Action<PointerEventData, CellItem> OnDragAction;
    private Action<PointerEventData, CellItem> OnEndDragAction;
    private Action<PointerEventData, CellItem> OnPointDownAction;

    public void OnShow(string name, Vector2Int pos, bool isDrawing, bool isFirstItem, Color color)
    {
        CellPos = pos;
        IsCanUse = isDrawing;
        IsPlayer = false;
        gameObject.name = "Cell " + name;
        gameObject.SetActive(true);
        defaultBg.gameObject.SetActive(IsCanUse);
        firstItem.gameObject.SetActive(isFirstItem);
        defaultBg.color = color;

        var tempColor = color * 0.6f;
        tempColor.a = 1;
        line1Image.color = tempColor;
        line2Image.color = tempColor;
        curLineImage.color = tempColor;
        circleImage.color = tempColor;
        curCircleImage.color = tempColor;
        firstItem.color = tempColor;
    }

    public void RegisterEvent(Action<PointerEventData, CellItem> onBeginDrag,
        Action<PointerEventData, CellItem> onDrag,
        Action<PointerEventData, CellItem> onEdDrag,
        Action<PointerEventData, CellItem> onPointDown)
    {
        if (IsCanUse)
        {
            OnBeginDragAction = onBeginDrag;
            OnDragAction = onDrag;
            OnEndDragAction = onEdDrag;
            OnPointDownAction = onPointDown;
        }
    }

    public void ShowDraw(bool enable)
    {
        IsDrawing = enable;
        drawObj.gameObject.SetActive(IsDrawing);
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
    }

    public void ShowLine(float angle, LineType lineType)
    {
        switch (lineType)
        {
            case LineType.Line1:
                line1Image.gameObject.SetActive(true);
                line1Image.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                circleImage.gameObject.SetActive(line1Image.gameObject.activeSelf && line2Image.gameObject.activeSelf);
                break;
            case LineType.Line2:
                line2Image.gameObject.SetActive(true);
                line2Image.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                circleImage.gameObject.SetActive(line1Image.gameObject.activeSelf && line2Image.gameObject.activeSelf);
                break;
            case LineType.curLine:
                curLineImage.gameObject.SetActive(true);
                curLineImage.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                circleImage.gameObject.SetActive(false);
                break;
        }
    }

    public void HideAllLine()
    {
        line1Image.gameObject.SetActive(false);
        line2Image.gameObject.SetActive(false);
        curLineImage.gameObject.SetActive(false);
        circleImage.gameObject.SetActive(false);
    }
    
    public void SetPlayer(bool enable)
    {
        IsPlayer = enable;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragAction?.Invoke(eventData, this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragAction?.Invoke(eventData, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragAction?.Invoke(eventData, this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointDownAction?.Invoke(eventData, this);
    }
}
