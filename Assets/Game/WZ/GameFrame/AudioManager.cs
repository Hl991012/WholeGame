using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameFrame;

namespace NMNH.Utility
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public enum SoundEffectType
        {
            None,
            BGM,
            CommonClick, // 通用点击
            TouchWall,
            Win,
            Stab,
        }

        private Dictionary<string, AudioClip> seDict = new ();
        private bool isMute;
        private Queue<ContinuousAudioSource> continuousAudioSourcePool = new ();
        private Dictionary<string, ContinuousAudioSource> activeContinuousAudioSources = new ();
        private AudioSource oneShotAudioSource;
        private AudioSource bgmAudioSource;

        private void Awake()
        {
            oneShotAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource = gameObject.AddComponent<AudioSource>();

            for (var i = 0; i < 2; i++)
            {
                continuousAudioSourcePool.Enqueue(new ContinuousAudioSource()
                {
                    EndTime = -1,
                    AudioSource = gameObject.AddComponent<AudioSource>()
                });
            }
        }

        private void Update()
        {//可以优化改为0.1s?

            if (activeContinuousAudioSources.Count <= 0)
            {
                return;
            }

            List<string> needRemove = new List<string>();

            foreach (var item in activeContinuousAudioSources)
            {
                if (item.Value.EndTime >= 0 && item.Value.EndTime <= Time.time)
                {
                    item.Value.AudioSource.Stop();
                    continuousAudioSourcePool.Enqueue(item.Value);
                    needRemove.Add(item.Key);
                }
            }

            foreach (var item in needRemove)
            {
                activeContinuousAudioSources.Remove(item);
            }
        }

        public void PlayBGM()
        {
            bgmAudioSource.clip = GetAudioClip(SoundEffectType.BGM.ToString());
            bgmAudioSource.Play();
        }

        /// <summary>
        /// 播放一次, 不会打断已播放, 不能被手动停止
        /// </summary>
        public void PlayOneShot(SoundEffectType soundEffectType)
        {
            // if (!SettingManager.IsSoundEffectOpen) return;

            if (soundEffectType is SoundEffectType.None) return;

            var soundEffectName = soundEffectType.ToString();
            
            if (!seDict.ContainsKey(soundEffectName) || seDict[soundEffectName] == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"不能播放该音频, name: {soundEffectName}");
#endif
                return;
            }

            var clip = seDict[soundEffectName];
            oneShotAudioSource.PlayOneShot(clip, GetVolumeBySoundEffectType(soundEffectType));
        }

        // 声音大小在这里处理
        private float GetVolumeBySoundEffectType(SoundEffectType soundEffectType)
        {
            return soundEffectType switch
            {
                SoundEffectType.CommonClick => 1.0f,
                _ => 1f
            };
        }

        public void PlayContinuousSound(SoundEffectType effectType, bool loop = false)
        {
            PlayContinuousSound(effectType.ToString(), GetVolumeBySoundEffectType(effectType), loop);
        }

        /// <summary>
        /// 播放音频, 若再次播放同一音频则会打断上一次的播放
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        private void PlayContinuousSound(string name, float volume = 1f, bool loop = false)
        {
            // if (!SettingManager.IsSoundEffectOpen) return;

            if (name == null)
            {
                return;
            }
            if (isMute)
            {
                return;
            }
            if (!seDict.ContainsKey(name) || seDict[name] == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"不能播放该音频, name: {name}");
#endif
                return;
            }

            var clip = seDict[name];

            if (!activeContinuousAudioSources.ContainsKey(name))
            {
                activeContinuousAudioSources[name] = GetFormPool();
            }
            else if (activeContinuousAudioSources[name].HaveTween)
            {
                activeContinuousAudioSources[name].AudioSource.DOKill();
            }

            PlayClip(activeContinuousAudioSources[name], clip, volume, loop);
        }

        /// <summary>
        /// 外部音源统一播放入口，统一管理静音等逻辑
        /// </summary>
        /// <param name="audioSource"></param>
        public void PlayExternalAudioSource(AudioSource audioSource)
        {
            if (isMute)
            {
                return;
            }
            if (audioSource == null || audioSource.clip == null)
            {
                return;
            }
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.Play();
        }

        public bool IsPlaying(SoundEffectType effectType)
        {
            return IsPlaying(effectType.ToString());
        }

        private bool IsPlaying(string name)
        {
            return activeContinuousAudioSources.ContainsKey(name) && activeContinuousAudioSources[name].AudioSource.isPlaying;
        }

        public void StopContinuousSound(SoundEffectType effectType)
        {
            StopContinuousSound(effectType.ToString());
        }

        private void StopContinuousSound(string name)
        {
            if (!activeContinuousAudioSources.ContainsKey(name))
            {
                return;
            }
            activeContinuousAudioSources[name].AudioSource.Stop();
            continuousAudioSourcePool.Enqueue(activeContinuousAudioSources[name]);
            activeContinuousAudioSources.Remove(name);
        }

        public void FadeOutContinuousSound(string name, float duration)
        {
            if (!activeContinuousAudioSources.ContainsKey(name))
            {
                return;
            }
            activeContinuousAudioSources[name].HaveTween = true;
            activeContinuousAudioSources[name].AudioSource.DOFade(0, duration)
                .OnComplete(() =>
                {
                    if (activeContinuousAudioSources.ContainsKey(name))
                    {
                        activeContinuousAudioSources[name].HaveTween = false;
                    }
                    StopContinuousSound(name);
                });
        }

        public void StopAll()
        {
            foreach (var item in activeContinuousAudioSources)
            {
                item.Value.AudioSource.Stop();
                continuousAudioSourcePool.Enqueue(item.Value);
            }

            oneShotAudioSource.Stop();
            activeContinuousAudioSources.Clear();
        }

        private void PlayClip(ContinuousAudioSource singleAudioSource,
            AudioClip clip,
            float volume,
            bool loop)
        {
            // if (!SettingManager.IsSoundEffectOpen) return;

            singleAudioSource.AudioSource.clip = clip;
            singleAudioSource.AudioSource.volume = volume;
            singleAudioSource.AudioSource.loop = loop;
            singleAudioSource.EndTime = loop ? -1 : Time.time + clip.length;
            singleAudioSource.AudioSource.time = 0;
            singleAudioSource.AudioSource.Play();
        }

        private ContinuousAudioSource GetFormPool()
        {
            if (continuousAudioSourcePool.Count > 0)
            {
                return continuousAudioSourcePool.Dequeue();
            }

            return new ContinuousAudioSource()
            {
                EndTime = Time.time,
                AudioSource = gameObject.AddComponent<AudioSource>()
            };
        }

        private class ContinuousAudioSource
        {
            public float EndTime { get; set; }
            public AudioSource AudioSource { get; set; }
            public bool HaveTween { get; set; }
        }
        
        public void LoadAllAudioClip()
        {
            var allAudioClip = Resources.LoadAll<AudioClip>("Audio");
            foreach (var item in allAudioClip)
            {
                seDict[item.name] = item;
            }
        }

        public void SetMute(bool mute)
        {
            this.isMute = mute;
            if (mute)
            {
                StopAll();
            }
            oneShotAudioSource.mute = mute;
        }

        public AudioClip GetAudioClip(string name)
        {
            if (seDict.ContainsKey(name))
            {
                return seDict[name];
            }
            return null;
        }
    }
}

