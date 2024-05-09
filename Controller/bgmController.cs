using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmController : MonoBehaviour
{
    GameObject BGM;
    AudioSource backmusic;
    private string NowBGMname = "";
    [System.Serializable]
    public struct BgmType
    {
        public string name;
        public AudioClip audio;
    }

    // Inspector ��ǥ���� ������� ���
    public BgmType[] BGMList;
    void Awake()
    {
        BGM = GameObject.Find("BGM");
        backmusic = BGM.GetComponent<AudioSource>(); //������� �����ص�
        if (backmusic.isPlaying) return; //��������� ����ǰ� �ִٸ� �н�
        else
        {
            PlayBGM("LobbyBGM");
            DontDestroyOnLoad(BGM); //������� ��� ����ϰ�(���� ��ư�Ŵ������� ����)
        }
    }
    void Update()
    {
        backmusic.volume = SoundManager.instance.volume;
    }
    public void PlayBGM(string name)
    {
        if (NowBGMname.Equals(name)) return;

        for (int i = 0; i < BGMList.Length; ++i)
            if (BGMList[i].name.Equals(name))
            {
                backmusic.clip = BGMList[i].audio;
                backmusic.Play();
                NowBGMname = name;
            }
    }
}