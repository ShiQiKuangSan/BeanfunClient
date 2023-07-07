using System.Windows;
using System.Windows.Input;

namespace Beanfun
{
    public class BaseWindow : Window
    {
        public BaseWindow()
        {
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            var position = e.GetPosition(this);

            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (position.X >= 0 && position.X < this.ActualWidth
                                && position.Y >= 0
                                && position.Y < this.ActualHeight)
            {
                this.DragMove();
            }
        }
    }
}