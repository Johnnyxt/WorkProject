using JW8307A.ViewModels;
using System.Windows;

namespace JW8307A
{
    /// <summary>
    /// MainLoginView.xaml 的交互逻辑
    /// </summary>
    public partial class MainLoginView : Window
    {
        public MainLoginView()
        {
            InitializeComponent();
            IsEnabledChanged += MainLoginView_IsEnabledChanged;
            DataContext = new MainLoginViewModel();
        }

        private void MainLoginView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Hide();
            }
        }
    }
}