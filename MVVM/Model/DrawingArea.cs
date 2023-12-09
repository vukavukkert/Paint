using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Paint.Core;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Paint.MVVM.Model
{
    public class DrawingArea : ObservableObject
    {
        private double zoom = 0;
        private Color _currentColor = new Color()
        {
            R = 0,
            G = 0,
            B = 0,
            A = 225
        };

        private Pen _stroke = new Pen()
        {
            Width = 1,
            Height = 10
        };

        public Pen Stroke
        {
            get { return _stroke; }
            set
            {
                if (value != null)
                {
                    _stroke = value;
                    DrawCanvas.EditingMode = _stroke.EditingMode;
                    DrawCanvas.DefaultDrawingAttributes = _stroke.StrokeSettings;
                }
            }
        }

        public Color CurrentColor
        {
            get { return _currentColor; }
            set
            {
                _currentColor = value;
                Stroke.PenColor = _currentColor;
                DrawCanvas.DefaultDrawingAttributes.Color = _currentColor;
            }
        }

        private InkCanvas _drawCanvas = new InkCanvas()
        {
            Width = 400,
            Height = 400,
            Opacity = 1,
            RenderTransformOrigin = new Point(0.5, 0.5),
            ClipToBounds = true
        };

        public InkCanvas DrawCanvas
        {
            get { return _drawCanvas; }
            set { _drawCanvas = value; }
        }

        private bool isLeftButtonPressed = false;
        private Point origin;

        private Point start;

        private Grid _container = new Grid()
        {
            Background = Brushes.Gray,
            ClipToBounds = true,
        };

        public Grid Container
        {
            get { return _container; }
            set { _container = value; }
        }

        public DrawingArea()
        {
            SetUpCanvas(DrawCanvas);
        }

        private void ZoomIn(object sender, MouseWheelEventArgs e)
        {
            var transform =
                (ScaleTransform)((TransformGroup)DrawCanvas.RenderTransform).Children.First(c => c is ScaleTransform);
           zoom = e.Delta > 0 ? .2 : -.2;

            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }

        private void SpacePressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;
            if (e.IsDown)
            {
                Mouse.OverrideCursor = Cursors.Hand;
                DrawCanvas.EditingMode = InkCanvasEditingMode.None;
                DrawCanvas.MouseLeftButtonDown += LeftButtonPressed;
                DrawCanvas.MouseLeftButtonUp += LeftButtonReleased;
                DrawCanvas.MouseMove += MoveMouse;
                DrawCanvas.CaptureMouse();

            }
            else
            {
                DrawCanvas.MouseMove -= MoveMouse;
                DrawCanvas.MouseLeftButtonUp -= LeftButtonReleased;
                DrawCanvas.MouseLeftButtonDown -= LeftButtonPressed;
                isLeftButtonPressed = false;
                DrawCanvas.ReleaseMouseCapture();
                Mouse.OverrideCursor = Cursors.Cross;
                DrawCanvas.EditingMode = Stroke.EditingMode;
            }
        }

        private void LeftButtonReleased(object sender, MouseButtonEventArgs e)
        {
            isLeftButtonPressed = false;
        }

        private void LeftButtonPressed(object sender, MouseButtonEventArgs e)
        {
            isLeftButtonPressed = true;
            var translateTransform =
                (TranslateTransform)((TransformGroup)DrawCanvas.RenderTransform).Children.First(c =>
                    c is TranslateTransform);
            start = e.GetPosition(Container);
            origin = new Point(translateTransform.X, translateTransform.Y);
        }

        private void MoveMouse(object sender, MouseEventArgs e)
        {
            if (!DrawCanvas.IsMouseCaptured) return;
            if (!isLeftButtonPressed) return;
            var translateTransform =
                (TranslateTransform)((TransformGroup)DrawCanvas.RenderTransform).Children.First(c =>
                    c is TranslateTransform);

            Vector v = start - e.GetPosition(Container);

            translateTransform.X = origin.X - v.X;

            translateTransform.Y = origin.Y - v.Y;
        }

        public void SaveToPng()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "PNG Files (*.png)|*.png|All files (*.*)|*.*";
            saveFile.DefaultExt = ".png";

            if (saveFile.ShowDialog() == true)
            {
                double dpi = 96;
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    (int)(Container.ActualWidth * dpi / 96.0),
                    (int)(Container.ActualHeight * dpi / 96.0),
                    dpi,
                    dpi,
                    PixelFormats.Pbgra32);


                
                // Рендерим InkCanvas на RenderTargetBitmap
                renderBitmap.Render(DrawCanvas);
                
                // Создаем BitmapEncoder для сохранения изображения в файл
                PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                
                // Создаем поток для записи в файл
                using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
                {
                    pngEncoder.Save(fs);
                }

            }
        }

        public void NewFile(int width, int height)
        {
            Container.Children.Clear();
            DrawCanvas = new InkCanvas()
            {
                Width = width,
                Height = height,
                Opacity = 1,
                RenderTransformOrigin = new Point(0.5, 0.5),
                ClipToBounds = true
            };
            SetUpCanvas(DrawCanvas);
        }

        public void ClearCanvas()
        {
            DrawCanvas.Strokes.Clear();
        }
        public void SetUpCanvas(InkCanvas canvas)
        {
            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform();
            TranslateTransform translateTransform = new TranslateTransform();

            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            canvas.RenderTransform = transformGroup;
            Container.Children.Add(canvas);
            Mouse.OverrideCursor = Cursors.Cross;
            canvas.Loaded += (sender, e) => { canvas.Focus(); };


            canvas.MouseWheel += ZoomIn;
            canvas.KeyDown += SpacePressed;
            canvas.KeyUp += SpacePressed;
            canvas.MouseMove += DisplayBrushOverlay;

            FocusManager.SetIsFocusScope(canvas, true);
        }

        private void DisplayBrushOverlay(object sender, MouseEventArgs e)
        {

            Stroke.Position = e.GetPosition(DrawCanvas);

            var border = new Border()
            {
                BorderBrush = new SolidColorBrush(Color.Multiply(CurrentColor, 100)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(100),
                Width = Stroke.Width * Stroke.Size + zoom,
                Height = Stroke.Height * Stroke.Size + zoom,
                RenderTransformOrigin = new Point(0.5,0.5)

            };

            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform();
            TranslateTransform translateTransform = new TranslateTransform();

            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            border.RenderTransform = transformGroup;
            Container.Children.Add(border);

            translateTransform.X = Stroke.Position.X;
            translateTransform.Y = Stroke.Position.Y;

        }

        public void Bind(Grid myGrid)
        {
            myGrid.Children.Add(Container);
        }
    }
}
