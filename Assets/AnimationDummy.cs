using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDummy : EntityLegacy
{
    public override void die()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsDead",true); 
        }
    }
}
