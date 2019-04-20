using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CheckVideoStop : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    //skip the story video
    public Button button_Skip;
    //12 videos
    public string[] mp4URL;
    //audios related to videos
    public AudioClip[] audioClips;
    
    //just for Test
    public Button SetStage;
    public Button TestStart;
    public Button SetMusic;

    public GameObject actorPanel;
    public GameObject gameMenu;
    public GameObject editMenu;


    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = this.GetComponent<VideoPlayer>();
        audioSource = this.GetComponent<AudioSource>();
        //videoPlayer.SetTargetAudioSource(0, audioSource);
        //listen to the skip button
        button_Skip.onClick.AddListener(SkipStory);

        ////just for test!
        SetStage.onClick.AddListener(setLoadVideo);
        TestStart.onClick.AddListener(PlayVideo);
        SetMusic.onClick.AddListener(setLoadMusic);
    }

    // Update is called once per frame
    void Update()
    {
        if (videoPlayer.isPaused)
        {
            this.gameObject.SetActive(false);
            editMenu.SetActive(true);
            gameMenu.SetActive(true);
            actorPanel.SetActive(true);
        }
    }

    public void SkipStory()
    {
        videoPlayer.Stop();
        audioSource.Stop();
        this.gameObject.SetActive(false);
        editMenu.SetActive(true);
        gameMenu.SetActive(true);
        actorPanel.SetActive(true);
    }
    
    public void LoadVideo(int index)
    {
        videoPlayer.url = GetFilePath(mp4URL[index]);
    }

    public void LoadAudio(int index)
    {
        audioSource.clip = audioClips[index];
    }

    public void PlayVideo()
    {
        if(!string.IsNullOrEmpty(videoPlayer.url))
        {
            this.gameObject.SetActive(true);
            videoPlayer.SetTargetAudioSource(0, audioSource);
            //播放视频
            videoPlayer.Play();
            //播放音频
            audioSource.Play();
        }
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        editMenu.SetActive(false);
        gameMenu.SetActive(false);
        actorPanel.SetActive(false);
    }
    //get the path of mp4 file
    public string GetFilePath(string videoName)
    {
        string filePath = Application.streamingAssetsPath + "/" + videoName + ".mp4";
        return filePath;
    }

    //just for test!
    public void setLoadVideo()
    {
        LoadVideo(9);
        //Debug.Log("log video done");
    }
    public void setLoadMusic()
    {
        LoadAudio(5);
        //Debug.Log("log music done");
    }

    public void setVideo(int index)
    {
        LoadVideo(index);
    }

    public void setMusic(int index)
    {
        LoadAudio(index);
    }
}
