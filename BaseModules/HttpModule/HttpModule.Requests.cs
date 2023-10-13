using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

//
public partial class HttpModule
{
    //
    private void Score(HttpListenerContext context)
    {
        if(context.Request.HttpMethod != "POST" || context.Request.ContentLength64 == 0)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.Close();
        }

        byte[] data = new byte[context.Request.ContentLength64];
        context.Request.InputStream.Read(data, 0, (int)context.Request.ContentLength64);

        try
        {
            JObject requestBody = JObject.Parse(Encoding.UTF8.GetString(data));
            if(requestBody.ContainsKey("matchmaking") == false)
            {
                throw new Exception();
            }

            JArray matchmakingScore = requestBody["matchmaking"].ToObject<JArray>();
            List<EvolutionAlgorithm> algorithms = MainModule.EvolutionModule.GetAlgorithms();

            foreach(JObject matchmaking in matchmakingScore)
            {
                string ID = matchmaking["name"].ToObject<string>();
                float score = matchmaking["score"].ToObject<float>();

                EvolutionAlgorithm algorithm = algorithms.Find(x => x.ID == ID);
                if(algorithm != null)
                {
                    algorithm.Score = score;
                }
                else
                {
                    throw new Exception("Algorithm: " + ID + " doesn't exist.");
                }
            }

            MainModule.EvolutionModule.NextGeneration();
        }
        catch(Exception e)
        {
            JObject exceptionResult = new JObject();
            exceptionResult.Add("status", "error " + e.Message);

            byte[] exceptionData = Encoding.UTF8.GetBytes(exceptionResult.ToString());
            context.Response.OutputStream.Write(exceptionData, 0, exceptionData.Length);
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.Close();
            return;
        }

        JObject result = new JObject();
        result.Add("status", "ok");

        byte[] resultData = Encoding.UTF8.GetBytes(result.ToString());
        context.Response.OutputStream.Write(resultData, 0, resultData.Length);
        context.Response.Close();
    }

    //
    private void GetGeneration(HttpListenerContext context)
    {
        byte[] data = Encoding.UTF8.GetBytes(MainModule.EvolutionModule.GetAlgorithmsJSON().ToString());
        context.Response.OutputStream.Write(data, 0, data.Length);
        context.Response.Close();
    }
}