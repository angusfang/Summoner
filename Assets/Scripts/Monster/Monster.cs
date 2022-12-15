using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Xml.Serialization;
//using static MonsterDataManager;

public class Monster : NetworkBehaviour
{
    
    //[SerializeField] MonsterStats_SO monster_stats_origin;
    
    //MonsterDataManager.MonsterID monster_id;
    //Vector3 original_position;
    //Quaternion original_rotation;
    [HideInInspector]

    ulong m_masterId;
    MonsterDataManager.MonsterID m_monsterId;
    Vector3 m_originalPosition;
    Quaternion m_originaRotation;
    MonsterStats_SO m_monsterStats;

    public Quaternion OriginaRotation => m_originaRotation;
    public Vector3 OriginalPosition => m_originalPosition;
    public ulong MasterId { get { return m_masterId; } set { m_masterId = value; } }
    public MonsterStats_SO MonsterStats { get { return m_monsterStats; } set { m_monsterStats = value; } }
    

    MonsterCanvas m_MonsterCanvas;
    public ulong getMasterId()
    {
        return MasterId;
    }
    public void InitIdPosRot(ulong master_id, MonsterDataManager.MonsterID monster_id, Vector3 original_position, Quaternion original_rotation)
    {
        m_masterId = master_id;
        m_monsterId = monster_id;
        m_originalPosition = original_position;
        m_originaRotation = original_rotation;
    }
    public override void OnNetworkSpawn()
    {

        //TODO: becaus now player is child class of this class
        if (gameObject.tag != "Monster") return;
        
        m_MonsterCanvas = gameObject.GetComponent<MonsterCanvas>();
        ulong NetworkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
        
        //map netID to object
        ObjManager.Instance.MonsterNetIDToObj.Add(NetworkObjectID, gameObject);
        //fill placeholder
        if (MasterId == OwnerClientId) ClientMonsterPositionManager.Instance.FillPlaceHolder(NetworkObjectID);

        //get master_id
        if (!IsServer) return;
        // Only server need state of monster
        MonsterStats = Instantiate(MonsterDataManager.Instance.MonsterIDMapToState(m_monsterId));
        // Client must get master_id of monster
        SetMasterIDClientRpc(NetworkObjectID, MasterId);
        SetVisualHealthClientRpc(MonsterStats.current_health);

    }
    public override void OnNetworkDespawn()
    {
        ulong NetworkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
        ObjManager.Instance.MonsterNetIDToObj.Remove(NetworkObjectID);
        ClientMonsterPositionManager.Instance.RealsePosition(NetworkObjectID);
        m_MonsterCanvas.DestroyCanvas();
    }

    [ClientRpc]
    public void SetVisualHealthClientRpc(int current_health)
    {
        m_MonsterCanvas.SetHealthBar(current_health);
    }

   

    [ClientRpc]
    void SetMasterIDClientRpc(ulong NetworkObjectID, ulong master_id)
    {
        ObjManager.Instance.MonsterNetIDToObj[NetworkObjectID].GetComponent<Monster>().MasterId = master_id;
    }

}
