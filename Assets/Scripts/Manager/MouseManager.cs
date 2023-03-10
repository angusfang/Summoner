using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[System.Serializable]
//public class EventVector3: UnityEvent<Vector3> { } 要從外部托跩才需要這兩行
public class MouseManager : Singleton<MouseManager>
{
    RaycastHit hitInfo;

    //public event Action<Vector3> OnMouseClicked;
    //public event Action<GameObject> OnMonster1Clicked;
    //public event Action<GameObject> OnMonster2Clicked;

    public Texture2D Target, Attack;

    void Update()
    {
        bool HaveInfo = GetHitInfo();
        if (HaveInfo)
        {
            SetCursor();
            ClientSelectManager.Instance.SetHoverObj(hitInfo.collider.gameObject);
            if(Input.GetMouseButtonUp(0))ClientSelectManager.Instance.SetClickObj();
        }
        
        //MouseControl();
    }

    bool GetHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hitInfo);
    }

    void SetCursor()
    {
            if (hitInfo.collider.gameObject.tag == "Ground") Cursor.SetCursor(Target, new Vector2(16, 16), CursorMode.Auto);
            if (hitInfo.collider.gameObject.tag == "Monster") Cursor.SetCursor(Attack, new Vector2(0, 0), CursorMode.Auto);
    }
   

    
}
