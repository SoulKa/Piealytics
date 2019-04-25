using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // need two mutexes, so DrawData and DrawGrid can render parallel
        private object resizeMutex1 = new object();
        private object resizeMutex2 = new object();

        // drawing properties
        private const float PADDING = 30.0f;
        private Color BACKGROUND_COLOR = Color.Black;
        private Pen DATA_COLOR = new Pen(Color.White, 2);
        private Pen AXES_COLOR = new Pen(Color.DarkGray, 1.0f);
        private Brush AXES_LABEL_COLOR = Brushes.DarkGray;
        private Font LABEL_FONT = new Font("Arial", 10);

        // bitmaps
        Image gridBitmap;
        Image dataBitmap;
        Graphics gridBitmapGraphics;
        Graphics dataBitmapGraphics;
        bool reallocateBitmaps = true;

        // axes properties
        private double stepY;
        private double lowerBound;
        private float rangeDiff;
        private int decimalPlacesDiff;
        private int stepX;
        private int historyLength;

        // call when the chart should redrawn
        public Action InvalidateCanvas;

        // minimum and maximum of values to display
        private Tuple<float, float> range;

        // list of data points
        public float[] DataPoints = new float[0];

        /// <summary>
        /// Creates a data renderer. This class renders given data in this.DataPoints on a given control.
        /// </summary>
        /// <param name="destComponent">The control to draw on</param>
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
        /// Sets the minimum and maximum value to display
        /// </summary>
        /// <param name="range">The range as a tuple</param>
        public void SetRange(Tuple<float, float> range)
        {

            // set new range
            this.range = range;

            // calculate axis properties
            CalcYAxesProperties();

        }

        /// <summary>
        /// Sets the history length so the time labels are drawn properly
        /// </summary>
        /// <param name="historyLength"></param>
        public void SetHistoryLength(int historyLength)
        {

            this.historyLength = historyLength;
            int multiplicator10 = 1;
            int multiplicatorV = 1;
            stepX = 1;
 
            // multiply by 5 and then by 2 alternating
            do
            {
                stepX = multiplicator10 * multiplicatorV;
                switch(multiplicatorV)
                {
                    case 1:
                        multiplicatorV = 2;
                        break;
                    case 2:
                        multiplicatorV = 5;
                        break;
                    case 5:
                        multiplicatorV = 1;
                        multiplicator10 *= 10;
                        break;
                }
            } while (historyLength / stepX > 5);
            
        }

        /// <summary>
        /// The resize procedure. Saves the new canvas size and triggers a redraw.
        /// </summary>
        /// <param name="sender">The control object</param>
        /// <param name="e"></param>
        private void OnComponentResize(object sender, EventArgs e)
        {

            // cast to Control
            Control destComponent = (Control) sender;

            // set new size
            lock (resizeMutex1)
            {
                lock (resizeMutex2)
                {
                    componentSize = destComponent.Size;
                }
            }

            // set reallocate bool
            reallocateBitmaps = true;

            // rerender
            InvalidateCanvas();

        }

        /// <summary>
        /// Draws the axes on a the bitmap
        /// </summary>
        public void DrawGrid()
        {

            if (range == null || gridBitmap == null || gridBitmapGraphics == null) return;

            lock (resizeMutex1)
            {

                // create bitmap to draw on
                gridBitmapGraphics.Clear(Color.Transparent);

                // iterate from the lowest value to the highest
                for (double v = lowerBound; v <= range.Item2; v += stepY)
                {

                    // ignore values lower than the minimum value
                    if (v < range.Item1)
                    {
                        lowerBound = v;
                        continue;
                    }

                    // round to make values like 2.4999999999 to 2.5
                    var correctedV = Math.Round(v, Math.Max(-decimalPlacesDiff + 2, 0));
                    var y = componentSize.Height - PADDING - (float)((correctedV - range.Item1) * (componentSize.Height - 2 * PADDING) / rangeDiff);
                    gridBitmapGraphics.DrawLine(AXES_COLOR, 0, y, componentSize.Width, y);
                    gridBitmapGraphics.DrawString(correctedV.ToString(), LABEL_FONT, AXES_LABEL_COLOR, 3, y + 3);

                }

                // iterate from 0ms to history length by stepX
                float widthPerMs = (float)componentSize.Width / (float)historyLength;
                for (int t = 0; t < historyLength; t += stepX)
                {
                    var x = componentSize.Width - t * widthPerMs;
                    var label = -t + "ms";
                    gridBitmapGraphics.DrawLine(AXES_COLOR, x, 0.0f, x, (float)componentSize.Height);
                    var labelSize = gridBitmapGraphics.MeasureString(label, LABEL_FONT);
                    gridBitmapGraphics.DrawString(label, LABEL_FONT, AXES_LABEL_COLOR, x - labelSize.Width - 3, componentSize.Height - labelSize.Height - 3);
                }

            }

        }

        /// <summary>
        /// Renderes the data on a new bitmap and returns it
        /// </summary>
        /// <returns>The bitmap with the data on</returns>
        private void DrawData()
        {

            lock (resizeMutex2)
            {

                // create bitmap to draw on
                dataBitmapGraphics.Clear(Color.Transparent);

                // clone list for thread safety
                float[] dataPoints = (float[])DataPoints.Clone();

                // never divide by zero bruh
                if (dataPoints.Length == 0) return;

                // calculate scale
                float widthPerDataPoint = (float)componentSize.Width / (dataPoints.Length - 1);
                float heightPerValue = (componentSize.Height - PADDING * 2) / (range.Item2 - range.Item1);

                // add points
                for (int i = 1; i < dataPoints.Length; i++)
                {
                    dataBitmapGraphics.DrawLine(DATA_COLOR, (i - 1) * widthPerDataPoint, CropToRange(dataPoints[i - 1]) * heightPerValue + PADDING, i * widthPerDataPoint, CropToRange(dataPoints[i]) * heightPerValue + PADDING);
                }

            }

        }

        /// <summary>
        /// The render procedure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Needed to access the Grapics object</param>
        private void Draw(object sender, PaintEventArgs e)
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var drawingTasks = new List<Task>(2);
            
            // check if the the bitmaps must be (re)created because of resizing
            if (reallocateBitmaps)
            {

                // unset bool
                reallocateBitmaps = false;

                // free old bitmaps if present
                if (gridBitmap != null) gridBitmap.Dispose();
                if (dataBitmap != null) dataBitmap.Dispose();
                if (gridBitmapGraphics != null) gridBitmapGraphics.Dispose();
                if (dataBitmapGraphics != null) dataBitmapGraphics.Dispose();

                // create new bitmaps
                lock (resizeMutex1)
                {
                    gridBitmap = new Bitmap(componentSize.Width, componentSize.Height, e.Graphics);
                    dataBitmap = new Bitmap(componentSize.Width, componentSize.Height, e.Graphics);
                }
                gridBitmapGraphics = Graphics.FromImage(gridBitmap);
                dataBitmapGraphics = Graphics.FromImage(dataBitmap);
                dataBitmapGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // only draw the grid if needed (after resize)
                drawingTasks.Add( Task.Run( () => DrawGrid() ) );

            }

            // wait for rendering to finish
            drawingTasks.Add( Task.Run( () => DrawData() ) );
            Task.WaitAll(drawingTasks.ToArray());

            // clear screen
            e.Graphics.Clear(BACKGROUND_COLOR);

            // draw images
            e.Graphics.DrawImage(gridBitmap, 0, 0);
            e.Graphics.DrawImage(dataBitmap, 0, 0);

            watch.Stop();
            //Console.WriteLine("Rendered in " + watch.ElapsedMilliseconds + "ms");

        }

        /// <summary>
        /// Crops a value so it is within the defined range of values to display
        /// </summary>
        /// <param name="v">The value to crop</param>
        /// <returns>The cropped value</returns>
        private float CropToRange(float v)
        {
            return range.Item2 - Math.Min(Math.Max(range.Item1, v), range.Item2);
        }

        /// <summary>
        /// Calculates the axes properties given by this.range
        /// </summary>
        private void CalcYAxesProperties()
        {

            if (range == null) return;

            // calculate axis positioning
            const int AXIS_AMOUNT = 10;
            rangeDiff = range.Item2 - range.Item1;
            decimalPlacesDiff = CalcDecimalPlace(rangeDiff);
            var decimalMultiplicator = Math.Pow(10, -decimalPlacesDiff);

            var unroundedStep = rangeDiff / AXIS_AMOUNT;
            var stepsPerUnit = 1;
            do
            {
                stepY = Math.Round(unroundedStep * stepsPerUnit * decimalMultiplicator * 10.0) / stepsPerUnit / decimalMultiplicator / 10.0;
                stepsPerUnit++;
            } while (stepY == 0.0 || rangeDiff / stepY < AXIS_AMOUNT);

            lowerBound = Math.Floor(range.Item1 / decimalMultiplicator) * decimalMultiplicator;

        }

        /// <summary>
        /// Function to calculate the decimal places a value has
        /// </summary>
        /// <param name="v">The float to calculate the decimal places of</param>
        /// <returns>The decimal places</returns>
        private static int CalcDecimalPlace(float v)
        {
            if (v == 0.0f) return 0;
            var log10 = Math.Log10(v);
            return Convert.ToInt32(Math.Floor(log10));
        }

    }
}
