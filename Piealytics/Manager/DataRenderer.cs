using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piealytics
{
    class DataRenderer
    {

        // size of the canvas
        private Size _componentSize;

        // call when the chart should redrawn
        public Invalidate InvalidateCanvas;
        public delegate void Invalidate();

        // minimum and maximum of values to display
        public Tuple<float, float> Range = new Tuple<float, float>(0.0f, 150.0f);

        // list of data points
        public List<float> DataPoints = new List<float>(1000);

        // constructor
        public DataRenderer(Control destComponent)
        {
            
            // get component size
            _componentSize = destComponent.Size;

            // set invalidate function
            InvalidateCanvas = destComponent.Invalidate;

            // listen to paint and resize event
            destComponent.Resize += OnComponentResize;
            destComponent.Paint += Draw;

        }

        // resize procedure
        private void OnComponentResize(object sender, EventArgs e)
        {

            // cast to Control
            Control destComponent = (Control) sender;

            // set new size
            _componentSize = destComponent.Size;

            // rerender
            InvalidateCanvas();

        }

        // render procedure
        private void Draw(object sender, PaintEventArgs e)
        {

            // clone list for thread safety
            float[] dataPoints = DataPoints.ToArray();

            // clear screen
            e.Graphics.Clear(Color.White);

            // never divide by zero bruh
            if (dataPoints.Length == 0) return;

            // calculate scale
            float widthPerDataPoint = _componentSize.Width*1.0f / dataPoints.Length;
            float heightPerValue = _componentSize.Height*1.0f / (Range.Item2 - Range.Item1);

            // add points
            for (int i = 1; i < dataPoints.Length; i++)
            {
                e.Graphics.DrawLine(Pens.Black, (i-1)*widthPerDataPoint, CropToRange(dataPoints[i - 1]) * heightPerValue, i*widthPerDataPoint, CropToRange(dataPoints[i]) * heightPerValue);
            }

        }

        // crops a value so it is within the defined range of values to display
        private float CropToRange(float v)
        {
            return Math.Min(Math.Max(Range.Item1, v), Range.Item2);
        }

    }
}
