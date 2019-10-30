using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Highscore : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // ハイスコアを表示する
    public TextMeshProUGUI highScoreText;

    // スコア
    private int score;

    // ハイスコア
    private int highScore;

    // PlayerPrefsで保存するためのキー
    private string highScoreKey = "highScore";

    void Start()
    {
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        //保存しておいたハイスコアをキーで呼び出し取得し保存されていなければ0になる
        highScoreText.text = highScore.ToString();
        //ハイスコアを表示
    }

    void Update()
    {
        score = int.Parse(scoreText.text);
        // スコアがハイスコアより大きければ
        if (highScore < score)
        {
            highScore = score;
            PlayerPrefs.SetInt(highScoreKey, highScore);
            PlayerPrefs.Save();
            highScoreText.text = highScore.ToString();
        }
    }
}
