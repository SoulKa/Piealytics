using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piealytics
{
    /// <summary>
    /// Renders given data values on any given control object.
    /// </summary>
    class DataRenderer
    {

        // size of the canvas
        private Size componentSize;
        private const float PADDING = 30.0f;
        private Color BACKGROUND_COLOR = Color.Black;
        private Pen DATA_COLOR = new Pen(Color.White, 2);
        private Pen AXES_COLOR = new Pen(Color.DarkGray, 0.5f);
        private Brush AXES_LABEL_COLOR = Brushes.DarkGray;

        // call when the chart should redrawn
        public Action InvalidateCanvas;

        // minimum and maximum of values to display
        public Tuple<float, float> Range;

        // list of data points
        public float[] DataPoints = new float[0];

        // constructor
        public DataRenderer(Control destComponent)
        {
            
            // get component size
            componentSize = destComponent.Size;

            // set invalidate function
            InvalidateCanvas = destComponent.Invalidate;

            // listen to paint and resize event
            destComponent.Resize += OnComponentResize;
            destComponent.Paint += Draw;

        }

        /// <summary>
        /// The resize procedure. Saves the new canvas size and triggers a redraw.
        /// </summary>
        /// <param name="sender">The control object</param>
        /// <param name="e"></param>
        private void OnComponentResize(object sender, EventArgs e)
        {

            if (Range == null) return;

            // cast to Control
            Control destComponent = (Control) sender;

            // set new size
            componentSize = destComponent.Size;

            // rerender
            InvalidateCanvas();

        }

        private void DrawAxes(Graphics g)
        {

            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            // calculate axis positioning
            const int AXIS_AMOUNT = 10;
            var rangeDiff = Range.Item2 - Range.Item1;
            var decimalPlaces_diff = CalcDecimalPlace(rangeDiff);
            var decimalMultiplicator = Math.Pow(10, -decimalPlaces_diff);

            var unroundedStep = rangeDiff / AXIS_AMOUNT;
            var stepsPerUnit = 1;
            double step;
            do
            {
                step = Math.Round(unroundedStep * stepsPerUnit * decimalMultiplicator * 10.0) / stepsPerUnit / decimalMultiplicator / 10.0;
                Console.WriteLine(step + " " + unroundedStep);
                stepsPerUnit++;
            } while (step == 0.0 || rangeDiff / step < AXIS_AMOUNT);

            var lowerBound = Math.Floor(Range.Item1 / decimalMultiplicator) * decimalMultiplicator;
            Console.WriteLine(lowerBound + " " + step);

            for (double v = lowerBound; v <= Range.Item2; v += step)
            {
                if (v < Range.Item1) continue;
                
                // round to make values like 2.4999999999 to 2.5
                var correctedV = Math.Round(v, Math.Max(-decimalPlaces_diff + 2, 0));
                var y = componentSize.Height - PADDING - (float)((correctedV-Range.Item1) * (componentSize.Height-2*PADDING) / rangeDiff);
                g.DrawLine(AXES_COLOR, 0, y, componentSize.Width, y);
                g.DrawString(correctedV.ToString(), new Font("Arial", 10), AXES_LABEL_COLOR, 3, y + 3);
                
            }

        }

        /// <summary>
        /// The render procedure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Needed to access the Grapics object</param>
        private void Draw(object sender, PaintEventArgs e)
        {

            // clone list for thread safety
            float[] dataPoints = (float[])DataPoints.Clone();

            // clear screen
            e.Graphics.Clear(BACKGROUND_COLOR);
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // never divide by zero bruh
            if (dataPoints.Length == 0) return;

            // calculate scale
            float widthPerDataPoint = (float)componentSize.Width / dataPoints.Length;
            float heightPerValue = (componentSize.Height-PADDING*2) / (Range.Item2 - Range.Item1);

            // add points
            for (int i = 1; i < dataPoints.Length; i++)
            {
                e.Graphics.DrawLine(DATA_COLOR, (i-1)*widthPerDataPoint, CropToRange(dataPoints[i - 1]) * heightPerValue + PADDING, i*widthPerDataPoint, CropToRange(dataPoints[i]) * heightPerValue + PADDING);
            }

            // draw axes
            DrawAxes(e.Graphics);

        }

        /// <summary>
        /// Crops a value so it is within the defined range of values to display
        /// </summary>
        /// <param name="v">The value to crop</param>
        /// <returns>The cropped value</returns>
        private float CropToRange(float v)
        {
            return Range.Item2 - Math.Min(Math.Max(Range.Item1, v), Range.Item2);
        }

        private static int CalcDecimalPlace(float v)
        {
            if (v == 0.0f) return 0;
            var log10 = Math.Log10(v);
            return Convert.ToInt32(Math.Floor(log10));
        }

    }
}
