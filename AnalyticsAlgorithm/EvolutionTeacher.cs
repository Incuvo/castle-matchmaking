using System;
using System.Collections.Generic;

//
public class EvolutionTeacher
{
    //
    private List<AnalyticsAlgorithm> _algorithms;
    private AnalyticsAlgorithm _bestAlgorithm;

    private float _mutationChance;
    private float _surviveRatio;
    private int _identifier;


    //
    private EvolutionTeacher() 
    { 
        _algorithms = new List<AnalyticsAlgorithm>();
        _identifier = 0;
    }

    //
    public EvolutionTeacher(int populationSize, int valuesCount, float mutationChance, float surviveRatio)
    {
        _identifier = 0;
        _algorithms = new List<AnalyticsAlgorithm>();
        
        for(int i = 0; i < populationSize; i++)
        {
            _algorithms.Add(new AnalyticsAlgorithm(valuesCount));
            _algorithms[_algorithms.Count - 1].ID = "algorithm" + _identifier++;
        }

        _mutationChance = mutationChance;
        _surviveRatio = surviveRatio;
    }

    //
    public void ScoringMethod(Func<List<float>, float> scoringMethod)
    {
        foreach(AnalyticsAlgorithm evolutionAlgorithm in _algorithms)
        {
            evolutionAlgorithm.Score = scoringMethod.Invoke(evolutionAlgorithm.GetValues());
        }
    }

    //
    public void SortAlgorithms()
    {
        _algorithms.Sort((x, y) => {
            if(x.Score > y.Score) return 1;
            else if(x.Score == y.Score) return 0;
            else return -1;
        });
    }

    //
    public void NextGeneration()
    {
        SortAlgorithms();
        _bestAlgorithm = new AnalyticsAlgorithm(_algorithms[_algorithms.Count - 1]);

        List<AnalyticsAlgorithm> algorithms = new List<AnalyticsAlgorithm>();

        for(int i = 0; i < (_algorithms.Count * _surviveRatio); i++)
        {
            algorithms.Add(_algorithms[i]);
            algorithms[i].ID = "algorithm" + _identifier++;
        }

        Console.WriteLine(algorithms.Count);

        Random rand = new Random();

        while(_algorithms.Count > algorithms.Count)
        {
            int index1 = rand.Next(_algorithms.Count);
            int index2 = rand.Next(_algorithms.Count);

            algorithms.Add(AnalyticsAlgorithm.Cross(_algorithms[index1], _algorithms[index2], 0.5f));
            algorithms[algorithms.Count - 1].Mutate(_mutationChance);
            algorithms[algorithms.Count - 1].ID = "algorithm" + _identifier++;
        }

        _algorithms = algorithms;
    }

    //
    public List<AnalyticsAlgorithm> GetAlgorithms()
    {
        return new List<AnalyticsAlgorithm>(_algorithms);
    }

    //
    public List<byte> Serialize()
    {
        List<byte> data = new List<byte>();

        data.AddRange(BitConverter.GetBytes(_identifier));

        foreach(AnalyticsAlgorithm evolutionAlgorithm in _algorithms)
        {
            List<byte> evolutionAlgorithmData = evolutionAlgorithm.Serialize();
            data.AddRange(BitConverter.GetBytes(evolutionAlgorithmData.Count));
            data.AddRange(evolutionAlgorithmData);
        }

        return data;
    }

    //
    public static EvolutionTeacher Deserialize(byte[] data)
    {
        int index = 0;

        EvolutionTeacher evolutionTeacher = new EvolutionTeacher();

        evolutionTeacher._identifier = BitConverter.ToInt32(data, index);
        index += sizeof(int);

        while(index < data.Length)
        {
            int length = BitConverter.ToInt32(data, index);
            index += sizeof(int);
            byte[] partData = new byte[length];

            Array.Copy(data, index, partData, 0, length);
            evolutionTeacher._algorithms.Add(AnalyticsAlgorithm.Deserialize(partData));
            index += length;
        }

        return evolutionTeacher;
    }
}