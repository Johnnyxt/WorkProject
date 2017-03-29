using JW8307A.ViewModels;
using System;
using System.Windows;

namespace JW8307A.Views
{
    /// <summary>
    /// TestView.xaml 的交互逻辑
    /// </summary>
    public partial class TestView
    {
        public TestView()
        {
            InitializeComponent();
            this.DataContext = new TestViewMode();

            //TbSerialNum.Focus();
            //BtnTest.Focus();
        }

        private void TestView_OnClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}