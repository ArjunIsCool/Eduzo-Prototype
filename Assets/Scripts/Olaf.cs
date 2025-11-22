using DG.Tweening;
using UnityEngine;

public class Olaf : MonoBehaviour
{

    public float minTime, maxTime;
    public float shakeAmt;

    float timer;

    bool shaking;

    void Start()
    {
        ResetShake();
    }

    void ResetShake()
    {
        shaking = false;
        timer = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        if (shaking) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            DoIdleShakeAnimation();
        }
    }

    void DoIdleShakeAnimation()
    {
        shaking = true;

        transform.GetComponent<RectTransform>().DOPunchRotation(new Vector3(0f, 0f, shakeAmt), 1f).OnComplete(() =>
        {
            ResetShake();
        });
    }


}
