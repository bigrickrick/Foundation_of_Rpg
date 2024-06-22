using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifiers : MonoBehaviour
{
    
    [Header("WhatCanAffectIt")]
    public bool CanBeIncapacited;
    public bool CanBedamage;
    public bool IsIncapacited;

    public enum Type
    {
        Interactiveterrain,
        IsAObject,
        IsAEntity,
        IsAHostileEntity,
        PartyMember,
        MainCharacter
    }
    [Header("What is it?")]
   

    public Type currentType = Type.Interactiveterrain;




}
