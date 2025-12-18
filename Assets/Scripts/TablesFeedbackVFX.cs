using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eduzo.Games.Tables.UI
{
    public class TablesFeedbackVFX : MonoBehaviour
    {
        public List<GameObject> feedbackObjs = new List<GameObject>();
        public Vector2 maxDistancePadding; //Padding from the edges of the screen

        public int noOfInsts;
        public int burstRate; 

        public void PlayFeedbackVFX()
        {
            StartCoroutine(PlayFeedbackVFXRoutine());
        }

        IEnumerator PlayFeedbackVFXRoutine()
        {
            int burstCount = 0;
            for (int i = 0; i < noOfInsts; i++)
            {
                int randomIndex = Random.Range(0, feedbackObjs.Count);

                float randomX = Random.Range(0 + maxDistancePadding.x, Screen.width - maxDistancePadding.x);
                float randomY = Random.Range(0 + maxDistancePadding.y, Screen.height - maxDistancePadding.y);

                Vector2 randomPos = Camera.main.ScreenToWorldPoint(new Vector2(randomX, randomY));
                
                Instantiate(feedbackObjs[randomIndex], randomPos, Quaternion.identity, transform);

                burstCount++;
                if(burstCount >= burstRate)
                {
                    burstCount = 0;
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        //Testing only
        //private void Update()
        //{
        //    if(Input.GetKeyDown(KeyCode.E))
        //    {
        //        PlayFeedbackVFX();
        //    }
        //}
    }
}
