using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour, IObserver
{
    private ISubject achievementController;
    private int playCount;

    public Achievement(ISubject _achievementController)
    {
        achievementController = _achievementController;

        // 옵저버 등록
        achievementController.RegisterObserver(this);
    }

    public void _update(UserDataIO.User user)
    {
        playCount = user.playCount;

        if (playCount >= 10) GetComponent<Image>().color = Color.green;
    }
}
