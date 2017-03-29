using JW8307A.Commands;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace JW8307A.ViewModels
{
    internal class ModifyLoginViewModel : NotificationObject
    {
        private string loginName;
        private string loginPsd;
        private string modifyLoginName;
        private string modifyLoginPsd;

        public ICommand ConfirmCommand { get; set; }

        private bool isConfirm = true;

        public ModifyLoginViewModel()
        {
            ConfirmCommand = new DelegateCommand(Confirm);
        }

        private void SaveXml()
        {
            string xPath = "Setting/MainLogin/LoginName";
            XmlNode node = Person.XmlDoc.SelectSingleNode(xPath);
            if (node != null)
            {
                node.InnerText = modifyLoginName;
            }
            xPath = "Setting/MainLogin/LoginPsd";
            node = Person.XmlDoc.SelectSingleNode(xPath);
            if (node != null)
            {
                node.InnerText = modifyLoginPsd;
            }
            Person.XmlDoc.Save(Person.XmlPath);
            IsConfirm = false;
        }

        private void Confirm(object obj)
        {
            if (string.IsNullOrEmpty(LoginName))
            {
                MessageBox.Show("请输入原登录名");
                return;
            }
            if (string.IsNullOrEmpty(LoginPsd))
            {
                MessageBox.Show("请输入原登录密码");
                return;
            }
            if (LoginName == Person.LoginName && LoginPsd == Person.LoginPsd)
            {
                if (string.IsNullOrEmpty(modifyLoginName))
                {
                    MessageBox.Show("请输入修改登录名");
                    return;
                }
                if (string.IsNullOrEmpty(modifyLoginPsd))
                {
                    MessageBox.Show("请输入修改密码");
                    return;
                }
                SaveXml();
            }
            else
            {
                MessageBox.Show("登录名或密码错误");
            }
        }

        public string LoginName
        {
            get { return loginName; }
            set
            {
                loginName = value;
                RaisePropertyChanged("LoginName");
            }
        }

        public string LoginPsd
        {
            get { return loginPsd; }
            set
            {
                loginPsd = value;
                RaisePropertyChanged("LoginPsd");
            }
        }

        public string ModifyLoginName
        {
            get { return modifyLoginName; }
            set
            {
                modifyLoginName = value;
                RaisePropertyChanged("ModifyLoginName");
            }
        }

        public string ModifyLoginPsd
        {
            get { return modifyLoginPsd; }
            set
            {
                modifyLoginPsd = value;
                RaisePropertyChanged("ModifyLoginPsd");
            }
        }

        public bool IsConfirm
        {
            get { return isConfirm; }
            set
            {
                isConfirm = value;
                RaisePropertyChanged("IsConfirm");
            }
        }
    }
}