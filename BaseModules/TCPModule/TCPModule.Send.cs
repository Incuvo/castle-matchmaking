using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;

//
public partial class TCPModule
{
    //
    public void SendMessageToSingle(TcpConnection tcpConnection, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        SendMessageToSingle(tcpConnection, data);
    }

    //
    public void SendMessageToSingle(TcpConnection tcpConnection, byte[] data)
    {
        SendState state = new SendState();
        state.handler = tcpConnection;
        tcpConnection.TcpClient.Client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSendCallback), state);
    }

    //
    public void BroadcastMessage(string message)
    {
        lock(this)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach(TcpConnection connection in _clients)
            {
                SendMessageToSingle(connection, data);
            }
        }
    }

    //
    public void BroadcastMessage(byte[] data)
    {
        lock(this)
        {
            foreach(TcpConnection connection in _clients)
            {
                SendMessageToSingle(connection, data);
            }
        }
    }

    //
    private void OnSendCallback(IAsyncResult asyncResult)
    {
        SendState state = (SendState)asyncResult.AsyncState;

        SocketError error;
        int bytesRecived = state.handler.TcpClient.Client.EndSend(asyncResult, out error);
    }
}