using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

//
public class EvolutionModule
{
    //
    private const string FILE_NAME = "/data.dat";

    //
    private string _saveDataPath = "";
    private int _evolutionPoolSize = 100;
    private int _evolutionValues = 5;
    private float _evolutionMutationChance = 0.05f;
    private float _evolutionSurvive = 0.4f;
    private EvolutionTeacher _evolutionTeacher;


    //
    public EvolutionModule()
    {

    }

    //
    public void Init()
    {
        Dictionary<string, string> configData = MainModule.ConfigModule.GetConfigData();

        if(configData.ContainsKey("algorithmSavePath"))
        {
            _saveDataPath = configData["algorithmSavePath"];
        }
        if(configData.ContainsKey("population"))
        {
            int population = 0;
            if(int.TryParse(configData["population"], out population))
            {
                _evolutionPoolSize = population;
            }
        }
        if(configData.ContainsKey("survive"))
        {
            float survive = 0;
            if(float.TryParse(configData["survive"], out survive))
            {
                _evolutionSurvive = survive;
            }
        }
        if(configData.ContainsKey("mutation"))
        {
            float mutation = 0;
            if(float.TryParse(configData["mutation"], out mutation))
            {
                _evolutionMutationChance = mutation;
            }
        }
        if(configData.ContainsKey("values"))
        {
            int values = 0;
            if(int.TryParse(configData["values"], out values))
            {
                _evolutionValues = values;
            }
        }

        _evolutionTeacher = Load();

        if(_evolutionTeacher == null)
        {
            _evolutionTeacher = new EvolutionTeacher(_evolutionPoolSize, _evolutionValues, _evolutionMutationChance, _evolutionSurvive);
            Save();
        }
        else
        {
            Console.WriteLine("LOADED");
        }
    }

    //
    private EvolutionTeacher Load()
    {
        if(File.Exists(_saveDataPath + FILE_NAME))
        {
            try
            {
                byte[] data = File.ReadAllBytes(_saveDataPath + FILE_NAME);

                Console.WriteLine(data.Length);

                return EvolutionTeacher.Deserialize(data);
            }
            catch(Exception)
            {
                return null;
            }
        }
        return null;
    }

    //
    private void Save()
    {
        if(Directory.Exists(_saveDataPath) == false)
        {
            Directory.CreateDirectory(_saveDataPath);
        }
        File.WriteAllBytes(_saveDataPath + FILE_NAME, _evolutionTeacher.Serialize().ToArray());
    }

    //
    public List<EvolutionAlgorithm> GetAlgorithms()
    {
        return _evolutionTeacher.GetAlgorithms();
    }

    //
    public void NextGeneration()
    {
        _evolutionTeacher.NextGeneration();
        Save();
    }

    //
    public JObject GetAlgorithmsJSON()
    {
        JObject result = new JObject();

        JArray evolutionAlgorithms = new JArray();

        List<EvolutionAlgorithm> list = _evolutionTeacher.GetAlgorithms();

        foreach(EvolutionAlgorithm evolutionAlgorithm in list)
        {
            JObject evolutionData = new JObject();
            evolutionData.Add("name", evolutionAlgorithm.ID);
            evolutionData.Add("score", evolutionAlgorithm.Score);
            
            JArray evolutionArray = new JArray();
            foreach(float valuePart in evolutionAlgorithm.GetValues())
            {
                evolutionArray.Add(valuePart);
            }
            evolutionData.Add("values", evolutionArray);
            evolutionAlgorithms.Add(evolutionData);
        }
        result.Add("algorithms", evolutionAlgorithms);
        return result;
    }
}