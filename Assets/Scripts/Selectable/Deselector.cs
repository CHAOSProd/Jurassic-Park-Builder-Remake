
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deselector : MonoBehaviour
{
    private Vector3 mouseDownPosition;
    [SerializeField] private float dragThreshold = 5f;
    private void OnMouseDown()
    {
        mouseDownPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (PointerOverUIChecker.Instance.IsPointerOverUIObject()) return;
        float distance = Vector3.Distance(mouseDownPosition, Input.mousePosition);
        if (distance > dragThreshold) return;
        SelectablesManager.Instance.UnselectAll();
    }
}
