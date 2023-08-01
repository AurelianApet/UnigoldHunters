using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorCtrl : MonoBehaviour
{
    public Sprite[] fir;
    public Sprite[] sec;
    public Sprite[] thi;
    public Sprite[] fou;
    public Sprite[] fir1;
    public Sprite[] sec1;
    public Sprite[] thi1;
    public Sprite[] fou1;
    public Sprite[,,] spr; 
    // Start is called before the first frame update
    void Start()
    {
        spr = new Sprite[4, 2, 6];
        for (int i = 0; i < fir.Length; i++) spr[0, 0, i] = fir[i];
        for (int i = 0; i < sec.Length; i++) spr[1, 0, i] = sec[i];
        for (int i = 0; i < thi.Length; i++) spr[2, 0, i] = thi[i];
        for (int i = 0; i < fou.Length; i++) spr[3, 0, i] = fou[i];
        for (int i = 0; i < fir1.Length; i++) spr[0, 1, i] = fir1[fir1.Length - 1 - i];
        for (int i = 0; i < sec1.Length; i++) spr[1, 1, i] = sec1[sec1.Length - 1 - i];
        for (int i = 0; i < thi1.Length; i++) spr[2, 1, i] = thi1[thi1.Length - 1 - i];
        for (int i = 0; i < fou1.Length; i++) spr[3, 1, i] = fou1[fou1.Length - 1 - i];
    }

    public void Play(Vector2 pos, int id)
    {
        gameObject.SetActive(true);
        RectTransform tmp = GetComponent<RectTransform>();
        int t;
        if (pos.x<360f)
        {
            tmp.pivot = new Vector2(0.2f, 0.1f);
            t = 1;
        } else
        {
            tmp.pivot = new Vector2(0.8f, 0.1f);
            t = 0;
        }
        tmp.localScale = new Vector2(3, 3);
        tmp.anchoredPosition = pos;
        StartCoroutine(Play(id, t));
    }
    IEnumerator Play(int id, int t)
    {
        Hiteffect[] objs = FindObjectsOfType<Hiteffect>();
        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i].gameObject);
        }
        for (int i=0; i<fir.Length; i++)
        {
            gameObject.GetComponent<Image>().sprite = spr[id, t, i];
            yield return new WaitForSeconds(0.25f / fir.Length);
        }
        gameObject.SetActive(false);
    }
}
