using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public static LoginController instance = null;

    public InputField emailField;

    public InputField passwordField;

    public static LoginController GetInstance()
    {
        if (instance == null) instance = new LoginController();
        return instance;
    }

    public void LoginClk()
    {

        //SceneManager.LoadScene("StartScene");
        string email = emailField.text;
        string pwd = passwordField.text;
        StartCoroutine(getlogin_thread(email, pwd));
    }

    IEnumerator getlogin_thread(string email, string pwd)
    {
        string param = "{\"api-key\":\"" + Global.api_key + "\",\"mode\":\"" + Global.mode_login + "\",\"id\":\"" + email + "\",\"pw\":\"" + pwd+"\"}";
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
                string point = jsonNode["point"].ToString().Replace("\"", "");
                switch (result)
                {
                    case "Y":
                        Global.email = email;
                        SceneManager.LoadScene("StartScene");
                        break;
                    case "10001":
                        Debug.Log("api_key:error");
                        string errorApi = "잘못된 접근입니다.";
                        LoginManager.GetInstance().ErroController(errorApi);
                        break;
                    case "10002":
                        Debug.Log("로그인 아이디 오류");
                        string errorEmail = "회원정보가 일치하지 않습니다. 다시확인해 주세요.";
                        LoginManager.GetInstance().ErroController(errorEmail);
                        break;
                    case "10003":
                        Debug.Log("포인트 부족");
                        Global.pointState = false;
                        SceneManager.LoadScene("StartScene");
                        //string errorPoint = "로그인 실패하였습니다. 잠시후 다시 시도해주세요";
                        //LoginManager.GetInstance().ErroController(errorPoint);
                        break;
                    case "10009":
                        Debug.Log("모드(구분)오류");
                        string error = "로그인 실패하였습니다. 잠시후 다시 시도해주세요";
                        LoginManager.GetInstance().ErroController(error);
                        break;

                }
            }
        }
    }

    IEnumerator getlogin_process(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            Debug.Log("result::" + jsonNode);

        }
        else
        {
            Debug.Log("success");
            //SceneManager.LoadScene("LoadingScene");
            Debug.Log(www.error);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Global.pointState = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
