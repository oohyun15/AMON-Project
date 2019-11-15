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
                    Debug.LogError("There's no active AudioManager object");
                }
            }
            return instance;
        }
    }

    [Header("Sound")]
    private AudioSource[] audioPlayers = new AudioSource[6];
    public AudioClip[] gmBgmAudioClips, gmEfAudioClips, playerAudioClips, patientAudioClips, lobbyBgmAudioClips, lobbyEfAudioClips;
    private float controllBgmVolume, controllEffectVolume;

    void Awake()
    {
        UserDataIO.User user = UserDataIO.ReadUserData();
        controllBgmVolume = user.BgmVolume;
        controllEffectVolume = user.EffectVolume;

        // 0 = GameManageAudioPlayer, 1 = PlayerAudioPlayer, 2 = PatientAudioPlayer
        for (int i = 0; i < 2; i++)
        {
            audioPlayers[i] = gameObject.AddComponent<AudioSource>() as AudioSource;
            audioPlayers[i].volume = controllBgmVolume;
        }
        
        for(int j = 2; j < 6; j++)
        {
            audioPlayers[j] = gameObject.AddComponent<AudioSource>() as AudioSource;
            audioPlayers[j].volume = controllEffectVolume;
        }
    }

    public void PlayAudio(string sourceName, int clipNum, float startTime, bool _isLoop)
    {
        switch (sourceName)
        {
            case "GameManagerBgm": // 0번 오디오 소스
                if (audioPlayers[0].clip == gmBgmAudioClips[clipNum]) return;

                audioPlayers[0].clip = gmBgmAudioClips[clipNum];
                audioPlayers[0].loop = _isLoop;
                audioPlayers[0].time = startTime;
                audioPlayers[0].Play();
                break;

            case "LobbyBgm": // 1번 오디오 소스
                if (audioPlayers[1].clip == lobbyBgmAudioClips[clipNum]) return;

                audioPlayers[1].clip = lobbyBgmAudioClips[clipNum];
                audioPlayers[1].loop = _isLoop;
                audioPlayers[1].time = startTime;
                audioPlayers[1].Play();
                break;

            case "GameManagerEffect": // 2번 오디오 소스
                if (audioPlayers[2].clip == gmEfAudioClips[clipNum]) return;

                audioPlayers[2].clip = gmEfAudioClips[clipNum];
                audioPlayers[2].loop = _isLoop;
                audioPlayers[2].time = startTime;
                audioPlayers[2].Play();
                break;

            case "LobbyEffect": // 3번 오디오 소스
                if (audioPlayers[3].clip == lobbyEfAudioClips[clipNum]) return;

                audioPlayers[3].clip = lobbyEfAudioClips[clipNum];
                audioPlayers[3].loop = _isLoop;
                audioPlayers[3].time = startTime;
                audioPlayers[3].Play();
                break;

            case "Player": // 4번 오디오 소스
                if (audioPlayers[4].clip == playerAudioClips[clipNum]) return;

                audioPlayers[4].clip = playerAudioClips[clipNum];
                audioPlayers[4].loop = _isLoop;
                audioPlayers[4].time = startTime;
                audioPlayers[4].Play();
                break;

            case "Patient": // 5번 오디오 소스
                if (audioPlayers[5].clip == patientAudioClips[clipNum]) return;

                audioPlayers[5].clip = patientAudioClips[clipNum];
                audioPlayers[5].loop = _isLoop;
                audioPlayers[5].time = startTime;
                audioPlayers[5].Play();
                break;
        }
    }

    public void StopAudio()
    {
        for (int i = 0; i < 3; i++) audioPlayers[i].clip = null;
    }

    public void BGMVolumeControll(float _audioVolume)
    {
        UserDataIO.User user = UserDataIO.ReadUserData();

        audioPlayers[0].volume = _audioVolume;  //GameManager BGM
        audioPlayers[1].volume = _audioVolume;  //Lobby BGm

        controllBgmVolume = _audioVolume;
        user.BgmVolume = controllBgmVolume;    
        UserDataIO.WriteUserData(user);
    }

    public void EffectVolumeControll(float _audioVolume)
    {
        UserDataIO.User user = UserDataIO.ReadUserData();

        audioPlayers[2].volume = _audioVolume;  //GameManager Effect
        audioPlayers[3].volume = _audioVolume;  //Lobby Effect
        audioPlayers[4].volume = _audioVolume;  //Player
        audioPlayers[5].volume = _audioVolume;  //Patient

        controllEffectVolume = _audioVolume;
        user.EffectVolume = controllEffectVolume;
        UserDataIO.WriteUserData(user);
    }
}
