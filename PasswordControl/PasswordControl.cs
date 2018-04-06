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

namespace PasswordControl
{
    [TemplatePart(Name = "PART_ContentHost", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ShowPassword", Type = typeof(FrameworkElement))]
    public class PasswordControl : Control
    {
        private TextBox _passwordTextBox = null;
        private CheckBox _showPasswordCheckBox = null;
        private bool _isShowPassword = false;
        static PasswordControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordControl), new FrameworkPropertyMetadata(typeof(PasswordControl)));
        }

        #region events

        public event RoutedEventHandler PasswordHasChanged
        {
            add
            {
                base.AddHandler(PasswordControl.PasswordHasChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(PasswordControl.PasswordHasChangedEvent, value);
            }
        }

        public static readonly RoutedEvent PasswordHasChangedEvent = EventManager.RegisterRoutedEvent("PasswordHasChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PasswordControl));

        #endregion

        #region dependencyProperties

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordControl), new PropertyMetadata(string.Empty));


        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(PasswordControl), new PropertyMetadata(0));



        public char PasswordChar
        {
            get { return (char)GetValue(PasswordCharProperty); }
            set { SetValue(PasswordCharProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PasswordChar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.Register("PasswordChar", typeof(char), typeof(PasswordControl), new PropertyMetadata('*'));


        #endregion

        #region override

        public override void OnApplyTemplate()
        {
            if (_passwordTextBox != null)
            {
                _passwordTextBox.TextChanged -= PasswordTextBox_TextChanged;
            }
            if (_showPasswordCheckBox != null)
            {
                _showPasswordCheckBox.Checked -= ShowPasswordCheckBox_Checked;
                _showPasswordCheckBox.Unchecked -= ShowPasswordCheckBox_Unchecked;
            }
            base.OnApplyTemplate();
            this._passwordTextBox = (TextBox)this.GetTemplateChild("PART_ContentHost");
            if (_passwordTextBox != null)
            {
                _passwordTextBox.TextChanged += PasswordTextBox_TextChanged;
                _passwordTextBox.Text = GetPasswordChars(Password.Length);
            }

            this._showPasswordCheckBox = (CheckBox)this.GetTemplateChild("PART_ShowPassword");
            if (_showPasswordCheckBox != null)
            {
                _showPasswordCheckBox.Checked += ShowPasswordCheckBox_Checked;
                _showPasswordCheckBox.Unchecked += ShowPasswordCheckBox_Unchecked;
            }
        }

        #endregion

        #region private methods

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null)
            {
                return;
            }

            if ((!checkBox.IsChecked.HasValue || !checkBox.IsChecked.Value) && this._passwordTextBox != null)
            {
                _isShowPassword = false;
                this._passwordTextBox.Text = GetPasswordChars(this.Password.Length);
            }
        }

        private string GetPasswordChars(int length)
        {
            if (length <= 0)
                return string.Empty;
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = PasswordChar;
            }
            return new string(chars);
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null)
            {
                return;
            }

            if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value && this._passwordTextBox != null)
            {
                _isShowPassword = true;
                this._passwordTextBox.Text = this.Password;
            }
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null)
            {
                return;
            }

            var passwordChars = this.GetPasswordChars(textbox.Text.Length);

            if (textbox.Text == passwordChars && textbox.Text.Length == Password.Length)
            {
                return;
            }

            if (_isShowPassword)
            {
                this.Password = this._passwordTextBox.Text;
                return;
            }

            var textChange = e.Changes.ElementAt(0);
            var startText = Password.Substring(0, textChange.Offset);
            var lastStartIndex = textChange.Offset + textChange.RemovedLength;
            var endText = Password.Substring(lastStartIndex, Password.Length - lastStartIndex);

            var addtext = textbox.Text.Substring(textChange.Offset, textChange.AddedLength);
            var newText = startText + addtext + endText;

            if (Password != newText)
                base.RaiseEvent(new RoutedEventArgs(PasswordControl.PasswordHasChangedEvent));

            this.Password = newText;

            this._passwordTextBox.Text = GetPasswordChars(Password.Length);
            _passwordTextBox.CaretIndex = textChange.Offset + textChange.AddedLength;
        }
        #endregion

    }
}
