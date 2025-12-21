using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        StartCoroutine(UpdateScoreRoutine());
    }

    IEnumerator UpdateScoreRoutine()
    {
        while (true)
        {
            scoreText.text = GameplayManager.Instance.GetScore().ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
