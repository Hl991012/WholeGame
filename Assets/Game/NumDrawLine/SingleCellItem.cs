using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NumDrawLine
{
    public class SingleCellItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image bgImg;
        [SerializeField] private TextMeshProUGUI numTmp;
        [SerializeField] private SerializableDictionary<int, Color> numToColor;

        public Vector2 AnchorPos => rectTransform.anchoredPosition;
        
        private Action<PointerEventData, SingleCellItem> OnPointDown;
        private Action<PointerEventData, SingleCellItem> OnPointUp;
        private Action<PointerEventData, SingleCellItem> OnPointEnter;
        private Action<PointerEventData, SingleCellItem> OnPointExit;
        private Action<PointerEventData, SingleCellItem> OnPointClick;

        public CellDataModel CellDataModel { get; private set; }

        public void Register(CellDataModel cellDataModel, 
            Action<PointerEventData, SingleCellItem> onPointDown, 
            Action<PointerEventData, SingleCellItem> onPointUp,
            Action<PointerEventData, SingleCellItem> onPointEnter, 
            Action<PointerEventData, SingleCellItem> onPointExit,
            Action<PointerEventData, SingleCellItem> onPointClick)
        {
            CellDataModel = cellDataModel;
            
            OnPointDown = onPointDown;
            OnPointUp = onPointUp;
            OnPointEnter = onPointEnter;
            OnPointExit = onPointExit;
            OnPointClick = onPointClick;
            
            RefreshNum();
        }

        #region 一些Pointer事件

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointDown?.Invoke(eventData, this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointEnter?.Invoke(eventData, this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointUp?.Invoke(eventData, this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointExit?.Invoke(eventData, this);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointClick?.Invoke(eventData, this);
        }

        #endregion

        public void SetNum(int num)
        {
            CellDataModel.Num = num;
            RefreshNum();
        }

        private void RefreshNum()
        {
            numTmp.text = CellDataModel.Num.ToString();
            bgImg.color = numToColor.TryGetValue(CellDataModel.Num, out var value) ? value : Color.yellow;
        }

        #region 动画相关

        // 展示合成动画，其它元素向当前内容合并
        public void ShowMergeAnim()
        {
            DOTween.Sequence()
                .SetLink(gameObject)
                .SetUpdate(true)
                .SetEase(Ease.Linear)
                .Append(transform.DOLocalRotate(new Vector3(0, 0, 360), 0.4f, RotateMode.FastBeyond360))
                .Join(numTmp.DOFade(0, 0.2f))
                .Insert(0.2f, numTmp.DOFade(1, 0.2f));
        }
        
        // 展示被合成动画，其它元素向其它内容合并
        public void ShowBeMergeAnim(Vector3 moveToPos)
        {
            DOTween.Sequence()
                .SetLink(gameObject)
                .SetUpdate(true)
                .SetEase(Ease.Linear)
                .Append(bgImg.DOFade(0, 0.4f))
                .Join(numTmp.DOFade(0, 0.4f))
                .Join(transform.DOMove(moveToPos, 0.4f))
                .Join(transform.DOLocalRotate(new Vector3(0, 0, 360), 0.4f, RotateMode.FastBeyond360))
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }

        private Sequence chooseAnimSeq;
        public void ShowChooseAnim()
        {
            chooseAnimSeq?.Kill();
            chooseAnimSeq = DOTween.Sequence()
                .SetUpdate(true)
                .SetLink(gameObject)
                .Append(transform.DOScale(1.1f, 0.15f))
                .Append(transform.DOScale(1, 0.1f));
        }

        #endregion

        private Sequence moveAnimSeq;
        
        public void SetPos(Vector2 anchorPos, bool needAnim)
        {
            moveAnimSeq?.Kill();
            if (needAnim)
            {
                // var moveTime = Mathf.Abs(rectTransform.anchoredPosition.y - anchorPos.y) / 110f * 0.2f;
                moveAnimSeq = DOTween.Sequence()
                    .AppendInterval(0.4f)
                    .Append(rectTransform.DOAnchorPos(anchorPos, 0.2f)
                        .SetEase(Ease.Linear))
                    .SetLink(gameObject)
                    .SetUpdate(true);
            }
            else
            {
                rectTransform.anchoredPosition = anchorPos;
            }
        }
    }   
}
