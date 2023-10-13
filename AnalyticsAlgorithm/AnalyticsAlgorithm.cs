using System;
using System.Text;
using System.Collections.Generic;

//
public class AnalyticsAlgorithm
{
    //
    private List<float> _values;

    //
    public float Score;
    public string ID;


    //
    private AnalyticsAlgorithm() 
    { 
        _values = new List<float>();
    }

    //
    public AnalyticsAlgorithm(int valuesCount)
    {
        _values = new List<float>();
        Random rand = new Random();

        for(int i = 0; i < valuesCount; i++)
        {
            _values.Add((float)rand.NextDouble());
        }
    }

    //
    public AnalyticsAlgorithm(AnalyticsAlgorithm evolutionAlgorithm)
    {
        _values = new List<float>();
        Score = evolutionAlgorithm.Score;
        ID = evolutionAlgorithm.ID;

        for(int i = 0; i < evolutionAlgorithm._values.Count; i++)
        {
            _values.Add(evolutionAlgorithm._values[i]);
        }
    }

    //
    public List<float> GetValues()
    {
        return new List<float>(_values);
    }

    //
    public static AnalyticsAlgorithm Cross(AnalyticsAlgorithm evo1, AnalyticsAlgorithm evo2, float ratio)
    {
        AnalyticsAlgorithm evolutionAlgorithm = new AnalyticsAlgorithm();

        for(int i = 0; i < evo1._values.Count && i < evo2._values.Count; i++)
        {
            float difference = evo2._values[i] - evo1._values[i];

            evolutionAlgorithm._values.Add(evo1._values[i] + (difference * ratio));
        }

        return evolutionAlgorithm;
    }

    //
    public void Mutate(float chance)
    {
        for(int i = 0; i < _values.Count; i++)
        {
            if(new Random().NextDouble() < chance)
            {
                _values[i] = (float)new Random().NextDouble();
            }
        }
    }

    //
    public List<byte> Serialize()
    {
        List<byte> data = new List<byte>();

        byte[] idByte = Encoding.UTF8.GetBytes(ID);
        data.AddRange(BitConverter.GetBytes(idByte.Length));
        data.AddRange(idByte);
        data.AddRange(BitConverter.GetBytes(Score));

        foreach(float partValue in _values)
        {
            data.AddRange(BitConverter.GetBytes(partValue));
        }

        return data;
    }

    //
    public static AnalyticsAlgorithm Deserialize(byte[] data)
    {
        AnalyticsAlgorithm evolutionAlgorithm = new AnalyticsAlgorithm();

        int index = 0;
        int idLength = BitConverter.ToInt32(data, index);

        index += sizeof(int);
        evolutionAlgorithm.ID = Encoding.UTF8.GetString(data, index, idLength); 
        index += idLength;

        evolutionAlgorithm.Score = BitConverter.ToSingle(data, 0);

        index += sizeof(float);
        while(index < data.Length)
        {
            evolutionAlgorithm._values.Add(BitConverter.ToSingle(data, index));
            index += sizeof(float);
        }

        return evolutionAlgorithm;
    }
}