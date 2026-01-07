using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Party : MonoBehaviour
{
    public List<EntityLegacy> PartyMembers;
    public EntityLegacy CurrentEntity;
    [SerializeField] private List<Node> NodeList;

    
    private float nodeSpacing = 4f;
    
    public Node NodePrefab;
    private void Start()
    {
        CreatePartyNodes();
        
        
    }

    


    private void CreatePartyNodes()
    {
        //NodeList.Clear();


        int numFollowers = PartyMembers.Count - 1;  
        int nodesToCreate = Mathf.CeilToInt(Mathf.Sqrt(numFollowers));  

        Vector3 centerPosition = CurrentEntity.transform.position;
        int y = 0;
        int x = 0;
        foreach(EntityLegacy follower in PartyMembers)
        {
            if(follower != CurrentEntity)
            {
                Vector3 offset = new Vector3(x * nodeSpacing - (nodesToCreate - 1) * nodeSpacing / 2f,
                                                0f,
                                                y * nodeSpacing - (nodesToCreate - 1) * nodeSpacing / 2f);
                Vector3 nodePosition = centerPosition + offset;
                x++;
                y++;
                Node newNode = Instantiate(NodePrefab, nodePosition, Quaternion.identity);
                follower.node = newNode;
                NodeList.Add(newNode);
            }
        }
        
    }





    public void Update()
    {
        foreach (EntityLegacy entity in PartyMembers)
        {
            if (entity != CurrentEntity)
            {
                float distanceToLeader = Vector3.Distance(entity.transform.position, CurrentEntity.transform.position);
                if (distanceToLeader > 5f)
                {
                    Gotoleader();
                    //break; 
                }
            }
        }
        
        UpdateNodePositions();

    }

    

    public void Gotoleader()
    {
        foreach (EntityLegacy entity in PartyMembers)
        {
            if (entity != CurrentEntity)
            {
                FollowLeader(CurrentEntity, entity);
            }
        }
    }

    public void FollowLeader(EntityLegacy leader, EntityLegacy follower)
    {
        if (leader != null)
        {
            float distanceToLeader = Vector3.Distance(follower.transform.position, leader.transform.position);

            if (distanceToLeader > 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(follower.node.transform.position, Vector3.down, out hit))
                {
                    follower.Move(hit);
                }
            }
        }
    }

    

    private void UpdateNodePositions() //relative to the leader
    {
        Vector3 centerPosition = CurrentEntity.transform.position;

        
        int nodesToCreate = Mathf.CeilToInt(Mathf.Sqrt(NodeList.Count));
        for (int y = 0; y < nodesToCreate; y++)
        {
            for (int x = 0; x < nodesToCreate; x++)
            {
                int index = x + y * nodesToCreate;
                if (index < NodeList.Count)
                {
                    Vector3 offset = new Vector3(x * nodeSpacing - (nodesToCreate - 1) * nodeSpacing / 2f,
                                                 0f,
                                                 y * nodeSpacing - (nodesToCreate - 1) * nodeSpacing / 2f);
                    NodeList[index].transform.position = centerPosition + offset;
                }
            }
        }
    }
    public void SwitchLeader(EntityLegacy newLeader)
    {
        if (newLeader == null || !PartyMembers.Contains(newLeader))
        {
            Debug.LogError("Invalid new leader entity.");
            return;
        }
        if (newLeader == CurrentEntity)
        {
            Debug.LogWarning("The selected entity is already the leader.");
            return;
        }
        Node tempNode = CurrentEntity.node;
        CurrentEntity.node = newLeader.node;
        newLeader.node = tempNode;
        CurrentEntity = newLeader;
        UpdateNodePositions();
    }

    public void AddFollower(EntityLegacy newFollower)
    {
        if (newFollower == null)
        {
            Debug.LogError("Cannot add null entity as a follower.");
            return;
        }

        if (PartyMembers.Contains(newFollower))
        {
            Debug.LogWarning("Entity is already a follower.");
            return;
        }

        PartyMembers.Add(newFollower);

        
        int numFollowers = PartyMembers.Count - 1;  
        int nodesToCreate = Mathf.CeilToInt(Mathf.Sqrt(numFollowers));

        Vector3 centerPosition = CurrentEntity.transform.position;
        int y = numFollowers / nodesToCreate; 
        int x = numFollowers % nodesToCreate; 

        Vector3 offset = new Vector3(x * nodeSpacing - (nodesToCreate - 1) * nodeSpacing / 2f,
                                     0f,
                                     y * nodeSpacing - (nodesToCreate - 1) * nodeSpacing / 2f);
        Vector3 nodePosition = centerPosition + offset;

        Node newNode = Instantiate(NodePrefab, nodePosition, Quaternion.identity);
        newFollower.node = newNode;
        NodeList.Add(newNode);

        
        UpdateNodePositions();
    }
}
    

