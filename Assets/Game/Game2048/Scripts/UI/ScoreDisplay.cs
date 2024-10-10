using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    private TMP_Text text;

    public void UpdateScore(int score)
    {
        text ??= GetComponent<TMP_Text>();
        text.text = score.ToString();
    }
}
