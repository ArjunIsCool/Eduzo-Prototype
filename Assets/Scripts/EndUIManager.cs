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

    public void UpdateEndScreenInfo(bool won, int score, int maxPossibleScore)
    {
        if(won)
        {
            wonRibbon.SetActive(true);
            lostRibbon.SetActive(false);
            
            wonGameSFX.Play();
        }else
        {
            wonRibbon.SetActive(false);
            lostRibbon.SetActive(true);

            lostGameSFX.Play();
        }

        scoreText.text = score.ToString();

        int stars;

        float percentage = (float)score / maxPossibleScore;

        if (percentage >= 0.75) // 75% - 3 stars
        {
            stars = 3;
        }
        else if (percentage >= 0.30) //30% - 2 stars
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
