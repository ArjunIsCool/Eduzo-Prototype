using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    public Sprite starOn, starOff;

    public void SetStar(bool on)
    {
        if(on)
        {
            GetComponent<Image>().sprite = starOn;
        } else
        {
            GetComponent<Image>().sprite = starOff;
        }
    }
}
