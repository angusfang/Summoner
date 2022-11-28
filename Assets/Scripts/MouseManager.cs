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
    public event Action<GameObject> OnMonster1Clicked;
    public event Action<GameObject> OnMonster2Clicked;

    public Texture2D Target, Attack;

    void Update()
    {
        bool HaveInfo = GetHitInfo();
        SetCursor(HaveInfo);
        MouseControl();
    }

    bool GetHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hitInfo);
    }

    void SetCursor(bool HaveInfo)
    {
        if (HaveInfo)
        {
            if (hitInfo.collider.gameObject.tag == "Ground") Cursor.SetCursor(Target, new Vector2(16, 16), CursorMode.Auto);
            if (hitInfo.collider.gameObject.tag == "Monsters1") Cursor.SetCursor(Attack, new Vector2(0, 0), CursorMode.Auto);
        }
    }

    void MouseControl()
    {

        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            //if (hitInfo.collider.gameObject.CompareTag("Ground")) OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Monsters1")) OnMonster1Clicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Monsters2")) OnMonster2Clicked?.Invoke(hitInfo.collider.gameObject);
        }
    }
}
