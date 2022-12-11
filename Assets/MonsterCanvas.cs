using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject CanvasPrefab;
    [SerializeField] MonsterDataManager.MonsterID MonsterID;
    GameObject m_CanvasPrefab;
    TextMeshPro m_health;
    ulong m_monsterSize;
    void Awake()
    {
        
        m_monsterSize = MonsterDataManager.Instance.MonsterIDMapToState(MonsterID).requreNumOFSlot;
        m_CanvasPrefab = Instantiate(CanvasPrefab);
        m_CanvasPrefab.transform.position = transform.position + new Vector3(0, m_monsterSize, 0) * 2f;
        m_health = m_CanvasPrefab.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        m_CanvasPrefab.transform.position = transform.position+ new Vector3(0, m_monsterSize, 0)*2f;
        m_CanvasPrefab.transform.LookAt(m_CanvasPrefab.transform.position+Camera.main.transform.forward);
    }

    public void SetHealthBar(int currentHealth)
    {
        m_health.text = currentHealth.ToString();
    }
}
