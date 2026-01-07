using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class EntityLegacy : Modifiers
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
    private (int, int, int) WorldToGridCoordsRounded(Vector3 world)
    {
        int gx = Mathf.RoundToInt(world.x);
        int gy = Mathf.RoundToInt(world.y);
        int gz = Mathf.RoundToInt(world.z);
        return (gx, gy, gz);
    }

    public virtual void Move(RaycastHit hit, GameObject target = null)
    {
        if (GameManager.Instance.movementMode == GameManager.MovementMode.Normal)
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
        else if (GameManager.Instance.movementMode == GameManager.MovementMode.Grid)
        {
            Block startBlock = MapLegacy.instance.GetClosetBlockFromEntity(transform.position);
            Block endBlock = MapLegacy.instance.GetBlockFromClick(hit);

            if (startBlock == null || endBlock == null)
            {
                Debug.LogWarning("[Move] Start or End block is null for grid movement.");
                return;
            }

            List<Pathnode> path = PathfindingLegacy.instance.FindPath(startBlock, endBlock);

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("[Move] No path found in grid mode.");
                return;
            }

            
            string debugPath = "Path: ";
            foreach (var n in path) debugPath += n.ToString() + " -> ";
            Debug.Log(debugPath);

            // Get only Corners
            List<Vector3> straightPath = GetOnlyCorners(path);

            // Visualize corners
            for (int i = 0; i < straightPath.Count; i++)
            {
                Debug.DrawRay(straightPath[i] + Vector3.up * 0.5f, Vector3.up * 3f, Color.cyan, 4f);
            }


            // Follow Each corner instead of each node
            targetInteractiveObject = target;
            StartCoroutine(FollowCorners(straightPath));
        }


    }
    private IEnumerator FollowCorners(List<Vector3> corners)
    {
        // If there are no corners, just return
        if (corners == null || corners.Count == 0)
            yield break;

        foreach (Vector3 point in corners)
        {
            agent.acceleration = 99;
            agent.SetDestination(point);
            hasReachedDestination = false;

            // Wait until the agent reaches the point.
            // Use a small tolerance for remainingDistance.
            while (agent.pathPending || agent.remainingDistance > 0.1f)
            {
                // if agent has no path and not moving, break to avoid infinite loop
                if (!agent.pathPending && !agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
                    break;

                yield return null;
            }

            // small yield to allow agent to settle rotation/velocity
            yield return null;
        }

        hasReachedDestination = true;
        OnEntityReachedDestination?.Invoke(this, EventArgs.Empty);
    }

    private List<Vector3> GetOnlyCorners(List<Pathnode> rawPath)
    {
        List<Vector3> finalPoints = new List<Vector3>();
        if (rawPath == null || rawPath.Count == 0) return finalPoints;

        // If the path only contains one node, just return its world position
        if (rawPath.Count == 1)
        {
            finalPoints.Add(rawPath[0].GetWorldPosition());
            return finalPoints;
        }

        Vector3 prevDir = Vector3.zero;

        for (int i = 1; i < rawPath.Count; i++)
        {
            Vector3 prev = rawPath[i - 1].GetWorldPosition();
            Vector3 cur = rawPath[i].GetWorldPosition();

            Vector3 dir = (cur - prev).normalized;

            // Add the starting location if its the first count
            if (i == 1)
            {
                finalPoints.Add(prev);
            }
            else
            {
                // Compare angle between node to check if it a corner
                float angle = Vector3.Angle(prevDir, dir);
                
                if (angle > 1.5f) 
                {
                    finalPoints.Add(prev);
                }
            }

            prevDir = dir;
        }

        
        finalPoints.Add(rawPath[rawPath.Count - 1].GetWorldPosition());

        // Remove duplicate
        for (int i = finalPoints.Count - 1; i > 0; i--)
        {
            if (Vector3.Distance(finalPoints[i], finalPoints[i - 1]) < 0.001f)
                finalPoints.RemoveAt(i);
        }

        return finalPoints;
    }


    



    public Block getBlockBellow()
    {
        
        Vector3 start = transform.position + Vector3.up * 0.1f;

        
        Ray ray = new Ray(start, Vector3.down);

        
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        
        if (hits.Length == 0)
            return null;

        
        RaycastHit closestHit = hits[0];
        float closestDistance = hits[0].distance;

        for (int i = 1; i < hits.Length; i++)
        {
            if (hits[i].distance < closestDistance)
            {
                closestHit = hits[i];
                closestDistance = hits[i].distance;
            }
        }

        
        return closestHit.collider.gameObject.GetComponent<Block>();
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
