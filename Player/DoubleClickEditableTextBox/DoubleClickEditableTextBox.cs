using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Player
{
    public class DoubleClickEditableTextBox : TextBox
    {
        static DoubleClickEditableTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleClickEditableTextBox), new FrameworkPropertyMetadata(typeof(DoubleClickEditableTextBox)));
        }

        public bool IsInEditMode
        {
            get { return (bool)GetValue(IsInEditModeProperty); }
            set { SetValue(IsInEditModeProperty, value); }
        }

        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(DoubleClickEditableTextBox),
                new PropertyMetadata(false, OnIsInEditModeChanged));

        private static void OnIsInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleClickEditableTextBox tb = d as DoubleClickEditableTextBox;

            var _em = (bool)e.OldValue;
            var em = (bool)e.NewValue;

            if (_em == em) return;

            if (em == true)
                tb.beforeEditText = tb.Text;

            if (em == false)
                tb.MoveFocusToParent();
        }

        private string beforeEditText;

        public DoubleClickEditableTextBox()
        {
            DefaultStyleKey = typeof(DoubleClickEditableTextBox);
            this.PreviewMouseDoubleClick += DoubleClickEditableTextBox_PreviewMouseDoubleClick;
            this.LostFocus += DoubleClickEditableTextBox_LostFocus;
            this.PreviewKeyDown += DoubleClickEditableTextBox_PreviewKeyDown;
        }

        private void DoubleClickEditableTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    IsInEditMode = false;
                    break;
                case Key.Escape:
                    IsInEditMode = false;
                    Text = beforeEditText;
                    beforeEditText = null;
                    break;
            }
        }

        private void DoubleClickEditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsInEditMode = false;
        }

        private void DoubleClickEditableTextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IsInEditMode = true;
        }

        void MoveFocusToParent()
        {
            FrameworkElement parent = (FrameworkElement)this.Parent;
            while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(this);
            FocusManager.SetFocusedElement(scope, parent as IInputElement);
        }
    }
}
