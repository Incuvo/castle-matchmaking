using System;
using System.Net;
using System.Threading.Tasks;

//
public partial class HttpModule
{
    private HttpListener _listener;
    private Task _getContextTask;

    //
    public HttpModule()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:6789/");
    }

    //
    public void Init()
    {
        _listener.Start();
        _getContextTask = _listener.GetContextAsync().ContinueWith(ProcessContext);

        Console.WriteLine("LISTEN!");
    }

    //
    private void ProcessContext(Task<HttpListenerContext> task)
    {
        if(task.Status == TaskStatus.RanToCompletion)
        {
            HttpListenerContext context = task.Result;
            Console.WriteLine(context.Request.RawUrl);

            if(context.Request.RawUrl.StartsWith("/score"))
            {
                Score(context);
                _getContextTask = _listener.GetContextAsync().ContinueWith(ProcessContext);
                return;
            }
            else if(context.Request.RawUrl.StartsWith("/algorithms"))
            {
                GetGeneration(context);
                _getContextTask = _listener.GetContextAsync().ContinueWith(ProcessContext);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.Close();
            Console.WriteLine("Not found!");
        }
        else
        {
            Console.WriteLine("NOT DONE");
        }
        _getContextTask = _listener.GetContextAsync().ContinueWith(ProcessContext);
    }
}