using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Canvas m_stateBar;
    private Dictionary<ulong, Canvas> monsterIDToCanvas;
    private Dictionary<ulong, TextMeshPro> monsterIDToText;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        monsterIDToCanvas = new Dictionary<ulong, Canvas>();
        monsterIDToText = new Dictionary<ulong, TextMeshPro>();
    }
    private void Update()
    {
        foreach(KeyValuePair<ulong, Canvas> entry in monsterIDToCanvas)
        {
            entry.Value.transform.position = ObjManager.Instance.MonsterNetIDToObj[entry.Key].transform.position + new Vector3(0f, 5f, 0f);
        }
    }
    public void AttachCanvasToMonster(ulong NetObjID)
    {
        Canvas monsterCanvas = Instantiate(m_stateBar);
        monsterCanvas.transform.LookAt(monsterCanvas.transform.position + Camera.main.transform.forward);
        monsterIDToCanvas.Add(NetObjID, monsterCanvas);
        TextMeshPro textMeshPro = monsterCanvas.gameObject.GetComponent<TextMeshPro>();
        Monster monster = ObjManager.Instance.MonsterNetIDToObj[NetObjID].GetComponent<Monster>();
        textMeshPro.text= monster.monster_stats.current_health.ToString();
        monsterIDToText.Add(NetObjID, textMeshPro);
        
    }
    public void SetNewObjHealthValueNearByHeart(ulong NetObjID, int current_health)
    {
        monsterIDToText[NetObjID].text= current_health.ToString();
    }
}
