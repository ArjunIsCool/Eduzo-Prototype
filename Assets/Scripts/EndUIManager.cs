using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndUIManager : MonoBehaviour
{
    public TMP_Text scoreText;

    public Transform starsHolder;

    public GameObject wonRibbon;
    public GameObject lostRibbon;

    [Header("SOUNDS")]
    public AudioSource wonGameSFX;
    public AudioSource lostGameSFX;

    public GameObject wonGameConfettiFX;

    public static EndUIManager Instance;

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

    public void UpdateEndScreenInfo(int scorePercentage)
    {
        if(scorePercentage >= 50) //Minimum 50% to win
        {
            wonRibbon.SetActive(true);
            lostRibbon.SetActive(false);
            
            wonGameSFX.Play();
            wonGameConfettiFX.SetActive(true);
        }else
        {
            wonRibbon.SetActive(false);
            lostRibbon.SetActive(true);

            lostGameSFX.Play();
            wonGameConfettiFX.SetActive(false);
        }

        scoreText.text = $"{scorePercentage}%";

        int stars;

        if (scorePercentage >= 75) // 75% - 3 stars
        {
            stars = 3;
        }
        else if (scorePercentage >= 30) //30% - 2 stars
        {
            stars = 2;
        } else //1 star guaranteed for playing
        {
            stars = 1;
        }

        for (int i = 2; i >= 0; i--)
        {
            if (i < stars)
            {
                starsHolder.GetChild(i).GetComponent<Star>().SetStar(true);
            }
            else
            {
                starsHolder.GetChild(i).GetComponent<Star>().SetStar(false);
            }
        }
    }

}
