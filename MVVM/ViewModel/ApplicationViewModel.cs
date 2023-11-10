using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Paint.Core;
using Paint.MVVM.Model;

namespace Paint.MVVM.ViewModel
{
    public class ApplicationViewModel : ObservableObject
    {
        #region MoveWindow
        RelayCommand moveWindow;
        public RelayCommand MoveWindow
        {
            get
            {
                return moveWindow ??
                       (moveWindow = new RelayCommand(obj =>
                       {
                           if (Mouse.LeftButton == MouseButtonState.Pressed)
                           {
                               Application.Current.MainWindow.DragMove();
                           }
                       }));
            }
        }
        #endregion
        #region CloseWindow
        private RelayCommand closeWindow;

        public RelayCommand CloseWindow
        {
            get { return closeWindow ??
                         (closeWindow = new RelayCommand(obj =>
                         {
                             Application.Current.MainWindow.Close();
                         })); }
        }
        #endregion
        #region MaximazeWindow
        private RelayCommand windowMax;

        public RelayCommand WindowMax
        {
            get
            {
                return windowMax ??
                       (windowMax = new RelayCommand(obj =>
                       {
                           var state = Application.Current.MainWindow.WindowState;
                           if (state != WindowState.Maximized)
                           {
                               Application.Current.MainWindow.WindowState = WindowState.Maximized;
                           }
                           else
                           {
                               Application.Current.MainWindow.WindowState = WindowState.Normal;
                           }
                       }));
            }
        }
        #endregion
        #region MinimazeWindow
        private RelayCommand windowMin;

        public RelayCommand WindowMin
        {
            get
            {
                return windowMin ??
                       (windowMin = new RelayCommand(obj =>
                       {
                          Application.Current.MainWindow.WindowState = WindowState.Minimized;
                          
                       }));
            }
        }
        #endregion

        private RelayCommand _scale;

        public RelayCommand Scale
        {
            get
            {
                return _scale ?? (_scale = new RelayCommand(obj =>
                {
                    double scaleStep = 10;

                    var mouseWheel = obj as MouseWheelEventArgs;
                    if (mouseWheel.Delta > 0)
                    {
                        ScaleFactor += scaleStep;
                    }
                    else
                    {
                        ScaleFactor -= scaleStep;
                    }
                }));
            }
        }


        
        private double _scaleFactor = 100;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (value <= 0)
                {
                    _scaleFactor = 1;
                    return;
                }
                _scaleFactor = value;
                OnPropertyChanged("ScaleFactor");
                
            }
        }

        private InkCanvas mycanvas = new InkCanvas()
        {
            Background = Brushes.Black,
            Width = 300,
            Height = 300
        };

        public InkCanvas Mycanvas
        {
            get { return mycanvas; }
            set { mycanvas = value; }
        }


        private ContentControl _drawingArea;

        public ContentControl DrawingSpace
        {
            get => _drawingArea;
            set
            {
                _drawingArea = value;
                OnPropertyChanged("DrawingArea");
            }
        }
        public ApplicationViewModel()
        {
        
        }
    }
}
