/****************************************
 * AudioManager.cs
 * 제작: 김태윤
 * AudioSource 및 Clip 관리하는 스크립트
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * 작성일자: 19.11.15.
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(AudioManager)) as AudioManager;

                if (instance == null)
                {
                    Debug.LogError("There's no active GameManager object");
                }
            }
            return instance;
        }
    }

    [Header("Sound")]
    private AudioSource[] audioPlayers = new AudioSource[3];
    public AudioClip[] gmAudioClips, playerAudioClips, patientAudioClips;

    public float audioVolume;

    // Start is called before the first frame update
    void Awake()
    {
        // 0 = GameManageAudioPlayer, 1 = PlayerAudioPlayer, 2 = PatientAudioPlayer
        for (int i = 0; i < 3; i++) audioPlayers[i] = gameObject.AddComponent<AudioSource>() as AudioSource;        
    }

    public void PlayAudio(string sourceName, int clipNum, float startTime, bool _isLoop)
    {
        switch (sourceName)
        {
            case "GameManager":
                if (audioPlayers[0].clip == gmAudioClips[clipNum]) return;

                audioPlayers[0].clip = gmAudioClips[clipNum];
                audioPlayers[0].loop = _isLoop;
                audioPlayers[0].time = startTime;
                audioPlayers[0].Play();
                break;

            case "Player":
                if (audioPlayers[1].clip == playerAudioClips[clipNum]) return;

                audioPlayers[1].clip = playerAudioClips[clipNum];
                audioPlayers[1].loop = _isLoop;
                audioPlayers[1].time = startTime;
                audioPlayers[1].Play();
                break;

            case "Patient":
                if (audioPlayers[1].clip == patientAudioClips[clipNum]) return;

                audioPlayers[1].clip = patientAudioClips[clipNum];
                audioPlayers[1].loop = _isLoop;
                audioPlayers[1].time = startTime;
                audioPlayers[1].Play();
                break;
        }
    }

    public void StopAudio()
    {
        for (int i = 0; i < 3; i++) audioPlayers[i].clip = null;
    }
}
