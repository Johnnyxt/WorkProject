using JW8307A.Commands;
using JW8307A.Models;
using JW8307A.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace JW8307A.ViewModels
{
    internal class MainLoginViewModel : NotificationObject
    {
        private readonly DispatcherTimer dPort = new DispatcherTimer();
        private readonly byte[] txBytes = new byte[1024];
        private string conState;
        private int gCnt;
        private bool isLink;
        private bool isMainLoginFailed = true;
        private string mainLoginName;
        private string mainLoginPsd;
        private int portCount;
        private string proType;
        private string selectedPort;
        private string title;
        private ObservableCollection<string> obsPort = new ObservableCollection<string>();

        public MainLoginViewModel()
        {
            Title = Person.Vision;
            LoadXmlDoc();
            SerialPortHelper.ConnectReceived += SpHelper_ConnectReceived;
            ConState = "连接";
            dPort.IsEnabled = true;
            dPort.Interval = TimeSpan.FromMilliseconds(100);
            dPort.Tick += DPort_Tick;
            ConnPortCommand = new DelegateCommand(ConnPort);
            MainLoginCommand = new DelegateCommand(MainLogin);
            CloseCommand = new DelegateCommand(Close);
        }

        public ICommand ConnPortCommand { get; set; }
        public ICommand MainLoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public string SelectedPort
        {
            get { return selectedPort; }
            set
            {
                selectedPort = value;
                RaisePropertyChanged("SelectedPort");
            }
        }

        public ObservableCollection<string> ObsPort
        {
            get { return obsPort; }
            set
            {
                obsPort = value;
                RaisePropertyChanged("ObsPort");
            }
        }

        public string MainLoginName
        {
            get { return mainLoginName; }
            set
            {
                mainLoginName = value;
                RaisePropertyChanged("MainLoginName");
            }
        }

        public string MainLoginPsd
        {
            get { return mainLoginPsd; }
            set
            {
                mainLoginPsd = value;
                RaisePropertyChanged("MainLoginPsd");
            }
        }

        public string ConState
        {
            get { return conState; }
            set
            {
                conState = value;
                RaisePropertyChanged("ConState");
            }
        }

        public string ProType
        {
            get { return proType; }
            set
            {
                proType = value;
                RaisePropertyChanged("ProType");
            }
        }

        public bool IsMainLoginFailed
        {
            get { return isMainLoginFailed; }
            set
            {
                isMainLoginFailed = value;
                RaisePropertyChanged("IsMainLoginFailed");
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        private static void Close(object obj)
        {
            Application.Current.Shutdown();
        }

        private void SpHelper_ConnectReceived(byte[] data)
        {
            isLink = true;
            MessageBox.Show("连接成功", "JW8307A", MessageBoxButton.OK);
            var revision = BitConverter.ToUInt32(data, 0);
            ProType = revision.ToString("X");
            if (revision == 0x8307220)
            {
                Person.UsingWaves.Waves = new List<string> { "1310nm", "1550nm" };
                Person.IsAddIl2Mode = true;
            }
            else
            {
                Person.UsingWaves.Waves = new List<string> { "1310nm", "1550nm" };
                Person.IsAddIl2Mode = false;
            }
        }

        private void MainLogin(object obj)
        {
            if (!isLink)
            {
                MessageBox.Show("串口未连接", "JW8307A", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            if (Person.LoginName == MainLoginName && Person.LoginPsd == mainLoginPsd)
            {
                IsMainLoginFailed = false;
                Person.IsBasicInfoViewEnable = true;
                Person.IsUserSetViewEnable = true;

                //跳转到数据库登录界面
                var view = new DbLoginView();
                view.ShowDialog();
            }
            else
            {
                Person.SqlHelper = new DbHelperSql(Person.ServerName, Person.DbName, Person.DbLoginName, Person.DbLoginPsd);
                if (Person.SqlHelper.IsConnect)
                {
                    IsMainLoginFailed = false;
                    Person.IsBasicInfoViewEnable = false;
                    Person.IsUserSetViewEnable = false;
                    DbLoginViewModel.SqlHelper = Person.SqlHelper;
                    var view = new BasicInfoView();
                    view.ShowDialog();
                }
                else
                {
                    MessageBox.Show("数据库登录失败,请联系管理员");
                }
            }
        }

        private static void LoadXmlDoc()
        {
            try
            {
                Person.XmlDoc.Load(Person.XmlPath);
                var selectSingleNode = Person.XmlDoc.SelectSingleNode("Setting/MainLogin");
                if (selectSingleNode == null)
                    return;
                var nodeList = selectSingleNode.ChildNodes;
                var xNode = nodeList.Item(0);
                if (xNode != null)
                {
                    Person.LoginName = xNode.InnerText.Trim();
                }
                xNode = nodeList.Item(1);
                if (xNode != null)
                {
                    Person.LoginPsd = xNode.InnerText.Trim();
                }

                selectSingleNode = Person.XmlDoc.SelectSingleNode("Setting/WorkOrder");
                if (selectSingleNode == null)
                {
                    return;
                }
                Person.WorkOrder = selectSingleNode.InnerText.Trim();
                selectSingleNode = Person.XmlDoc.SelectSingleNode("Setting/ServerLogin");
                if (selectSingleNode == null)
                    return;
                nodeList = selectSingleNode.ChildNodes;
                xNode = nodeList.Item(0);
                if (xNode != null)
                {
                    Person.ServerName = xNode.InnerText.Trim();
                }
                xNode = nodeList.Item(1);
                if (xNode != null)
                {
                    Person.DbName = xNode.InnerText.Trim();
                }
                xNode = nodeList.Item(2);
                if (xNode != null)
                {
                    Person.DbLoginName = xNode.InnerText.Trim();
                }
                xNode = nodeList.Item(3);
                if (xNode != null)
                {
                    Person.DbLoginPsd = xNode.InnerText.Trim();
                }
                selectSingleNode = Person.XmlDoc.SelectSingleNode("Setting/BasicInfo");
                if (selectSingleNode == null)
                {
                    return;
                }
                nodeList = selectSingleNode.ChildNodes;
                xNode = nodeList.Item(0);
                if (xNode != null) Person.OpName = xNode.InnerText.Trim();
                xNode = nodeList.Item(1);
                if (xNode != null) Person.Description = xNode.InnerText.Trim();
                xNode = nodeList.Item(2);
                if (xNode != null) Person.OpWorkId = xNode.InnerText.Trim();
                xNode = nodeList.Item(3);
                if (xNode != null) Person.Product = xNode.InnerText.Trim();
                xNode = nodeList.Item(4);
                if (xNode != null) Person.OpTeam = xNode.InnerText.Trim();
                xNode = nodeList.Item(5);
                if (xNode != null) Person.ProductLine = xNode.InnerText.Trim();
                xNode = nodeList.Item(6);
                if (xNode != null) Person.Department = xNode.InnerText.Trim();
                xNode = nodeList.Item(7);
                if (xNode != null) Person.AteName = xNode.InnerText.Trim();
                xNode = nodeList.Item(8);
                if (xNode != null) Person.OperationSequence = xNode.InnerText.Trim();
                xNode = nodeList.Item(9);
                if (xNode != null) Person.AteVersion = xNode.InnerText.Trim();
                xNode = nodeList.Item(10);
                if (xNode != null) Person.SiteCode = xNode.InnerText.Trim();
                xNode = nodeList.Item(11);
                if (xNode != null) Person.TestSoftName = xNode.InnerText.Trim();
                xNode = nodeList.Item(12);
                if (xNode != null) Person.SaveExcelPath = xNode.InnerText.Trim();
                xNode = nodeList.Item(13);
                if (xNode != null) Person.SaveExcelName = xNode.InnerText.Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DPort_Tick(object sender, EventArgs e)
        {
            var keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            if (keyCom == null) return;
            var subKeys = keyCom.GetValueNames();
            if (portCount == subKeys.Length) return;
            obsPort.Clear();
            foreach (var name in subKeys)
            {
                var port = (string)keyCom.GetValue(name);
                obsPort.Add(port);
                portCount = subKeys.Length;
                SelectedPort = port;
            }
        }

        private void ConnPort(object obj)
        {
            if (isLink)
            {
                Person.SpHelper.ClosePort();
                isLink = false;
                ConState = "连接";
                return;
            }

            if (!Person.SpHelper.IsOpen)
            {
                try
                {
                    Person.SpHelper.PortName = SelectedPort;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            try
            {
                Person.SpHelper.OpenPort();
                //发送连接指令
                gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(txBytes, 0x40);
                Person.SpHelper.SerialPortWrite(txBytes, gCnt);
                Array.Clear(txBytes, 0, txBytes.Length);
                Thread.Sleep(200);
                if (!isLink)
                {
                    Person.SpHelper.ClosePort();
                    return;
                }
                ConState = "断开";
                SerialPortHelper.DTimer.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}