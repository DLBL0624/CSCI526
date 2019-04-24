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
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("video->menu");
        if (videoPlayer.isPaused)
        {
            activeSetting(true);
        }
    }

    public void SkipStory()
    {
        videoPlayer.Pause();
        audioSource.Pause();
        this.transform.GetChild(0).gameObject.SetActive(false);
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
        activeSetting(false);
    }
    //get the path of mp4 file
    public string GetFilePath(string videoName)
    {
        string filePath = Application.streamingAssetsPath + "/" + videoName + ".mp4";
        return filePath;
    }

    public void setVideo(int index)
    {
        LoadVideo(index);
    }

    public void setMusic(int index)
    {
        LoadAudio(index);
    }

    public void activeSetting(bool show)
    {
        editMenu.SetActive(show);
        gameMenu.SetActive(show);
        actorPanel.SetActive(show);
    }
}
