using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    bool isShown = false;
    // Use this for initialization
    void Start()
    {
        UpdateMenuState();
    }

    // Update is called once per frame
    void UpdateMenuState()
    {
        gameObject.SetActive(isShown);
    }

    public void ChangeMenuState()
    {
        isShown = !isShown;
        UpdateMenuState();
    }
}
