using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject selectedGAmeObject;


    public void Hide()
    {
        meshRenderer.enabled = false;
    }

    public void Show(Material matetial)
    {
        meshRenderer.enabled = true;
        meshRenderer.material = matetial;
    }

    public void HideSelected()
    {
        selectedGAmeObject.SetActive(false);
    }

    public void ShowSelected()
    {
        selectedGAmeObject.SetActive(true);
    }
}
