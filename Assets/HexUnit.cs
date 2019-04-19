﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HexUnit : MonoBehaviour
{
    List<HexCell> pathToTravel;

    public static HexUnit[] unitPrefab;

    const float travelSpeed = 4f;

    const float rotationSpeed = 180f;

    public int unitType = 0;

    private bool isQunar = false;

    //public AnimationClip[] animations;
    Animator m_anim;
    skillAnimationEffect mySkill;

    UnitAttribute unitAttribute;

    public UnitAttribute UnitAttribute
    {
        get
        {
            return unitAttribute;
        }
    }

    public HexCell Location//cell
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                location.Unit = null;
            }
            location = value;
            value.Unit = this;
            transform.localPosition = value.Position;
        }
    }

    HexCell location;

    void OnEnable()
    {
        if (location)
        {
            transform.localPosition = location.Position;
        }
        if (GetComponent<UnitAttribute>())
        {
            unitAttribute = GetComponent<UnitAttribute>();
            m_anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
            mySkill = GetComponent<skillAnimationEffect>();
        }
    }


    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    float orientation;

    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }

    void Update()
    {
        if(!isQunar)
        {
            m_anim.SetInteger("aniState", -1);
        }
    }

    public void Die()
    {
        //加动画
        m_anim.SetInteger("aniState", 4);
        StartCoroutine(DealDie());
    }

    IEnumerator DealDie()
    {
        isQunar = true;
        yield return new WaitForSeconds(3f);
        yield return null;
        location.Unit = null;
        Destroy(gameObject);
        Debug.Log("Die!!!!");
        isQunar = false;
    }

    public void Save(BinaryWriter writer)
    {
        location.coordinates.Save(writer);
        writer.Write(orientation);
        writer.Write(unitType);

    }

    public static void Load(BinaryReader reader, HexGrid grid)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        int type = reader.ReadInt32();//读取棋子类型
        grid.AddUnit(
            Instantiate(unitPrefab[type]), grid.GetCell(coordinates), orientation
        );
    }

    public bool isValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater && !cell.Unit;
    }

    public void Travel(List<HexCell> path)//欢乐神游
    {
        //加动画
        m_anim.SetInteger("aniState", 1);
        Location = path[path.Count - 1];
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
        //获取动画层 0 指Base Layer.
        AnimatorStateInfo stateinfo = m_anim.GetCurrentAnimatorStateInfo(0);
        //如果正在播放walk动画.
        //Debug.Log(m_anim.GetParameter(0).name);
        
    }

    IEnumerator TravelPath()//欢乐神游，一格格走
    {
        isQunar = true;
        Vector3 a, b, c = pathToTravel[0].Position;
        transform.localPosition = c;
        yield return LookAt(pathToTravel[1].Position);

        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + pathToTravel[i].Position) * 0.5f;
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            t -= 1f;
        }
        a = c;
        b = pathToTravel[pathToTravel.Count - 1].Position;
        c = b;
        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }
        transform.localPosition = location.Position;
        orientation = transform.localRotation.eulerAngles.y;
        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
        isQunar = false;
    }

    IEnumerator LookAt(Vector3 point)//父亲，快看！诸葛亮！
    {
        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =
            Quaternion.LookRotation(point - transform.localPosition);

        float angle = Quaternion.Angle(fromRotation, toRotation);
        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;
            for (
                float t = Time.deltaTime * speed;
                t < 1f;
                t += Time.deltaTime
            )
            {
                transform.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }
        orientation = transform.localRotation.eulerAngles.y;
    }

    void LookAtTarget(Vector3 point)//父亲，快看！诸葛亮！
    {
        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =
            Quaternion.LookRotation(point - transform.localPosition);

        float angle = Quaternion.Angle(fromRotation, toRotation);
        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;
            for (
                float t = Time.deltaTime * speed;
                t < 1f;
                t += Time.deltaTime
            )
            {
                transform.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
            }
        }
        orientation = transform.localRotation.eulerAngles.y;
    }

    IEnumerator FightAnimation(HexUnit target)//欢乐神游，一格格走
    {
        isQunar = true;
        
        yield return LookAt(target.location.Position);

        yield return null;
        isQunar = false;
    }

    public void Fight(HexUnit target)//欢乐战斗
    {
        m_anim.SetInteger("aniState", 2);
        //看向对手

        StartCoroutine(FightAnimation(target));
        //攻击方必先手
        //加动画
        target.unitAttribute.DoDamage(this.unitAttribute.Att - target.UnitAttribute.Def);
        //如果对方还活着
        if (target.unitAttribute.hp > 0)
        {
            this.unitAttribute.DoDamage(target.unitAttribute.Att - this.UnitAttribute.Def);
            //m_anim.SetInteger("aniState", 2);
            //如果我方比对方速度快3以上 追加攻击
            if (this.unitAttribute.Sp >= target.unitAttribute.Sp + 3)
            {
                target.unitAttribute.DoDamage(this.unitAttribute.Att - target.UnitAttribute.Def);
                m_anim.SetInteger("aniState", 2);
                StartCoroutine(FightAnimation(target));
            }
            //如果对方比我方速度快3以上 对方追加攻击
            else if (target.unitAttribute.Sp >= this.unitAttribute.Sp + 3)
            {
                this.unitAttribute.DoDamage(target.unitAttribute.Att - this.UnitAttribute.Def);
                //m_anim.SetInteger("aniState", 2);
            }
        }
    }

    public void Spell(HexUnit target)//欢乐施法
    {
        LookAtTarget(target.location.Position);
        //加动画
        //m_anim.GetParameter(0).defaultInt = 2;
        mySkill.skillAt(target);
        Debug.Log(m_anim.GetParameter(0).name + " now is " + m_anim.GetParameter(0).defaultInt);
        unitAttribute.activeSkill.Spell(target);
    }

    public bool checkTeam(HexCell target)
    {
        return this.unitAttribute.team != target.Unit.unitAttribute.team;
    }

    //EnableHighLight;
}
