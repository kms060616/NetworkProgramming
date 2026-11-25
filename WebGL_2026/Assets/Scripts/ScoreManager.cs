using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public int score;
    public GameObject resultPanel;
    public TMP_Text scoreText;
    public TMP_Text bestText;
    public TMP_Text hudText;
    public GameObject startPanel;

    void Start()                          // 추가
    {
        Time.timeScale = 0f;
        startPanel.SetActive(true);
    }

    public void StartGame()               // 추가
    {
        startPanel.SetActive(false);
        Time.timeScale = 1f;
    }


    public GameObject newBestText;

    public void AddScore(int amount)
    {
        score = Mathf.Max(0, score + amount);
        hudText.text = score.ToString();
    }

    public void GameOver()
    {
        string key = "Best_" + SceneManager.GetActiveScene().name;
        int best = PlayerPrefs.GetInt(key, 0);

        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt(key, best);

            PlayerPrefs.Save();
            newBestText.SetActive(true);
            ScoreSaver.Instance.Save(best);
        }
        scoreText.text = "SCORE " + score;
        bestText.text = "BEST " + best;
        resultPanel.SetActive(true);
        Time.timeScale = 0f;

        resultPanel.SetActive(true);                                       // GameOver 안에 원래 있던 줄
        doubleButton.SetActive(DummyAds.Instance.IsRewardedReady());

    }

    static int playCount;
    static float lastAdTime = -999f;                     // 마지막 전면 광고를 띄운 시각
    public float adCooldown = 30f;                       // 전면 광고 사이의 최소 간격 (초)

    public void Restart()                                // 실습 2 의 Restart 를 이렇게 교체
    {
        playCount++;
        bool enoughPlays = playCount >= 2;                                       // 첫 판 뒤에는 띄우지 않음
        bool enoughTime = Time.realtimeSinceStartup - lastAdTime > adCooldown;   // 마지막 광고 뒤 일정 시간
        if (enoughPlays && enoughTime && DummyAds.Instance.IsInterstitialReady())
        {
            lastAdTime = Time.realtimeSinceStartup;
            AdStats.Instance.Count("interstitial");          // Restart 의 ShowInterstitial 바로 위에 추가
            

            DummyAds.Instance.ShowInterstitial(Reload);
        }
        else Reload();
    }


    void Reload()                                        // 기존 Restart 의 내용을 여기로
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public GameObject doubleButton;                      // 결과 화면의 '광고 보고 점수 2배' 버튼

    public void WatchAdForDouble()                       // 버튼의 On Click
    {
        DummyAds.Instance.ShowRewarded(OnRewardClosed);
    }

    void OnRewardClosed(bool earned)                     // 광고가 닫히면 DummyAds 가 부름
    {
        if (!earned) return;
        AdStats.Instance.Count("rewarded");                  // OnRewardClosed 의 score *= 2; 바로 위에 추가
        
        
        score *= 2;
        GameOver();                                      // 최고 점수 비교, 표시, 서버 저장을 다시
        doubleButton.SetActive(false);                   // 한 판에 한 번만
    }


}
