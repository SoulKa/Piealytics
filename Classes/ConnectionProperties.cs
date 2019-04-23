using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Piealytics
{
    class ConnectionProperties
    {

        public readonly IPEndPoint EndPoint;
        public readonly int Frequency;
        public readonly Tuple<float, float> Range;

        public ConnectionProperties(IPEndPoint endPoint, int frequency, Tuple<float, float> range)
        {
            EndPoint = endPoint;
            Frequency = frequency;
            Range = range;
        }

        /// <summary>
        /// Parses the bytes from a dgram to connection properties and appends its origin endpoint
        /// </summary>
        /// <param name="dgram">The dgram</param>
        /// <returns>The connection properties or null if not valid dgram</returns>
        public static ConnectionProperties FromDgram(UdpReceiveResult dgram)
        {
            // get bytes from dgram and check if their size and content is valid
            var bytes = dgram.Buffer;
            if (bytes.Length != 13 || bytes[0] != (byte) NetworkManager.MSG_TYPES.CONNECTION)
            {
                Console.WriteLine("Connection properties dgram was not 13 bytes but " + bytes.Length + " or type did not fit: " + bytes[0]);
                return null;
            }

            // get data from bytes
            int frequency = BitConverter.ToInt32(bytes, 1);
            Tuple<float, float> range = new Tuple<float, float>(BitConverter.ToSingle(bytes, 5), BitConverter.ToSingle(bytes, 9));

            // create and return
            return new ConnectionProperties(dgram.RemoteEndPoint, frequency, range);
        }

        public override string ToString()
        {
            return "Freq.: " + Frequency + ", Range: " + Range;
        }
    }
}
