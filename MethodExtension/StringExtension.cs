using System;
using System.Collections.Generic;

//
public static class StringExtension
{
    //
    public static KeyValuePair<string, string> GetParameter(this String str)
    {
        if(str.StartsWith('#') == true)
        {
            return new KeyValuePair<string, string>(null, null);
        }

        int spliter = str.IndexOf('=');
        
        if(spliter == -1)
        {
            return new KeyValuePair<string, string>(null, null);
        }

        string key = str.Substring(0, spliter);
        string valuePair = str.Substring(spliter + 1);

        return new KeyValuePair<string, string>(key.Trim(), valuePair);
    }
}