using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net;
using SimpleJSON;
using UnityEngine.Networking;

public class MineralGameManager : MonoBehaviour
{
    // count continous hit
    public int conHit;

    // tools
    public Transform tools;

    // current selected tool object
    public Transform curSelTool;

    // Is the game paused?
    internal bool isPaused = true;

    // Is the game over?
    internal bool isGameover = false;

    //How many seconds are left before game over
    public float timeLeft = /*Global.time*/30f;

    // maximum time limit
    public float maximumTimeLimit = /*Global.time*/30f;

    // How long to wait before starting the game.
    internal float startDelay = 1.5f;

    // How long to wait before showing the targets
    public float showDelay = 6.0f;
    internal float showDelayCount = 0;

    // How many targets to show at once
    public int maximumTargets = 5;

    // How long to wait before hiding the targets again
    public float hideDelay = 0.5f;
    internal float hideDelayCount = 0;

    // A list of positions where the targets can appear
    public Transform[] targetPositions;
    public bool[] vstPos;

    // A slider to show left time.
    public Slider ShowTimeCount;

    // mineral object
    public GameObject targetMineral;

    // tool number is selected
    public int curToolId = 1;

    // tool sprites
    public Sprite[] toolSprites;

    // best score
    public Text bestScore;

    // current score
    public Text curScore, curScore1;

    // current coin
    public Text curCoin, curCoin1;

    // pickaxe
    public Image pickaxeCursor;

    // game over canvas
    public Transform GameoverCanvas;

    // result canvas
    public Transform ResultCanvas;

    // menu canvas
    public GameObject MenuCanvas;

    // option canvas
    public GameObject OptionCanvas;

    // real value of score, top score and coin
    public float _score, _tscore, _coin;

    // sound data
    public AudioSource backSound, effectSound;
    public AudioClip back, buttonClick, gameOverSound, pickaxeDown;

    public int curLevel;

    public static MineralGameManager instance = null;

    public int leftMineralCnt = 0;

    public Slider backSoundSlider, effectSoundSlider;

    public Text ErrorTxt;

    public Transform ErroCanvas;

    // p
    //public Sprite[] sprites;

    public static MineralGameManager GetInstance()
    {
        if (instance == null) instance = new MineralGameManager();
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        isPaused = true;
        timeLeft = 30f;
        maximumTimeLimit = 30f;
        showDelayCount = 0;
        _score = 0;
        _tscore = PlayerPrefs.GetFloat("top_score", 0);
        _coin = PlayerPrefs.GetFloat("coin", 0);
        ShowScore();
        UpdateTime();
        backSoundSlider.value = backSound.volume = PlayerPrefs.GetFloat("backSound", 0.3f);
        effectSoundSlider.value = effectSound.volume = PlayerPrefs.GetFloat("effectSound", 0.3f);
        backSound.clip = back;
        backSound.loop = true;
        backSound.Play();
        curLevel = 1;
        curToolId = 1;
        isPaused = false;
        conHit = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("------------time" + Global.time);
        timeLeft = timeLeft > maximumTimeLimit ? maximumTimeLimit : timeLeft;
        if (isPaused) return;
        // Delay the start of the game
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        //If the game is over, listen for MainMenu buttons
        if (isGameover)
        {
            return;
        }

        // Count down the time until game over
        if (timeLeft > 0)
        {
            if (conHit>=6)
            {
                conHit -= 6;
                _coin++;
                UpdateCoin();
            }

            // Count down the time
            timeLeft -= (Time.deltaTime);

            // Update the timer
            UpdateTime();

            // Show pickaxe animation
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.y > 180f && Input.mousePosition.y < 5000f)
            {
                
                pickaxeCursor.GetComponent<CursorCtrl>().Play(Input.mousePosition, curToolId-1);
            }
        }
        else
        {
            // the game is end.
            StartCoroutine(GameOver());
        }

