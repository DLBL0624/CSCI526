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

    const int visionRange = 3;

    public int unitType = 0;

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
            if(location)
            {
                Grid.DecreaseVisbility(location, visionRange);
                location.Unit = null;
            }
            location = value;
            value.Unit = this;
            Grid.IncreaseVisbility(value, visionRange);
            transform.localPosition = value.Position;
        }
    }

    HexCell location, currentTravelLocation;

    void OnEnable()
    {
        if (location)
        {
            transform.localPosition = location.Position;
            if(currentTravelLocation)
            {
                Grid.IncreaseVisbility(location, visionRange);
                Grid.DecreaseVisbility(currentTravelLocation, visionRange);
                currentTravelLocation = null;
            }
        }
        if(GetComponent<UnitAttribute>())//如果单位有属性
        {
            unitAttribute = GetComponent<UnitAttribute>();//启用属性
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

    public void Die()
    {
        if(location)
        {
            Grid.DecreaseVisbility(location, visionRange);
        }
        location.Unit = null;
        Destroy(gameObject);
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

    public bool isValidDestination (HexCell cell)
    {
        return !cell.IsUnderwater && !cell.Unit;
    }

    public void Travel(List<HexCell> path)//欢乐神游
    {
        location.Unit = null;
        location = path[path.Count - 1];
        location.Unit = this;
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());

    }

    IEnumerator TravelPath()//欢乐神游，一格格走
    {
        Vector3 a, b, c = pathToTravel[0].Position;
        yield return LookAt(pathToTravel[1].Position);
        Grid.DecreaseVisbility(
            currentTravelLocation? currentTravelLocation : pathToTravel[0], 
            visionRange
        );

        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            currentTravelLocation = pathToTravel[i];
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + currentTravelLocation.Position) * 0.5f;
            Grid.IncreaseVisbility(pathToTravel[i], visionRange);
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Grid.DecreaseVisbility(pathToTravel[i], visionRange);
            t -= 1f;
        }
        currentTravelLocation = null;
        a = c;
        b = location.Position;
        c = b;
        Grid.IncreaseVisbility(location, visionRange);

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
            ){
                transform.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }
        orientation = transform.localRotation.eulerAngles.y;
    }

    public void Fight(HexUnit target)//欢乐战斗
    {
        this.unitAttribute.hp -= target.unitAttribute.att;
        target.unitAttribute.hp -= this.unitAttribute.att;
    }

    public bool checkTeam(HexCell target)
    {
        return this.unitAttribute.team != target.Unit.unitAttribute.team;
    }

    public HexGrid Grid { get; set; }

    //EnableHighLight;
}
