using System;
using DG.Tweening;
using NMNH.Utility;
using UniRx;
using UnityEngine;

public class Stab : MonoBehaviour
{
    [SerializeField] private Transform leftStab;
    [SerializeField] private Transform rightStab;
    [SerializeField] private Transform upStab;
    [SerializeField] private Transform downStab;

    private bool isLeftTrigger;
    private bool isRightTrigger;
    private bool isUpTrigger;
    private bool isDownTrigger;

    public void TriggerOrgan(string dirStr)
    {
        Transform organ = null; 
        var organEndValue = Vector3.zero;
        var direction = Direction.None;
        switch (dirStr)
        {
            case "Left":
                if(isLeftTrigger) return;
                isLeftTrigger = true;
                organ = leftStab;
                organEndValue = Vector3.left;
                direction = Direction.Left;
                break;
            case "Right":
                if(isRightTrigger) return;
                isRightTrigger = false;
                organ = rightStab;
                organEndValue = Vector3.right;
                direction = Direction.Right;
                break;
            case "Up":
                if(isUpTrigger) return;
                isUpTrigger = true;
                organ = upStab;
                organEndValue = Vector3.up;
                direction = Direction.Up;
                break;
            case "Down":
                if(isDownTrigger) return;
                isDownTrigger = true;
                organ = downStab;
                organEndValue = Vector3.down;
                direction = Direction.Down;
                break;
        }

        if (organ != null)
        {
            AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Stab);
            Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
            {
                DOTween.Sequence()
                    .AppendInterval(0.15f)
                    .Append(organ.DOLocalMove(organEndValue, 0.15f).SetEase(Ease.OutCubic))
                    .AppendInterval(0.15f)
                    .Append(organ.DOLocalMove(Vector3.zero, 0.1f))
                    .SetUpdate(true)
                    .SetLink(gameObject)
                    .onComplete += () =>
                {
                    switch (direction)
                    {
                        case Direction.Left:
                            isLeftTrigger = false;
                            break;
                        case Direction.Right:
                            isRightTrigger = false;
                            break;
                        case Direction.Up:
                            isUpTrigger = false;
                            break;
                        case Direction.Down:
                            isDownTrigger = false;
                            break;
                    }
                };
            }).AddTo(this);
        }
    }
}
