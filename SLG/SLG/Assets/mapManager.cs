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
                    Debug.LogError("Invalid Mesh Type: " + m_meshType);
                    break;
                }
        }
    }

    /// coefficient代表边的网格数，最小为2
    private void ShowSquareGrid()
    {
        Vector3[] pos;
        int rn = this.m_arrRow / (this.coefficient - 1);
        int cn = this.m_arrCol / (this.coefficient - 1);
        if (this.m_arrRow % (this.coefficient - 1) > 0)
            ++rn;
        if (this.m_arrCol % (this.coefficient - 1) > 0)
            ++cn;
        this.m_lines = new GameObject[rn, cn];

        for (int i = 0; i < this.m_arrRow - 1;)
        {
            int lastr = i + this.coefficient - 1;
            if (lastr >= this.m_arrRow)
            {
                lastr = this.m_arrRow - 1;
            }

            for (int j = 0; j < this.m_arrCol - 1;)
            {
                int lastc = j + this.coefficient - 1;
                if (lastc >= this.m_arrCol)
                {
                    lastc = this.m_arrCol - 1;
                }

                if (lastr < this.m_arrRow - 1 && lastc < this.m_arrCol - 1)
                {
                    pos = new Vector3[this.coefficient * 4];
                    for (int k = 0; k < this.coefficient; ++k)
                    {
                        pos[0 * this.coefficient + k] = this.m_array[i, j + k];
                        pos[1 * this.coefficient + k] = this.m_array[i + k, lastc];
                        pos[2 * this.coefficient + k] = this.m_array[lastr, lastc - k];
                        pos[3 * this.coefficient + k] = this.m_array[lastr - k, j];
                    }
                    this.CreatLine(i / (this.coefficient - 1), j / (this.coefficient - 1), pos);
                }
                else
                {
                    int cr = lastr - i + 1;
                    int cl = lastc - j + 1;
                    pos = new Vector3[(cr + cl) * 2];
                    for (int k = 0; k < cr; ++k)
                    {
                        pos[cl + k] = this.m_array[i + k, lastc];
                        pos[cr + 2 * cl + k] = this.m_array[lastr - k, j];
                    }
                    for (int k = 0; k < cl; ++k)
                    {
                        pos[k] = this.m_array[i, j + k];
                        pos[cr + cl + k] = this.m_array[lastr, lastc - k];
                    }
                    this.CreatLine(i / (this.coefficient - 1), j / (this.coefficient - 1), pos);
                }

                j = lastc;
            }
            i = lastr;
        }
    }

    private void ShowRegularHexagon()
    {
        this.coefficient = this.coefficient / 5;
        Vector3[] pos_1;
        Vector3[] pos_2;
        int num_1 = this.m_arrCol / (this.coefficient * (3 + 5)) * (this.coefficient * 5 + 1);
        int num_2 = this.m_arrCol % (this.coefficient * (3 + 5));
        if (num_2 > 0)
        {
            if (num_2 < 3 * this.coefficient)
            {
                num_2 = 1;
            }
            else
            {
                num_2 = num_2 - 3 * this.coefficient + 2;
            }
        }

        pos_1 = new Vector3[num_1 + num_2];
        pos_2 = new Vector3[num_1 + num_2];

        int rn = this.m_arrRow / (this.coefficient * (3 + 5));
        this.m_lines = new GameObject[rn, 2];
        for (int i = 4 * this.coefficient; i < this.m_arrRow;)
        {
            int index_1 = 0;
            int index_2 = 0;
            int r_1 = i - 4 * this.coefficient;
            int r_2 = i + 4 * this.coefficient;
            bool flag_1 = true;
            bool flag_2 = false;
            if (r_2 >= this.m_arrRow)
            {
                flag_1 = false;
            }

            for (int j = 0; j < this.m_arrCol;)
            {
                if (j % (this.coefficient * (3 + 5)) == 0)
                {
                    flag_2 = !flag_2;
                    if (flag_2)
                    {
                        pos_1[index_1++] = this.m_array[i, j];
                        if (flag_1)
                        {
                            pos_2[index_2++] = this.m_array[i, j];
                        }
                    }
                    else
                    {
                        pos_1[index_1++] = this.m_array[r_1, j];
                        if (flag_1)
                        {
                            pos_2[index_2++] = this.m_array[r_2, j];
                        }
                    }

                    j += 3 * this.coefficient;
                }
                else
                {
                    if (flag_2)
                    {
                        pos_1[index_1++] = this.m_array[r_1, j];
                        if (flag_1)
                        {
                            pos_2[index_2++] = this.m_array[r_2, j];
                        }
                    }
                    else
                    {
                        pos_1[index_1++] = this.m_array[i, j];
                        if (flag_1)
                        {
                            pos_2[index_2++] = this.m_array[i, j];
                        }
                    }

                    ++j;
                }
            }

            this.CreatLine(i / (2 * 4 * this.coefficient), 0, pos_1);
            if (flag_1)
            {
                this.CreatLine(i / (2 * 4 * this.coefficient), 1, pos_2);
            }

            i += (4 * this.coefficient * 2);
        }
    }

    private void CreatLine(int row, int col, Vector3[] pos)
    {
        if (this.m_lines[row, col] != null)
        {
            GameObject.Destroy(this.m_lines[row, col]);
        }
        this.m_lines[row, col] = new GameObject();

        LineRenderer _lineRenderer = this.m_lines[row, col].AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        _lineRenderer.SetColors(this.gridColor, this.gridColor);
        _lineRenderer.SetWidth(this.gridLine, this.gridLine);
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.SetVertexCount(pos.Length);
        for (int i = 0; i < pos.Length; ++i)
        {
            _lineRenderer.SetPosition(i, pos[i]);
        }

        this.m_lines[row, col].name = "CreateLine " + row + "  " + col;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
