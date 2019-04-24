using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public CheckVideoStop video;
    public GameObject videoPlayer;
    public whichLevel levelSwitcher;
    public UnitManager um;
    public GameObject unitManager;
    private bool CGplaying = false;
    public bool win = false;
    public bool fail = false;
    private bool CGLoading = false;
    private bool[] events = { false, false, false, false, false };
    private bool[] starts = { false, false, false, false, false };

    public void Update()
    {
        unitManager = GameObject.FindGameObjectWithTag("UnitManager");
        videoPlayer = GameObject.FindGameObjectWithTag("CGplayer");
        if(videoPlayer&&!CGLoading)
        {
            
            if(levelSwitcher.LevelIndex!=-1&&!starts[levelSwitcher.LevelIndex])
            {//load 开场动画
                win = false;
                fail = false;
                starts[levelSwitcher.LevelIndex] = true;
                CGLoading = true;
                video = videoPlayer.GetComponent<CheckVideoStop>();
                Debug.Log("CG not Loading and Level is " + levelSwitcher.LevelIndex);
                if (!CGplaying)
                {
                    CG(levelSwitcher.LevelIndex, 0);
                }
            }
            else
            {
                CGLoading = true;
                video = videoPlayer.GetComponent<CheckVideoStop>();
            }
        }
        if(videoPlayer)
        {
            checkVideoStop();
        }
        else
        {
            CGplaying = false;
            CGLoading = false;
        }
        if(unitManager)
        {
            um = unitManager.GetComponent<UnitManager>();
            checkEvent(levelSwitcher.LevelIndex, um);
            if(CheckWinOrNot(levelSwitcher.LevelIndex, um)==1)
            {
                win = true;
                fail = false;
            }
            else if(CheckWinOrNot(levelSwitcher.LevelIndex, um) == 2)
            {
                fail = true;
                win = false;
            }
        }
    }


    //判断胜利与否的接口 2失败，1胜利，0没有胜利
    public int CheckWinOrNot(int level, UnitManager Umanager) {

        if (level == 0)
        {
            //胜利条件
            if (!win && Umanager.friendUnits.Count > 1 && Umanager.enemyUnits.Count == 0) {
                //胜利并且播放动画
                if (!CGplaying)
                {
                    this.CG(0, 1);
                }
                return 1;
            }
            //失败条件
            else if(!fail && Umanager.friendUnits.Count==0)
            {
                return 2;
            }
            return 0;
        }

        if (level == 1)
        {
            
            GameObject orc_boss = GameObject.FindGameObjectWithTag("OrcBoss");
            //胜利条件
            if (!win && (roundManager.getRound()>14 || !orc_boss)) {
                //胜利并且播放动画
                if (!CGplaying)
                {
                    this.CG(1, 1);
                }
                return 1;
            }
            //失败条件
            else if(!fail&&Umanager.friendUnits.Count < 3)
            {
                return 2;
            }
            return 0;
        }


        //谷仓是neuUnit list中的item
        if (level == 2)
        {
            //胜利条件
            if(!win && Umanager.neutralUnits.Count==0)
            {
                if (!CGplaying)
                {
                    this.CG(2, 1);
                    
                }
                return 1;
            }
            //失败条件
            else if (!fail && Umanager.friendUnits.Count < 4)
            {
                return 2;
            }
            return 0;
        }


        if (level == 3)
        {
            //胜利条件
            if (!win && Umanager.enemyUnits.Count == 0 && Umanager.friendUnits.Count == 5)
            {
                if (!CGplaying)
                {
                    this.CG(3, 1);
                }
                return 1;
            }
            else if (!fail && Umanager.friendUnits.Count == 0)
            {
                return 2;
            }
            return 0;
        }


        if (level == 4)
        {
            //Debug.Log("friends" + Umanager.friendUnits.Count);
            if(Umanager.friendUnits.Count < 5)
            {
                if (!CGplaying)
                {
                    this.CG(4, 1);
                }
                return 1;
            }
            for (int i = 0; i < Umanager.enemyUnits.Count; i++)
            {
                if (Umanager.enemyUnits[i].unitType == 24)
                {
                    return 0;
                }
            }
            if(!win)
            {
                //胜利并且播放动画
                if (!CGplaying)
                {
                    this.CG(4, 1);
                }
                return 1;
            }
        }

        return 0;
    }




    //开场的动画以及音频
    public void CG(int level, int StartOrEnd)
    {
        CGplaying = true;
        levelSwitcher.stopBGM();
        //StartOrEnd 为0代表开场动画，1代表结束动画0
        if (StartOrEnd == 0) {
            if (level == 0)
            {
                video.LoadVideo(0);
                video.LoadAudio(0);
            }

            if (level == 1)
            {
                video.LoadVideo(2);
                video.LoadAudio(1);
            }

            if (level == 2)
            {
                video.LoadVideo(4);
                video.LoadAudio(2);
            }

            if (level == 3)
            {
                video.LoadVideo(6);
                video.LoadAudio(3);
            }

            if (level == 4)
            {
                video.LoadVideo(8);
                video.LoadAudio(4);
            }
        }
        else if (StartOrEnd == 1){
            if (level == 0)
            {
                video.LoadVideo(1);
                video.LoadAudio(0);
            }

            if (level == 1)
            {
                video.LoadVideo(3);
                video.LoadAudio(1);
            }

            if (level == 2)
            {
                video.LoadVideo(5);
                video.LoadAudio(2);
            }

            if (level == 3)
            {
                video.LoadVideo(7);
                video.LoadAudio(3);
            }

            if (level == 4)
            {
                video.LoadVideo(9);
                video.LoadAudio(5);
            }
        }
        else if (StartOrEnd == 2)
        {
            if (level == 0)
            {
                video.LoadVideo(10);
                video.LoadAudio(0);
            }

        }

        video.PlayVideo();
        video.transform.GetChild(0).gameObject.SetActive(true);
        if(levelSwitcher.gameManager)levelSwitcher.gameManager.GetComponent<gameSystem>().hexGameUI.resetSelect();
    }

    public int checkVideoStop()
    {
        if (video.videoPlayer.isPaused)
        {
            video.activeSetting(true);
            video.gameObject.SetActive(false);
            
            Debug.Log("Video Stop");
            CGplaying = false;
            levelSwitcher.playBGM();
            if (win)
            {
                win = false;
                levelSwitcher.SwitchToVictoryScene();
            }
            else if(fail)
            {
                fail = false;
                levelSwitcher.SwitchToFailureScene();
            }
        }
        return 0;
    }

    public int checkEvent(int level, UnitManager Umanager)
    {
        
        if (level == 0)
        {
            if(!events[levelSwitcher.LevelIndex])
            {
                GameObject chromie = GameObject.FindGameObjectWithTag("Chromie");
                GameObject arthus = GameObject.FindGameObjectWithTag("2234");
                if (chromie && arthus)
                {
                    HexUnit chromieUnit = chromie.GetComponent<HexUnit>();
                    HexUnit arthusUnit = arthus.GetComponent<HexUnit>();
                    if (HexMetrics.FindDistanceBetweenCells(chromieUnit.Location, arthusUnit.Location) == 1)
                    {
                        events[levelSwitcher.LevelIndex] = true;
                        Umanager.removeUnit(chromieUnit);
                        chromieUnit.UnitAttribute.team = 0;
                        Umanager.loadUnit(chromieUnit);
                        Debug.Log("克罗米加入队伍");
                        if (!CGplaying)
                        {
                            this.CG(0, 2);
                        }
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            
            else return 0;
        }
        return 0;
    }

    public void resizeBool()
    {
        events[0] = false;
        events[1] = false;
        events[2] = false;
        events[3] = false;
        events[4] = false;
        starts[0] = false;
        starts[1] = false;
        starts[2] = false;
        starts[3] = false;
        starts[4] = false;
    }
}

