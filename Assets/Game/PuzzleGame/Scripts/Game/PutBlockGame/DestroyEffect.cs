using System;
using System.Collections.Generic;
using PuzzleGame.Themes;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    public int coordinate;
    
    #region 高亮特效

    [SerializeField] private ParticleSystem highLightEffect;
    [SerializeField] private List<ParticleSystem> needSetColorHighLightEffects;

    private bool isShowHighLightEffect = false;
    
    public void SetHighLightEffectVisible(bool visible, int colorIndex = -1)
    {
        if (visible == isShowHighLightEffect)
        {
            return;
        }

        isShowHighLightEffect = visible;
        
        highLightEffect.gameObject.SetActive(visible);
        destroyEffect.gameObject.SetActive(false);
        if (visible)
        {
            foreach (var item in needSetColorHighLightEffects)
            {
                var tempMain = item.main;
                tempMain.startColor = new ParticleSystem.MinMaxGradient()
                {
                    color = ThemeController.Instance.CurrentTheme.GetColor(ColorType.BrickSprite, colorIndex),
                    colorMax = ThemeController.Instance.CurrentTheme.GetColor(ColorType.BrickSprite, colorIndex),
                    colorMin = ThemeController.Instance.CurrentTheme.GetColor(ColorType.BrickSprite, colorIndex)
                };
            }
            highLightEffect.Play();   
        }
    }

    #endregion

    #region 销毁特效

    [SerializeField] private ParticleSystem destroyEffect;
    [SerializeField] private List<ParticleSystem> needSetColorDestroyEffects;
    
    public void ShowDestroyEffect(int colorIndex)
    {
        highLightEffect.gameObject.SetActive(false);
        destroyEffect.gameObject.SetActive(true);
        foreach (var item in needSetColorDestroyEffects)
        {
            var tempMain = item.main;
            tempMain.startColor = new ParticleSystem.MinMaxGradient()
            {
                color = ThemeController.Instance.CurrentTheme.GetColor(ColorType.BrickSprite, colorIndex),
                colorMax = ThemeController.Instance.CurrentTheme.GetColor(ColorType.BrickSprite, colorIndex),
                colorMin = ThemeController.Instance.CurrentTheme.GetColor(ColorType.BrickSprite, colorIndex)
            };
        }
        destroyEffect.Play();
    }

    #endregion
}
