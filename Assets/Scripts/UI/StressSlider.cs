﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressSlider : MonoBehaviour
{
    private Image stressSlider;
    private Text stressText;

    private UserDataIO.User user;

    // Start is called before the first frame update
    // (19.10.10.예진) 피로도 텍스트 설정
    void Start()
    {
        stressSlider = GetComponent<Image>();
        stressText = transform.GetChild(0).GetComponent<Text>();

        user = UserDataIO.ReadUserData();

        stressText.text = user.stress + "%";

    }

    // Update is called once per frame
    void Update()
    {
        stressSlider.fillAmount = user.stress * 0.01f;
    }
}
