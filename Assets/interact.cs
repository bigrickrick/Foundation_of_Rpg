using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems; // Import EventSystems for UI detection

public class interact : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Party party;
    [SerializeField] private GameObject ClickMarker;
    [SerializeField] private GameObject MouseLocation;
    [SerializeField] private float PathHeighOffset;
    private LineRenderer Path;
    private NavMeshTriangulation Triangulation;
    private Vector3 targetPosition; 
    

    private void Start()
    {
        gameInput.OnShoot += GameInput_OnShoot;
        Path = GetComponent<LineRenderer>();
        Path.positionCount = 0;
        Path.startWidth = 0.15f;
        Path.endWidth = 0.15f;
        ClickMarker.SetActive(false);


    }

    private void Update()
    {
        Triangulation = NavMesh.CalculateTriangulation();
        MoveMouseAim();

        if (party.CurrentEntity.HasReachHisdestination())
        {
            // Clear path or take other actions
            Path.positionCount = 0;
            ClickMarker.SetActive(false);
        }
        else if (party.CurrentEntity.agent.hasPath)
        {
            DrawPath();
        }
    }

    // Mouse aim
    private void MoveMouseAim()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        
        List<RaycastHit> sortedHits = new List<RaycastHit>(hits);

        
        sortedHits.Sort((a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in sortedHits)
        {
            if (hit.collider != null)
            {
                SeeThroughTerrain seeThroughComponent = hit.collider.GetComponent<SeeThroughTerrain>();

                if (seeThroughComponent != null && seeThroughComponent.IsSeeThrough())
                {
                    // If see-through, continue to the next hit
                    continue;
                }

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hit.point, out navHit, 1.0f, NavMesh.AllAreas))
                {
                    MouseLocation.transform.position = navHit.position + Vector3.up * PathHeighOffset;
                    return;
                }
            }
        }
    }







    // Draw Movement path
    private void DrawPath()
    {
        NavMeshPath path = new NavMeshPath();
        NavMeshAgent agent = party.CurrentEntity.GetComponent<NavMeshAgent>();

        if (agent.CalculatePath(targetPosition, path))
        {
            
            List<Vector3> sampledPath = new List<Vector3>();
            for (int i = 0; i < path.corners.Length; i++)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(path.corners[i], out hit, 1.0f, NavMesh.AllAreas))
                {
                    sampledPath.Add(hit.position + Vector3.up * PathHeighOffset);
                }
                else
                {
                    sampledPath.Add(path.corners[i]); 
                }
            }

            Path.positionCount = sampledPath.Count;
            for (int i = 0; i < sampledPath.Count; i++)
            {
                Path.SetPosition(i, sampledPath[i]);
            }
        }
    }


    private void GameInput_OnShoot(object sender, System.EventArgs e)
    {
        if (Input.GetMouseButtonDown(0))
        {
           
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null)
                {
                    GameObject hitObject = hit.collider.gameObject;
                    SeeThroughTerrain seeThroughComponent = hitObject.GetComponent<SeeThroughTerrain>();

                   
                    if (seeThroughComponent != null && seeThroughComponent.IsSeeThrough())
                    {
                        // If see-through, continue to the next hit
                        continue;
                    }

                    
                    Interact(hitObject, hit);
                    return; 
                }
            }
        }
    }
    private void Interact(GameObject hit, RaycastHit location)
    {
        var whatIsIt = Whatisit(hit);
        if (whatIsIt != null)
        {
            Modifiers.Type type = whatIsIt.currentType;
           
            
            if (type == Modifiers.Type.Interactiveterrain)
            {
                var interactable = hit.GetComponent<Interactable>();
                
                if (!interactable.IsEntityCloseEnough(party.CurrentEntity.transform.position))
                {
                    location.point = interactable.InteractablePoint(party.CurrentEntity);
                    party.CurrentEntity.Move(location, hit);
                    
                }
                else
                {
                    interactable.interact(party.CurrentEntity);
                }
            }
            else if (type == Modifiers.Type.IsAEntity)
            {
                party.CurrentEntity.Move(location);
                // start conversation
            }
            else if (type == Modifiers.Type.IsAHostileEntity)
            {
                // Attack the entity
            }
            else if (type == Modifiers.Type.IsAObject)
            {
                // pickup object and add it to inventory
            }
            targetPosition = location.point;
        }
        else
        {
            
            Debug.Log("It is ground/place where you can move");
            ClickMarker.SetActive(true);
            ClickMarker.transform.position = MouseLocation.transform.position;
            location.point = MouseLocation.transform.position;
            party.CurrentEntity.Move(location);
            Debug.Log("Click location: " + location.point);
            
            targetPosition = location.point;

        }
    }


    private Modifiers Whatisit(GameObject hitObject)
    {
        return hitObject.GetComponent<Modifiers>();
    }
}
