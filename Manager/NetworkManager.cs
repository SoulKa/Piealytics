using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Piealytics
{
    /// <summary>
    /// Searches the data sender in the local network and receives data packets.
    /// </summary>
    class NetworkManager : IDisposable
    {

        public enum MSG_TYPES {
            DISCONNECT = 0,
            DATA = 1,
            KEEPALIVE = 2,
            CONNECTION = 69
        };

        private readonly UdpClient udpSocket;
        private IPEndPoint clientEndPoint;
        private Task keepAliveTask;

        public bool Running { get; private set; } = false;
        public bool Connected { get; private set; } = false;

        /// <summary>
        /// Creates a network manager instance.
        /// </summary>
        /// <param name="port">The port to listen on</param>
        public NetworkManager(int port)
        {
            udpSocket = new UdpClient(port);
        }

        /// <summary>
        /// Listens on UDP for a pairing message
        /// </summary>
        public async Task<ConnectionProperties> SearchClientAsync()
        {
            Console.WriteLine("Searching for the Sender in the local network");
            SendPairingMessagesAsync(1000);
            return await WaitForConnectionAsync();
        }

        /// <summary>
        /// Sends out a UDP broadcast in the local network to find the Sender
        /// </summary>
        /// <param name="interval">The interval to broadcast the message given in milliseconds</param>
        private async void SendPairingMessagesAsync(int interval = 1000)
        {

            byte[] bytes = { (byte) MSG_TYPES.CONNECTION };

            while (!Connected)
            {
                udpSocket.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, 15000));
                await Task.Delay(interval);
            }

        }

        /// <summary>
        /// Waits for an incoming UDP packet from the data sender (raspberry)
        /// </summary>
        private async Task<ConnectionProperties> WaitForConnectionAsync()
        {
            Console.WriteLine("Waiting for pairing message");
            udpSocket.EnableBroadcast = true;

            UdpReceiveResult dgram;
            ConnectionProperties connectionProperties;
            do
            {
                dgram = await udpSocket.ReceiveAsync();
                connectionProperties = ConnectionProperties.FromDgram(dgram);
            } while (connectionProperties == null);

            udpSocket.EnableBroadcast = false;

            // save ip and port of raspberry
            clientEndPoint = dgram.RemoteEndPoint;

            // connect to the remote endpoints
            udpSocket.Connect(clientEndPoint);
            Connected = true;

            // start task to keep connection alive
            keepAliveTask = Task.Run(async () =>
            {
                while (Running)
                {
                    await udpSocket.SendAsync(new byte[] { (byte) MSG_TYPES.KEEPALIVE }, 1, clientEndPoint);
                    await Task.Delay(5000);
                }
            });

            Console.WriteLine("Connected to " + clientEndPoint + " with properties: " + connectionProperties);
            return connectionProperties;
        }

        /// <summary>
        /// Starts listening for incoming data packets on UDP
        /// </summary>
        /// <param name="callback">The callback to pass the received bytes to</param>
        public async void StartListenerAsync(Action<byte[]> callback)
        {
            if (!Connected) throw new InvalidOperationException("Cannot start the data listener while not being connected yet!");
            if (Running) throw new InvalidOperationException("The network manager is already listening for incoming data!");

            Console.WriteLine("Listening for UDP data packets");
            Running = true;

            // run loop until user stops the listener
            while (Running)
            {
                var data = (await udpSocket.ReceiveAsync()).Buffer;
                if (Running) callback(data);
            }
        }

        /// <summary>
        /// Stops listening for data
        /// </summary>
        public void StopListener()
        {
            Running = false;
        }

        /// <summary>
        /// Gets called when this instance is getting disposed
        /// </summary>
        public void Dispose()
        {

            Connected = false;
            keepAliveTask.Wait();

            if (udpSocket != null)
            {
                udpSocket.Close();
                udpSocket.Dispose();
            }

        }
    }
}
