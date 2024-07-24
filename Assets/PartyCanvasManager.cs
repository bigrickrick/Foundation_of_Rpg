using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyCanvasManager : MonoBehaviour
{
    public Party party;
    public GameObject buttonParent; 
    public List<Button> buttonlist; 

    private void Start()
    {
        InitializePartyMemberButtons();
    }

   
    public void InitializePartyMemberButtons()
    {
        
        foreach (Button button in buttonlist)
        {
            button.onClick.RemoveAllListeners();
            button.image.sprite = null;
        }

        int i = 0;

        // Assign each button to a party member
        foreach (Entity member in party.PartyMembers)
        {
            if (i >= buttonlist.Count)
            {
                break; 
            }

            Button button = buttonlist[i];

            if (member.EntityPortrait != null)
            {
                button.image.sprite = member.EntityPortrait;
            }

            
            Entity capturedMember = member;
            button.onClick.AddListener(() => party.SwitchLeader(capturedMember));

            i++;
        }

        
        for (int j = i; j < buttonlist.Count; j++)
        {
            buttonlist[j].image.sprite = null;
            buttonlist[j].onClick.RemoveAllListeners();
            buttonlist[j].gameObject.SetActive(false); 
        }
    }
}
