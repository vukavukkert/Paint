using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Paint.Core;
using Paint.MVVM.Model;
using Paint.MVVM.View;
using Pen = Paint.MVVM.Model.Pen;

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

        #region SaveFile

        private RelayCommand _saveFile;
        public RelayCommand SaveFile
        {
            get { return _saveFile ?? 
                         (_saveFile = new RelayCommand(obj =>
                        {
                            CurrentDrawingArea.SaveToPng();
                        })); }
        }

        #endregion
        #region AddBrush

        private RelayCommand _addBrush;

        public RelayCommand AddBrush
        {
            get
            {
                return _addBrush ?? (_addBrush = new RelayCommand(obj =>
                    {
                        var newPen = new Pen();
                        Pens.Add(newPen);
                    },
                    (o => CurrentPen != null)));
            }
        }

        #endregion
        #region DeleteBrush

        private RelayCommand _deleteBrush;

        public RelayCommand DeleteBrush
        {
            get
            {
                return _deleteBrush ?? (_deleteBrush = new RelayCommand(obj =>
                {

                    var index = Pens.IndexOf(CurrentPen);
                    if (index >= 0)
                    {
                        CurrentPen = new Pen();
                        Pens.RemoveAt(index);
                    }
                }, (o => CurrentPen != null)));
            }
        }

        #endregion
        #region Clear Canvas

        private RelayCommand _clearCanvas;

        public RelayCommand ClearCanvas
        {
            get => _clearCanvas ?? (_clearCanvas = new RelayCommand(o =>
            {
                CurrentDrawingArea.ClearCanvas();
            }));
        }

        #endregion
        #region New File
        private RelayCommand _newFile;
        public RelayCommand NewFile
        {
            get => _newFile ?? (_newFile = new RelayCommand(o =>
            {
                var window = new CreateNewFile();
                window.Show();

                void CreateFile (object sender, EventArgs args)
                {

                    if (int.TryParse(window.WidthBox.Text, out var width)&& int.TryParse(window.HeightBox.Text, out var height))
                    {
                        CurrentDrawingArea.NewFile(width,height);
                        window.CreateButton.Click -= CreateFile;
                    }
                    window.Close();
                };
                window.CreateButton.Click += CreateFile;

            }));
        }
        #endregion

        #region Open File
        private RelayCommand _openFile;

        public RelayCommand OpenFile
        {
            get { return _openFile ?? (_openFile = new RelayCommand(o =>
            {
                CurrentDrawingArea.OpenFile();
            })); }
        }


        #endregion
        #region Undo Stroke

        private RelayCommand _undoStroke;       

        public RelayCommand UndoStroke
        {
            get => _undoStroke ?? (_undoStroke = new RelayCommand(o =>
            {
                CurrentDrawingArea.Undo();
            }));
        }

        #endregion
        #region Redo Stroke

        private RelayCommand _redoStroke;

        public RelayCommand RedoStroke
        {
            get => _redoStroke ?? (_redoStroke = new RelayCommand(o =>
            {
                CurrentDrawingArea.Redo();
            }));
        }

        #endregion
        private ObservableCollection<Pen> _pens = new ObservableCollection<Pen>()
        {
            new Pen()
            {
                Name = "Default"
            },
            new Pen()
            {
                Name = "Eraser",
                Width = 30,
                IsEraser = true,
                Height = 30,
                TipShape = StylusTip.Rectangle
            },
            new Pen()
            {
                Name = "Highliter",
                Width = 20,
                IsEraser = false,
                Height = 40,
                TipShape = StylusTip.Rectangle,
                IgnorePressure = true,
                IsHighlighter = true,
                PenColor = Colors.Yellow
            }
        };

        public ObservableCollection<Pen> Pens
        {
            get { return _pens; }
            set
            {
                _pens = value;
                OnPropertyChanged("Pens");
            }
        }

        private Grid _container = new Grid();

        public Grid ContainerGrid
        {
            get { return _container; }
            set { _container = value; }
        }
        private DrawingArea _drawingArea = new DrawingArea();

        public DrawingArea  CurrentDrawingArea
        {
            get
            {
                return _drawingArea;

            }
            set
            {
                _drawingArea = value; 
            }
        }

        private Pen _currentPen;

        public Pen CurrentPen
        {
            get
            {
                _currentPen = _currentPen ?? new Pen();
                CurrentDrawingArea.Stroke = _currentPen;
                return _currentPen;
            }
            set
            {
                if (_currentPen == null) _currentPen = new Pen();
                _currentPen = value;
                CurrentDrawingArea.Stroke = _currentPen;
                OnPropertyChanged("CurrentPen");
            }
        }
        private byte _alpha = 255;

        public byte Alpha
        {
            get { return _alpha; }
            set
            {
                MainColor.Color = new Color()
                {
                    A = value,
                    R = Red,
                    B = Blue,
                    G = Green,
                };
                CurrentDrawingArea.CurrentColor = MainColor.Color;
                _alpha = value;
                OnPropertyChanged("Alpha");
            }
        }

        private byte _red;

        public byte Red
        {
            get { return _red; }
            set
            {
                MainColor.Color = new Color()
                {
                    A = Alpha,
                    R = value,
                    B = Blue,
                    G = Green,
                };
                CurrentDrawingArea.CurrentColor = MainColor.Color;
                _red = value; 
                OnPropertyChanged("Red");
            }
        }
        private byte _blue;

        public byte Blue
        {
            get { return _blue; }
            set
            {
                MainColor.Color = new Color()
                {
                    A = Alpha,
                    R = Red,
                    B = value,
                    G = Green,
                };
                CurrentDrawingArea.CurrentColor = MainColor.Color;
                _blue = value;
                OnPropertyChanged("Blue");
            }
        }

        private byte _green;

        public byte Green
        {
            get { return _green; }
            set
            {
                MainColor.Color = new Color()
                {
                    A = Alpha,
                    R = Red,
                    B = Blue,
                    G = value,
                };
                CurrentDrawingArea.CurrentColor = MainColor.Color;
                _green = value;
                OnPropertyChanged("Green");
            }
        }

        private SolidColorBrush _mainColor = new SolidColorBrush(Colors.Black);
        public SolidColorBrush MainColor
        {
            get
            {
                return _mainColor;
            }
            set
            {
                _mainColor = value;
                OnPropertyChanged("MainColor");
            }
        }

        private ObservableCollection<StylusTip> _StylusTips = new ObservableCollection<StylusTip>()
        {
            StylusTip.Ellipse,
            StylusTip.Rectangle
        };

        public ObservableCollection<StylusTip> StylusTips
        {
            get { return _StylusTips; }
            set
            {
                _StylusTips = value;
                OnPropertyChanged("StylusTips");
            }
        }


        public ApplicationViewModel()
        {
          CurrentDrawingArea.Bind(ContainerGrid);
        }
    }
}
