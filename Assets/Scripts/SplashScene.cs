using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
    //AsyncOperation cur;
    // Start is called before the first frame update
    void Start()
    {

        //StartCoroutine(LoadScene());
    }

    public void playBtn()
    {
        SceneManager.LoadScene("LogScene");
    }
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
