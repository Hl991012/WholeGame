using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(CanvasScaler))]
[DisallowMultipleComponent]
public abstract class BaseDialog : MonoBehaviour
{
    [SerializeField] protected Button closeButton;
    [SerializeField] protected RectTransform bgImageRect;
    private CanvasGroup canvasGroup;
    // protected Canvas canvas;
    private bool isShowing;

    private Sequence show;
    private Sequence dismiss;

    private bool hasBeDestroyed = false;

    public bool IsShowing => isShowing;

    public bool HasBeDestroyed => hasBeDestroyed;

    protected virtual void Awake()
    {
        hasBeDestroyed = false;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        bgImageRect.gameObject.SetActive(false);
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() =>
            {
                Dismiss();
            });
        }

        // canvas = transform.GetComponent<Canvas>();
        // canvas.worldCamera ??= Camera.main;
    }

    public abstract void Show();
    public abstract void Dismiss();
    protected abstract void OnShow();
    protected abstract void OnDismiss();

    protected void ShowImmediately()
    {
        if (isShowing)
        {
            return;
        }
        isShowing = true;
        dismiss?.Kill();
        canvasGroup.alpha = 1;
        bgImageRect.localScale = Vector3.one;
        bgImageRect.gameObject.SetActive(true);
        OnShow();
    }
    
    protected void ShowWithFade()
    {
        if (isShowing)
        {
            return;
        }
        isShowing = true;
        dismiss?.Kill();
        canvasGroup.alpha = 0;
        bgImageRect.localScale = Vector3.one;
        bgImageRect.gameObject.SetActive(true);
        show = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1f, 0.2f)).SetUpdate(true).SetLink(gameObject);
        OnShow();
    }

    protected void ShowWithScale()
    {
        if (isShowing)
        {
            return;
        }
        isShowing = true;
        dismiss?.Kill();
        canvasGroup.alpha = 0;
        bgImageRect.localScale = new Vector3(1.01f, 1.01f, 1.01f);
        bgImageRect.gameObject.SetActive(true);
        bgImageRect.localScale = Vector3.zero;
        show = DOTween.Sequence()
            .Append(bgImageRect.DOScale(1.02f, 0.05f).SetEase(Ease.Linear))
            .Append(bgImageRect.DOScale(0.99f, 0.1f).SetEase(Ease.Linear))
            .Append(bgImageRect.DOScale(1f, 0.05f).SetEase(Ease.Linear))
            .Insert(0,canvasGroup.DOFade(1f, 0.02f).SetEase(Ease.Linear))
            .SetLink(gameObject)
            .SetUpdate(true);
        OnShow();
    }

    protected void DismissWithDestroy()
    {
        if (!isShowing)
        {
            return;
        }
        isShowing = false;
        hasBeDestroyed = true;
        show?.Kill();
        bgImageRect.gameObject.SetActive(false);
        Addressables.ReleaseInstance(gameObject);
        OnDismiss();
    }

    protected void DismissWithFade()
    {
        if (!isShowing)
        {
            return;
        }
        show?.Kill();
        isShowing = false;
        dismiss = DOTween.Sequence()
            .Append(canvasGroup.DOFade(0, 0.1f))
            .AppendCallback(() => bgImageRect.gameObject.SetActive(false))
            .SetLink(gameObject)
            .SetUpdate(true);
        OnDismiss();
    }

    protected void DismissWithScale()
    {
        if (!isShowing)
        {
            return;
        }
        show?.Kill();
        isShowing = false;
        dismiss = DOTween.Sequence()
            .Append(bgImageRect.DOScale(0, 0.1f))
            .Join(canvasGroup.DOFade(0, 0.1f))
            .AppendCallback(() => bgImageRect.gameObject.SetActive(false))
            .SetLink(gameObject)
            .SetUpdate(true);
        OnDismiss();
    }

    protected void DismissImmediately()
    {
        if (!isShowing)
        {
            return;
        }
        show?.Kill();
        isShowing = false;
        canvasGroup.alpha = 0;
        bgImageRect.gameObject.SetActive(false);
        OnDismiss();
    }
}