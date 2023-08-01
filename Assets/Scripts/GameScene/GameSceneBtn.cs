using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSceneBtn : MonoBehaviour, IPointerClickHandler
{
    // button identity number
    public int id;

    // click event
    public void OnPointerClick(PointerEventData e)
    {
        MineralGameManager.GetInstance().OnClickToolBtn(id);
    }
}
