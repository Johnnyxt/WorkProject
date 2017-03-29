using JW8307A.ViewModels;
using System;
using System.Windows;

namespace JW8307A.Views
{
    /// <summary>
    /// BasicInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class BasicInfoView
    {
        public BasicInfoView()
        {
            InitializeComponent();
            IsEnabledChanged += BasicInfoView_IsEnabledChanged;
            DataContext = new BasicInfoViewModel();
        }

        private void BasicInfoView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Hide();
            }
        }

        private void BasicInfoView_OnClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}