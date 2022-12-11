using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Monster : NetworkBehaviour
{
    
    //[SerializeField] MonsterStats_SO monster_stats_origin;
    public ulong master_id;
    public Vector3 original_position;
    [HideInInspector]
    public MonsterStats_SO monster_stats;
    

    MonsterCanvas m_MonsterCanvas;
    public override void OnNetworkSpawn()
    {

        //TODO: becaus now player is child class of this class

        if (gameObject.tag != "Monster") return;
        m_MonsterCanvas = gameObject.GetComponent<MonsterCanvas>();
        ulong NetworkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
        //map netID to object
        ObjManager.Instance.MonsterNetIDToObj.Add(NetworkObjectID, gameObject);
        //instatiate canvas to monster
        //UIManager.Instance.AttachCanvasToMonster(NetworkObjectID);
        //fill placeholder
        if (master_id == OwnerClientId) ClientMonsterPositionManager.Instance.FillPlaceHolder(NetworkObjectID);

        //get master_id
        if (!IsServer) return;
        // Client must master_id of monster
        SetMasterIDClientRpc(NetworkObjectID, master_id);
        SetVisualHealthClientRpc(monster_stats.current_health);
        //SetMonsterVisualHealthClientRpc(NetworkObjectID, monster_stats.current_health);

    }
    public override void OnNetworkDespawn()
    {
        //if (!IsServer) return;
        ulong NetworkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
        //DeleteCanvasClientRpc(NetworkObjectID);
        //UIManager.Instance.DeleteObjInDictionart(NetworkObjectID);
        ObjManager.Instance.MonsterNetIDToObj.Remove(NetworkObjectID);
        ClientMonsterPositionManager.Instance.RealsePosition(NetworkObjectID);
    }

    [ClientRpc]
    void SetVisualHealthClientRpc(int current_health)
    {
        m_MonsterCanvas.SetHealthBar(current_health);
    }

    //[ClientRpc]
    //void SetMonsterVisualHealthClientRpc(ulong NetworkObjectID, int health)
    //{
    //    UIManager.Instance.InitHealthValueNearByHeart(NetworkObjectID, health);
    //}

    [ClientRpc]
    void SetMasterIDClientRpc(ulong NetworkObjectID, ulong master_id)
    {
        ObjManager.Instance.MonsterNetIDToObj[NetworkObjectID].GetComponent<Monster>().master_id = master_id;
    }

}
