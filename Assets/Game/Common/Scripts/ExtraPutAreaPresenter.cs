using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExtraPutAreaPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool InArea { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InArea = true;
        Debug.LogError("进入");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InArea = false;
        Debug.LogError("出去");
    }
}
