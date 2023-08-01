using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.IO;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Transform LoginCanvas;
    public Transform HelpCanvas;
    public Transform NoticeObj;
    public Transform ErrorBord;
    public static LoginManager instance = null;

    public Text ErroMessage;

    public AudioSource backSound, effectSound;
    public AudioClip back, buttonClick;
    public static LoginManager GetInstance()
    {
        if (instance == null) instance = new LoginManager();
        return instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        backSound.volume = PlayerPrefs.GetFloat("backSound", 0.3f);
        effectSound.volume = PlayerPrefs.GetFloat("effectSound", 0.3f);
        backSound.clip = back;
        backSound.loop = true;
        backSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClickBtn(int id)
    {
        effectSound.clip = buttonClick;
        effectSound.Play();
        if (id==0) // login button clicked
        {
            LoginCanvas.gameObject.SetActive(true);
        }
        if (id==1) // ok button clicked
        {
            NoticeObj.gameObject.SetActive(true);
        }
        if (id==2) // close button clicked
        {
            SceneManager.LoadScene("SplashScene");
            //LoginCanvas.gameObject.SetActive(false);
        }
        if (id==3)
        {
            ErrorBord.gameObject.SetActive(false);
        }
        
    }
    public void ErroController(string _erroMessage)
    {
        ErroMessage.text = _erroMessage;
        ErrorBord.gameObject.SetActive(true);
    }
}
