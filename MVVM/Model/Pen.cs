using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using Paint.Core;

namespace Paint.MVVM.Model
{
    public class Pen : ObservableObject
    {
        private Point _position;

        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value; 
                OnPropertyChanged("Position");
            }
        }

        private Color DebugColor = Colors.Black;
       
        public DrawingAttributes StrokeSettings;

        #region PropertyOfStroke

        public InkCanvasEditingMode EditingMode;

        private bool _isEraser = false;
        public bool IsEraser
        {
            get => _isEraser;
            set
            {
                _isEraser = value;
                if (value)
                {
                    EditingMode = InkCanvasEditingMode.EraseByPoint;
                }
                else
                {
                    EditingMode = InkCanvasEditingMode.Ink;
                }
            } 
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private int _size = 1;
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                StrokeSettings.Height = Height * _size;
                StrokeSettings.Width = Width * _size;
                OnPropertyChanged("Size");
            }
        }


        private int _ActualHeight = 10;
        private int _ActualWidth = 10;
        public double Height
        {
            get { return  _ActualHeight; }
            set
            {
                _ActualHeight = (int)value;
                StrokeSettings.Height = _ActualHeight * Size;
                OnPropertyChanged("Height");
            }
        }
     
        public double Width
        {
            get { return _ActualWidth;}
            set
            {
                _ActualWidth = (int)value;
                StrokeSettings.Width = _ActualWidth * Size;
                OnPropertyChanged("Width");
            }
        }

        public StylusTip TipShape
        {
            get { return StrokeSettings.StylusTip; }
            set
            {
                StrokeSettings.StylusTip = value;
                OnPropertyChanged("TipShape");
            }
        }

        public Color PenColor
        {
            get { return StrokeSettings.Color; }
            set
            {
                StrokeSettings.Color = value;
                OnPropertyChanged("PenColor");
            }
        }

        public bool IgnorePressure
        {
            get { return StrokeSettings.IgnorePressure; }
            set
            {
                StrokeSettings.IgnorePressure = value;
                OnPropertyChanged("IgnorePressure");
            }
        }

        public bool Smoothing
        {
            get { return StrokeSettings.FitToCurve; }
            set
            {
                StrokeSettings.FitToCurve = value;
                OnPropertyChanged("Smoothing");
            }
        }

        public bool IsHighlighter
        {
            get { return StrokeSettings.IsHighlighter; }
            set
            {
                StrokeSettings.IsHighlighter = value;
                OnPropertyChanged("IsHighlighter");
            }
        }

        #endregion
        public Pen(string name = "brush",
                double height = 5,
                double width = 5,
                StylusTip tip = StylusTip.Ellipse,
                bool pressure = true,
                bool smothing = true,
                bool highlighter = false,
                bool isEraser = false,
                InkCanvasEditingMode editingMode = InkCanvasEditingMode.Ink)
        {
            StrokeSettings = new DrawingAttributes();
            Height = height;
            Width = width;
            TipShape = tip;
            IgnorePressure = pressure;
            Smoothing = smothing;
            IsHighlighter = highlighter;
            EditingMode = editingMode;
            Name = name;
            StrokeSettings.Color = DebugColor;
            IsEraser = isEraser;
        }
    }
}
