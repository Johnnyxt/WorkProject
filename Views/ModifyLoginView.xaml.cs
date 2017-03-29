using JW8307A.ViewModels;
using System.Windows;

namespace JW8307A.Views
{
    /// <summary>
    /// ModifyLoginView.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyLoginView : Window
    {
        public ModifyLoginView()
        {
            InitializeComponent();
            IsEnabledChanged += ModifyLoginView_IsEnabledChanged;
            this.DataContext = new ModifyLoginViewModel();
        }

        private void ModifyLoginView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Hide();
            }
        }
    }
}