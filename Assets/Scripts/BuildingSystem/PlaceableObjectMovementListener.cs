using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class PlaceableObjectMovementListener : MonoBehaviour
{
    public bool IsMoving;

    private PlaceableObject _placeableObject;
    private bool _isCanMove = true;

    private void Start()
    {
        UIManager.Instance.ChangeCameraPanningStatus(true);
        _placeableObject = GetComponentInParent<PlaceableObject>();
    }

    private void OnMouseDown()
    {
        if (GridBuildingSystem.Instance.TempPlaceableObject != _placeableObject || PointerOverUIChecker.Instance.IsPointerOverUIObject())
        {
            _isCanMove = false;
            UIManager.Instance.ChangeCameraPanningStatus(true);
            return;
        }
        UIManager.Instance.ChangeCameraPanningStatus(false);
        GridBuildingSystem.Instance.SaveObjectOffset();
        _isCanMove = true;
    }

    private void OnMouseDrag()
    {
        if (!_isCanMove)
            return;

        GridBuildingSystem.Instance.MoveObjectWithOffset();
        IsMoving = true;
    }

    private void OnMouseUp()
    {
        if (!_isCanMove)
            return;

        IsMoving = false;
    }
}
