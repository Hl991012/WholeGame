// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using DG.Tweening;
// using Game.Manager;
// using GameFrame;
//
// namespace Game.Sound
// {
//     public class SoundPresenter : MonoSingleton<SoundPresenter>
//     {
//         public enum BGMType
//         {
//             CampHunting,
//             Forest,
//         }
//
//         private AudioSource audioSource;
//
//         private AudioClip currentBGM;
//
//         AudioClip bgm;
//
//         Sequence seq;
//
//         private void Awake()
//         {
//             audioSource = gameObject.AddComponent<AudioSource>();
//             audioSource.volume = 1f;
//             audioSource.loop = true;
//         }
//
//         private void Start()
//         {
//             RefreshCurrentBGM();
//         }
//         
//
//
//         private void RefreshCurrentBGM()
//         {
//             if (currentBGM != SkinLoader.DefaultLoader.GetBGM())
//             {
//                 currentBGM = SkinLoader.DefaultLoader.GetBGM();
//             }
//         }
//
//         public void PlayBGM()
//         {
//             if (!SettingManager.IsMusicOpen || currentBGM == null)
//             {
//                 return;
//             }
//             audioSource.clip = currentBGM;
//             audioSource.Play();
//         }
//
//         public void AutoChangeBGM()
//         {
//             RefreshCurrentBGM();
//             ChangeBGM();
//         }
//
//         // public void ChangeBGM(BGMType type)
//         // {
//         //     var clip = type switch
//         //     {
//         //         BGMType.CampHunting => campFightingBGM,
//         //         BGMType.Forest => forestBGM
//         //     };
//         //     if (clip == audioSource.clip)
//         //     {
//         //         return;
//         //     }
//         //     currentBGM = clip;
//         //     if (!SettingManager.IsMusicOpen)
//         //     {
//         //         return;
//         //     }
//         //     if (!audioSource.isPlaying)
//         //     {
//         //         PlayBGM();
//         //     }
//         //
//         //     seq?.Kill();
//         //     seq = DOTween.Sequence()
//         //         .Append(audioSource.DOFade(0, 1f))
//         //         .AppendCallback(() =>
//         //         {
//         //             audioSource.clip = currentBGM;
//         //         })
//         //         .Append(audioSource.DOFade(1, 1f))
//         //         .SetUpdate(true);
//         // }
//
//         public void ChangeBGM()
//         {
//             if (currentBGM == audioSource.clip)
//             {
//                 return;
//             }
//             if (!SettingManager.IsMusicOpen)
//             {
//                 return;
//             }
//             if (!audioSource.isPlaying)
//             {
//                 PlayBGM();
//             }
//
//             seq?.Kill();
//             seq = DOTween.Sequence()
//                 .Append(audioSource.DOFade(0, 0.3f))
//                 .AppendCallback(() =>
//                 {
//                     audioSource.clip = currentBGM;
//                     audioSource.Play();
//                 })
//                 .Append(audioSource.DOFade(1, 0.3f))
//                 .SetUpdate(true)
//                 .SetLink(gameObject);
//         }
//
//         public void StopBGM()
//         {
//             audioSource.Stop();
//         }
//
//         public AudioSource MainBGMAudioSource()
//         {
//             return audioSource;
//         }
//     }
// }