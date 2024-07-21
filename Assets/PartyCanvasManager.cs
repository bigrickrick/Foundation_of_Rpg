using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PartyCanvasManager : MonoBehaviour
{
    public Party party;
    public List<Button> PartyList;
    public Canvas canvas;
    public Button buttonPrefab;

    private void Start()
    {
        if (party == null || party.PartyMembers == null || canvas == null || buttonPrefab == null)
        {
            Debug.LogError("PartyCanvasManager is not properly initialized.");
            return;
        }
        InitializePartyMemberButtons();
        SortButtons();
    }
    private void Update()
    {
        SortButtons();
    }

    public void InitializePartyMemberButtons()
    {
        foreach (Entity member in party.PartyMembers)
        {
           
            Button newButton = Instantiate(buttonPrefab, canvas.transform);


            if (member.EntityPortrait != null)
            {
                newButton.image.sprite = member.EntityPortrait;
            }

            
            newButton.onClick.AddListener(() => party.SwitchLeader(member));

           
            PartyList.Add(newButton);
        }
    }
    public void SortButtons()
    {
        // Assuming all buttons have the same width and height
        if (PartyList.Count == 0) return;

        float spacing = 10f; // Space between buttons
        float totalWidth = (PartyList.Count * buttonPrefab.GetComponent<RectTransform>().sizeDelta.x) + ((PartyList.Count - 1) * spacing);
        float startX = -totalWidth / 2; // Center the buttons
        float startY = 10f;  // Starting Y offset from the bottom

        for (int i = 0; i < PartyList.Count; i++)
        {
            RectTransform rectTransform = PartyList[i].GetComponent<RectTransform>();

            // Set anchor and pivot to bottom center
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);

            // Calculate the position for each button
            float posX = startX + i * (rectTransform.sizeDelta.x + spacing);
            float posY = startY;

            // Set the position of the button relative to the screen size
            rectTransform.anchoredPosition = new Vector2(posX, posY);
        }
    }
}
