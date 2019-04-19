using System.Collections;
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

    private bool isSelected = false;

    private HexUnit targetUnit;

    private List<int> animationOperator = new List<int>();//1-> wall 2-> att 3-> wound 4-> die 5-> spell

    //public AnimationClip[] animations;
    Animator m_anim;


    public UnitAttribute unitAttribute;

    void Start()
    {
        StartCoroutine(AnimationProcess());
    }

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
            if(!isSelected)
            {
                m_anim.SetInteger("aniState", -1);//站立动作
            }
            else
            {
                m_anim.SetInteger("aniState", -1);//准备动作
            }
        }
    }

    public void Die(int loading)
    {
        if(loading == 0)
        {
            location.Unit = null;
            if(this)Destroy(gameObject);
        }
        else
        {
            animationOperator.Add(4);
        }
    }

    IEnumerator DealDie()
    {
        m_anim.SetInteger("aniState", 4);
        isQunar = true;
        yield return new WaitForSeconds(4f);
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
        Location = path[path.Count - 1];
        pathToTravel = path;
        animationOperator.Add(1);
        //StopAllCoroutines();
        //StartCoroutine(TravelPath());
    }


    

    IEnumerator TravelPath()//欢乐神游，一格格走
    {
        m_anim.SetInteger("aniState", 1);
        isQunar = true;
        Debug.Log(isQunar);
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

    IEnumerator FightAnimation()//欢乐神游，一格格走
    {
        isQunar = true;
        Debug.Log(isQunar);
        yield return LookAt(targetUnit.location.Position);
        m_anim.SetInteger("aniState", 2);
        yield return new WaitForSeconds(1f);
        isQunar = false;
        m_anim.SetInteger("aniState", -1);
    }

    public IEnumerator WoundAnimation()
    {
        isQunar = true;
        Debug.Log(isQunar);
        yield return new WaitForSeconds(1f);
        m_anim.SetInteger("aniState", 3);
        yield return LookAt(targetUnit.location.Position);
        isQunar = false;
        m_anim.SetInteger("aniState", -1);
    }

    IEnumerable SpellAnimation()
    {
        isQunar = true;
        yield return LookAt(targetUnit.location.Position);
        m_anim.SetInteger("aniState", 5);
        //if(targetUnit.unitAttribute.activeSkill.TargetTeam!=this.unitAttribute.team)
        //{
        //    targetUnit.WoundAnimation(this);
        //}
        isQunar = false;
    }


    public void Fight(HexUnit target)//欢乐战斗
    {
        this.targetUnit = target;
        target.unitAttribute.DoDamage(this.unitAttribute.Att - target.UnitAttribute.Def);
        animationOperator.Add(2);
        //如果对方还活着
        //if (target.unitAttribute.hp > 0)
        //{
        //    this.unitAttribute.DoDamage(target.unitAttribute.Att - this.UnitAttribute.Def);
        //    animationOperator.Add(3);
        //    //如果我方比对方速度快3以上 追加攻击
        //    if (this.unitAttribute.Sp >= target.unitAttribute.Sp + 3)
        //    {
        //        target.unitAttribute.DoDamage(this.unitAttribute.Att - target.UnitAttribute.Def);
        //        animationOperator.Add(2);
        //    }
        //    //如果对方比我方速度快3以上 对方追加攻击
        //    else if (target.unitAttribute.Sp >= this.unitAttribute.Sp + 3)
        //    {
        //        this.unitAttribute.DoDamage(target.unitAttribute.Att - this.UnitAttribute.Def);
        //        animationOperator.Add(3);
        //    }
        //}
    }

    public void Wound(HexUnit target)//欢乐受伤
    {
        Debug.Log("unit "+this.unitAttribute.actorName+" is hit by "+target.unitAttribute.actorName);
        this.targetUnit = target;
        this.unitAttribute.DoDamage(target.unitAttribute.Att - this.UnitAttribute.Def);
        animationOperator.Add(3);
    }

    public void Spell(HexUnit target)//欢乐施法
    {
        this.targetUnit = target;
        unitAttribute.activeSkill.Spell(target);
        animationOperator.Add(5);
    }

    public bool checkTeam(HexCell target)
    {
        return this.unitAttribute.team != target.Unit.unitAttribute.team;
    }

    //EnableHighLight;

    IEnumerator AnimationProcess()
    {
        while(true)//动画循环
        {
            
            //指令信息
            while (this.animationOperator.Count == 0)
            {
                //Debug.Log(this.unitAttribute.actorName + " animationOperator count = " + animationOperator.Count);
                yield return null;
            }
            while (this.animationOperator.Count > 0)
            {
                
                for (; animationOperator.Count > 0;)
                {
                    Debug.Log(this.unitAttribute.actorName + " animationOperator count = " + animationOperator.Count + " opeation[0] = " + animationOperator[0]);
                    switch (animationOperator[0])
                    {
                        case 1:
                            animationOperator.RemoveAt(0);
                            yield return TravelPath();
                            break;
                        case 2:
                            animationOperator.RemoveAt(0);
                            yield return FightAnimation();
                            break;
                        case 3:
                            animationOperator.RemoveAt(0);
                            yield return WoundAnimation();
                            break;
                        case 4:
                            animationOperator.RemoveAt(0);
                            yield return DealDie();
                            break;
                        case 5:
                            animationOperator.RemoveAt(0);
                            yield return SpellAnimation();
                            
                            break;
                        default:
                            animationOperator.RemoveAt(0);
                            break;
                    }
                }
            }
        }
    }
}
