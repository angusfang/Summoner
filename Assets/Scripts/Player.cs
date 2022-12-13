using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class Player :NetworkBehaviour
{
    private float createCD = 0f;
    private float createCD_const => 1f;
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
        Debug.Log("SPAWN");
        
        MoveToSpawnPositionServerRpc(OwnerClientId);
        
        GameObject mainCamera = GameObject.Find("Camera");
        GameObject cameraPlayer;
       
        Debug.Log("Camera" + (OwnerClientId+1));

        cameraPlayer = GameObject.Find("CameraTransform" + (OwnerClientId + 1));
        mainCamera.transform.SetPositionAndRotation(cameraPlayer.transform.position, cameraPlayer.transform.rotation);
        ClientSelectManager.Instance.SetClientID(OwnerClientId);
        ClientSelectManager.Instance.SetPlayerRef(this);
    }
    [ServerRpc]
    public void SetSelectStateServerRpc(ulong UserID, ulong monsterID1, ulong monsterID2)
    {
        ServerGameManager.Instance.SetSelectState(UserID, monsterID1, monsterID2);
    }
    private void Update()
    {
        if (!IsOwner) return;
        createCD -= Time.deltaTime;
        if (Input.GetKey(KeyCode.G) && createCD <= 0f)
        {
            MonsterDataManager.MonsterID monsterID = MonsterDataManager.MonsterID.NightmareDragon;
            createCD = createCD_const;
            ulong requreNumOFSlot = MonsterDataManager.Instance.MonsterIDMapToState(MonsterDataManager.MonsterID.NightmareDragon).requreNumOFSlot;
            float spawnDegree;

            //find monster position
            if(!ClientMonsterPositionManager.Instance.RequirePosition(requreNumOFSlot,out spawnDegree))return;
            Debug.Log("spawnDegree"+spawnDegree);
            SpawnMonsterServerRpc(OwnerClientId, spawnDegree, monsterID);
        }
        
    }
   

    [ServerRpc]
    void MoveToSpawnPositionServerRpc(ulong ClientId)
    {
        Transform PlayerTransform = GameObject.Find("PlayerTransform"+(ClientId+1)).transform;
        NetworkManager.Singleton.ConnectedClients[ClientId].PlayerObject.transform.SetPositionAndRotation(PlayerTransform.position, PlayerTransform.rotation);
            
    }
    [ServerRpc]
    void SpawnMonsterServerRpc(ulong ClientId, float spawnDegree, MonsterDataManager.MonsterID monsterID)
    {
        GameObject MonsterPrefab = MonsterDataManager.Instance.MonsterIDMapToPrefab(monsterID);
        GameObject MonsterG = Instantiate(MonsterPrefab, new Vector3(ClientId, 0f, ClientId), Quaternion.identity);

        Quaternion rotation = Quaternion.Euler(0f, spawnDegree, 0f);
        Vector3 myVector = transform.forward * 12f;
        Vector3 rotateVector = rotation * myVector;
        MonsterG.transform.position = transform.position + rotateVector;
        if(ClientId==0) MonsterG.transform.LookAt(MonsterG.transform.position + new Vector3 (0,0,-1));
        else MonsterG.transform.LookAt(MonsterG.transform.position + new Vector3(0, 0, 1));

        Monster monster = MonsterG.GetComponent<Monster>();
        monster.InitIdPosRot(ClientId, monsterID, MonsterG.transform.position, MonsterG.transform.rotation);

        NetworkObject networkObject = MonsterG.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }


}
