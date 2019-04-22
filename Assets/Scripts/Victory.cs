using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public CheckVideoStop video;




    //判断胜利与否的接口 2失败，1胜利，0没有胜利
    public int CheckWinOrNot(int level, UnitManager Umanager) {

        if (level == 0)
        {
            if (Umanager.friendUnits.Count > 1) {
                //胜利并且播放动画
                this.CG(0, 1);
                return 1;
            }
            return 0;
        }

        if (level == 1)
        {
            if (roundManager.getRound()>14) {
                //胜利并且播放动画
                this.CG(1, 1);
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


}

