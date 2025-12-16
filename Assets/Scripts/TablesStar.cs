using UnityEngine;
using UnityEngine.UI;

namespace Eduzo.Games.Tables.UI
{
    public class TablesStar : MonoBehaviour
    {
        public Sprite starOn, starOff;

        public void SetStar(bool on)
        {
            if (on)
            {
                GetComponent<Image>().sprite = starOn;
            }
            else
            {
                GetComponent<Image>().sprite = starOff;
            }
        }
    }
}