using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : Singleton<ObjManager>
{
    [HideInInspector]
    public Dictionary<ulong,GameObject> MonsterNetIDToObj;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        MonsterNetIDToObj = new Dictionary<ulong, GameObject>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) Debug.Log(MonsterNetIDToObj.Count);
    }
}
