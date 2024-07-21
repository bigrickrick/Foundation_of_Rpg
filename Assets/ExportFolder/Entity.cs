using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Entity : Modifiers
{
    public int HealthPoints;
    public int maxHealthPoints;
    public GameObject entity;
    public delegate void OnEntityDeath();
    public event OnEntityDeath onEntityDeath;
    public Sprite EntityPortrait;
    public event EventHandler OnEntityReachedDestination;

    private GameObject targetInteractiveObject;

    public NavMeshAgent agent;
    protected float Entitybasespeed;
    protected float EntityWalkspeed;
    protected float EntityRunSpeed;
    public Node node;
    public string Entity_Name;
    private Modifiers WhereIsGoing;
    private bool hasReachedDestination = false; 

    public void SetUpNode(Node newnode) // should only be use when become a party member of the player
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
                onEntityDeath?.Invoke();
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
                    if (!hasReachedDestination)
                    {
                        OnEntityReachedDestination?.Invoke(this, EventArgs.Empty);
                        hasReachedDestination = true; 
                    }
                    return true;
                }
            }
        }
        hasReachedDestination = false; 
        return false;
    }

    private void Start()
    {
        maxHealthPoints = HealthPoints;
        onEntityDeath += die;
        agent = GetComponent<NavMeshAgent>();
        Entitybasespeed = agent.speed;
        EntityWalkspeed = Entitybasespeed + 5;
        EntityRunSpeed = Entitybasespeed + 5 * 2;
        OnEntityReachedDestination += LogDestinationReached;
        OnEntityReachedDestination += HandleInteraction; 

        otherSettings();
        if (Entity_Name == null)
        {
            Entity_Name = currentType.ToString();
        }

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

    private void LogDestinationReached(object sender, EventArgs e)
    {
        Debug.Log(Entity_Name + " Reached destination");
    }

    private void HandleInteraction(object sender, EventArgs e)
    {
        if (targetInteractiveObject != null)
        {
            var interactiveTerrain = targetInteractiveObject.GetComponent<InteractiveTerrain>();
            if (interactiveTerrain != null)
            {
                interactiveTerrain.interact(this);
            }
            targetInteractiveObject = null; // Reset the target
        }
    }

    public virtual void Move(RaycastHit hit, GameObject target = null)
    {
        float distance = Vector3.Distance(transform.position, hit.point);
        WhereIsGoing = hit.collider.GetComponent<Modifiers>();

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
        hasReachedDestination = false; 
        targetInteractiveObject = target; 
    }
    
    public void InstantMove(Vector3 position)
    {
        agent.enabled = false;
        transform.position = position;
        agent.enabled = true;
    }

    public float runThresholdDistance = 10f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, runThresholdDistance);
    }
}
