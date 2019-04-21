using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piealytics
{
    class DataManager
    {

        public DataRenderer Renderer;

        private List<float> _data;

        // constructor
        public DataManager(DataRenderer renderer)
        {

            // set renderer
            this.Renderer = renderer;

            this._data = this.CreateDummyValues();
            this.Renderer.DataPoints = this._data;

        }

        // create dummy values
        private List<float> CreateDummyValues()
        {

            const int amount = 1000;
            float[] range = { 0.0f, 150.0f };

            List<float> res = new List<float>(amount);
            Random random = new Random();

            for (var i = 0; i < amount; i++)
            {
                res.Add((float)random.NextDouble()*150.0f);
            }

            return res;

        }

    }
}
