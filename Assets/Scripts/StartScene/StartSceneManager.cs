using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Data.Common;
using UnityEngine.Networking;
using SimpleJSON;
using System;

public enum BtnID
{
    WATCHREPLAY,
    HELP,
    STARTGAME,
    OPTION,
    RANK,
    NOTICE,
    OPTIONOK,
    OPTIONEXIT,
    LOGOUT,
    BACKSOUNDPLUS,
    BACKSOUNDMINUS,
    EFFECTSOUNDPLUS,
    EFFECTSOUNDMINUS
}

public class StartSceneManager : MonoBehaviour
{
    public Transform mOptionMenu;
    public Transform RankCanvas;
    public Transform NoticeCanvas;
    public GameObject rank;
    public GameObject notice;

    public Sprite[] sp;
    public Image[] img;

    public AudioSource backSound, effectSound;
    public AudioClip back, buttonClick;


    public Slider backM, effectM;

    public int curr;

    public int noticePangeNum = 0;

    public int noticeCnt = 0;

    public int rankCnt = 0;

    public static StartSceneManager instance = null;
    public static StartSceneManager GetInstance()
    {
        if (instance == null) instance = new StartSceneManager();
        return instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Global.pointState = true;
        instance = this;
        noticePangeNum = 0;
        backM.value = backSound.volume = PlayerPrefs.GetFloat("backSound", 0.3f);
        effectM.value = effectSound.volume = PlayerPrefs.GetFloat("effectSound", 0.3f);
        backSound.clip = back;
        backSound.loop = true;
        backSound.Play();
        StartCoroutine(get_ranking());
        StartCoroutine(get_notice());
        //StartCoroutine(get_gameInfo());
    }

    public void showNotice(int c)
    {
        int tmp = 0;
        tmp = c;
        if(c < 0)
        {
            tmp = 0;
            this.noticePangeNum = 0;
        }
        if (c >= this.noticeCnt)
        {
            tmp = this.noticeCnt - 1;
            this.noticePangeNum = this.noticeCnt-1;
        }
        Notices[] all = FindObjectsOfType<Notices>();
        foreach (Notices a in all) Destroy(a.gameObject);
        AddNotice(Global.notices[tmp].title, Global.notices[tmp].contents, Global.notices[tmp].date);
    }
        

    public void Add(int id, string name, int score)
    {
        id=((id-1)%10+10)% 10 + 1;
        Transform newObj = Instantiate(rank, Vector3.zero, Quaternion.identity).transform;
        newObj.SetParent(RankCanvas);
        newObj.localScale = new Vector3(1, 1, 1);
        RectTransform newTrans = newObj.gameObject.GetComponent<RectTransform>();
        newTrans.offsetMax = Vector2.zero;
        newTrans.offsetMin = Vector2.zero;
        if (id > 4 && id < 9)
        {
            //newTrans.anchoredPosition = new Vector2(0, -120 * (id-4 - 1));
        }
        //else
        {
            newTrans.anchoredPosition = new Vector2(0, -98 * (id - 1));
        }
        newObj.gameObject.GetComponent<Ranking>().SetText(id, name, score);
    }
    public void AddNotice(string title, string contents, string date)
    {
        Transform newObj = Instantiate(notice, Vector3.zero, Quaternion.identity).transform;
        newObj.SetParent(NoticeCanvas);
        newObj.localScale = new Vector3(1, 1, 1);
        RectTransform newTrans = newObj.gameObject.GetComponent<RectTransform>();
        newTrans.offsetMax = Vector2.zero;
        newTrans.offsetMin = Vector2.zero;
        newTrans.anchoredPosition = new Vector2(0, -98);
        newObj.gameObject.GetComponent<Notices>().SetText(title, contents, date);
    }

    public void LeftBtnClick()
    {
        this.noticePangeNum--;
        showNotice(this.noticePangeNum);
        
    }

    public void RightBtnClick()
    {
        this.noticePangeNum++;

        showNotice(this.noticePangeNum);
    }
    
