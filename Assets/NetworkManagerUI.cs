using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button hostBtn;
    [SerializeField] Button serverBtn;
    [SerializeField] Button clientBtn;
    private void Awake()
    {
        hostBtn.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        serverBtn.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
        clientBtn.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
    }
}
