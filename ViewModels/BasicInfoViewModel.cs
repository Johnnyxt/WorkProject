﻿using JW8307A.Commands;
using JW8307A.Views;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using Application = System.Windows.Application;

namespace JW8307A.ViewModels
{
    internal class BasicInfoViewModel : NotificationObject
    {
        //基本信息

        private string opName; //操作员姓名
        private string opWorkId;//操作员工号
        private string opTeam;//操作员班组
        private string department;//主制部门
        private string operationSequence;//测试工序
        private string siteCode;//测试工位
        private string description;//厂家标识
        private string product;//产品类别
        private string productLine;//产品线
        private string ateName;//测试设备编号
        private string ateVersion;//测试设备版本
        private string testSoftName;//测试软件名称
        public ICommand ConfirmCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SetExcelDataCommand { get; set; }

        private bool isLogin = true;
        private bool isBasicInfoViewEnable;

        public string OpName
        {
            get { return opName; }
            set
            {
                opName = value;
                this.RaisePropertyChanged("OpName");
            }
        }

        public string OpWorkId
        {
            get { return opWorkId; }
            set
            {
                opWorkId = value;
                this.RaisePropertyChanged("OpWorkId");
            }
        }

        public string OpTeam
        {
            get { return opTeam; }
            set
            {
                opTeam = value;
                this.RaisePropertyChanged("OpTeam");
            }
        }

        public string Department
        {
            get { return department; }
            set
            {
                department = value;
                this.RaisePropertyChanged("Department");
            }
        }

        public string OperationSequence
        {
            get { return operationSequence; }
            set
            {
                operationSequence = value;
                this.RaisePropertyChanged("operationSequence");
            }
        }

        public string SiteCode
        {
            get { return siteCode; }
            set
            {
                siteCode = value;
                this.RaisePropertyChanged("siteCode");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                this.RaisePropertyChanged("description");
            }
        }

        public string Product
        {
            get { return product; }
            set
            {
                product = value;
                this.RaisePropertyChanged("product");
            }
        }

        public string ProductLine
        {
            get { return productLine; }
            set
            {
                productLine = value;
                this.RaisePropertyChanged("productLine");
            }
        }

        public string AteName
        {
            get { return ateName; }
            set
            {
                ateName = value;
                this.RaisePropertyChanged("ateName");
            }
        }

        public string AteVersion
        {
            get { return ateVersion; }
            set
            {
                ateVersion = value;
                this.RaisePropertyChanged("ateVersion");
            }
        }

        public string TestSoftName
        {
            get { return testSoftName; }
            set
            {
                testSoftName = value;
                this.RaisePropertyChanged("TestSoftName");
            }
        }

        public bool IsLogin
        {
            get { return isLogin; }
            set
            {
                isLogin = value;
                RaisePropertyChanged("IsLogin");
            }
        }

        public bool IsBasicInfoViewEnable
        {
            get { return isBasicInfoViewEnable; }
            set
            {
                isBasicInfoViewEnable = value;
                RaisePropertyChanged("IsBasicInfoViewEnable");
            }
        }

        public BasicInfoViewModel()
        {
            IsBasicInfoViewEnable = true;
            GetSet();
            ConfirmCommand = new DelegateCommand(Confirm);
            CloseCommand = new DelegateCommand(Close);
        }

        private static void Close(object obj)
        {
            Application.Current.Shutdown();
        }

        private void GetSet()
        {
            opName = Person.OpName;
            Description = Person.Description;
            OpWorkId = Person.OpWorkId;
            Product = Person.Product;
            OpTeam = Person.OpTeam;
            ProductLine = Person.ProductLine;
            Department = Person.Department;
            AteName = Person.AteName;
            OperationSequence = Person.OperationSequence;
            AteVersion = Person.AteVersion;
            SiteCode = Person.SiteCode;
            TestSoftName = Person.TestSoftName;
        }

        private void SaveXmlDoc()
        {
            var selectSingleNode = Person.XmlDoc.SelectSingleNode("Setting");
            if (selectSingleNode == null) return;
            var nodelist = selectSingleNode.ChildNodes;
            foreach (XmlNode xn in nodelist)
            {
                if (xn.Name == "BasicInfo")
                {
                    var node = xn.ChildNodes;
                    var xmlNode = node.Item(0);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = opName;
                    }
                    xmlNode = node.Item(1);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = Description;
                    }
                    xmlNode = node.Item(2);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = OpWorkId;
                    }
                    xmlNode = node.Item(3);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = Product;
                    }
                    xmlNode = node.Item(4);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = OpTeam;
                    }
                    xmlNode = node.Item(5);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = ProductLine;
                    }
                    xmlNode = node.Item(6);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = Department;
                    }
                    xmlNode = node.Item(7);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = AteName;
                    }
                    xmlNode = node.Item(8);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = OperationSequence;
                    }
                    xmlNode = node.Item(9);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = AteVersion;
                    }
                    xmlNode = node.Item(10);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = SiteCode;
                    }
                    xmlNode = node.Item(11);
                    if (xmlNode != null)
                    {
                        xmlNode.InnerText = TestSoftName;
                    }
                }
            }
            Person.XmlDoc.Save(Person.XmlPath);
        }

        private void Confirm(object obj)
        {
            SaveXmlDoc();
            IsLogin = false;
            //跳转到测试界面
            TestView view = new TestView();
            view.ShowDialog();
        }
    }
}