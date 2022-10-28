using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCSimulation
{
    class ListenerSocket
    {
        public event ListenerResponseEvent ResponseEvent;

        const int LISTEN_SOCKET_BUFFER = 10;

        Socket socket;
        IPEndPoint remoteEP;
        ProtocolType pt;

        CancellationTokenSource cancelTokenSource;
        Task listenTask;

        string ip;
        int port;
        bool isRunning;

        public string IP
        {
            get { return ip; }
        }
        public int Port
        {
            get { return port; }
        }
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                if (isRunning == value)
                    return;

                isRunning = value;

                if(value == true)
                {
                    cancelTokenSource = new CancellationTokenSource();
                    listenTask = Task.Factory.StartNew((canceltoken) => Run(cancelTokenSource.Token), cancelTokenSource.Token, TaskCreationOptions.LongRunning);
                }
                else
                {
                    if(listenTask != null)
                    {
                        cancelTokenSource.Cancel();

                        if(socket != null)
                        {
                            if (socket.Connected)
                            {
                                socket.Shutdown(SocketShutdown.Both);
                            }
                            socket.Close();
                            socket = null;
                        }

                        listenTask.Wait();
                        listenTask.Dispose();
                        listenTask = null;
                        cancelTokenSource = null;
                    }
                }
            }
        }

        public ListenerSocket(string ip, int port, ProtocolType pt)
        {
            isRunning = false;

            this.ip = ip;
            this.port = port;
            this.pt = pt;

            IPAddress ipAddr = IPAddress.Parse(ip);
            //remoteEP = new IPEndPoint(ipAddr, port);
            remoteEP = new IPEndPoint(IPAddress.Any, port);

            switch (pt)
            {
                case ProtocolType.Tcp:
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Bind(remoteEP);
                    socket.Listen(LISTEN_SOCKET_BUFFER);
                    break;
                case ProtocolType.Udp:
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.Bind(remoteEP);
                    break;
            }
        }

        private async void Run(CancellationToken ct)
        {
            try
            {
                switch (pt)
                {
                    case ProtocolType.Tcp:
                        while (true)
                        {
                            await AcceptTCP()
                                .ContinueWith(result => ReceiveAndSendTCP(result.Result)).ConfigureAwait(false);

                            if (ct.IsCancellationRequested)
                                ct.ThrowIfCancellationRequested();
                        }
                    case ProtocolType.Udp:
                        while (true)
                        {
                            await ReceiveAndSendUDP(remoteEP).ConfigureAwait(false);

                            if (ct.IsCancellationRequested)
                                ct.ThrowIfCancellationRequested();
                        }
                }
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Trace.WriteLine("socket task cancel");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Trace.WriteLine("socket token cancel");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                IsRunning = false;
            }
        }

        private async Task<Socket> AcceptTCP()
        {
            return await socket.AcceptAsync();
        }

        private async Task<bool> ReceiveAndSendTCP(Socket client)
        {
            StringBuilder sb = new StringBuilder();
            string message = string.Empty;
            byte[] bytes;

            try
            {
                while (true)
                {
                    bytes = new byte[1024 * 1024];

                    var byteLength = await Task.Factory.FromAsync<int>(client.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, null, client), client.EndReceive);

                    var responseBytes = ResponseEvent?.Invoke(bytes.ToList().GetRange(0, byteLength).ToArray());

                    await Task.Factory.FromAsync(
                        client.BeginSend(responseBytes, 0, responseBytes.Length, SocketFlags.None, null, client),
                        client.EndSend);

                    if (cancelTokenSource.IsCancellationRequested)
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);

                client.Shutdown(SocketShutdown.Both);
                client.Close();
                return false;
            }

            return true;
        }

        private async Task<bool> ReceiveAndSendUDP(EndPoint senderEP)
        {
            byte[] bytes;

            try
            {
                bytes = new byte[1024 * 1024];
                int byteRec = socket.ReceiveFrom(bytes, ref senderEP);

                var receiveBytes = bytes.ToList().GetRange(0, byteRec).ToArray();

                var responseBytes = ResponseEvent?.Invoke(receiveBytes);

                await Task.Factory.FromAsync(
                    socket.BeginSendTo(responseBytes, 0, responseBytes.Length, SocketFlags.None, senderEP, null, socket),
                    socket.EndSendTo);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}
