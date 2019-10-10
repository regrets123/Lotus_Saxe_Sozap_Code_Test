using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FillBar : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    public float StartValue { get; set; }
    public float EndValue { get; set; }
    public bool StartFilling { get; set; }
    private float time = 0;
    private float fillspeed = 1;
    [SerializeField]
    public GameObject Winner;

    public IEnumerator AnimateBar(float intervall, float currentFill)
    {   //calls on itself with intervals until the bar is filled. This way we avoid having a constant check in update for bools if I used a Lerp method instead.

        if (currentFill > EndValue)
        {
            if (EndValue > 149.9)
            {
                Winner.SetActive(true);
                GameManager.Instance.roundsManager.StartCeleb();
                yield return null;
            }

        }
        else
        {
            yield return new WaitForSeconds(intervall);
            currentFill += 0.2f;
            _image.fillAmount = currentFill / 150;
            StartCoroutine(AnimateBar(time, currentFill));
        }
    }
}