    IEnumerator get_ranking()
    {
        DateTime time = DateTime.Now;
        string date = time.Year.ToString() + "-" + time.Month.ToString() + "-" + time.Day.ToString();
        string param = "{\"api-key\":\"" + Global.api_key + "\",\"mode\":\"" + Global.mode_getrank + "\",\"chk-date\":\"" + date + "\"}";
        byte[] myData = System.Text.Encoding.UTF8.GetBytes(param);
        using (UnityWebRequest request = UnityWebRequest.Put(Global.api_domain, myData))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                JSONNode jsonNode = SimpleJSON.JSON.Parse(request.downloadHandler.text);
                Debug.Log("Ranking:" + request.downloadHandler.text);
                if(jsonNode != null && jsonNode["rank"] != null)
                {
                    rankCnt = jsonNode["rank"].Count;
                    if (jsonNode["rank"].Count > 5)
                    {
                        rankCnt = 5;
                    }
                    for (int i = 0; i < this.rankCnt; i++)
                    {
                        string mem_name = jsonNode["rank"][i]["mem_name"].ToString().Replace("\"", "");
                        int g_point = jsonNode["rank"][i]["g_point"].AsInt;
                        Global.ranks.Add(new Global.Global_Rank(mem_name, g_point));
                    }
                }
                show();
            }
        }
    }

    IEnumerator get_notice()
    {
        string param = "{\"api-key\":\"" + Global.api_key + "\",\"mode\":\"" + Global.mode_getnotice + "\"}";
        byte[] myData = System.Text.Encoding.UTF8.GetBytes(param);
        using (UnityWebRequest request = UnityWebRequest.Put(Global.api_domain, myData))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                JSONNode jsonNode = SimpleJSON.JSON.Parse(request.downloadHandler.text);
                Debug.Log("Notice:" + request.downloadHandler.text);
                this.noticeCnt = jsonNode["notice"].Count;
                if (jsonNode["notice"].Count > 10)
                {
                    this.noticeCnt = 10;
                }
                for (int i = 0; i < this.noticeCnt; i++)
                {
                    string title = jsonNode["notice"][i]["title"].ToString().Replace("\"", "");
                    string contents = jsonNode["notice"][i]["contents"];
                    Debug.Log("---------" + contents);
                    string date = jsonNode["notice"][i]["regdate"].ToString().Replace("\"", "");
                    Global.notices.Add(new Global.Global_Notice(title, contents, date));
                }
                showNotice(noticePangeNum);
            }
        }
    }

    public void show()
    {
        Ranking[] all = FindObjectsOfType<Ranking>();
        foreach (Ranking a in all) Destroy(a.gameObject);

        //Global.notices.
        for(int i = 0; i < this.rankCnt; i++)
        {
            Add(i+1, Global.ranks[i].mem_name, Global.ranks[i].g_point);
        }
        
    }

    // show loading scene
    public Sprite[] loading;
    AsyncOperation cur;
    public Image loadimg;
    IEnumerator Loading()
    {
        int val = 0;
        while (cur.progress<1-1e-5)
        {
            if(val >= 3)
            {
                loadimg.sprite = loading[3];

            }
            else
            {

                loadimg.sprite = loading[val++];
            }
            //val %= 3;
            yield return new WaitForSeconds(0.3f);
        }
    }
    IEnumerator startGame()
    {
        string param = "{\"api-key\":\"" + Global.api_key + "\",\"mode\":\"" + Global.mode_startgame + "\",\"pw\":\"" + Global.email + "\"}";
        byte[] myData = System.Text.Encoding.UTF8.GetBytes(param);
        using (UnityWebRequest request = UnityWebRequest.Put(Global.api_domain, myData))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                JSONNode jsonNode = SimpleJSON.JSON.Parse(request.downloadHandler.text);
                string result = jsonNode["stat"].ToString().Replace("\"", "");
                Debug.Log("start"+result);
                switch (result)
                {
                    case "Y":
                        SceneManager.LoadScene("GameScene");
                        break;
                    case "N":
                        string error = "코인부족";
                        //SceneManager.LoadScene("GameScene");
                        break;
                }
            }
        }
    }
    
    public void OnBtnClick(BtnID id)
    {
        effectSound.clip = buttonClick;
        effectSound.Play();
        if (id == BtnID.STARTGAME)
        {
            //StartCoroutine(startGame());
            SceneManager.LoadScene("LoadingScene");
        }
        if (id == BtnID.OPTION)
        {
            mOptionMenu.gameObject.SetActive(true);
        }
        if (id==BtnID.OPTIONOK || id==BtnID.OPTIONEXIT)
        {
            mOptionMenu.gameObject.SetActive(false);
        }
        if (id==BtnID.RANK)
        {
            RankCanvas.gameObject.SetActive(true);
        }
        if (id==BtnID.NOTICE)
        {
            NoticeCanvas.gameObject.SetActive(true);
        }
        if(id == BtnID.LOGOUT)
        {
            SceneManager.LoadScene("LogScene");
        }
        if(id == BtnID.BACKSOUNDPLUS)
        {
            backM.value += 0.1f;
            backSound.volume += 0.1f;
            PlayerPrefs.SetFloat("backSound", backM.value);
        }
        if (id == BtnID.BACKSOUNDMINUS)
        {
            backM.value -= 0.1f;
            backSound.volume -= 0.1f;
            PlayerPrefs.SetFloat("backSound", backM.value);
        }
        if (id == BtnID.EFFECTSOUNDPLUS)
        {
            effectM.value += 0.1f;
            effectSound.volume += 0.1f;
            PlayerPrefs.SetFloat("effectSound", backM.value);
        }
        if (id == BtnID.EFFECTSOUNDMINUS)
        {
            effectM.value -= 0.1f;
            effectSound.volume -= 0.1f;
            PlayerPrefs.SetFloat("effectSound", backM.value);
        }

    }

    public void OnBackVolumnChange()
    {
        backSound.volume = backM.value;
        PlayerPrefs.SetFloat("backSound", backM.value);
    }

    public void OnEffectVolumnChange()
    {
        effectSound.volume = effectM.value;
        PlayerPrefs.SetFloat("effectSound", effectM.value);
    }
}
