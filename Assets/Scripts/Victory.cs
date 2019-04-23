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
    private bool win = false;

    public void Update()
    {
        unitManager = GameObject.FindGameObjectWithTag("UnitManager");
        videoPlayer = GameObject.FindGameObjectWithTag("CGplayer");
        if(videoPlayer)
        {
            video = videoPlayer.GetComponent<CheckVideoStop>();
            checkVideoStop();
        }
        if(unitManager)
        {
            um = unitManager.GetComponent<UnitManager>();
            checkEvent(levelSwitcher.LevelIndex, um);
            if(CheckWinOrNot(levelSwitcher.LevelIndex, um)==1)
            {
                win = true;
            }
        }
    }


    //判断胜利与否的接口 2失败，1胜利，0没有胜利
    public int CheckWinOrNot(int level, UnitManager Umanager) {

        if (level == 0)
        {
            if (Umanager.friendUnits.Count > 1&&!win) {
                //胜利并且播放动画
                if (!CGplaying)
                {
                    CGplaying = true;
                    this.CG(0, 1);
                }
                return 1;
            }
            return 0;
        }

        if (level == 1)
        {
            if (roundManager.getRound()>14) {
                //胜利并且播放动画
                if (!CGplaying)
                {
                    CGplaying = true;
                    this.CG(1, 1);
                }
                return 1;
            }
            return 0;
        }


        //假设谷仓是eneyUnit list中的item
        if (level == 2)
        {
            for (int i = 0; i < Umanager.enemyUnits.Count; i++)
            {
                if (Umanager.enemyUnits[i].unitType == 99)
                {
                    return 0;
                }
            }

            int herosCount = 0;

            for (int i = 0; i < Umanager.friendUnits.Count; i++)
            {
                if (Umanager.friendUnits[i].unitType != 0)
                {
                    herosCount++;
                }
            }

            //== 后面的值对应本关最后应该幸存的英雄数量
            if (herosCount == 4)
            {
                this.CG(2, 1);
                return 1;
            }
            else
            {
                return 0;
            }
        }


        if (level == 3)
        {
            int herosCount = 0;

            for (int i = 0; i < Umanager.friendUnits.Count; i++)
            {
                if (Umanager.friendUnits[i].unitType != 0)
                {
                    herosCount++;
                }
            }

            if (Umanager.enemyUnits.Count == 0 && herosCount == 5)
            {
                this.CG(3, 1);
                return 1;
            }
            else {
                return 0;
            }
        }


        if (level == 4)
        {
            for (int i = 0; i < Umanager.enemyUnits.Count; i++)
            {
                if (Umanager.enemyUnits[i].unitType == 3)
                {
                    return 0;
                }
            }

            //胜利并且播放动画
            this.CG(4, 1);
            return 1;
        }

        return 0;
    }




    //开场的动画以及音频
    public void CG(int level, int StartOrEnd)
    {
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
        else {
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

        video.PlayVideo();
    }

    public int checkVideoStop()
    {
        if (video.videoPlayer.isPaused)
        {
            CGplaying = false;
            if(win)
            {
                levelSwitcher.SwitchToVictoryScene();
            }
        }
        return 0;
    }

    public int checkEvent(int level, UnitManager Umanager)
    {
        
        if (level == 0)
        {
            GameObject chromie = GameObject.FindGameObjectWithTag("Chromie");
            GameObject arthus = GameObject.FindGameObjectWithTag("2234");
            if (chromie&&arthus)
            {
                HexUnit chromieUnit = chromie.GetComponent<HexUnit>();
                HexUnit arthusUnit = arthus.GetComponent<HexUnit>();
                if (HexMetrics.FindDistanceBetweenCells(chromieUnit.Location,arthusUnit.Location)==1)
                {
                    Umanager.removeUnit(chromieUnit);
                    chromieUnit.UnitAttribute.team = 0;
                    Umanager.loadUnit(chromieUnit);
                    Debug.Log("克罗米加入队伍");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else return 0;
        }
        return 0;
    }
}

