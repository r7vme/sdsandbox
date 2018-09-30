using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using tk;

namespace tk
{

    [Serializable]
    public class NetPacket
    {
        public NetPacket(string m, string data)
        {
            msg = m;
            payload = data;
        }

        public string msg;
        public string payload;
    }

    
    //Wrap a tcpclient and dispatcher to handle network events over a tcp connection.
    //We create a NetPacket header to wrap all sends and recv. Should be pretty portable
    //over languages.
    [RequireComponent(typeof(tk.TcpClient))]
    public class JsonTcpClient : MonoBehaviour {

        public string nnIPAddress = "127.0.0.1";
        public int nnPort = 9090;
        private tk.TcpClient client;

        public tk.Dispatcher dispatcher;


        void Awake()
        {
            dispatcher = new tk.Dispatcher();
            dispatcher.Init();
            client = GetComponent<tk.TcpClient>();
            Initcallbacks();
        }

        void Initcallbacks()
        {
            client.onDataRecvCB += new TcpClient.OnDataRecv(OnDataRecv);
        }

        public bool Connect()
        {
            return client.Connect(nnIPAddress, nnPort);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public void Reconnect()
        {
            Disconnect();
            Connect();
        }

        public void SendMsg(JSONObject msg)
        {
            string packet = msg.ToString() + "\n";

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(packet);

            client.SendData( bytes );
        }

        void OnDataRecv(byte[] bytes)
        {
            string str = System.Text.Encoding.UTF8.GetString(bytes);

            try
            {
                JSONObject j = new JSONObject(str);

                string msg_type = j["msg_type"].str;

                dispatcher.Dipatch(msg_type, j);

            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

    }
}