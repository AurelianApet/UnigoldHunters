using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    public void SetText(int rank, string name, int score)
    {
        Text[] cur = GetComponentsInChildren<Text>();
        cur[0].text = name;
        cur[1].text = score.ToString();
        cur[2].text = rank.ToString(); 
    }
    public void SetColor(Color c)
    {
        Text[] cur = GetComponentsInChildren<Text>();
        cur[0].color = c;
        cur[1].color = c;
    }
}
