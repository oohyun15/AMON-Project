/****************************************
 * AudioManager.cs
 * 제작: 김태윤
 * AudioSource 및 Clip 관리하는 스크립트
 * 함수 추가 및 수정 시 누가 작성했는지 꼭 해당 함수 주석으로 명시해주세요!
 * (19.12.01 예진) 로비씬인지 확인하는 변수 추가
 * 작성일자: 19.11.15.
 * 수정일자: 19.12.01. 
 ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

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

    public class MultiKeyDictionary<K1, K2, V> : Dictionary<K1, Dictionary<K2, V>>
    {

        public V this[K1 key1, K2 key2]
        {
            get
            {
                if (!ContainsKey(key1) || !this[key1].ContainsKey(key2))
                    throw new System.ArgumentOutOfRangeException();
                return base[key1][key2];
            }
            set
            {
                if (!ContainsKey(key1))
                    this[key1] = new Dictionary<K2, V>();
                this[key1][key2] = value;
            }
        }

        public void Add(K1 key1, K2 key2, V value)
        {
            if (!ContainsKey(key1))
                this[key1] = new Dictionary<K2, V>();
            this[key1][key2] = value;
        }

        public bool ContainsKey(K1 key1, K2 key2)
        {
            return base.ContainsKey(key1) && this[key1].ContainsKey(key2);
        }
    }

    private bool isLobby;
   

    [Header("Sound")]
    public AudioMixer masterMixer;
    public AudioSource[] audioPlayers = new AudioSource[9];
    public AudioClip[] gmBgmAudioClips, lobbyBgmAudioClips, gmEfAudioClips, lobbyEfAudioClips,
        playerAudioClips, UIAudioClips, ObsAudioClips, WarningAudioClips, TimeOutAudioClips;
    private Dictionary<string, AudioSource> audioDic;
    private Dictionary<string, AudioClip[]> clipDic;

    [Header("Button")]
    public Sprite onImage;
    public Sprite offImage;
    //public bool onOff;
    public Image BgmOnOff;
    public Image EffectOnOff;

    public GameObject fireParent;
    public List<AudioSource> fires;

    private MultiKeyDictionary<string, int, AudioMixerGroup> matchMixerGroup;

    void Awake()
    {
        if (GameManager.Instance != null) isLobby = false;
        else isLobby = true;

        UserDataIO.User user = UserDataIO.ReadUserData();
        if (!isLobby) fireParent = GameObject.Find("Fire");
        matchMixerGroup = new MultiKeyDictionary<string, int, AudioMixerGroup>();

        GetMixerGroup();

        //게임 시작 시 BGM 사운드 조절
        for (int i = 0; i < 2; i++)
        {
            audioPlayers[i] = gameObject.AddComponent<AudioSource>() as AudioSource;
            audioPlayers[i].volume = user.BgmVolume;
        }

        //게임 시작 시 이펙트 사운드 조절
        for (int j = 2; j < audioPlayers.Length; j++)
        {
            audioPlayers[j] = gameObject.AddComponent<AudioSource>() as AudioSource;
            audioPlayers[j].playOnAwake = false;
            audioPlayers[j].volume = user.EffectVolume;
        }
        
        //Dictionary 사용
        audioDic = new Dictionary<string, AudioSource>
        {
            ["GameManagerBgm"] = audioPlayers[0],
            ["LobbyBgm"] = audioPlayers[1],
            ["GameManagerEffect"] = audioPlayers[2],
            ["LobbyEffect"] = audioPlayers[3],
            ["Player"] = audioPlayers[4],
            ["UI"] = audioPlayers[5],
            ["Obstacle"] = audioPlayers[6],
            ["Warning"] = audioPlayers[7],
            ["TimeOut"] = audioPlayers[8]
        };

        clipDic = new Dictionary<string, AudioClip[]>
        {
            ["GameManagerBgm"] = gmBgmAudioClips,
            ["LobbyBgm"] = lobbyBgmAudioClips,
            ["GameManagerEffect"] = gmEfAudioClips,
            ["LobbyEffect"] = lobbyEfAudioClips,
            ["Player"] = playerAudioClips,
            ["UI"] = UIAudioClips,
            ["Obstacle"] = ObsAudioClips,
            ["Warning"] = WarningAudioClips,
            ["TimeOut"] = TimeOutAudioClips
        };

        //게임 시작시 fire fx 사운드 조절
        if (!isLobby)
        {
            for (int j = 0; j < fireParent.GetComponentsInChildren<AudioSource>().Length; j++)
            {
                fires.Insert(j, fireParent.transform.GetChild(j).GetComponent<AudioSource>());
                fires[j].volume = user.EffectVolume;
            }
        }

        Debug.Log(user.EffectVolume);

        //SettingButton 초기화
        Text onText = BgmOnOff.transform.GetChild(0).GetComponent<Text>();
        if (user.BgmVolume == 1f)
        {
            onText.text = "ON";
            BgmOnOff.overrideSprite = onImage;
        }
        else
        {
            onText.text = "OFF";
            BgmOnOff.overrideSprite = offImage;
        }

        onText = EffectOnOff.transform.GetChild(0).GetComponent<Text>();
        if (user.EffectVolume == 1f)
        {
            onText.text = "ON";
            EffectOnOff.overrideSprite = onImage;
        }
        else
        {
            onText.text = "OFF";
            EffectOnOff.overrideSprite = offImage;
        }
    }

    public void PlayAudio(string sourceName, int clipNum, float startTime, bool _isLoop)
    {
        SetMixerGroup(sourceName, clipNum);
        
        audioDic[sourceName].clip = clipDic[sourceName][clipNum];
        audioDic[sourceName].loop = _isLoop;
        audioDic[sourceName].time = startTime;
        audioDic[sourceName].Play();
        /* switch (sourceName)
         {
             case "GameManagerBgm": // 0번 오디오 소스
                 audioPlayers[0].clip = gmBgmAudioClips[clipNum];
                 audioPlayers[0].loop = _isLoop;
                 audioPlayers[0].time = startTime;
                 audioPlayers[0].Play();
                 break;

             case "LobbyBgm": // 1번 오디오 소스
                 audioPlayers[1].clip = lobbyBgmAudioClips[clipNum];
                 audioPlayers[1].loop = _isLoop;
                 audioPlayers[1].time = startTime;
                 audioPlayers[1].Play();
                 break;

             case "GameManagerEffect": // 2번 오디오 소스
                 audioPlayers[2].clip = gmEfAudioClips[clipNum];
                 audioPlayers[2].loop = _isLoop;
                 audioPlayers[2].time = startTime;
                 audioPlayers[2].Play();
                 break;

             case "LobbyEffect": // 3번 오디오 소스
                 audioPlayers[3].clip = lobbyEfAudioClips[clipNum];
                 audioPlayers[3].loop = _isLoop;
                 audioPlayers[3].time = startTime;
                 audioPlayers[3].Play();
                 break;

             case "Player": // 4번 오디오 소스
                 audioPlayers[4].clip = playerAudioClips[clipNum];
                 audioPlayers[4].loop = _isLoop;
                 audioPlayers[4].time = startTime;
                 audioPlayers[4].Play();
                 break;

             case "UI": // 5번 오디오 소스
                 audioPlayers[5].clip = UIAudioClips[clipNum];
                 audioPlayers[5].loop = _isLoop;
                 audioPlayers[5].time = startTime;
                 audioPlayers[5].Play();
                 break;

             case "Obstacle": // 6번 오디오 소스
                 audioPlayers[6].clip = ObsAudioClips[clipNum];
                 audioPlayers[6].loop = _isLoop;
                 audioPlayers[6].time = startTime;
                 audioPlayers[6].Play();
                 break;

             case "Warning": // 7번 오디오 소스
                 audioPlayers[7].clip = WarningAudioClips[clipNum];
                 audioPlayers[7].loop = _isLoop;
                 audioPlayers[7].time = startTime;
                 audioPlayers[7].Play();
                 break;

             case "TimeOut": // 8번 오디오 소스
                 audioPlayers[8].clip = WarningAudioClips[clipNum];
                 audioPlayers[8].loop = _isLoop;
                 audioPlayers[8].time = startTime;
                 audioPlayers[8].Play();
                 break;
         }*/
    }

    public void StopAllAudio()
    {
        for (int i = 0; i < audioPlayers.Length; i++) audioPlayers[i].clip = null;
        for (int j = 0; j < fires.Count; j++)
        {
            fires[j].volume = 0f;
        }
    }

    public void StopAudio(string sourceName)
    {
        audioDic[sourceName].clip = null;

        /* switch (sourceName)
         {
             case "GameManagerBgm": // 0번 오디오 소스
                 audioPlayers[0].clip = null;
                 break;

             case "LobbyBgm": // 1번 오디오 소스
                 audioPlayers[1].clip = null;
                 break;

             case "GameManagerEffect": // 2번 오디오 소스
                 audioPlayers[2].clip = null;
                 break;

             case "LobbyEffect": // 3번 오디오 소스
                 audioPlayers[3].clip = null;
                 break;

             case "Player": // 4번 오디오 소스
                 audioPlayers[4].clip = null;
                 break;

             case "UI": // 5번 오디오 소스
                 audioPlayers[5].clip = null;
                 break;

             case "Obstacle": // 7번 오디오 소스
                 audioPlayers[6].clip = null;
                 break;

             case "Warning": // 8번 오디오 소스
                 audioPlayers[7].clip = null;
                 break;
         }*/
    }

    public void BGMVolumeControll()
    {
        UserDataIO.User user = UserDataIO.ReadUserData();

        if (user.BgmVolume == 0f)
        {
            BgmOnOff.overrideSprite = onImage;
            user.BgmVolume = 1f;

            for (int i = 0; i < 2; i++)
            {
                audioPlayers[i].volume = user.BgmVolume;
            }

            Text onText = BgmOnOff.transform.GetChild(0).GetComponent<Text>();
            onText.text = "ON";
        }

        else
        {
            BgmOnOff.overrideSprite = offImage;
            user.BgmVolume = 0f;

            for (int i = 0; i < 2; i++)
            {
                audioPlayers[i].volume = user.BgmVolume;
            }

            Text offText = BgmOnOff.transform.GetChild(0).GetComponent<Text>();
            offText.text = "OFF";

        }
        UserDataIO.WriteUserData(user);
    }

    public void EffectVolumeControll()
    {
        UserDataIO.User user = UserDataIO.ReadUserData();
        
        if (user.EffectVolume == 0f)
        {
            EffectOnOff.overrideSprite = onImage;
            user.EffectVolume = 1f;

            for (int i = 2; i < audioPlayers.Length; i++)
            {
                audioPlayers[i].volume = user.EffectVolume;
            }

            for(int j = 0; j < fires.Count; j++)
            {
                fires[j].volume = user.EffectVolume;
            }

            Text onText = EffectOnOff.transform.GetChild(0).GetComponent<Text>();
            onText.text = "ON";
        }

        else
        {
            EffectOnOff.overrideSprite = offImage;
            user.EffectVolume = 0f;

            for (int i = 2; i < audioPlayers.Length; i++)
            {
                audioPlayers[i].volume = user.EffectVolume;
            }


            for (int j = 0; j < fires.Count; j++)
            {
                fires[j].volume = user.EffectVolume;
            }

            Text offText = EffectOnOff.transform.GetChild(0).GetComponent<Text>();
            offText.text = "OFF";
        }
        UserDataIO.WriteUserData(user);
    }

    public void ClickSFX(bool _avaliable)
    {
        if (_avaliable) PlayAudio("UI", 0, 0.01f, false);
        else PlayAudio("UI", 1, 0.01f, false);
    }

    private void SetMixerGroup(string _sourceName, int _clipNum)
    {
        if (matchMixerGroup.ContainsKey(_sourceName, _clipNum))
            audioDic[_sourceName].outputAudioMixerGroup = matchMixerGroup[_sourceName][_clipNum];
        else audioDic[_sourceName].outputAudioMixerGroup = null;
    }

    private void GetMixerGroup()
    {
        matchMixerGroup.Add("GameManagerBgm", 0, masterMixer.FindMatchingGroups("GmBgm_0")[0]);
        matchMixerGroup.Add("GameManagerBgm", 1, masterMixer.FindMatchingGroups("GmBgm_1")[0]);
        matchMixerGroup.Add("GameManagerBgm", 2, masterMixer.FindMatchingGroups("GmBgm_2")[0]);
        matchMixerGroup.Add("GameManagerBgm", 3, masterMixer.FindMatchingGroups("GmBgm_3")[0]);
        matchMixerGroup.Add("GameManagerBgm", 4, masterMixer.FindMatchingGroups("GmBgm_4")[0]);
        
        matchMixerGroup.Add("LobbyBgm", 0, masterMixer.FindMatchingGroups("LobbyBgm_0")[0]);
        
        matchMixerGroup.Add("GameManagerEffect", 0, masterMixer.FindMatchingGroups("GmEffect_0")[0]);
        matchMixerGroup.Add("GameManagerEffect", 1, masterMixer.FindMatchingGroups("GmEffect_1")[0]);
        matchMixerGroup.Add("GameManagerEffect", 2, masterMixer.FindMatchingGroups("GmEffect_2")[0]);
        matchMixerGroup.Add("GameManagerEffect", 3, masterMixer.FindMatchingGroups("GmEffect_3")[0]);
        matchMixerGroup.Add("GameManagerEffect", 4, masterMixer.FindMatchingGroups("GmEffect_4")[0]);
        matchMixerGroup.Add("GameManagerEffect", 5, masterMixer.FindMatchingGroups("GmEffect_5")[0]);

        matchMixerGroup.Add("LobbyEffect", 0, masterMixer.FindMatchingGroups("LobbyEffect_0")[0]);
        matchMixerGroup.Add("LobbyEffect", 1, masterMixer.FindMatchingGroups("LobbyEffect_1")[0]);
        matchMixerGroup.Add("LobbyEffect", 2, masterMixer.FindMatchingGroups("LobbyEffect_2")[0]);

        matchMixerGroup.Add("Player", 0, masterMixer.FindMatchingGroups("Player_0")[0]);
        matchMixerGroup.Add("Player", 1, masterMixer.FindMatchingGroups("Player_1")[0]);
        matchMixerGroup.Add("Player", 2, masterMixer.FindMatchingGroups("Player_2")[0]);
        matchMixerGroup.Add("Player", 3, masterMixer.FindMatchingGroups("Player_3")[0]);

        matchMixerGroup.Add("UI", 0, masterMixer.FindMatchingGroups("UI_0")[0]);
        matchMixerGroup.Add("UI", 1, masterMixer.FindMatchingGroups("UI_1")[0]);
        matchMixerGroup.Add("UI", 2, masterMixer.FindMatchingGroups("UI_2")[0]);
        
        matchMixerGroup.Add("Obstacle", 0, masterMixer.FindMatchingGroups("Obs_0")[0]);
        matchMixerGroup.Add("Obstacle", 1, masterMixer.FindMatchingGroups("Obs_1")[0]);
        matchMixerGroup.Add("Obstacle", 2, masterMixer.FindMatchingGroups("Obs_2")[0]);

        matchMixerGroup.Add("Warning", 0, masterMixer.FindMatchingGroups("Warning_0")[0]);
        matchMixerGroup.Add("Warning", 1, masterMixer.FindMatchingGroups("Warning_1")[0]);

        matchMixerGroup.Add("TimeOut", 0, masterMixer.FindMatchingGroups("TimeOut_0")[0]);
    }

    private void AudioMixerControll(string _sourceName, int _clipNum)
    {
        audioDic[_sourceName].outputAudioMixerGroup = matchMixerGroup[_sourceName][_clipNum];
    }
    
}
