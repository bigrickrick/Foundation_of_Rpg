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
    
    private LineRenderer LinePath;
    private NavMeshTriangulation Triangulation;
    private Vector3 targetPosition; 
    

    private void Start()
    {
        gameInput.OnShoot += GameInput_OnShoot;
        LinePath = GetComponent<LineRenderer>();
        LinePath.positionCount = 0;
        LinePath.startWidth = 0.15f;
        LinePath.endWidth = 0.15f;
        LinePath.material = new Material(Shader.Find("Sprites/Default")); 
        LinePath.startColor = Color.red;
        LinePath.endColor = Color.red;
        ClickMarker.SetActive(false);


    }

    private void Update()
    {
        Triangulation = NavMesh.CalculateTriangulation();
        MoveMouseAim();

        if (GameManager.Instance.movementMode == GameManager.MovementMode.Grid)
        {
           
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                
                DrawPath(hit);
            }
            else
            {
                
                LinePath.positionCount = 0;
                LinePath.enabled = false;
            }
        }

        
        else if (party.CurrentEntity.agent.hasPath)
        {
            //DrawPath(); 
        }
    }


    // Mouse aim
    private void MoveMouseAim()
    {
        Vector3 mousePos = Input.mousePosition;
        Debug.Log("Mouse aim P1:" + mousePos);
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

            if (GameManager.Instance.movementMode == GameManager.MovementMode.Normal)
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

                LinePath.positionCount = sampledPath.Count;
                for (int i = 0; i < sampledPath.Count; i++)
                {
                    LinePath.SetPosition(i, sampledPath[i]);
                }
            }
           
        }
    }
    private void DrawPath(RaycastHit hit)
    {
        if (GameManager.Instance.movementMode != GameManager.MovementMode.Grid)
            return;

        Block startBlock = MapLegacy.instance.GetClosetBlockFromEntity(party.CurrentEntity.transform.position);
        Block endBlock = MapLegacy.instance.GetBlockFromClick(hit);

        if (startBlock == null || endBlock == null)
        {
            LinePath.positionCount = 0;
            LinePath.enabled = false;
            return;
        }

        List<Pathnode> path = PathfindingLegacy.instance.FindPath(startBlock, endBlock);

        if (path == null || path.Count == 0)
        {
            LinePath.positionCount = 0;
            LinePath.enabled = false;
            return;
        }

        List<Vector3> pathPositions = new List<Vector3>();

        for (int i = 0; i < path.Count; i++)
        {
            Pathnode node = path[i];
            if (node == null) continue;

            
            Vector3 pos = node.transform.position;

            
            pos.y += node.transform.localScale.y / 2f;

            pathPositions.Add(pos);

            
            if (i < path.Count - 1)
            {
                Pathnode nextNode = path[i + 1];
                Vector3 nextPos = nextNode.transform.position;
                nextPos.y += nextNode.transform.localScale.y / 2f;

                
                Vector3 midpoint = (pos + nextPos) / 2f;
                midpoint.y = Mathf.Max(pos.y, nextPos.y);
                pathPositions.Add(midpoint);
            }
        }

        if (pathPositions.Count > 0)
        {
            LinePath.positionCount = pathPositions.Count;
            LinePath.SetPositions(pathPositions.ToArray());
            LinePath.enabled = true;
        }
        else
        {
            LinePath.positionCount = 0;
            LinePath.enabled = false;
        }
    }

  





    private void GameInput_OnShoot(object sender, System.EventArgs e)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null) continue;

            GameObject hitObject = hit.collider.gameObject;
            SeeThroughTerrain seeThroughComponent = hitObject.GetComponent<SeeThroughTerrain>();

            if (seeThroughComponent != null && seeThroughComponent.IsSeeThrough())
                continue;

            Interact(hitObject, hit);
            return;
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
            DrawPath(location);
            party.CurrentEntity.Move(location);
            Debug.Log("Click location: " + location.point);
            targetPosition = location.point;
            ShowPathCoroutine(location);
        }
    }
    private IEnumerator ShowPathCoroutine(RaycastHit location)
    {
        
        DrawPath(location);

        
        yield return new WaitForSeconds(5f);

        
    }


    private Modifiers Whatisit(GameObject hitObject)
    {
        return hitObject.GetComponent<Modifiers>();
    }
}
