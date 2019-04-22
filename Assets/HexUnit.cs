using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HexUnit : MonoBehaviour
{
    List<HexCell> pathToTravel;

    public static HexUnit[] unitPrefab;

    const float travelSpeed = 4f;

    const float rotationSpeed = 360f;

    public int unitType = 0;

    public bool isQunar = false;

    public bool isSelected = false;

    private HexUnit targetUnit;

    private List<int> animationOperator = new List<int>();//1-> wall 2-> att 3-> wound 4-> die 5-> spell

    public HexGrid hexGrid;

    //public AnimationClip[] animations;
    Animator m_anim;

    skillAnimationEffect mySkill;


    UnitAttribute unitAttribute;

    void Start()
    {
        StartCoroutine(AnimationProcess());
        hexGrid = this.GetComponentInParent<HexGrid>();
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
            if(!isSelected)
            {
                m_anim.SetInteger("aniState", -1);//站立动作
            }
            else
            {
                m_anim.SetInteger("aniState", 0);//准备动作
            }
        }
    }

    public void Die(int loading)
    {
        if (loading == 0)
        {
            location.Unit = null;
            if(this)Destroy(this.gameObject);
        }
        else if (loading == 1)
        {
            animationOperator.Add(4);
        }
        else
        {
            location.Unit = null;
            Destroy(this.gameObject);
            hexGrid.unitManager.removeUnit(this);
            hexGrid.RemoveUnitFromList(this);
        }
    }

    IEnumerator DealDie()
    {
        isQunar = true;
        m_anim.SetInteger("aniState", 4);
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
        isQunar = true;
        
        Debug.Log(isQunar);
        Vector3 a, b, c = pathToTravel[0].Position;
        transform.localPosition = c;
        yield return LookAt(pathToTravel[1].Position);
        m_anim.SetInteger("aniState", 1);
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
        m_anim.SetInteger("aniState", 0);
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
        yield return LookAt(targetUnit.location.Position);
        m_anim.SetInteger("aniState", 2);
        yield return new WaitForSeconds(1f);
        targetUnit.Wound(this);
        m_anim.SetInteger("aniState", 0);
        isQunar = false;
    }

    public IEnumerator WoundAnimation()
    {
        isQunar = true;
        m_anim.SetInteger("aniState", 3);
        yield return LookAt(targetUnit.location.Position);
        yield return new WaitForSeconds(.25f);
        m_anim.SetInteger("aniState", 0);
        if (UnitAttribute.hp <= 0) Die(1);
        isQunar = false;
    }

    IEnumerator SpellAnimations()
    {
        Debug.Log("施法");
        isQunar = true;
        yield return LookAt(targetUnit.location.Position);
        m_anim.SetInteger("aniState", 5);
        if (targetUnit.unitAttribute.activeSkill.TargetTeam != this.unitAttribute.team)
        {
            targetUnit.Wound(this);
        }
        yield return new WaitForSeconds(2f);
        mySkill.skillAt(targetUnit);
        m_anim.SetInteger("aniState", 0);
        isQunar = false;
        //m_anim.SetInteger("aniState", -1);
    }


    public void Fight(HexUnit target)//欢乐战斗
    {
        this.targetUnit = target;
        target.unitAttribute.DoDamage(this.unitAttribute.Att - target.UnitAttribute.Def);
        animationOperator.Add(2);
        isQunar = true;
    }

    public void Wound(HexUnit target)//欢乐受伤
    {
        Debug.Log("unit "+this.unitAttribute.actorName+" is hit by "+target.unitAttribute.actorName);
        this.targetUnit = target;
        this.unitAttribute.DoDamage(target.unitAttribute.Att - this.UnitAttribute.Def);
        animationOperator.Add(3);
        isQunar = true;
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
                            yield return SpellAnimations();
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
