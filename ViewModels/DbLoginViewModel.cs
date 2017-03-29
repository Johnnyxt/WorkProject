using JW8307A.Commands;
using JW8307A.Views;
using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace JW8307A.ViewModels
{
    internal class DbLoginViewModel : NotificationObject
    {
        private string serverName;  //服务器名称
        private string dbName;     //数据库名称
        private string dbLoginName;//登录名
        private string dbLoginPsd;//登录密码
        public static DbHelperSql SqlHelper;
        public ICommand DbLoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        private bool isDbLoginFailed = true;

        public string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                RaisePropertyChanged("ServerName");
            }
        }

        public string DbName
        {
            get { return dbName; }
            set
            {
                dbName = value;
                RaisePropertyChanged("DbName");
            }
        }

        public string DbLoginName
        {
            get { return dbLoginName; }
            set
            {
                dbLoginName = value;
                RaisePropertyChanged("DbLoginName");
            }
        }

        public string DbLoginPsd
        {
            get { return dbLoginPsd; }
            set
            {
                dbLoginPsd = value;
                RaisePropertyChanged("DbLoginPsd");
            }
        }

        public bool IsDbLoginFailed
        {
            get { return isDbLoginFailed; }
            set
            {
                isDbLoginFailed = value;
                RaisePropertyChanged("IsDbLoginFailed");
            }
        }

        public DbLoginViewModel()
        {
            GetSet();
            DbLoginCommand = new DelegateCommand(DbLogin);
            CloseCommand = new DelegateCommand(Close);
        }

        private void GetSet()
        {
            ServerName = Person.ServerName;
            DbName = Person.DbName;
            DbLoginName = Person.DbLoginName;
            DbLoginPsd = Person.DbLoginPsd;
        }

        private static void Close(object obj)
        {
            Application.Current.Shutdown();
        }

        private void SaveXmlDoc()
        {
            string node = "Setting/ServerLogin";
            var selectSingleNode = Person.XmlDoc.SelectSingleNode(node);
            if (selectSingleNode != null)
            {
                XmlNodeList nodeList = selectSingleNode.ChildNodes;
                if (nodeList[0] != null)
                {
                    nodeList[0].InnerText = ServerName;
                }
                if (nodeList[1] != null)
                {
                    nodeList[1].InnerText = DbName;
                }
                if (nodeList[2] != null)
                {
                    nodeList[2].InnerText = DbLoginName;
                }
                if (nodeList[3] != null)
                {
                    nodeList[3].InnerText = DbLoginPsd;
                }
            }
            Person.XmlDoc.Save(Person.XmlPath);
        }

        private void DbLogin(object obj)
        {
            SqlHelper = new DbHelperSql(ServerName, DbName, DbLoginName, DbLoginPsd);
            SaveXmlDoc();
            if (!SqlHelper.IsConnect) return;
            IsDbLoginFailed = false;
            BasicInfoView view = new BasicInfoView();
            view.ShowDialog();
        }
    }
}