using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace NSTest {
    [TemplatePart(Name = "UpButtonElement", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "DownButtonElement", Type = typeof(RepeatButton))]
    [TemplateVisualState(Name = "Positive", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "Negative", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusedStates")]
    public class NumericUpDown : Control {
        public NumericUpDown() {
            DefaultStyleKey = typeof(NumericUpDown);
            this.IsTabStop = true;
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(int), typeof(NumericUpDown),
                new PropertyMetadata(
                    new PropertyChangedCallback(ValueChangedCallback)));

        public int Value { get { return (int)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        static void ValueChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            NumericUpDown ctl = (NumericUpDown)obj;
            int newValue = (int)args.NewValue;

            // Call UpdateStates because the Value might have caused the
            // control to change ValueStates.
            ctl.UpdateStates(true);

            // Call OnValueChanged to raise the ValueChanged event.
            ctl.OnValueChanged(
                new ValueChangedEventArgs(NumericUpDown.ValueChangedEvent,
                    newValue));
        }

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct,
                          typeof(ValueChangedEventHandler), typeof(NumericUpDown));

        public event ValueChangedEventHandler ValueChanged {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        protected virtual void OnValueChanged(ValueChangedEventArgs e) {
            // Raise the ValueChanged event so applications can be alerted
            // when Value changes.
            RaiseEvent(e);
        }

        void UpdateStates(bool useTransitions) {
            if (Value >= 0) {
                VisualStateManager.GoToState(this, "Positive", useTransitions);
            } else {
                VisualStateManager.GoToState(this, "Negative", useTransitions);
            }

            if (IsFocused) {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
            } else {
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
            }

        }

        public override void OnApplyTemplate() {
            UpButtonElement = GetTemplateChild("UpButton") as RepeatButton;
            DownButtonElement = GetTemplateChild("DownButton") as RepeatButton;
            //TextElement = GetTemplateChild("TextBlock") as TextBlock;

            UpdateStates(false);
        }

        RepeatButton downButtonElement;

        RepeatButton DownButtonElement {
            get { return downButtonElement; }

            set {
                if (downButtonElement != null)
                    downButtonElement.Click -= new RoutedEventHandler(downButtonElement_Click);
                downButtonElement = value;
                if (downButtonElement != null)
                    downButtonElement.Click += new RoutedEventHandler(downButtonElement_Click);
            }
        }

        void downButtonElement_Click(object sender, RoutedEventArgs e) {
            Value--;
        }

        RepeatButton upButtonElement;

        RepeatButton UpButtonElement {
            get { return upButtonElement; }

            set {
                if (upButtonElement != null)
                    upButtonElement.Click -= new RoutedEventHandler(upButtonElement_Click);
                upButtonElement = value;
                if (upButtonElement != null)
                    upButtonElement.Click += new RoutedEventHandler(upButtonElement_Click);
            }
        }

        void upButtonElement_Click(object sender, RoutedEventArgs e) {
            Value++;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            Focus();
        }

        protected override void OnGotFocus(RoutedEventArgs e) {
            base.OnGotFocus(e);
            UpdateStates(true);
        }

        protected override void OnLostFocus(RoutedEventArgs e) {
            base.OnLostFocus(e);
            UpdateStates(true);
        }
    }

    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

    public class ValueChangedEventArgs : RoutedEventArgs {
        private int _value;

        public ValueChangedEventArgs(RoutedEvent id, int num) {
            _value = num;
            RoutedEvent = id;
        }

        public int Value { get { return _value; } }
    }
}