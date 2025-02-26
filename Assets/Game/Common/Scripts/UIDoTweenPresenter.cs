using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Game.Scripts.Component
{
    public class UIDoTweenPresenter: MonoBehaviour
    {
        [Serializable]
        public class CustomAnimationModel
        {
            public enum AnimationType
            {
                Move,
                Rotate,
                Scale,
            }
            
            public enum DealType
            {
                Append,
                AppendInterval,
                Insert,
            }

            public AnimationType animationType;

            public Vector3 v;

            public float duration;

            public float ts;

            public DealType dealType;

            public RotateMode rotateMode;

            public Ease ease = DOTween.defaultEaseType;
        }
        
        /// <summary>
        /// 是否无限循环
        /// </summary>
        [SerializeField] private int loopTimes;

        [SerializeField] private float delay;

        [SerializeField] private CustomAnimationModel[] animationModels =  {};

        private RectTransform rectTransform;
        
        private Vector2 oldPosition;
        private Vector3 oldRotationAngle;
        private Vector3 oldScale;

        private Sequence seq; 

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            oldPosition = new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y);
            oldRotationAngle = rectTransform.localRotation.eulerAngles;
            oldScale = rectTransform.localScale;

            if (delay <= 0)
            {
                Play();
            }
            else
            {
                Observable.Timer(TimeSpan.FromSeconds(delay)).First().Subscribe((a) =>
                {
                    Play();
                }).AddTo(this);
            }
        }
        
        private void Play()
        {
            Clear();
            seq = DOTween.Sequence();
            
            Tween tmpTween = null; 
            foreach (var customAnimationModel in animationModels)
            {
                if (customAnimationModel.dealType == CustomAnimationModel.DealType.AppendInterval)
                {
                    seq.AppendInterval(customAnimationModel.duration);
                    continue;
                }
                
                switch (customAnimationModel.animationType)
                {
                    case CustomAnimationModel.AnimationType.Move:
                        tmpTween = rectTransform.DOLocalMove(oldPosition + (Vector2)customAnimationModel.v, customAnimationModel.duration);
                        break;
                    case CustomAnimationModel.AnimationType.Rotate:
                        tmpTween = rectTransform.DOLocalRotate(oldRotationAngle + (Vector3)customAnimationModel.v, customAnimationModel.duration, customAnimationModel.rotateMode);
                        break;
                    case CustomAnimationModel.AnimationType.Scale:
                        tmpTween = rectTransform.DOScale(oldScale + (Vector3)customAnimationModel.v, customAnimationModel.duration);
                        break;
                }

                tmpTween.SetEase(customAnimationModel.ease);

                switch (customAnimationModel.dealType)
                {
                    case CustomAnimationModel.DealType.Append:
                        seq.Append(tmpTween);
                        break;
                    case CustomAnimationModel.DealType.Insert:
                        seq.Insert(customAnimationModel.ts, tmpTween);
                        break;
                }
            }
            
            seq.SetLoops(loopTimes);
            seq.SetLink(gameObject);
            seq.Play();
        }

        private void Stop()
        {
            Clear();
        }

        public void Clear()
        {
            seq?.Kill();
            seq = null;
        }
        
        public void Pause()
        {
            seq?.Pause();
        }
        
        public void RePlay()
        {
            seq?.Restart();
        }

        public void Reset()
        {
            if (rectTransform == null) return;
            rectTransform.localPosition = oldPosition;
            rectTransform.localRotation = Quaternion.Euler(oldRotationAngle);
            rectTransform.localScale = oldScale;
        }
    }
}