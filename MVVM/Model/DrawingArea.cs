using System;
using System.Collections.Generic;
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

namespace Paint.MVVM.Model
{
    public class DrawingArea : UIElement
    {

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
            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform();
            TranslateTransform translateTransform = new TranslateTransform();

            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            DrawCanvas.RenderTransform = transformGroup;
            Container.Children.Add(DrawCanvas);
            Mouse.OverrideCursor = Cursors.Cross;

            DrawCanvas.Loaded += (sender, e) =>
            {
                DrawCanvas.Focus();
            };


            DrawCanvas.MouseWheel += ZoomIn;
            DrawCanvas.KeyDown += SpacePressed;
            DrawCanvas.KeyUp += SpacePressed;

            FocusManager.SetIsFocusScope(DrawCanvas, true);
        }   
        private void ZoomIn(object sender, MouseWheelEventArgs e)
        {
            var transform =
                (ScaleTransform)((TransformGroup)DrawCanvas.RenderTransform).Children.First(c => c is ScaleTransform);
            double zoom = e.Delta > 0 ? .2 : -.2;

            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }

        private void SpacePressed(object sender,KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;
            if (e.IsDown)
            {
                Mouse.OverrideCursor = Cursors.Hand;
                DrawCanvas.EditingMode = InkCanvasEditingMode.None;
                DrawCanvas.MouseLeftButtonDown += LeftButtonPressed;
                DrawCanvas.MouseMove+= MoveMouse;
                DrawCanvas.CaptureMouse();

            }
            else
            {
                DrawCanvas.MouseMove -= MoveMouse;
                DrawCanvas.MouseLeftButtonDown -= LeftButtonPressed;
                isLeftButtonPressed = false;
                DrawCanvas.ReleaseMouseCapture();
                Mouse.OverrideCursor = Cursors.Cross;
                DrawCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
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

        public void Bind(Grid myGrid)
        {
            myGrid.Children.Add(Container);
        }
    }
}
