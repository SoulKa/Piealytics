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

        private DataRenderer _dataRenderer;
        private DataManager _dataManager;

        // constructor
        public MainWindow()
        {

            // init window
            InitializeComponent();

            // create new data drawer on picture box
            _dataRenderer = new DataRenderer(chartFrame);
            _dataManager = new DataManager(_dataRenderer);


        }

    }
}
