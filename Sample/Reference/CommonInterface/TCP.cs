using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace CommonInterface
{
    public class TCP
    {
        Socket socket;
        IPAddress ipAddress;
        int port;
        string strHostName;
        int timeout;
        bool isConnected = false;
        bool isError = false;

        public int Port
        {
            get
            {
                return port;
            }
        }
        public Socket Sock => socket;
        public string StrHostName
        {
            get
            {
                return strHostName;
            }
        }
        public int Timeout
        {
            get
            {
                return timeout;
            }
        }
        public bool IsConnected
        {
            get
            {
                if(socket == null)
                    return isConnected;

                return socket.Connected;
            }
        }
        public bool IsError
        {
            get
            {
                return isError;
            }
        }


        protected TCP(string ipAddr, int port, string hostName = "", int timeout = 1000)
        {
            ipAddress = IPAddress.Parse(ipAddr);
            strHostName = hostName;
            this.port = port;
            this.timeout = timeout;
        }

        public bool Open()
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                if (socket != null)
                    socket.Dispose();

                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Socket Receive/Send Timeout Test
                socket.ReceiveTimeout = this.timeout;
                socket.SendTimeout = this.timeout;


                IAsyncResult asyncResult = socket.BeginConnect(remoteEP, null, null);
                if (asyncResult.AsyncWaitHandle.WaitOne(this.timeout, false))
                {
                    socket.EndConnect(asyncResult);
                    isConnected = socket.Connected;
                    isError = false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                isError = true;
                return false;
            }

            return false;
        }

        public bool Close()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                isConnected = false;
                isError = false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        protected bool Write(byte[] writeByte)
        {
            try
            {
                if (socket == null)
                {
                    Trace.WriteLine("socket is null");
                    return false;
                }
                else if(socket.Connected == false)
                {
                    // reconnect
                    if (Open() == false)
                    {
                        return false;
                    }
                }

                socket.Send(writeByte, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                isError = true;
                return false;
            }

            isError = false;
            return true;
        }

        protected bool Read(ref byte[] readByte)
        {
            try
            {
                if (socket == null)
                {
                    Trace.WriteLine("socket is null");
                    return false;
                }
                else if (socket.Connected == false)
                {
                    return false;
                }

                socket.Receive(readByte);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                isError = true;
                return false;
            }

            isError = false;
            return true;
        }
    }
}
