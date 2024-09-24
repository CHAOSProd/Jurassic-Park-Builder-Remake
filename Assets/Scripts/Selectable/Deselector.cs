using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deselector : MonoBehaviour
{
    private void OnMouseUp()
    {
        if (PointerOverUIChecker.Instance.IsPointerOverUIObject()) return;
        SelectablesManager.Instance.UnselectAll();
    }
}
