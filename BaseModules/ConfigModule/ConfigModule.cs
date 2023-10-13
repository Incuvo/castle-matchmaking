using System;
using System.IO;
using System.Collections.Generic;

//
public partial class ConfigModule
{
    //
    private const string CONFIG_PATH = "config.cfg";

    //
    private Dictionary<string, string> _data;


    //
    public void Init()
    {
        _data = LoadData();
        
        if(_data == null)
        {
            CreateAndLoadDefaultConfig();
        }
    }

    //
    private void CreateAndLoadDefaultConfig()
    {
        _data = new Dictionary<string, string>();
    }

    //
    private Dictionary<string, string> LoadData()
    {
        if(File.Exists(CONFIG_PATH) == false)
        {
            return null;
        }

        string[] lines = File.ReadAllLines(CONFIG_PATH);
        Dictionary<string, string> data = new Dictionary<string, string>();

        foreach(string line in lines)
        {
            KeyValuePair<string, string> keyValuePair = line.GetParameter();

            if(keyValuePair.Key != null)
            {
                data.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        return data;
    }

    //
    public Dictionary<string, string> GetConfigData()
    {
        return new Dictionary<string, string>(_data);
    }
}