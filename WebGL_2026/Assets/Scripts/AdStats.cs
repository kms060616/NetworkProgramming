using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AdStats : MonoBehaviour
{
    public string dbUrl;                                   // Realtime Database 주소. 끝에 / 없이
    public static AdStats Instance;

    void Awake() { Instance = this; }

    public void Count(string kind)                         // "interstitial" 또는 "rewarded"
    {
        int n = PlayerPrefs.GetInt("ad_" + kind, 0) + 1;   // 기기에 누적
        PlayerPrefs.SetInt("ad_" + kind, n); PlayerPrefs.Save();
        if (AuthManager.Uid != "") StartCoroutine(Put(kind, n));
    }

    IEnumerator Put(string kind, int n)
    {
        string url = dbUrl + "/adstats/" + AuthManager.Uid + "/" + kind + ".json?auth=" + AuthManager.IdToken;
        UnityWebRequest req = UnityWebRequest.Put(url, n.ToString());         // 본문은 숫자 하나
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        Debug.Log(req.result == UnityWebRequest.Result.Success ? "adstats " + kind + " = " + n : "adstats failed " + req.error);
    }
}

