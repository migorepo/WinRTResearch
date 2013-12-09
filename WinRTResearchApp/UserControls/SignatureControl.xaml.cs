using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
namespace WinRTResearchApp.UserControls
{
    public sealed partial class SignatureControl
    {
        #region Drawing Declarations
        private readonly InkManager _mInkManager = new InkManager();


        private uint _mPenId;
        private uint _touchId;
        private Point _previousContactPt;
        private Point _currentContactPt;
        private double _x1;
        private double _y1;
        private double _x2;
        private double _y2;

        private readonly Color _mCurrentDrawingColor = Colors.Black;

        private const double MCurrentDrawingSize = 1.0;

        private string _mCurrentMode = "Ink";

        public InkManager CurrentManager
        {
            get { return _mInkManager; }
        }

        private readonly Stack<InkStroke> _undoStack = new Stack<InkStroke>();

        #endregion

        #region c.tor
        public SignatureControl()
        {
            InitializeComponent();
            InkMode(); // setup drawing infastructure

            SignatureCanvas.PointerPressed += OnCanvasPointerPressed;
            SignatureCanvas.PointerMoved += OnCanvasPointerMoved;
            SignatureCanvas.PointerReleased += OnCanvasPointerReleased;
            SignatureCanvas.PointerExited += OnCanvasPointerReleased;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the Mode.
        /// </summary>
        private void InkMode()
        {
            _mCurrentMode = "Ink";
            _mInkManager.Mode = InkManipulationMode.Inking;
            SetDefaults(MCurrentDrawingSize, _mCurrentDrawingColor);
        }

        // Change the color and width in the default (used for new strokes) to the values
        // currently set in the current context.
        private void SetDefaults(double strokeSize, Color color)
        {
            var newDrawingAttributes = new InkDrawingAttributes { Size = new Size(strokeSize, strokeSize), Color = color, FitToCurve = true };

            CurrentManager.SetDefaultDrawingAttributes(newDrawingAttributes);
        }

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            double d;
            d = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            return d;
        }

        /// <summary>
        /// Method for resynthesizing the canvas ...
        /// </summary>
        private void RefreshCanvas()
        {
            SignatureCanvas.Children.Clear();
            RenderStrokes();
        }

        /// <summary>
        /// Initial stroke handling method()
        /// </summary>
        private void RenderStrokes()
        {
            var strokes = _mInkManager.GetStrokes();
            foreach (var stroke in strokes)
            {
                if (stroke.Selected)
                {
                    RenderStroke(stroke, stroke.DrawingAttributes.Color, stroke.DrawingAttributes.Size.Width * 2);
                }
                else
                {
                    RenderStroke(stroke, stroke.DrawingAttributes.Color, stroke.DrawingAttributes.Size.Width);
                }
            }
        }

        /// <summary>
        /// Function for adding ink strokes to U
        /// </summary>
        /// <param name="inkStroke"></param>
        private void AddToUndoStack(InkStroke inkStroke)
        {
            _undoStack.Push(inkStroke);
        }

        /// <summary>
        /// Method that renders the stroke back to canvas 
        /// </summary>
        /// <param name="stroke"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="opacity"></param>
        private void RenderStroke(InkStroke stroke, Color color, double width, double opacity = 1)
        {
            AddToUndoStack(stroke);
            var renderingStrokes = stroke.GetRenderingSegments();
            var path = new Path { Data = new PathGeometry() };
            ((PathGeometry)path.Data).Figures = new PathFigureCollection();
            var pathFigure = new PathFigure { StartPoint = renderingStrokes.First().Position };
            ((PathGeometry)path.Data).Figures.Add(pathFigure);
            foreach (var renderStroke in renderingStrokes)
            {
                pathFigure.Segments.Add(new BezierSegment
                {
                    Point1 = renderStroke.BezierControlPoint1,
                    Point2 = renderStroke.BezierControlPoint2,
                    Point3 = renderStroke.Position
                });
            }

            path.StrokeThickness = width;
            path.Stroke = new SolidColorBrush(color);

            path.Opacity = opacity;

            SignatureCanvas.Children.Add(path);
        }
        #endregion

        #region Pointer Event Handlers

        public void OnCanvasPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == _mPenId)
            {
                var pt = e.GetCurrentPoint(SignatureCanvas);

                if (_mCurrentMode == "Erase")
                {
                    Debug.WriteLine("Erasing : Pointer Released");

                    _mInkManager.ProcessPointerUp(pt);
                }
                else
                {
                    // Pass the pointer information to the InkManager. 
                    CurrentManager.ProcessPointerUp(pt);
                }
            }
            else if (e.Pointer.PointerId == _touchId)
            {
                // Process touch input 
            }

            _touchId = 0;
            _mPenId = 0;

            // Call an application-defined function to render the ink strokes. 

            RefreshCanvas();

            e.Handled = true;
        }

        private void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
        {

            if (e.Pointer.PointerId == _mPenId)
            {
                var pt = e.GetCurrentPoint(SignatureCanvas);

                // Render a red line on the canvas as the pointer moves.  
                // Distance() is an application-defined function that tests 
                // whether the pointer has moved far enough to justify  
                // drawing a new line. 
                _currentContactPt = pt.Position;
                _x1 = _previousContactPt.X;
                _y1 = _previousContactPt.Y;
                _x2 = _currentContactPt.X;
                _y2 = _currentContactPt.Y;

                var color = _mCurrentDrawingColor;
                const double size = MCurrentDrawingSize;

                if (Distance(_x1, _y1, _x2, _y2) > 2.0 && _mCurrentMode != "Erase")
                {
                    var line = new Line
                    {
                        X1 = _x1,
                        Y1 = _y1,
                        X2 = _x2,
                        Y2 = _y2,
                        StrokeThickness = size,
                        Stroke = new SolidColorBrush(color)
                    };


                    if (_mCurrentMode == "Highlight") line.Opacity = 0.4;
                    _previousContactPt = _currentContactPt;

                    // Draw the line on the canvas by adding the Line object as 
                    // a child of the Canvas object. 
                    SignatureCanvas.Children.Add(line);
                }

                if (_mCurrentMode == "Erase")
                {
                    Debug.WriteLine("Erasing : Pointer Update");

                    _mInkManager.ProcessPointerUpdate(pt);
                }
                else
                {
                    // Pass the pointer information to the InkManager. 
                    CurrentManager.ProcessPointerUpdate(pt);
                }
            }

            else if (e.Pointer.PointerId == _touchId)
            {
                // Process touch input 
            }


        }

        public void OnCanvasPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location. 
            PointerPoint pt = e.GetCurrentPoint(SignatureCanvas);
            _previousContactPt = pt.Position;

            // Accept input only from a pen or mouse with the left button pressed.  
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen ||
                pointerDevType == PointerDeviceType.Mouse &&
                pt.Properties.IsLeftButtonPressed)
            {
                if (_mCurrentMode == "Erase")
                {
                    Debug.WriteLine("Erasing : Pointer Pressed");

                    _mInkManager.ProcessPointerDown(pt);
                }
                else
                {
                    // Pass the pointer information to the InkManager. 
                    CurrentManager.ProcessPointerDown(pt);
                }

                _mPenId = pt.PointerId;

                e.Handled = true;
            }

            else if (pointerDevType == PointerDeviceType.Touch)
            {
                // Process touch input 
            }
        }

        #endregion

    }
}
