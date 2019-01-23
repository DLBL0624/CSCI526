using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridShapeType
{
    Square,

    RegularHexagon,
}



public class mapManager : MonoBehaviour
{

    private const float MAXDIS = 10000001f;

    //grid Grid
    public bool showGrid = true;

    //grid width
    public float gridLine = 0.1f;

    //grid color
    public Color gridColor = Color.red;

    //grid line
    private GameObject[,] m_lines;

    //coefficient 网格基数
    public int coefficient = 8;

    //Terrain
    public Terrain m_terrian;

    //map_terrain_row
    private int m_arrRow = 0;

    //map_terrain_Column
    private int m_arrCol = 0;

    //Recent Map Vector3
    private Vector3[,] m_array;

    //grid Shape
    public GridShapeType m_meshType;

    // Start is called before the first frame update
    void Start()
    {
        this.LoadMap();
    }

    public void LoadMap()
    {

        if(this.m_terrian == null)
        {
            Debug.Log("Terrian Null");
            return;
        }
        if (this.m_meshType == GridShapeType.Square && this.coefficient < 2)
        {
            Debug.Log("C<2");
            return;
        }

        TerrainData data = m_terrian.terrainData;
        int mapz = (int)(data.size.x / data.heightmapScale.x);
        int mapx = (int)(data.size.z / data.heightmapScale.z);

        this.m_arrRow = Math.Min(data.heightmapWidth, mapz);
        this.m_arrCol = Math.Min(data.heightmapWidth, mapx);

        float[,] heightPosArray = data.GetHeights(0, 0, this.m_arrRow, this.m_arrCol);

        this.m_array = new Vector3[this.m_arrRow, this.m_arrCol];

        for(int i = 0; i<this.m_arrRow; ++i)
        {
            for (int j = 0; j < this.m_arrCol; ++j)
            {
                this.m_array[i, j] = new Vector3(j * data.heightmapScale.x, heightPosArray[i, j] * data.heightmapScale.y, i * data.heightmapScale.z);
            }
        }

        if (this.showGrid)
        {
            this.ShowGrid();
        }
    }

    private void ShowGrid()
    {
        switch (m_meshType)
        {
            case GridShapeType.Square:
                {
                    this.ShowSquareGrid();
                    break;
                }
            case GridShapeType.RegularHexagon:
                {
                    this.ShowRegularHexagon();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void ShowSquareGrid()
    {

    }

    private void ShowRegularHexagon()
    {

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
