using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public Slider progress;

    private float progressTime = 0;

    public Canvas ProgresCanvas;

    public Canvas CoinCanvas;

    public Canvas ErrorBoard;

    public Text ErrorMessage;

    private float spoint;

    private bool loadScene = false;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        if (Global.pointState == false)
        {
            string error = "잔액이 충분하지 않습니다. 지갑을 확인해주세요!";
            ErrorMessage.text = error;
            ErrorBoard.gameObject.SetActive(true);

        }
        else
        {
            StartCoroutine(get_gameInfo());
        }

    }
    // Update is called once per frame
    void Update()
    {
        if(loadScene == false)
        {
            return;
        }
        else
        {
            progressTime += Time.deltaTime;
            progress.value = (float)progressTime / 2;
            if (progressTime > 2)
            {
                SceneManager.LoadScene("GameScene");
                loadScene = false;
            }

        }
    }

    IEnumerator startGame()
    {
        //ProgresCanvas.gameObject.SetActive(true);
        //loadScene = true;
        string param = "{\"api-key\":\"" + Global.api_key + "\",\"mode\":\"" + Global.mode_startgame + "\",\"id\":\"" + Global.email + "\",\"point\":\"" + this.spoint+  "\"}";
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
                Debug.Log("start" + result);
                switch (result)
                {
                    case "Y":
                        ProgresCanvas.gameObject.SetActive(true);
                        loadScene = true;
                        //SceneManager.LoadScene("GameScene");
                        break;
                    case "N":
                        //ProgresCanvas.gameObject.SetActive(true);
                        //loadScene = true;
                        string error = "잔액이 충분하지 않습니다. 지갑을 확인해주세요!";
                        ErrorMessage.text = error;
                        ErrorBoard.gameObject.SetActive(true);
                        break;
                }
            }
        }
    }

    public void OkBtnClick()
    {
        
        StartCoroutine(startGame());
        //loadScene = true;
    }

    public void CloseBtnClick()
    {
        SceneManager.LoadScene("StartScene");
        //SceneManager.LoadScene("StartScene");
    }

    public void ErrorCloseBtn()
    {
        SceneManager.LoadScene("StartScene");
        //ErrorBoard.gameObject.SetActive(false);
        //CoinCanvas.gameObject.SetActive(true);
    }

    IEnumerator get_gameInfo()
    {
        //Global.time = 30;
        string param = "{\"api-key\":\"" + Global.api_key + "\",\"mode\":\"" + Global.mode_getinfo + "\"}";
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
                this.spoint = jsonNode["ginfo"][0]["spoint"].AsFloat;
                this.text.text = "고객님의 보유코인에서 "+ this.spoint + " 코인이 소모됩니다.";
                Debug.Log(this.text.text);
                CoinCanvas.gameObject.SetActive(true);
                Global.time = jsonNode["ginfo"][0]["playtime"].AsFloat;

                for (int i = 0; i < jsonNode["item"].Count; i++)
                {
                    string type = jsonNode["item"][i]["level"].ToString().Replace("\"", "");
                    int score = jsonNode["item"][i]["pt"].AsInt;
                    switch (type)
                    {
                        case "A":
                            Global.Gload_bar1 = score;
                            break;
                        case "B":
                            Global.Gload_bar2 = score;
                            break;
                        case "C":
                            Global.Emo1 = score;
                            break;
                        case "D":
                            Global.Emo2 = score;
                            break;
                        case "E":
                            Global.Emo3 = score;
                            break;
                        case "F":
                            Global.Diamond = score;
                            break;
                        case "G":
                            Global.Glod = score;
                            break;
                        case "H":
                            Global.Stone = score;
                            break;
                        case "I":
                            Global.Skeloton = score;
                            break;
                    }
                }
            }
        }

    }
}
