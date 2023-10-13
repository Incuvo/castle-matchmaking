using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

//
public partial class TCPModule
{
    //
    public class TcpConnection
    {
        //
        private TcpClient _client;
        public TcpClient TcpClient
        {
            get
            {
                return _client;
            }
        }
        

        //
        public TcpConnection(TcpClient client)
        {
            _client = client;
        }
    }

    //
    public class ReciveBufferState
    {
        //
        public TcpConnection handler;
        public string recivedData = "";
        public byte[] buffer;
    }

    //
    public class SendState
    {
        //
        public TcpConnection handler;
    }

    //
    private const int BYTE_SIZE = 65535;

    private TcpListener _listener;
    private List<TcpConnection> _clients;

    //
    public TCPModule()
    {
        _clients = new List<TcpConnection>();

        _listener = new TcpListener(7000);
        
    }

    //
    public void Init()
    {
        _listener.Start();
        _listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptTcpConnection), null);
    }

    //
    private void OnAcceptTcpConnection(IAsyncResult asyncResult)
    {
        TcpConnection connection = new TcpConnection(_listener.EndAcceptTcpClient(asyncResult));

        lock(this)
        {
            _clients.Add(connection);
        }

        ReciveBufferState state = new ReciveBufferState();
        state.handler = connection;
        state.buffer = new byte[BYTE_SIZE];

        connection.TcpClient.Client.BeginReceive(state.buffer, 0, BYTE_SIZE, 0, new AsyncCallback(OnReadCallback), state);
        Console.WriteLine("Client connected!");

        JObject matchmaking = MainModule.EvolutionModule.GetAlgorithmsJSON();
        matchmaking.Add("type", "last_matchmaking");
        SendMessageToSingle(connection, matchmaking.ToString());
              
        _listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptTcpConnection), null);
    }

    //
    private void OnReadCallback(IAsyncResult asyncResult)
    {
        ReciveBufferState state = (ReciveBufferState)asyncResult.AsyncState;

        SocketError error;
        int bytesRecived = state.handler.TcpClient.Client.EndReceive(asyncResult, out error);

        if(bytesRecived > 0)
        {
            state.recivedData += Encoding.UTF8.GetString(state.buffer, 0, bytesRecived);

            if(state.handler.TcpClient.Available == 0)
            {
                SendMessageToSingle(state.handler, "Message: " + state.recivedData);

                ReciveBufferState reciveState = new ReciveBufferState();
                reciveState.handler = _clients[_clients.Count - 1];
                reciveState.buffer = new byte[BYTE_SIZE];

                if(state.handler.TcpClient.Client != null)
                {
                    state.handler.TcpClient.Client.BeginReceive(reciveState.buffer, 0, BYTE_SIZE, 0, new AsyncCallback(OnReadCallback), reciveState);
                }
            }
            else
            {
                state.handler.TcpClient.Client.BeginReceive(state.buffer, 0, BYTE_SIZE, 0, new AsyncCallback(OnReadCallback), state);
            }
        }
        else
        {
            lock(this)
            {
                _clients.Remove(state.handler);
            }
            Console.WriteLine("Client disconnected!");
        }
    }
}