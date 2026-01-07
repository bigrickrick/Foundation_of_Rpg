using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInputTest : MonoBehaviour
{
    private Vector3 m_lastPosition;
    public LayerMask groundLayerMask;
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (Physics.Raycast(ray, out hit, 100,groundLayerMask))
        {
            m_lastPosition = hit.point;
        }
        return m_lastPosition;
        
    }

    
}
