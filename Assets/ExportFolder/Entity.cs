using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
public abstract class Entity : Modifiers
{
    public int HealthPoints;
    public int maxHealthPoints;
    public GameObject entity;
    public delegate void OnEntityDeath();
    public static event OnEntityDeath onEntityDeath;
    public NavMeshAgent agent;
    protected float Entitybasespeed;
    protected float EntityWalkspeed;
    protected float EntityRunSpeed;
    public Node node;

    public void SetUpNode(Node newnode)// should only be use when become a party member of the player
    {
        node = newnode;
    }
    public virtual void DamageRecieve(int damage)
    {

        if (CanBedamage == true)
        {
            HealthPoints = HealthPoints - damage;


            if (HealthPoints < 0)
            {

                if (onEntityDeath != null)
                {
                    onEntityDeath?.Invoke();
                }
            }

        }



    }
    public bool HasReachHisdestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    Debug.Log("Reached destination");
                    return true;
                }
            }
        }
        Debug.Log("Didn't reach destination");
        return false;
    }
    private void Start()
    {
        maxHealthPoints = HealthPoints;
        onEntityDeath += die;
        agent = GetComponent<NavMeshAgent>();
        Entitybasespeed = agent.speed;
        EntityWalkspeed = Entitybasespeed + 5;
        EntityRunSpeed = Entitybasespeed +5* 2;
        otherSettings();

        
        agent.updatePosition = true;
        agent.updateRotation = true;
        
        agent.angularSpeed = 9999f; 
        agent.autoBraking = true;   
    }
    public virtual void otherSettings()
    {
        
    }
    public virtual void die()
    {
        if (HealthPoints <= 0)
        {
            Destroy(entity);
            
            
        }
    }
    

    public virtual void Move(RaycastHit hit)
    {
        float distance = Vector3.Distance(transform.position, hit.point);

       
        if (distance > runThresholdDistance)
        {
            agent.speed = EntityRunSpeed;
        }
        else
        {
            agent.speed = EntityWalkspeed;
        }
        agent.acceleration = 99;
        
        agent.SetDestination(hit.point);
    }
    public float runThresholdDistance = 10f; 

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, runThresholdDistance);
    }

   // public Entity leader;

    
}
