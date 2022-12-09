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
    [SerializeField] GameObject MonsterPrefab;
    
    private Vector3[] SpawnPositions = { new Vector3(10f, 0f, 10f), new Vector3(-10f, 0f, 10f), new Vector3(10f, 0f, -100f), new Vector3(-10f, 0f, -10f) };
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
        
    }

    private void Update()
    {
        if (!IsOwner) return;
        createCD -= Time.deltaTime;
        if (Input.GetKey(KeyCode.G) && createCD <= 0f)
        {
            createCD = createCD_const;
            Vector3 spawnPosition = transform.position + transform.forward * MonsterPrefab.GetComponent<NavMeshAgent>().radius*1.5f;
            SpawnMonsterServerRpc(OwnerClientId, spawnPosition);
        }
        if (Input.GetMouseButtonUp(0)) SendRayServerRpc(OwnerClientId, Camera.main.ScreenPointToRay(Input.mousePosition));
        
    }
   

    [ServerRpc]
    void SendRayServerRpc(ulong clientID, Ray ray)
    {
        GameManager.Instance.PlayerRayToSelectState(clientID, ray);
    }

    [ServerRpc]
    void MoveToSpawnPositionServerRpc(ulong ClientId)
    {
        Transform PlayerTransform = GameObject.Find("PlayerTransform"+(ClientId+1)).transform;
        NetworkManager.Singleton.ConnectedClients[ClientId].PlayerObject.transform.SetPositionAndRotation(PlayerTransform.position, PlayerTransform.rotation);
            
    }
    [ServerRpc]
    void SpawnMonsterServerRpc(ulong ClientId, Vector3 spawnPosition)
    {
        GameObject Monster = Instantiate(MonsterPrefab, new Vector3(ClientId, 0f, ClientId), Quaternion.identity);
        Monster.GetComponent<Monster>().master_id = ClientId;
        NetworkObject networkObject = Monster.GetComponent<NetworkObject>();
        networkObject.transform.position = spawnPosition;
        networkObject.transform.LookAt(Vector3.zero);
        networkObject.Spawn();
    }


}
