using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ClickToMove : Entity
{
    
    [SerializeField] private GameInput gameInput;
    
    public override void otherSettings()
    {
        gameInput.OnShoot += GameInput_OnShoot;
    }
    private void GameInput_OnShoot(object sender, System.EventArgs e)
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {

                Move(hit);
            }
        }
        
    }


}
