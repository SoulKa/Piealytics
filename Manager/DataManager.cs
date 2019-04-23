using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Piealytics
{
    /// <summary>
    /// Processes incoming data from the network manager and passes it to the renderer.
    /// </summary>
    class DataManager
    {

        private DataRenderer renderer;
        private InsertableQueue<float> data = new InsertableQueue<float>(1);
        private ConnectionProperties connectionProperties;

        private int historyLength;

        // constructor
        public DataManager(DataRenderer renderer)
        {
            this.renderer = renderer;
        }

        public void SetConnectionProperties(ConnectionProperties connectionProperties)
        {
            this.connectionProperties = connectionProperties;
            this.data = new InsertableQueue<float>(connectionProperties.Frequency * historyLength / 1000);
        }

        public void SetHistoryLength(int historyLength)
        {
            this.historyLength = historyLength;
            this.data.Resize(connectionProperties.Frequency * historyLength / 1000);
            renderer.DataPoints = data.GetAllItems();
            renderer.SetHistoryLength(historyLength);
            renderer.InvalidateCanvas();
        }

        /// <summary>
        /// Converts bytes to data values and adds them to the queue.
        /// Triggers a rerender afterwards.
        /// </summary>
        /// <param name="bytes"></param>
        public void HandleData(byte[] bytes)
        {

            // check if has correct size
            if (bytes.Length < 6 || bytes[0] != (byte) NetworkManager.MSG_TYPES.DATA) return;

            // get number of values in this packet
            var numValues = bytes[1];

            if ((numValues)*4 + 6 != bytes.Length)
            {
                Console.WriteLine("Packet size did not match the given amount of values: Is " + bytes.Length + "bytes for " + numValues + " values");
                return;
            }

            // get start index of data
            var startIndex = BitConverter.ToInt32(bytes, 2);

            // convert data
            for (int i = 0; i < numValues; i++)
            {
                data.SetItem(BitConverter.ToSingle(bytes, i*4+6), startIndex+i);
            }

            // set new data and rerender
            renderer.DataPoints = data.GetAllItems();
            renderer.InvalidateCanvas();

        }

    }
}
