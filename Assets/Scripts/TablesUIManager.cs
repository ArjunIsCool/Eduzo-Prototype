using Eduzo.Games.Tables.Core;
using Eduzo.Games.Tables.Data;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Eduzo.Games.Tables.UI
{
    public class TablesUIManager : MonoBehaviour
    {
        public GameObject menuUI, gameUI, endUI;

        public static TablesUIManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
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

            TablesGameSettings gameSettings = new TablesGameSettings(TablesMenuUIManager.Instance.GetGameSettings());
            TablesGameplayManager.Instance.InitializeGameplay(gameSettings);
        }

        public void ShowEndScreenUI()
        {
            menuUI.SetActive(false);
            gameUI.SetActive(false);
            endUI.SetActive(true);
        }
    }
}