using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Mineral : MonoBehaviour, IPointerClickHandler
{

    // Mineral type
    public int type;
    public int cnt;

    // All Sprites are appeared in the game
    public Sprite[] sprites;
    public Sprite[] ex;

    // is this mineral died.
    public bool isDead;

    // time to show
    public float showTime = 6f;

    public float delayTime, delayTime1;
    public Animator ani;
    public GameObject child;

    public bool ok;
    // Start is called before the first frame update
    void Start()
    {
        type = Random.Range(0f, 1f)<0.6f?Random.Range(0, 5):Random.Range(0f, 1f)<0.6?5:Random.Range(6, 9);
        gameObject.GetComponent<Image>().sprite = sprites[type];
        isDead = false;
        ok = true;
        showTime = 3.0f;
        delayTime = Random.Range(1f, 4f);
        ani = gameObject.GetComponent<Animator>();
        delayTime1 = delayTime - 0.8f;
        //delayTime1 = 0;
        //Debug.Log("delaytime" + delayTime1);
        cnt = 1;
        //if (type > 4) cnt++;
        //if (type > 5) cnt++;
        showTime += cnt * 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        if (delayTime1 > 0) delayTime1 -= Time.deltaTime;
        else
        {
            child.SetActive(true);
        }
        if (delayTime > 0) delayTime -= Time.deltaTime;
        else
        {
            if (ok) gameObject.GetComponent<Animator>().Play("ShowMineral");
            ok = false;
            if (showTime > 0) showTime -= Time.deltaTime;
            else
            {
                if (!isDead)
                {
                    isDead = true;
                    //MineralGameManager.GetInstance().notHit(this);
                }
                Die();
            }
        }
    }


    public void OnPointerClick(PointerEventData e)
    {
        cnt--;
        if (isDead == false)
        {
            if (cnt<1) isDead = true;
            MineralGameManager.GetInstance().HitBonus(this);
            if (cnt<1)
            {
                if (type == 3 || type == 4) StartCoroutine(Explose());
                else ani.Play("HideMineral");
            }
            MineralGameManager.GetInstance().HitSomething();
        }
    }
    IEnumerator Explose()
    {
        gameObject.transform.localScale = new Vector3(3, 3, 3);
        for (int i=0; i<ex.Length; i++)
        {
            gameObject.GetComponent<Image>().sprite = ex[i];
            yield return new WaitForSeconds(0.25f / ex.Length);
        }
        gameObject.SetActive(false);
    }
    public void Die()
    {
        isDead = true;
        ani.Play("HideMineral");
    }
}
