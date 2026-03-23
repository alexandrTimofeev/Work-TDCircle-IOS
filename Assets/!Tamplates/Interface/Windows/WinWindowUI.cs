using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WinWindowUI : WindowUI
{
    [Header("Win text")]
    [SerializeField] private TextMeshProUGUI textCurrentScore;
    [SerializeField] private TextMeshProUGUI textBestScore;
    [SerializeField] private string prefixTextScore = "Score: ";
    [SerializeField] private string prefixTextBestScore = "Best Score: ";

    [Header("Анимация счёта")]
    [SerializeField] private float countDuration = 1f;
    [SerializeField] private Ease countEase = Ease.OutCubic;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Punch End")]
    [SerializeField] private Vector3 punchEndScale = new Vector3(0.2f, 0.2f, 0);
    [SerializeField] private float punchEndDuration = 0.3f;
    [SerializeField] private int punchEndVibrato = 8;
    [SerializeField] private float punchEndElasticity = 1f;

    [SerializeField] private float finalPunchMultiplier = 0.5f; // для последнего удара

    public void Show(int currentScore, int bestScore)
    {
        textCurrentScore.text = "0";
        textBestScore.text = prefixTextBestScore + bestScore.ToString();
        textCurrentScore.color = normalColor;
        textCurrentScore.transform.localScale = Vector3.one;

        int displayedScore = 0;
        bool hasBeatenBest = false;

        DOTween.To(() => displayedScore, x => {
            displayedScore = x;
            textCurrentScore.text = prefixTextScore + displayedScore.ToString();

            if (!hasBeatenBest && displayedScore > bestScore)
            {
                hasBeatenBest = true;
                textCurrentScore.color = highlightColor;
                textCurrentScore.transform.DOPunchScale(punchEndScale, punchEndDuration, punchEndVibrato, punchEndElasticity);
            }
        }, currentScore, countDuration)
        .SetEase(countEase)
        .OnComplete(() =>
        {
            textCurrentScore.transform
                .DOPunchScale(punchEndScale * finalPunchMultiplier, punchEndDuration, punchEndVibrato, punchEndElasticity);
        });
    }
}