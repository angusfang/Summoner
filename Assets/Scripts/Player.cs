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
        MoveToSpawnPositionServerRpc(OwnerClientId);
    }

    private void Update()
    {
        if (!IsOwner) return;
        createCD -= Time.deltaTime;
        if (Input.GetKey(KeyCode.G) && createCD <= 0f)
        {
            createCD = createCD_const;
            Vector3 spawnPosition = transform.position + transform.forward * MonsterPrefab.GetComponent<NavMeshAgent>().radius;
            SpawnMonsterServerRpc(OwnerClientId, spawnPosition);
        }
        if (Input.GetMouseButtonUp(0)) SendRayServerRpc(OwnerClientId, Camera.main.ScreenPointToRay(Input.mousePosition));
        
    }
   

    [ServerRpc]
    void SendRayServerRpc(ulong clientID, Ray ray)
    {
        GameManager.Instance.PlayerRayToSelectState(clientID, ray);
        //RaycastHit hitinfo;
        //if (!Physics.Raycast(ray, out hitinfo)) return;
        //if (hitinfo.collider == null) return;
        //GameObject hitGameObject = hitinfo.collider.gameObject;
        //if (!hitGameObject.CompareTag("Monster")) return;
        //Debug.Log(hitGameObject.GetComponent<Monster>().master_id);
    }

    [ServerRpc]
    void MoveToSpawnPositionServerRpc(ulong ClientId)
    {
        NetworkManager.Singleton.ConnectedClients[ClientId].PlayerObject.transform.position = SpawnPositions[OwnerClientId];
    }
    [ServerRpc]
    void SpawnMonsterServerRpc(ulong ClientId, Vector3 spawnPosition)
    {
        GameObject Monster = Instantiate(MonsterPrefab, new Vector3(ClientId, 0f, ClientId), Quaternion.identity);
        Monster.GetComponent<Monster>().master_id = ClientId;
        NetworkObject networkObject = Monster.GetComponent<NetworkObject>();
        networkObject.transform.position = spawnPosition;
        networkObject.Spawn();
    }


}