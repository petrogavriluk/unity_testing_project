using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    Button openMenuButton;
    bool isShown = false;
    // Use this for initialization
    void Start()
    {
        openMenuButton.onClick.AddListener(() => ChangeMenuState());
        UpdateMenuState();
    }

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
