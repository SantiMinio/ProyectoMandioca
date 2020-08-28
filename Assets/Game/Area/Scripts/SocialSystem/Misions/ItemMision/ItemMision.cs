﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[System.Serializable]
public class ItemMision 
{
    [SerializeField] string name = "";
    [SerializeField] protected bool iscompleted = false;
    [SerializeField] private byte currentvalue = 1;
    [SerializeField] private byte maxvalue = 1;

    public enum ItemType { one_objective_Bool, multiple_int }
    public ItemType itemType;

    internal bool IsCompleted { get { return iscompleted; } }
    Action OnRefresh = delegate { };
    [Multiline(5)] [SerializeField] protected string description;
    internal string Description { get { return description; } }
    internal int CurrentValue { get { return (int)currentvalue; } }
    internal int MaxValue { get { return (int)maxvalue; } }

    public override string ToString()
    {
        string aux = "";

        if (itemType == ItemType.one_objective_Bool)
        {
            aux = RematchString( description );
        }

        if (itemType == ItemType.multiple_int)
        {
            aux = RematchString( "[" + currentvalue + "/" + maxvalue + "] " + description );
        }

        return aux; 
    }


    string RematchString(string s) => iscompleted ? "<s>" + s + "</s>" : s;

    public void SubscribeTo_ItemSelfUpdate(Action Callback_Refresh) => OnRefresh = Callback_Refresh;

    internal void CheckMemory(int id, int index)
    {
        MissionMemory.instance.CheckFromMemory(id,index, AddItemFromMemory);
    }

    void AddItemFromMemory()
    {
        if (iscompleted) return;

        if (itemType == ItemType.one_objective_Bool)
        {
            iscompleted = true;

        }
        else
        {
            currentvalue++;
            if (currentvalue >= maxvalue)
            {
                currentvalue = maxvalue;
                iscompleted = true;
            }
        }
    }

    public void Execute() 
    {
        if (iscompleted) return;

        if (itemType == ItemType.one_objective_Bool)
        {
            iscompleted = true;
            
        }
        else
        {
            currentvalue++;
            if (currentvalue >= maxvalue)
            {
                currentvalue = maxvalue;
                iscompleted = true;
            }
        }

        OnRefresh.Invoke();
    }
}