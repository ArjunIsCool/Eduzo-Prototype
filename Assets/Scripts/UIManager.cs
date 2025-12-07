using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject menuUI, gameUI, endUI;

    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void ShowMenuUI()
    {
        menuUI.SetActive(true);
        gameUI.SetActive(false);
        endUI.SetActive(false);
    }

    public void ShowGameUI()
    {
        menuUI.SetActive(false);
        gameUI.SetActive(true);
        endUI.SetActive(false);

        GameplayManager.Instance.InitializeGameplay(MenuUIManager.Instance.GetGameSettings());
    }

    public void ShowEndScreenUI()
    {
        menuUI.SetActive(false);
        gameUI.SetActive(false);
        endUI.SetActive(true);
    }
}
