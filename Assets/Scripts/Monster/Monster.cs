using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Monster : NetworkBehaviour
{
    
    [SerializeField] MonsterStats_SO monster_stats_origin;
    public ulong master_id;
    [HideInInspector]
    public MonsterStats_SO monster_stats;
    public override void OnNetworkSpawn()
    {

        //TODO: becaus now player is child class of this class

        if (gameObject.tag != "Monster") return;
        ulong NetworkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
        //map netID to object
        ObjManager.Instance.MonsterNetIDToObj.Add(NetworkObjectID, gameObject);
        //instatiate canvas to monster
        UIManager.Instance.AttachCanvasToMonster(NetworkObjectID);
        //get master_id
        if (!IsServer) return;
        //initial monster state
        monster_stats = Instantiate(monster_stats_origin);
        Debug.Log(master_id);
        SetMasterIDClientRpc(NetworkObjectID, master_id);
        SetMonsterVisualHealthClientRpc(NetworkObjectID, monster_stats.current_health);
    }
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        ulong NetworkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
        DeleteCanvasClientRpc(NetworkObjectID);
    }

    [ClientRpc]
    void DeleteCanvasClientRpc(ulong NetworkObjectID)
    {
        UIManager.Instance.DeleteObjInDictionart(NetworkObjectID);
    }

    [ClientRpc]
    void SetMonsterVisualHealthClientRpc(ulong NetworkObjectID, int health)
    {
        UIManager.Instance.InitHealthValueNearByHeart(NetworkObjectID, health);
    }

    [ClientRpc]
    void SetMasterIDClientRpc(ulong NetworkObjectID, ulong master_id)
    {
        ObjManager.Instance.MonsterNetIDToObj[NetworkObjectID].GetComponent<Monster>().master_id= master_id;
    }

}
