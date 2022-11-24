using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LenovoLegionToolkit.WPF.Controls
{
    public class SelectableControl : UserControl
    {
        public class SelectedEventArgs : EventArgs
        {
            public Func<FrameworkElement, bool> ContainsCenter { get; }

            public SelectedEventArgs(Func<FrameworkElement, bool> containsCenter) => ContainsCenter = containsCenter;
        }

        private readonly Grid _grid = new()
        {
            Background = new SolidColorBrush(Colors.Transparent)
        };

        private readonly ContentPresenter _contentPresenter = new();

        private readonly Canvas _canvas = new();

        private readonly Rectangle _selection = new()
        {
            StrokeThickness = 2,
            Visibility = Visibility.Collapsed
        };

        private bool _mouseDown;
        private Point _mouseDownPosition;

        public Brush Stroke
        {
            get => _selection.Stroke;
            set => _selection.Stroke = value;
        }

        public Brush Fill
        {
            get => _selection.Fill;
            set => _selection.Fill = value;
        }

        public event EventHandler<SelectedEventArgs>? Selected;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _contentPresenter.Content = Content;

            _canvas.Children.Add(_selection);

            _grid.Children.Add(_contentPresenter);
            _grid.Children.Add(_canvas);

            _grid.MouseDown += Grid_OnMouseDown;
            _grid.MouseMove += Grid_OnMouseMove;
            _grid.MouseUp += Grid_OnMouseUp;

            Content = _grid;
        }

        private void Grid_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;

            _grid.CaptureMouse();
            _mouseDownPosition = e.GetPosition(_grid);

            Canvas.SetLeft(_selection, _mouseDownPosition.X);
            Canvas.SetTop(_selection, _mouseDownPosition.Y);

            _selection.Width = 0;
            _selection.Height = 0;

            _selection.Visibility = Visibility.Visible;
        }

        private void Grid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown)
                return;

            var mousePosition = e.GetPosition(_grid);

            if (_mouseDownPosition.X < mousePosition.X)
            {
                Canvas.SetLeft(_selection, _mouseDownPosition.X);
                _selection.Width = mousePosition.X - _mouseDownPosition.X;
            }
            else
            {
                Canvas.SetLeft(_selection, mousePosition.X);
                _selection.Width = _mouseDownPosition.X - mousePosition.X;
            }

            if (_mouseDownPosition.Y < mousePosition.Y)
            {
                Canvas.SetTop(_selection, _mouseDownPosition.Y);
                _selection.Height = mousePosition.Y - _mouseDownPosition.Y;
            }
            else
            {
                Canvas.SetTop(_selection, mousePosition.Y);
                _selection.Height = _mouseDownPosition.Y - mousePosition.Y;
            }
        }

        private void Grid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_mouseDown)
                return;

            _mouseDown = false;
            _grid.ReleaseMouseCapture();
            _selection.Visibility = Visibility.Collapsed;

            var mouseUpPosition = e.GetPosition(_grid);

            var minX = Math.Min(_mouseDownPosition.X, mouseUpPosition.X);
            var minY = Math.Min(_mouseDownPosition.Y, mouseUpPosition.Y);
            var maxX = Math.Max(_mouseDownPosition.X, mouseUpPosition.X);
            var maxY = Math.Max(_mouseDownPosition.Y, mouseUpPosition.Y);

            var rectangle = new Rect(minX, minY, maxX - minX, maxY - minY);

            bool ContainsCenter(FrameworkElement element)
            {
                var elementRect = element.TransformToVisual(_grid)
                    .TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));

                var elementCenterX = elementRect.X + elementRect.Width / 2;
                var elementCenterY = elementRect.Y + elementRect.Height / 2;

                return rectangle.Contains(elementCenterX, elementCenterY);
            }

            Selected?.Invoke(this, new SelectedEventArgs(ContainsCenter));
        }
    }
}
