using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyButton : MonoBehaviour, IPointerClickHandler
{
    public int id;
    public void OnPointerClick(PointerEventData e)
    {
        LoginManager.GetInstance().OnClickBtn(id);
    }
}
