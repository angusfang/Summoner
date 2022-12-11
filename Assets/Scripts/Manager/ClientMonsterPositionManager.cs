using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMonsterPositionManager : Singleton<ClientMonsterPositionManager>
{
    private int[] positionSlot = new int[12];
    private int[] checkOrder = {6,5,7,4,8,3,9,2,10,1,11,0 };
    private int empty = -1;
    private int placeHolder = -2;
    private bool waitFillPlaceHolder = false;
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < 12; i++) positionSlot[i] = empty;
    }
    public bool RequirePosition(ulong numOFSlot,out float spawnDegree)
    {
        spawnDegree = 0f;
        if (waitFillPlaceHolder) return false;
        bool hasPosition;
        foreach (ulong check in checkOrder)
        {
            hasPosition = true;
            for (ulong i = 0; i < numOFSlot; i++)
            {
                if (check + i >=12 || positionSlot[check+i] != empty)
                {
                    hasPosition = false;
                    break;
                }
            }
            if (hasPosition) {
                for (ulong i = 0; i < numOFSlot; i++)
                {
                    positionSlot[check + i] = placeHolder;
                    
                }
                
                spawnDegree = ((float)check + 0.5f * (float)numOFSlot) * 0.08333333333f * 180f + -90f;
                //spawnDegree = (check + 0.5f * numOFSlot) / 12f * 180f + -90f;
                waitFillPlaceHolder = true;
                return true;
            }
        }

        return false;
    }
    public void FillPlaceHolder(ulong netObjID)
    {
        foreach (ulong check in checkOrder)
        {
            if (positionSlot[check] == placeHolder) positionSlot[check] = (int)netObjID;
        }
        waitFillPlaceHolder = false;
    }
    public void RealsePosition(ulong netObjID)
    {
        foreach (ulong check in checkOrder)
        {
            if (positionSlot[check] == (int)netObjID) positionSlot[check] = empty;
        }
    }
}

