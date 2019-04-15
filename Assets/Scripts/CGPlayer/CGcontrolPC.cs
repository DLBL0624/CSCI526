using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;

public class CGcontrolPC : MonoBehaviour
{
    private VideoPlayer videoplayer;
    private RawImage rawimage;

    void Start()
    {
        videoplayer = this.GetComponent<VideoPlayer>();
        rawimage = this.GetComponent<RawImage>();
    }

    void Update()
    {
        if(videoplayer.texture == null)
        {
            return;
        }
        rawimage.texture = videoplayer.texture;
    }
}
