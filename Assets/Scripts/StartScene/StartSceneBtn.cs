using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartSceneBtn : MonoBehaviour, IPointerClickHandler
{
    public BtnID id;

    public void OnPointerClick(PointerEventData e)
    {
        StartSceneManager.GetInstance().OnBtnClick(id);
    }
}
