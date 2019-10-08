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

    public void AnimateBar()
    {
        _image.fillAmount = Mathf.Lerp(StartValue, EndValue, time);
        time += fillspeed * Time.deltaTime;
        if (_image.fillAmount == EndValue) StartFilling = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (StartFilling)
            AnimateBar();
    }
}