        if (showDelayCount > 0)
        {
            if (!LeftMineral() && leftMineralCnt != 0)
            {
                showDelayCount = 0;
            }
            else
            {
                showDelayCount -= Time.deltaTime;
            }
        }
        else
        {
            showDelayCount = showDelay;
            // time to show new targets.
            ShowTargets(maximumTargets);
        }
    }

    public GameObject spark;

    public void HitSomething()
    {
        GameObject tmp = Instantiate(spark);
        tmp.transform.SetParent(pickaxeCursor.transform);
        RectTransform rc = tmp.GetComponent<RectTransform>();
        rc.offsetMax = Vector2.zero;
        rc.offsetMin = Vector2.zero;
        rc.localScale = new Vector2(1, 1);
        if (Input.mousePosition.x < 360f) rc.anchoredPosition -= new Vector2(40, 40);
        else rc.anchoredPosition -= new Vector2(-40, 40);
    }

    // the game is over
    IEnumerator GameOver()
    {
        isGameover = true;

        // show gameover canvas.
        backSound.Stop();
        backSound.clip = gameOverSound;
        backSound.Play();
        backSound.loop = false;
        GameoverCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);

        // show result
        //ResultCanvas.gameObject.SetActive(true);
    }

    public void GoMainMenu()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        MenuCanvas.SetActive(false);
    }

    public void Options()
    {
        MenuCanvas.SetActive(false);
        OptionCanvas.SetActive(true);
    }

    public void Quit()
    {
        MenuCanvas.SetActive(true);
        OptionCanvas.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("StartScene");
    }

    // update time 
    public void UpdateTime()
    {
        
        ShowTimeCount.value = timeLeft / (maximumTimeLimit);
        //Debug.Log(ShowTimeCount.value);
    }

    public void UpdateCoin()
    {
        //curCoin.text = curCoin1.text = _coin.ToString("0");
    }

    // If to hit a mineral, then call the time and score will be added.
    public void HitBonus(Mineral hitSource)
    {
        
        int t = hitSource.type;
        switch (t)
        {
            case 0:
                _score += Global.Diamond;
                break;
            case 1:
                _score += Global.Gload_bar1;
                break;
            case 2:
                _score += Global.Glod;
                break;
            case 3:
                _score += Global.Skeloton;
                break;
            case 4:
                _score += Global.Stone;
                break;
            case 5:
                _score += Global.Gload_bar2;
                break;
            case 6:
                _score += Global.Emo1;
                break;
            case 7:
                _score += Global.Emo2;
                break;
            case 8:
                _score += Global.Emo3;
                break;

        }
        UpdateScore();
        UpdateTime();
        effectSound.clip = pickaxeDown;
        effectSound.Play();
    }

    // update score
    public void UpdateScore()
    {
        _tscore = _tscore < _score ? _score : _tscore;
        ShowScore();
    }

    // show score
    public void ShowScore()
    {
        curScore.text = _score.ToString("0");
        curScore1.text = _score.ToString("0");
        bestScore.text = _tscore.ToString("0");
        PlayerPrefs.SetFloat("top_score", _tscore);
        PlayerPrefs.SetFloat("coin", _coin);
    }
    public int GetVal(Mineral a)
    {
        if (a.type < 5) return 1;
        if (a.type == 5) return 2;
        return 3;
    }
    // If not to hit a mineral, then the time is decreased.
    //public void notHit(Mineral hitSource)
    //{
    //    int t = hitSource.type;
    //    if (t >= 3 && t <= 4)
    //    {
    //    }
    //    else
    //    {
    //    }
    //    UpdateTime();
    //}

    public bool LeftMineral()
    {
        bool result = false;
        Mineral[] previousTargets = FindObjectsOfType<Mineral>();
        foreach (Mineral a in previousTargets)
        {
            if (a.type != 3 && a.type != 4 && a.isDead == false)
            {
                result = true;
            }
            leftMineralCnt++;
        }
        return result;
    }

    public void ShowTargets(int targetCount)
    {
        leftMineralCnt = 0;
        ++curLevel;
        if (curLevel % 3 == 0)
        {
            UpdateTime();
        }
        if (curLevel % 3 == 0) maximumTargets++;
        maximumTargets = maximumTargets > 15 ? 15 : maximumTargets;
        // Find any targets from previous levels
        Mineral[] previousTargets = FindObjectsOfType<Mineral>();
        foreach (Mineral a in previousTargets)
        {
            Destroy(a.gameObject);
        }

        vstPos = new bool[targetPositions.Length];
        for (int i = 0; i < maximumTargets; i++) vstPos[i] = false;
        int maximumTries = targetCount * 5;

        while (targetCount > 0 && maximumTries > 0)
        {
            maximumTries--;

            // Choose a random spawn position
            int randomPosition = Random.Range(0, maximumTargets);
            while (vstPos[randomPosition] && maximumTries > 0)
            {
                maximumTries--;
                randomPosition = Random.Range(0, maximumTargets);
            }
            if (!vstPos[randomPosition])
            {
                vstPos[randomPosition] = true;
                targetCount--;
                Transform newTarget = Instantiate(targetMineral, targetPositions[randomPosition].position, Quaternion.identity).transform;
                newTarget.SetParent(targetPositions[randomPosition]);
                newTarget.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void OnClickToolBtn(int id)
    {
        effectSound.clip = buttonClick;
        effectSound.Play();
        if (id == 0)
        {
            bool cur = curSelTool.gameObject.GetComponent<Toggle>().isOn;
            tools.gameObject.SetActive(cur);
        }

        if (id >= 1 && id <= 4)
        {
            bool cur = curSelTool.gameObject.GetComponent<Toggle>().isOn;
            curSelTool.gameObject.GetComponent<Toggle>().isOn = !cur;
            tools.gameObject.SetActive(false);
            curToolId = id;
            curSelTool.gameObject.GetComponent<Image>().sprite = toolSprites[id - 1];
            pickaxeCursor.GetComponent<Image>().sprite = toolSprites[id - 1];
        }

        if (id == 7 || id == 8)
        {
            SceneManager.LoadSceneAsync("StartScene");
        }

        if (id==9 || id==10)
        {
            OptionCanvas.SetActive(false);
            MenuCanvas.SetActive(true);
        }
        if(id == 11)
        {
            backSound.volume += 0.1f;
            backSoundSlider.value += 0.1f;
            PlayerPrefs.SetFloat("backSound", backSound.volume);
        }
        if (id == 12)
        {
            backSound.volume -= 0.1f;
            backSoundSlider.value -= 0.1f;
            PlayerPrefs.SetFloat("backSound", backSound.volume);
        }
        if (id == 13)
        {
            effectSound.volume += 0.1f;
            effectSoundSlider.value += 0.1f;
            PlayerPrefs.SetFloat("effectSound", effectSound.volume);
        }
        if (id == 14)
        {
            effectSound.volume -= 0.1f;
            effectSoundSlider.value -= 0.1f;
            PlayerPrefs.SetFloat("effectSound", effectSound.volume);
        }
        if(id == 15)
        {
            StartCoroutine(sendResult());
            
        }
        if(id == 16)
        {
            SceneManager.LoadScene("StartScene");
        }
    }
    IEnumerator sendResult()
    {
        string param = "{\"api-key\":\"" + Global.api_key + "\",\"mode\":\"" + Global.mode_regpoint + "\",\"id\":\"" + Global.email+ "\",\"point\":\"" + _score + "\"}";
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
                if(result == "Y")
                {
                    ResultCanvas.gameObject.SetActive(true);
                }
                else
                {
                    GameoverCanvas.gameObject.SetActive(false);
                    ErrorTxt.text = "잘못된 접근입니다.\n관리자에게 문의해주세요";
                    ErroCanvas.gameObject.SetActive(true);
                }
                Debug.Log(jsonNode.ToString());
            }
        }
    }

    public void OnBackVolChange()
    {
        backSound.volume = backSoundSlider.value;
        PlayerPrefs.SetFloat("backSound", backSound.volume);
    }

    public void OnEffectVolChange()
    {
        effectSound.volume = effectSoundSlider.value;
        PlayerPrefs.SetFloat("effectSound", effectSound.volume);
    }
}
