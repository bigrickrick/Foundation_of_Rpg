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
        None,
        Interactiveterrain,
        IsABlock,
        IsAObject,
        IsAEntity,
        IsAHostileEntity,
        PartyMember,
        MainCharacter
    }

    public Type currentType = Type.None;





}
