using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject menuUI, gameUI, endUI;

    [Header("SETTINGS")]
    public AudioMixer gameAudioMixer;

    public Image soundIcon, musicIcon;
    public Sprite soundOnIcon, soundOffIcon, musicOnIcon, musicOffIcon;

    bool soundOn, musicOn;

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

        soundOn = true;
        musicOn = true;
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
    }


    public void ShowEndScreenUI()
    {
        menuUI.SetActive(false);
        gameUI.SetActive(false);
        endUI.SetActive(true);
    }
    
    public void ToggleSound()
    {
        soundOn = !soundOn;

        if (soundOn)
        {
            gameAudioMixer.SetFloat("SoundVol", 0f);
            soundIcon.sprite = soundOnIcon;
        } else
        {
            gameAudioMixer.SetFloat("SoundVol", -80f);
            soundIcon.sprite = soundOffIcon;
        }
    }

    public void ToggleMusic()
    {
        musicOn = !musicOn;

        if (musicOn)
        {
            gameAudioMixer.SetFloat("MusicVol", 0f);
            musicIcon.sprite = musicOnIcon;
        }
        else
        {
            gameAudioMixer.SetFloat("MusicVol", -80f);
            musicIcon.sprite = musicOffIcon;
        }
    }

}
