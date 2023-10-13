using System;

//
public class MainModule
{
    //
    private static MainModule _instance;
    public static MainModule Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new MainModule();
            }
            return _instance;
        }
    }

    //
    private HttpModule _httpModule;
    public static HttpModule HttpModule
    {
        get
        {
            return _instance._httpModule;
        }
    }

    //
    private ConfigModule _configModule;
    public static ConfigModule ConfigModule
    {
        get
        {
            return _instance._configModule;
        }
    }

    //
    private EvolutionModule _evolutionModule;
    public static EvolutionModule EvolutionModule
    {
        get
        {
            return _instance._evolutionModule;
        }
    }

    //
    private TCPModule _tcpModule;
    public static TCPModule TCPModule
    {
        get
        {
            return _instance._tcpModule;
        }
    }

    //
    public bool IsInit = false;

    //
    private MainModule()
    {
        _configModule = new ConfigModule();
        _evolutionModule = new EvolutionModule();
        _httpModule = new HttpModule();
        _tcpModule = new TCPModule();
    }

    //
    public void Init()
    {
        if(IsInit)
        {
            return;
        }

        _configModule.Init();
        _evolutionModule.Init();
        _httpModule.Init();
        _tcpModule.Init();

        IsInit = true;
    }
}