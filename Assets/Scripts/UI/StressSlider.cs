using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressSlider : MonoBehaviour
{
    private Image stressSlider;

    private UserDataIO.User user;

    // Start is called before the first frame update
    void Start()
    {
        stressSlider = GetComponent<Image>();
        user = UserDataIO.ReadUserData();
    }

    // Update is called once per frame
    void Update()
    {
        stressSlider.fillAmount = user.stress * 0.01f;
    }
}
