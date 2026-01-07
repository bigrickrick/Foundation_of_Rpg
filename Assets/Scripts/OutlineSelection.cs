using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelection : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        
        Gizmos.color = Color.red;

      
        Gizmos.DrawRay(ray.origin, ray.direction * 100000f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            
            Gizmos.color = Color.green;

            
            Gizmos.DrawSphere(hit.point, 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(hit.point, hit.point + hit.normal * 0.5f);
        }
    }
    void Update()
    {
        
        if (highlight != null)
        {
            Outline prevOutline = highlight.GetComponent<Outline>();
            if (prevOutline != null) prevOutline.enabled = false;
            highlight = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log("Mouse aim P2:" + Input.mousePosition);



        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if (!overUI && Physics.Raycast(ray, out raycastHit))
        {
            Debug.Log("Mouse aim P3:" + raycastHit.transform.localPosition);
            highlight = raycastHit.transform;

            Modifiers mod = highlight.GetComponent<Modifiers>();
            if (mod != null && highlight != selection && mod.currentType != Modifiers.Type.IsABlock)
            {
                Outline outline = highlight.GetComponent<Outline>();
                if (outline == null) outline = highlight.gameObject.AddComponent<Outline>();

                outline.enabled = true;
                outline.OutlineColor = Color.magenta;
                outline.OutlineWidth = 7.0f;
            }
            else if (raycastHit.transform.GetComponent<Block>())
            {
                Outline outline = highlight.GetComponent<Outline>();
                if (outline == null) outline = highlight.gameObject.AddComponent<Outline>();

                outline.enabled = true;
                outline.OutlineColor = Color.magenta;
                outline.OutlineWidth = 7.0f;

            }
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            if (highlight != null)
            {
                if (selection != null)
                {
                    Outline selOutline = selection.GetComponent<Outline>();
                    if (selOutline != null) selOutline.enabled = false;
                }

                selection = raycastHit.transform;
                Outline newOutline = selection.GetComponent<Outline>();
                if (newOutline != null) newOutline.enabled = true;

                highlight = null;
            }
            else
            {
                if (selection != null)
                {
                    Outline selOutline = selection.GetComponent<Outline>();
                    if (selOutline != null) selOutline.enabled = false;
                    selection = null;
                }
            }
        }
    }
}
