using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notices : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetText(string title, string contents, string date)
    {
        Text[] cur = GetComponentsInChildren<Text>();
        cur[0].text = title;
        cur[1].text = contents;
        cur[2].text = date;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
