using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    // Start is called before the first frame update

    static AudioManger crrunt;

    [Header("环境音")]
    public AudioClip environmentClip;
    public AudioClip musicClip;
   


    [Header("FX音")]
    public AudioClip deathFxClip;
    public AudioClip shrineFxClip;
    public AudioClip openDoorClip;

    public AudioClip startLevelClip;
    public AudioClip winClip;


    [Header("人物音")]
    public AudioClip[] stepClip;  //走路
    public AudioClip[] crouchStepClip; //蹲走（同上）
    public AudioClip jumpClip; //跳跃地面音
    public AudioClip jumpSayClip; //跳跃人物音
    public AudioClip deathClip;
    public AudioClip deathVoiceClip;
    public AudioClip coinClip;


    AudioSource environmentSource;
    AudioSource musicSource;
    AudioSource fxSource;
    AudioSource playerSource;
    AudioSource voiceSource;

    private void Awake()
    {
        if (crrunt != null) {
            Destroy(gameObject);//如果不为空则销毁该对象，否则重置场景时会一直添加
            return;
        }
        crrunt = this;

        //每个场景都需要声音管理，所以需要切换场景时不会销毁该声音管理对象
        DontDestroyOnLoad(gameObject);

        //用代码的方式添加声源，方便管理
        environmentSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playerSource = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();
        StartLevelAudio();
    }

    void StartLevelAudio() {
        crrunt.environmentSource.clip = crrunt.environmentClip;
        crrunt.environmentSource.loop = true;
        crrunt.environmentSource.Play();

        crrunt.musicSource.clip = crrunt.musicClip;
        crrunt.musicSource.loop = true;
        crrunt.musicSource.Play();
        PlayLevelAudio();
    }

    public static void PlayFootStepAudio() {
        //要在4个音效中随机播放，先随机生成下标
        int index = Random.Range(0, crrunt.stepClip.Length);
        crrunt.playerSource.clip = crrunt.stepClip[index];
        crrunt.playerSource.Play();

    }

    public static void PlayCrouchStepAudio()
    {
        //要在4个音效中随机播放，先随机生成下标
        int index = Random.Range(0, crrunt.crouchStepClip.Length);
        crrunt.playerSource.clip = crrunt.crouchStepClip[index];
        crrunt.playerSource.Play();
    }

    public static void PlayJumpAudio() {
        crrunt.playerSource.clip = crrunt.jumpClip;
        crrunt.playerSource.Play();
        crrunt.voiceSource.clip = crrunt.jumpSayClip;
        crrunt.voiceSource.Play();
    }

    public static void PlayDeathAudio() {
        crrunt.fxSource.clip = crrunt.deathFxClip;
        crrunt.fxSource.Play();
        crrunt.voiceSource.clip = crrunt.deathVoiceClip;
        crrunt.voiceSource.Play();
        crrunt.playerSource.clip = crrunt.deathClip;
        crrunt.playerSource.Play();
    }

    public static void PlayShrineAudio() {
        crrunt.fxSource.clip = crrunt.shrineFxClip;
        crrunt.fxSource.Play();
        crrunt.voiceSource.clip = crrunt.coinClip;
        crrunt.voiceSource.Play();
    }
    public static void PlayOpenDoorAudio() {
        crrunt.fxSource.clip = crrunt.openDoorClip;
        crrunt.fxSource.Play();
    }

    public static void PlayWinAudio()
    {
        crrunt.fxSource.clip = crrunt.winClip;
        crrunt.fxSource.Play();
        crrunt.playerSource.Stop();
    }

    public static void PlayLevelAudio() {
        crrunt.fxSource.clip = crrunt.startLevelClip;
        crrunt.fxSource.Play();
    }

}
