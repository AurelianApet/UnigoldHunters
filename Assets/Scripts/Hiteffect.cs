using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hiteffect : MonoBehaviour
{
    public float lifetime;
    public bool vis;
    // Start is called before the first frame update
    void Start()
    {
        lifetime = 2.0f;
        vis = true;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.SetActive(vis);
        if (!vis) Destroy(gameObject);
    }
}
