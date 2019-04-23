using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piealytics
{
    public partial class MainWindow : Form
    {

        private DataRenderer dataRenderer;
        private DataManager dataManager;
        private NetworkManager networkManager;

        // constructor
        public MainWindow()
        {

            // init window
            InitializeComponent();

            // create managers
            dataRenderer = new DataRenderer(chartFrame);
            networkManager = new NetworkManager(15000);
            dataManager = new DataManager(dataRenderer);

            // start tasks
            Run();
        }

        private async void Run()
        {
            // search raspberry in local network and remember its IP
            var connectionProperties = await networkManager.SearchClientAsync();
            statusBar_statusLabel.Text = "Status: Verbunden mit " + connectionProperties.EndPoint.Address;
            statusBar_frequency.Text = connectionProperties.Frequency + "Hz";
            statusBar_range.Text = connectionProperties.Range.Item1 + " - " + connectionProperties.Range.Item2;

            // set connection properties on data manager and renderer
            dataRenderer.SetRange( connectionProperties.Range );
            dataManager.SetConnectionProperties(connectionProperties);
            dataManager.SetHistoryLength(decimal.ToInt32(input_historyLength.Value));

            // start listening for incoming data and assign the datamanager to it
            networkManager.StartListenerAsync(dataManager.HandleData);
        }

        private void Input_historyLength_ValueChanged(object sender, EventArgs e)
        {
            dataManager.SetHistoryLength(decimal.ToInt32(input_historyLength.Value));
            dataRenderer.InvalidateCanvas();
        }
    }
}
