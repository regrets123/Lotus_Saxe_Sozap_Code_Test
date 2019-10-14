using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FillBar : MonoBehaviour
{
    [SerializeField]
    public GameObject Winner;
    public float StartValue, EndValue;
    public bool StartFilling { get; set; }
    [SerializeField]
    private Image _image;


    public IEnumerator AnimateBar(float intervall, float currentFill,int playerIndex)
    {   //calls on itself with intervals until the bar is filled. This way we avoid having a constant check in update for bools if I used a Lerp method instead.

        if (currentFill > EndValue)
        {
            if (EndValue > 149.9)
            {
                Winner.SetActive(true);
                RoundsManager.Instance.StartCelebration(playerIndex);
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(intervall);
            currentFill += 0.5f;
            _image.fillAmount = currentFill / 150;
            StartCoroutine(AnimateBar(intervall, currentFill,playerIndex));
        }
    }

    public void SetFill(float score)
    {
       _image.fillAmount =  score / 150;
    }
}
