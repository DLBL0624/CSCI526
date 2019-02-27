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
        if(GetComponent<UnitAttribute>())
        {
            unitAttribute = GetComponent<UnitAttribute>();
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
        Location = path[path.Count - 1];
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());

    }

    IEnumerator TravelPath()//欢乐神游，一格格走
    {
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

    //EnableHighLight;
}
