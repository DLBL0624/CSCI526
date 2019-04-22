using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveLoadMenu : MonoBehaviour
{
    public Text menuLabel, actionButtonLabel;

    bool saveMode;

    public InputField nameInput;

    public RectTransform listContent;

    public SaveLoadItem itemPrefab;

    public HexGrid hexGrid;


    public void Open(bool saveMode)
    {
        this.saveMode = saveMode;
        if(saveMode)
        {
            menuLabel.text = "Save Map";
            actionButtonLabel.text = "Save";
        }
        else
        {
            menuLabel.text = "Load Map";
            actionButtonLabel.text = "Load";
        }
        FillList();
        //string pathses = Application.dataPath +;
        //menuLabel.text = pathses;
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }

    string GetSelectedPath()
    {
        string mapName = nameInput.text;
        if(mapName.Length == 0)
        {
            return null;
        }
        //string pathses = Application.dataPath + "/Resources/" + mapName + ".map";
        //menuLabel.text = pathses;
        //Debug.Log(Application.dataPath + "/Resources/" + mapName + ".map");
        
        return Application.dataPath + @"/Resources/" + mapName + ".map";
        Debug.Log(Application.dataPath + "/map/" + mapName + ".map");
        return Path.Combine(Application.persistentDataPath, mapName + ".map");
        //C:\Users\xjn06\Desktop\526-master\Assets\map
        
        
    }

    void Save(string path)
    {
        using (
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))
        )
        {
            writer.Write(2);
            hexGrid.Save(writer);
        }
    }

    public void Load(string path)
    {
        if(!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }
        using (
            BinaryReader reader = new BinaryReader(File.OpenRead(path))
        )
        {
            int header = reader.ReadInt32();
            if (header <= 2)
            {
                hexGrid.Load(reader, header);
                HexMapCamera.ValidatePosition();
            }
            else
            {
                Debug.LogWarning("Unkown map format" + header);
            }
        }
    }

    public void Action()
    {
        string path = GetSelectedPath();
        Debug.Log(path);
        if(path == null)
        {
            return;
        }
        if(saveMode)
        {
            Save(path);
        }
        else
        {
            Load(path);
        }
        Close();
    }

    public void Delete()
    {
        string path = GetSelectedPath();
        if (path == null)
        {
            return;
        }
        if(File.Exists(path))
        {
            File.Delete(path);
        }
        nameInput.text = "";
        FillList();
    }

    public void SelectItem (string name)
    {
        nameInput.text = name;
    }

    void FillList ()
    {
        for (int i = 0; i<listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }
        //string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
        string[] paths = Directory.GetFiles(Application.dataPath+ "/Resources/", "*.map");
        //Debug.Log(Resources.Load("*.map").GetType());
        //object[] resource_paths = Resources.LoadAll("*.map");
        Array.Sort(paths);
        //Array.Sort(resource_paths);

        for (int i = 0; i<paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(itemPrefab);
            item.menu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(listContent, false);
        }

        //for (int i = 0; i<resource_paths.Length; i++)
        //{
        //    SaveLoadItem item = Instantiate(itemPrefab);
        //    item.menu = this;
        //}
    }
}
