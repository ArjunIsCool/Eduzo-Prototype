using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public Slider loadingBar;

    float targetProgress;

    private void Start()
    {
        StartCoroutine(LoadGameSmoothly());
    }

    IEnumerator LoadGameSmoothly()
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);

        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            targetProgress = progress;

            if (loadingBar.value >= 1f) //This is necessary as isDone flas never really works because of the 90% rule
            {
                loadingOperation.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }
    }

    private void Update()
    {
        loadingBar.value = Mathf.MoveTowards(loadingBar.value, targetProgress, 2  * Time.deltaTime);
    }
}
