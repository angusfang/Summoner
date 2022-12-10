using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.PackageManager;

public class ClientSelectManager : Singleton<ClientSelectManager>
{
    public Color HoverColor;
    public Color SelectColor;
    
    private GameObject hover;
    private GameObject select1;
    private GameObject select2;
    private Player m_player;
    private ulong clientID;

    protected override void Awake()
    {
        base.Awake();
    }
    void Update()
    {
        //TODO: send to server?
    }
    public void SetClientID(ulong clientID)
    {
        this.clientID = clientID;
    }
    public void SetPlayerRef(Player player)
    {
        m_player = player;
    }
    public void SetHoverObj(GameObject newHover)
    {
        //if switch hover must turn origin outline off
        if (hover != newHover && hover != null && select1 == null) {
            Outline outline = hover.GetComponent<Outline>();
            outline.enabled= false;
            hover = null;
        }
        
        if (newHover.tag == "Monster")
        {
            hover = newHover;
            hover.GetComponent<Outline>().enabled = true;
        }
    }
    public void SetClickObj()
    {
        if (hover == null)
        {
            //if(select1!= null)
            //{
            //    select1.gameObject.GetComponent<Outline>().OutlineColor = HoverColor;
            //}
            select1 = select2 = null;
        }
        else if (select1 == null)
        {
            select1 = hover;
            select1.gameObject.GetComponent<Outline>().OutlineColor = SelectColor;
        }
        else {
            //send selsect state to server
           
            select2 = hover;
            m_player.SetSelectStateServerRpc(clientID, select1.GetComponent<NetworkObject>().NetworkObjectId, select2.GetComponent<NetworkObject>().NetworkObjectId);
            
            Outline outline1 = select1.gameObject.GetComponent<Outline>();
            outline1.OutlineColor = HoverColor;
            outline1.enabled = false;
            select1 = select2 = null;
        }

    }
    
}
