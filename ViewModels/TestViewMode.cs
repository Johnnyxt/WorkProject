using JW8307A.Commands;
using JW8307A.Models;
using JW8307A.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using DataTable = System.Data.DataTable;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace JW8307A.ViewModels
{
    /// <summary>
    /// Test视图模式
    /// </summary>
    internal class TestViewMode : NotificationObject
    {
        private ObservableCollection<TestItemModel> testItemList;
        private ObservableCollection<TestDataDetailModel> testDataDetail;
        private ObservableCollection<ThreSetModel> threSet;
        private ObservableCollection<string> obsHeader;
        private List<string> lstProCoreOrder = new List<string>();
        private readonly List<string> lstReTestProCoreOrder = new List<string>();
        private readonly DataStore dataStore = new DataStore();
        private TestDataDetailModel testDataDetailModel = new TestDataDetailModel();
        private readonly DataTable dataTable = new DataTable();
        private XmlDocument xmlSaveFile = new XmlDocument();
        private XmlElement root;
        private readonly string startupPath = AppDomain.CurrentDomain.BaseDirectory;
        private string sn;
        private const string IniFilePath = @".//UserSet";
        private string itemCode; //产品编码
        private string itemName; //产品描述
        private string workOrder;//产品任务令
        private int proCore; //产品芯数
        private bool isSingleHead; //单端头
        private bool isDoubleHead; //双端头
        private string pinAconnType; //A端接头类型
        private string pinBconnType; //B端接头类型
        private bool isCoresTest; //同芯测试
        private bool isPinsTest; //同端测试
        private string connSerial1; //将要测试接头
        private string connType1; //将要测试接头类型
        private string connSerial2; //正在测试接头
        private string connType2; //正在测试接头类型
        private int RemainProCore;  //剩余待测芯数
        private string result;
        private Brush testResColor;
        private bool isUserSetEnable;
        private string savePath;
        private string curModel;
        private string curWave;
        private string curDisplayData1;
        private string curDisplayData2;
        private string saveExcelPath;
        private string saveExcelName;
        private string saveXmlPath;
        private string il1Value;
        private string il2Value;
        private string rl1Value;
        private string rl2Value;
        private bool isIl1Pass;
        private bool isRl1Pass;
        private bool isIl2Pass;
        private bool isRl2Pass;
        private bool isTested;
        private bool isWorkState;
        private bool isTestComplete;
        private bool isCommonTest;
        private bool isQuickTest;
        private bool isReTestCheck;
        private bool isReTestEnable;
        private bool isSelHeaderEnable;
        private bool isTestCompleteEnable;
        private int testListRow;
        private string subSn;
        private string title;
        private string oldProductCode;
        private string selHeader;
        private int selHeaderIndex;
        private readonly string[] TestName = { "R101:1310_IL_", "R101:1310_RL_", "R101:1550_IL_", "R101:1550_RL_" };

        public ICommand SwitchWaveCommand { get; set; }
        public ICommand SignCommand { get; set; }
        public ICommand SwitchModeCommand { get; set; }
        public ICommand ShutDownCommand { get; set; }
        public ICommand StartTestCommand { get; set; }
        public ICommand EnableTestCommand { get; set; }
        public ICommand TestCompleteCommand { get; set; }
        public ICommand ThreCellChangedCommand { get; set; }
        public ICommand ReadUserSetConfigCommand { get; set; }
        public ICommand SaveUserSetConfigCommand { get; set; }
        public ICommand StartNextTestCommand { get; set; }
        public ICommand SetDataSavePathCommand { get; set; }
        public ICommand ModifyLoginCommand { get; set; }
        public ICommand SetExcelDataCommand { get; set; }
        public ICommand SetXmlDataCommand { get; set; }
        public ICommand CompleteInputSnCommand { get; set; }
        public ICommand StartReTestCommand { get; set; }

        private static string GetMode(int num)
        {
            var lstMode = new List<string> { "OPM", "IL", "RL", "IL2", "IL&RL", "ILRL2" };

            if (!Person.IsAddIl2Mode)
            {
                lstMode.Remove("IL2");
            }

            return lstMode[num];
        }

        private static string GetUnit(int num)
        {
            var lstUnit = new List<string> { "dBm", "dB", "Watt" };
            return lstUnit[num];
        }

        private void InitXml()
        {
            XmlNode declare = xmlSaveFile.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlSaveFile.AppendChild(declare);
            root = xmlSaveFile.CreateElement("TestResults");
            XmlAttribute xsispace = xmlSaveFile.CreateAttribute("xsi", "schemaLocation", "http://www.ieee.org/ATML/2007/TestResults");
            xsispace.Value = "http://www.ieee.org/ATML/2007/TestResults TestResults.xsd";
            root.Attributes.Append(xsispace);
            root.SetAttribute("xmlns", "http://www.ieee.org/ATML/2007/TestResults");
            root.SetAttribute("xmlns:n2", "http://www.altova.com/samplexml/other-namespace");
            root.SetAttribute("xmlns:c", "http://www.ieee.org/ATML/2007/02/Common");
            root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlSaveFile.AppendChild(root);
        }

        public TestViewMode()
        {
            Title = Person.Vision;
            WorkOrder = Person.WorkOrder;
            IsTestComplete = false;
            SaveExcelPath = Person.SaveExcelPath;
            SaveExcelName = Person.SaveExcelName;
            InitXml();
            IsUserSetEnable = Person.IsUserSetViewEnable;
            TestDataDetail = new ObservableCollection<TestDataDetailModel>();
            TestItemList = new ObservableCollection<TestItemModel>();
            ThreSet = new ObservableCollection<ThreSetModel>();
            ObsHeader = new ObservableCollection<string>();
            TestItemModel testItemModel = new TestItemModel();
            testItemList.Add(testItemModel);
            ResetResult();
            SerialPortHelper.ReadValueReceived += SpHelper_ReadValueReceived;
            InitDelegateCmd();
            InitDataTable();
            InitDataStore();
        }

        private void InitDataStore()
        {
            dataStore.ItemTestname = new List<string>();
            dataStore.ItemTestOutcome = new List<string>();
            dataStore.ItemTestStartDateTime = new List<string>();
            dataStore.ItemTestEndDateTime = new List<string>();
            dataStore.SubItemTestname = new List<string>();
            dataStore.SubItemTestOutcome = new List<string>();
            dataStore.SubItemTestDescription = new List<string>();
            dataStore.SubItemTestcValue = new List<string>();
        }

        private void InitDelegateCmd()
        {
            SwitchWaveCommand = new DelegateCommand(SwitchWave);
            SignCommand = new DelegateCommand(Sign);
            SwitchModeCommand = new DelegateCommand(SwitchMode);
            StartTestCommand = new DelegateCommand(StartTest);
            TestCompleteCommand = new DelegateCommand(TestComplete);
            CompleteInputSnCommand = new DelegateCommand(CompleteInputSn);
            ReadUserSetConfigCommand = new DelegateCommand(ReadUserSetConfig);
            SaveUserSetConfigCommand = new DelegateCommand(SaveUserSetConfig);
            StartNextTestCommand = new DelegateCommand(StartNextTest);
            ModifyLoginCommand = new DelegateCommand(ModifyLogin);
            SetXmlDataCommand = new DelegateCommand(SetXmlData);
            SetExcelDataCommand = new DelegateCommand(SetExcelData);
            StartReTestCommand = new DelegateCommand(StartReTest);
        }

        private void StartReTest(object obj)
        {
            if (IsReTestCheck)
            {
                ConnSerial1 = "NA";
                ConnSerial2 = "NA";
                IsSelHeaderEnable = true;
                IsTestCompleteEnable = true;
            }
            else
            {
                if (IsQuickTest)
                {
                    IsTestCompleteEnable = false;
                }
                IsSelHeaderEnable = false;
                testListRow = 0;
                GetProCoreOrder();
                if (IsSingleHead)
                {
                    if (ProCore == 1)
                    {
                        ConnSerial1 = "NA";
                        ConnType1 = "NA";
                    }
                    else
                    {
                        ConnSerial1 = "2A";
                        ConnType1 = PinAconnType;
                    }
                    ConnSerial2 = "1A";
                    ConnType2 = PinAconnType;
                }
                else
                {
                    ConnSerial1 = "1B";
                    ConnType1 = PinBconnType;
                    ConnSerial2 = "1A";
                    ConnType2 = PinAconnType;
                }
            }
        }

        private void SetExcelData(object obj)
        {
            var fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
            {
                SaveExcelPath = fbd.SelectedPath;
            }
        }

        private void SetXmlData(object obj)
        {
            var fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
            {
                SaveXmlPath = fbd.SelectedPath;
            }
        }

        private void ResetResult()
        {
            TestItemList[0].Wave1Il = string.Empty;
            TestItemList[0].Wave1Rl = string.Empty;
            TestItemList[0].Wave2Il = string.Empty;
            TestItemList[0].Wave2Rl = string.Empty;
            Result = string.Empty;
            TestResColor = Brushes.White;
        }

        private static void ModifyLogin(object obj)
        {
            ModifyLoginView modifyLoginView = new ModifyLoginView();
            modifyLoginView.ShowDialog();
        }

        private void ReTest()
        {
            testListRow = SelHeaderIndex;
            GetDataToDataTable();
            testDataDetailModel.Connector = SelHeader;
            UpDataToDataTable();
            ResetResult();
        }

        private string GetTestResult()
        {
            TestItemList[0].Wave1Il = il1Value;
            TestItemList[0].Wave1Rl = rl1Value;
            TestItemList[0].Wave2Il = il2Value;
            TestItemList[0].Wave2Rl = rl2Value;

            isIl1Pass = GetResult(Convert.ToSingle(TestItemList[0].Wave1Il), ThreSet[0].IlLower, ThreSet[0].IlUpper);
            isIl2Pass = GetResult(Convert.ToSingle(TestItemList[0].Wave2Il), ThreSet[1].IlLower, ThreSet[1].IlUpper);
            isRl1Pass = GetResult(Convert.ToSingle(TestItemList[0].Wave1Rl), ThreSet[0].RlLower, ThreSet[0].RlUpper);
            isRl2Pass = GetResult(Convert.ToSingle(TestItemList[0].Wave2Rl), ThreSet[1].RlLower, ThreSet[1].RlUpper);
            if (isIl1Pass && isIl2Pass && isRl1Pass && isRl2Pass)
            {
                TestResColor = Brushes.Green;
                return "Passed";
            }
            TestResColor = Brushes.Red;
            return "Failed";
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        private void UpDataToDataTable()
        {
            TestDataDetail.RemoveAt(testListRow);
            TestDataDetail.Insert(testListRow, testDataDetailModel);
            int col = 4 * testListRow;
            Result = GetTestResult();
            dataStore.ItemTestEndDateTime[testListRow] = DateTime.Now.ToString("s");
            dataStore.SubItemTestOutcome[col] = isIl1Pass ? "Passed" : "Failed";
            dataStore.SubItemTestOutcome[col + 1] = isRl1Pass ? "Passed" : "Failed";
            dataStore.SubItemTestOutcome[col + 2] = isIl2Pass ? "Passed" : "Failed";
            dataStore.SubItemTestOutcome[col + 3] = isRl2Pass ? "Passed" : "Failed";
            dataStore.SubItemTestDescription[col] = testDataDetailModel.Il1;
            dataStore.SubItemTestDescription[col + 1] = testDataDetailModel.Rl1;
            dataStore.SubItemTestDescription[col + 2] = testDataDetailModel.Il2;
            dataStore.SubItemTestDescription[col + 3] = testDataDetailModel.Rl2;
            dataStore.SubItemTestcValue[col] = string.Concat(ThreSet[0].IlLower, ":", ThreSet[0].IlUpper);
            dataStore.SubItemTestcValue[col + 1] = string.Concat(ThreSet[0].RlLower, ":", ThreSet[0].RlUpper);
            dataStore.SubItemTestcValue[col + 2] = string.Concat(ThreSet[1].IlLower, ":", ThreSet[1].IlUpper);
            dataStore.SubItemTestcValue[col + 3] = string.Concat(ThreSet[1].RlLower, ":", ThreSet[1].RlUpper);
            string strConnType = ConnSerial2.Contains('A') ? PinAconnType : PinBconnType;
            for (int i = 0; i < 4; i++)
            {
                dataStore.SubItemTestname[col + i] = string.Concat(TestName[i], strConnType);
            }
            dataStore.ItemTestOutcome[testListRow] = Result;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        private void InsertDataToDataTable()
        {
            TestDataDetail.Add(testDataDetailModel);
            dataStore.ItemTestStartDateTime.Add(DateTime.Now.ToString("s"));
            if (ProCore == 1 && IsSingleHead)
            {
                dataStore.ItemTestEndDateTime.Add(DateTime.Now.AddSeconds(1).ToString("s"));
            }
            else
            {
                dataStore.ItemTestEndDateTime.Add(DateTime.Now.ToString("s"));
            }
            dataStore.SubItemTestOutcome.Add(isIl1Pass ? "Passed" : "Failed");
            dataStore.SubItemTestOutcome.Add(isRl1Pass ? "Passed" : "Failed");
            dataStore.SubItemTestOutcome.Add(isIl2Pass ? "Passed" : "Failed");
            dataStore.SubItemTestOutcome.Add(isRl2Pass ? "Passed" : "Failed");
            dataStore.SubItemTestDescription.Add(testDataDetailModel.Il1);
            dataStore.SubItemTestDescription.Add(testDataDetailModel.Rl1);
            dataStore.SubItemTestDescription.Add(testDataDetailModel.Il2);
            dataStore.SubItemTestDescription.Add(testDataDetailModel.Rl2);
            dataStore.SubItemTestcValue.Add(string.Concat(ThreSet[0].IlLower, ":", ThreSet[0].IlUpper));
            dataStore.SubItemTestcValue.Add(string.Concat(ThreSet[0].RlLower, ":", ThreSet[0].RlUpper));
            dataStore.SubItemTestcValue.Add(string.Concat(ThreSet[1].IlLower, ":", ThreSet[1].IlUpper));
            dataStore.SubItemTestcValue.Add(string.Concat(ThreSet[1].RlLower, ":", ThreSet[1].RlUpper));
            string strConnType = ConnSerial2.Contains('A') ? PinAconnType : PinBconnType;

            for (int i = 0; i < 4; i++)
            {
                dataStore.SubItemTestname.Add(string.Concat(TestName[i], strConnType));
            }
            dataStore.ItemTestOutcome.Add(Result);
        }

        private void NewTest()
        {
            GetDataToDataTable();
            if (isTested)
            {
                UpDataToDataTable();
            }
            else
            {
                InsertDataToDataTable();
            }
            if (RemainProCore > 0)
            {
                RemainProCore--;
            }

            if (RemainProCore == 0)
            {
                MessageBox.Show("测试完毕");
                ResetResult();
                return;
            }
            if (IsSingleHead)
            {
                ConnSerial2 = lstProCoreOrder[ProCore - RemainProCore];
                ConnSerial1 = lstProCoreOrder[ProCore - RemainProCore + 1];
            }
            else
            {
                ConnSerial2 = lstProCoreOrder[2 * ProCore - RemainProCore];
                ConnSerial1 = lstProCoreOrder[2 * ProCore - RemainProCore + 1];
            }
            testListRow++;
            if (!string.IsNullOrEmpty(ConnSerial1))
            {
                ConnType1 = ConnSerial1.Contains("A") ? PinAconnType : PinBconnType;
            }
            if (!string.IsNullOrEmpty(ConnSerial2))
            {
                ConnType2 = ConnSerial2.Contains("A") ? PinAconnType : PinBconnType;
            }
            ResetResult();
        }

        private void CommonTest()
        {
            if (string.IsNullOrEmpty(TestItemList[0].Wave1Il))
            {
                MessageBox.Show("测试数据不为空!");
                return;
            }
            if (RemainProCore == 0)
            {
                return;
            }

            if (isTested)
            {
                if (IsReTestCheck)
                {
                    ReTest();
                }
                else
                {
                    NewTest();
                }
            }
            else
            {
                dataStore.ItemTestname.Add(ConnSerial2);
                NewTest();
            }
        }

        private void StartNextTest(object obj)
        {
            CommonTest();
        }

        private void QuickTest()
        {
            if (string.IsNullOrEmpty(TestItemList[0].Wave1Il))
            {
                MessageBox.Show("测试数据不为空!");
                return;
            }

            if (RemainProCore == 0)
            {
                return;
            }

            if (isTested)
            {
                if (IsReTestCheck)
                {
                    ReQuickTest();
                }
                else
                {
                    NewQuickTest();
                }
            }
            else
            {
                dataStore.ItemTestname.Add(ConnSerial2);
                NewQuickTest();
            }
        }

        private void GetDataToDataTable()
        {
            testDataDetailModel = new TestDataDetailModel
            {
                Connector = connSerial2,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Time = DateTime.Now.ToString("HH:mm:ss"),
                SerialNumber = Sn,
                Il1 = TestItemList[0].Wave1Il,
                Rl1 = TestItemList[0].Wave1Rl,
                Il2 = TestItemList[0].Wave2Il,
                Rl2 = TestItemList[0].Wave2Rl,
                Oper = Person.OpName,
                WorkId = Person.OpWorkId,
                Result = Result
            };
        }

        private void NewQuickTest()
        {
            GetDataToDataTable();
            if (isTested)
            {
                UpDataToDataTable();
            }
            else
            {
                InsertDataToDataTable();
            }
            if (RemainProCore > 0)
            {
                RemainProCore--;
            }

            if (RemainProCore == 0)
            {
                //dataStore.Outcome = dataStore.ItemTestOutcome.Contains("Failed") ? "Failed" : "Passed";
                UploadData();
                return;
            }
            if (IsSingleHead)
            {
                ConnSerial2 = lstProCoreOrder[ProCore - RemainProCore];
                ConnSerial1 = lstProCoreOrder[ProCore - RemainProCore + 1];
            }
            else
            {
                ConnSerial2 = lstProCoreOrder[2 * ProCore - RemainProCore];
                ConnSerial1 = lstProCoreOrder[2 * ProCore - RemainProCore + 1];
            }
            testListRow++;
            if (!string.IsNullOrEmpty(ConnSerial1))
            {
                ConnType1 = ConnSerial1.Contains("A") ? PinAconnType : PinBconnType;
            }
            if (!string.IsNullOrEmpty(ConnSerial2))
            {
                ConnType2 = ConnSerial2.Contains("A") ? PinAconnType : PinBconnType;
            }
        }

        private void ReQuickTest()
        {
            testListRow = SelHeaderIndex;
            GetDataToDataTable();
            testDataDetailModel.Connector = SelHeader;
            UpDataToDataTable();
        }

        private void ReadUserSetConfig(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(startupPath, "UserSet"),
                Filter = "IniFiles(*.ini)|*.ini"
            };

            var res = dlg.ShowDialog();
            if (res == true)
            {
                var selFile = dlg.FileName;
                LoadUserSetConfig(selFile);
            }
        }

        private void SaveUserSetConfig(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = @"|*.ini",
                InitialDirectory = Path.Combine(startupPath, "UserSet"),
                FileName = Path.GetFileNameWithoutExtension(GetIniPath()),
                CreatePrompt = true,
                OverwritePrompt = true,
                AddExtension = true
            };
            var res = saveFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                var saveFileName = saveFileDialog.FileName;
                string path = Path.Combine(startupPath, "UserSet", saveFileName);
                IniFileHelper.IniWriteValue(path, "UserSet", "itemCode", ItemCode);
                IniFileHelper.IniWriteValue(path, "UserSet", "itemName", ItemName);
                //  IniFileHelper.IniWriteValue(path, "UserSet", "workOrder", WorkOrder);
                IniFileHelper.IniWriteValue(path, "UserSet", "ProCore", ProCore.ToString());
                IniFileHelper.IniWriteValue(path, "UserSet", "SingleHead", IsSingleHead.ToString());
                IniFileHelper.IniWriteValue(path, "UserSet", "DoubleHead", IsDoubleHead.ToString());
                IniFileHelper.IniWriteValue(path, "UserSet", "PinAconnType", PinAconnType);
                IniFileHelper.IniWriteValue(path, "UserSet", "PinBconnType", PinBconnType);
                IniFileHelper.IniWriteValue(path, "UserSet", "CoresTest", IsCoresTest.ToString());
                IniFileHelper.IniWriteValue(path, "UserSet", "PinsTest", IsPinsTest.ToString());
                IniFileHelper.IniWriteValue(path, "UserSet", "CommonTest", IsCommonTest.ToString());
                IniFileHelper.IniWriteValue(path, "UserSet", "QuickTest", IsQuickTest.ToString());
                IniFileHelper.IniWriteValue(path, "UserSet", "XmlPath", SaveXmlPath);
                IniFileHelper.IniWriteValue(path, "UserSet", "ExcelPath", SaveExcelPath);
                for (int i = 0; i < Person.TestWave.Length; i++)
                {
                    List<string> lst = new List<string>
                    {
                        threSet[i].IsWaveEnable.ToString(),
                        threSet[i].IlLower.ToString("F2"),
                        threSet[i].IlUpper.ToString("F2"),
                        threSet[i].RlLower.ToString("F2"),
                        threSet[i].RlUpper.ToString("F2")
                    };
                    var temp = string.Join(",", lst.ToArray());
                    IniFileHelper.IniWriteValue(path, "Threshold", Person.TestWave[i], temp);
                }

                lstProCoreOrder = GetProCoreOrder();
            }
        }

        private List<string> GetProCoreOrder()
        {
            List<string> lst = new List<string>();
            if (IsSingleHead)
            {
                RemainProCore = ProCore;
                for (int i = 1; i <= ProCore; i++)
                {
                    string temp = string.Concat(i, "A");
                    lst.Add(temp);
                }
                lst.Add("NA");
            }
            else
            {
                RemainProCore = 2 * ProCore;
                if (IsCoresTest)
                {
                    for (int i = 1; i <= ProCore; i++)
                    {
                        string temp = string.Concat(i, "A");
                        lst.Add(temp);
                        temp = string.Concat(i, "B");
                        lst.Add(temp);
                    }
                }
                else
                {
                    for (int i = 1; i <= ProCore; i++)
                    {
                        string temp = string.Concat(i, "A");
                        lst.Add(temp);
                    }
                    for (int i = 1; i <= ProCore; i++)
                    {
                        string temp = string.Concat(i, "B");
                        lst.Add(temp);
                    }
                }
                lst.Add("NA");
            }
            return lst;
        }

        private void LoadUserSetConfig(string filePath)
        {
            ItemCode = IniFileHelper.IniGetStringValue(filePath, "UserSet", "itemCode", null);
            ItemName = IniFileHelper.IniGetStringValue(filePath, "UserSet", "itemName", null);
            ProCore = Convert.ToInt32(IniFileHelper.IniGetStringValue(filePath, "UserSet", "ProCore", null));
            IsSingleHead = Convert.ToBoolean(IniFileHelper.IniGetStringValue(filePath, "UserSet", "SingleHead", null));
            IsDoubleHead = Convert.ToBoolean(IniFileHelper.IniGetStringValue(filePath, "UserSet", "DoubleHead", null));
            PinAconnType = IniFileHelper.IniGetStringValue(filePath, "UserSet", "PinAconnType", null);
            PinBconnType = IniFileHelper.IniGetStringValue(filePath, "UserSet", "PinBconnType", null);
            IsCoresTest = Convert.ToBoolean(IniFileHelper.IniGetStringValue(filePath, "UserSet", "CoresTest", null));
            IsPinsTest = Convert.ToBoolean(IniFileHelper.IniGetStringValue(filePath, "UserSet", "PinsTest", null));
            IsCommonTest = Convert.ToBoolean(IniFileHelper.IniGetStringValue(filePath, "UserSet", "CommonTest", null));
            IsTestCompleteEnable = IsCommonTest;
            IsQuickTest = Convert.ToBoolean(IniFileHelper.IniGetStringValue(filePath, "UserSet", "QuickTest", null));
            SaveXmlPath = IniFileHelper.IniGetStringValue(filePath, "UserSet", "XmlPath", null);
            string[] threkey = IniFileHelper.IniGetAllItemKeys(filePath, "Threshold");
            threSet.Clear();
            Person.IlLowerThre.Clear();
            Person.IlUpperThre.Clear();
            Person.RlLowerThre.Clear();
            Person.RlUpperThre.Clear();
            foreach (string str in threkey)
            {
                ThreSetModel t = new ThreSetModel();
                var value = IniFileHelper.IniGetStringValue(filePath, "Threshold", str, null);
                var arrayValue = value.Split(',');
                t.Wave = str;
                t.IlLower = Convert.ToSingle(arrayValue[1]);
                t.IlUpper = Convert.ToSingle(arrayValue[2]);
                t.RlLower = Convert.ToSingle(arrayValue[3]);
                t.RlUpper = Convert.ToSingle(arrayValue[4]);
                Person.IlLowerThre.Add(t.IlLower);
                Person.IlUpperThre.Add(t.IlUpper);
                Person.RlLowerThre.Add(t.RlLower);
                Person.RlUpperThre.Add(t.RlUpper);
                threSet.Add(t);
            }
            lstProCoreOrder = GetProCoreOrder();
            if (IsSingleHead)
            {
                if (ProCore == 1)
                {
                    ConnSerial1 = "NA";
                    ConnType1 = "NA";
                }
                else
                {
                    ConnSerial1 = "2A";
                    ConnType1 = PinAconnType;
                }
                ConnSerial2 = "1A";
                ConnType2 = PinAconnType;
            }
            else
            {
                ConnSerial1 = "1B";
                ConnType1 = PinBconnType;
                ConnSerial2 = "1A";
                ConnType2 = PinAconnType;
            }
        }

        private void CompleteInputSn(object obj)
        {
            InitDataStore();
            xmlSaveFile = new XmlDocument();
            InitXml();
            testListRow = 0;
            RemainProCore = 0;
            dataTable.Clear();
            string path = GetIniPath();
            if (path != null)
            {
                ResetResult();
                LoadUserSetConfig(path);
            }
            else
            {
                return;
            }
            testDataDetail.Clear();
            CheckServer();
            isWorkState = true;
        }

        private bool IsSnInput()
        {
            Sn = Sn.Trim();
            if (!string.IsNullOrEmpty(Sn))
            {
                if (Sn.Length == 28)
                {
                    string s1 = Sn.Substring(10, 1);
                    string s2 = Sn.Substring(21, 1);
                    if (s1.Equals("/") && s2.Equals("S"))
                    {
                        return true;
                    }
                }
            }
            MessageBox.Show("SN错误，请输入正确的SN");
            return false;
        }

        private string GetIniPath()
        {
            if (IsSnInput())
            {
                string newProductCode = Sn.Substring(2, 8);
                string fileName = string.Format("{0}.ini", newProductCode);
                if (!string.IsNullOrEmpty(oldProductCode))
                {
                    if (!oldProductCode.Equals(newProductCode))
                    {
                        MessageBox.Show("产品编码不同!");
                    }
                }
                oldProductCode = newProductCode;
                string iniPath = Path.Combine(IniFilePath, fileName);
                if (File.Exists(iniPath))
                {
                    return iniPath;
                }
                MessageBox.Show("无该产品对应的编码配置信息");
                return null;
            }
            return null;
        }

        private void TestComplete(object obj)
        {
            UploadData();
        }

        private void WriteData()
        {
            WriteToServer();
            WriteToXml();
            WriteToExcel();
            TestDataDetail.Clear();
            ConnSerial1 = string.Empty;
            ConnSerial2 = string.Empty;
            ConnType1 = string.Empty;
            ConnType2 = string.Empty;
            Sn = string.Empty;
            SubSn = string.Empty;
            IsReTestEnable = false;
            IsSelHeaderEnable = false;
            ObsHeader.Clear();
            ResetResult();
        }

        private void UploadData()
        {
            if (isTested && IsReTestCheck)
            {
                if (dataStore.ItemTestOutcome.Contains("Failed"))
                {
                    dataStore.Outcome = "Failed";
                }
                else
                {
                    dataStore.Outcome = "Passed";
                }
                string msgText = string.Format("测试完毕，{0}，是否上传到数据库", dataStore.Outcome);
                var res = MessageBox.Show(msgText, "JW8307A", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    WriteData();
                }
            }
            if ((isTested && !IsReTestCheck) || !isTested)
            {
                if (TestDataDetail.Count == lstProCoreOrder.Count - 1 && RemainProCore == 0)
                {
                    if (dataStore.ItemTestOutcome.Contains("Failed"))
                    {
                        dataStore.Outcome = "Failed";
                    }
                    else
                    {
                        dataStore.Outcome = "Passed";
                    }
                    string msgText = string.Format("测试完毕，{0}，是否上传到数据库", dataStore.Outcome);
                    var res = MessageBox.Show(msgText, "JW8307A", MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        WriteData();
                    }
                }
                else
                {
                    MessageBox.Show("未测试完成");
                }
            }
        }

        private void WriteToConfig()
        {
            string xnNode = "Setting/WorkOrder";
            var selectSingleNode = Person.XmlDoc.SelectSingleNode(xnNode);
            if (selectSingleNode != null)
                selectSingleNode.InnerText = WorkOrder;
            xnNode = "Setting/BasicInfo/SaveExcelPath";
            selectSingleNode = Person.XmlDoc.SelectSingleNode(xnNode);
            if (selectSingleNode != null)
            {
                selectSingleNode.InnerText = SaveExcelPath;
            }
            xnNode = "Setting/BasicInfo/SaveExcelName";
            selectSingleNode = Person.XmlDoc.SelectSingleNode(xnNode);
            if (selectSingleNode != null)
            {
                selectSingleNode.InnerText = saveExcelName;
            }
            Person.XmlDoc.Save(Person.XmlPath);
        }

        private void WriteToXml()
        {
            WriteToConfig();
            WriteToSave();
        }

        private void WriteToSave()
        {
            try
            {
                XmlNode resultSetNode = xmlSaveFile.CreateElement("ResultSet");
                xmlSaveFile.DocumentElement.PrependChild(resultSetNode);
                XmlAttribute attribute = xmlSaveFile.CreateAttribute("ID");
                attribute.Value = "1";
                resultSetNode.Attributes.Append(attribute);
                attribute = xmlSaveFile.CreateAttribute("startDateTime");
                attribute.Value = dataStore.StartDateTime;
                resultSetNode.Attributes.Append(attribute);
                attribute = xmlSaveFile.CreateAttribute("endDateTime");
                attribute.Value = dataStore.EndDateTime;
                resultSetNode.Attributes.Append(attribute);
                attribute = xmlSaveFile.CreateAttribute("operationSequence");
                attribute.Value = dataStore.OperationSequence.ToString();
                resultSetNode.Attributes.Append(attribute);
                attribute = xmlSaveFile.CreateAttribute("siteCode");
                attribute.Value = dataStore.SiteCode;
                resultSetNode.Attributes.Append(attribute);
                XmlNode xn = xmlSaveFile.CreateElement("Description");
                xn.InnerText = dataStore.Description;
                resultSetNode.AppendChild(xn);
                xn = xmlSaveFile.CreateElement("TestResult");
                attribute = xmlSaveFile.CreateAttribute("BoardFlag");
                attribute.Value = "1";
                xn.Attributes.Append(attribute);
                attribute = xmlSaveFile.CreateAttribute("ID");
                attribute.Value = "1";
                xn.Attributes.Append(attribute);
                resultSetNode.AppendChild(xn);
                XmlNode node1 = xmlSaveFile.CreateElement("Outcome");
                attribute = xmlSaveFile.CreateAttribute("value");
                attribute.Value = dataStore.Outcome;
                node1.Attributes.Append(attribute);
                xn.AppendChild(node1);
                int row = testDataDetail.Count;
                for (int i = 0; i < row; i++)
                {
                    XmlNode node = xmlSaveFile.CreateElement("ItemTest");
                    XmlAttribute att = xmlSaveFile.CreateAttribute("ID");
                    att.Value = (i + 1).ToString();
                    node.Attributes.Append(att);
                    att = xmlSaveFile.CreateAttribute("name");
                    att.Value = string.Concat("IL_RL Test", "(", lstProCoreOrder[i], ")");
                    node.Attributes.Append(att);
                    att = xmlSaveFile.CreateAttribute("startDateTime");
                    att.Value = dataStore.ItemTestStartDateTime[i];
                    node.Attributes.Append(att);
                    att = xmlSaveFile.CreateAttribute("endDateTime");
                    att.Value = dataStore.ItemTestEndDateTime[i];
                    node.Attributes.Append(att);
                    resultSetNode.AppendChild(node);
                    XmlNode rootNode = xmlSaveFile.CreateElement("TestResult");
                    node.AppendChild(rootNode);
                    XmlAttribute rootAttribute = xmlSaveFile.CreateAttribute("BoardFlag");
                    rootAttribute.Value = "1";
                    rootNode.Attributes.Append(rootAttribute);
                    rootAttribute = xmlSaveFile.CreateAttribute("ID");
                    rootAttribute.Value = "1";
                    rootNode.Attributes.Append(rootAttribute);
                    rootAttribute = xmlSaveFile.CreateAttribute("ValueFlag");
                    rootAttribute.Value = "0";
                    rootNode.Attributes.Append(rootAttribute);
                    rootAttribute = xmlSaveFile.CreateAttribute("SerialNumber");
                    rootAttribute.Value = "NOCODE";
                    rootNode.Attributes.Append(rootAttribute);
                    XmlNode subRootNode = xmlSaveFile.CreateElement("Outcome");
                    rootAttribute = xmlSaveFile.CreateAttribute("value");
                    rootAttribute.Value = dataStore.SubItemTestOutcome[i];
                    subRootNode.Attributes.Append(rootAttribute);
                    rootNode.AppendChild(subRootNode);
                    subRootNode = xmlSaveFile.CreateElement("Description");
                    subRootNode.InnerText = "NoDesc";
                    rootNode.AppendChild(subRootNode);
                    for (int j = 0; j < 4; j++)
                    {
                        rootNode = xmlSaveFile.CreateElement("SubItemTest");
                        node.AppendChild(rootNode);
                        rootAttribute = xmlSaveFile.CreateAttribute("ID");
                        rootAttribute.Value = (j + 1).ToString();
                        rootNode.Attributes.Append(rootAttribute);
                        rootAttribute = xmlSaveFile.CreateAttribute("name");
                        rootAttribute.Value = dataStore.SubItemTestname[4 * i + j];
                        rootNode.Attributes.Append(rootAttribute);
                        XmlNode subItemNode1 = xmlSaveFile.CreateElement("TestResult");
                        rootNode.AppendChild(subItemNode1);
                        rootAttribute = xmlSaveFile.CreateAttribute("BoardFlag");
                        rootAttribute.Value = "1";
                        subItemNode1.Attributes.Append(rootAttribute);
                        rootAttribute = xmlSaveFile.CreateAttribute("ID");
                        rootAttribute.Value = "1";
                        subItemNode1.Attributes.Append(rootAttribute);
                        rootAttribute = xmlSaveFile.CreateAttribute("SerialNumber");
                        rootAttribute.Value = "NOCODE";
                        subItemNode1.Attributes.Append(rootAttribute);
                        XmlNode subItemNode2 = xmlSaveFile.CreateElement("Outcome");
                        rootAttribute = xmlSaveFile.CreateAttribute("value");
                        rootAttribute.Value = dataStore.SubItemTestOutcome[4 * i + j];
                        subItemNode1.AppendChild(subItemNode2);
                        subItemNode2.Attributes.Append(rootAttribute);
                        subItemNode2 = xmlSaveFile.CreateElement("Description");
                        subItemNode2.InnerText = dataStore.SubItemTestDescription[4 * i + j];
                        subItemNode1.AppendChild(subItemNode2);
                        subItemNode2 = xmlSaveFile.CreateElement("TestData");
                        subItemNode1.AppendChild(subItemNode2);
                        XmlNode subItemNode3 = xmlSaveFile.CreateElement("c", "Datum", "http://www.ieee.org/ATML/2007/02/Common");
                        rootAttribute = xmlSaveFile.CreateAttribute("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance");
                        rootAttribute.Value = "c:string";
                        subItemNode3.Attributes.Append(rootAttribute);
                        subItemNode2.AppendChild(subItemNode3);
                        XmlNode subItemNode4 = xmlSaveFile.CreateElement("c", "Value", "http://www.ieee.org/ATML/2007/02/Common");
                        subItemNode4.InnerText = dataStore.SubItemTestcValue[4 * i + j];
                        subItemNode3.AppendChild(subItemNode4);
                    }
                }
                resultSetNode = xmlSaveFile.CreateElement("Personnel");
                XmlNode xNode = xmlSaveFile.CreateElement("SystemOperator");
                XmlAttribute xmlAttribute = xmlSaveFile.CreateAttribute("ID");
                xmlAttribute.Value = dataStore.SystemOperator;
                xNode.Attributes.Append(xmlAttribute);
                resultSetNode.AppendChild(xNode);
                xmlSaveFile.DocumentElement.AppendChild(resultSetNode);
                resultSetNode = xmlSaveFile.CreateElement("UUT");
                xmlAttribute = xmlSaveFile.CreateAttribute("UutType");
                xmlAttribute.Value = "hardware";
                resultSetNode.Attributes.Append(xmlAttribute);
                xNode = xmlSaveFile.CreateElement("SerialNumber");
                xNode.InnerText = dataStore.SerialNumber;
                resultSetNode.AppendChild(xNode);
                xNode = xmlSaveFile.CreateElement("ItemName");
                xNode.InnerText = dataStore.ItemName;
                resultSetNode.AppendChild(xNode);
                xNode = xmlSaveFile.CreateElement("ItemCode");
                xNode.InnerText = dataStore.ItemCode;
                resultSetNode.AppendChild(xNode);
                xmlSaveFile.DocumentElement.AppendChild(resultSetNode);
                resultSetNode = xmlSaveFile.CreateElement("TestProgram");
                xmlAttribute = xmlSaveFile.CreateAttribute("Productline");
                xmlAttribute.Value = dataStore.ProductLine;
                resultSetNode.Attributes.Append(xmlAttribute);
                xmlAttribute = xmlSaveFile.CreateAttribute("Product");
                xmlAttribute.Value = dataStore.Product;
                resultSetNode.Attributes.Append(xmlAttribute);

                xNode = xmlSaveFile.CreateElement("c", "Definition", "http://www.ieee.org/ATML/2007/02/Common");
                xmlAttribute = xmlSaveFile.CreateAttribute("name");
                xmlAttribute.Value = dataStore.CDefinitionName;
                xNode.Attributes.Append(xmlAttribute);
                xmlAttribute = xmlSaveFile.CreateAttribute("version");
                xmlAttribute.Value = dataStore.CDefinitionVersion;
                xNode.Attributes.Append(xmlAttribute);
                resultSetNode.AppendChild(xNode);
                xmlSaveFile.DocumentElement.AppendChild(resultSetNode);
                resultSetNode = xmlSaveFile.CreateElement("TestStation");
                xmlAttribute = xmlSaveFile.CreateAttribute("AteName");
                xmlAttribute.Value = dataStore.AteName;
                resultSetNode.Attributes.Append(xmlAttribute);
                xmlAttribute = xmlSaveFile.CreateAttribute("AteVersion");
                xmlAttribute.Value = dataStore.AteVersion;
                resultSetNode.Attributes.Append(xmlAttribute);
                xmlSaveFile.DocumentElement.AppendChild(resultSetNode);
                resultSetNode = xmlSaveFile.CreateElement("WorkOrder");
                xNode = xmlSaveFile.CreateElement("WorkItem");
                xmlAttribute = xmlSaveFile.CreateAttribute("name");
                xmlAttribute.Value = "-";
                xNode.Attributes.Append(xmlAttribute);
                resultSetNode.AppendChild(xNode);

                XmlNode xNode1 = xmlSaveFile.CreateElement("c", "Datum", "http://www.ieee.org/ATML/2007/02/Common");
                xmlAttribute = xmlSaveFile.CreateAttribute("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance");
                xmlAttribute.Value = "c:string";
                xNode1.Attributes.Append(xmlAttribute);
                xNode.AppendChild(xNode1);
                XmlNode xNode2 = xmlSaveFile.CreateElement("c", "Value", "http://www.ieee.org/ATML/2007/02/Common");
                xNode2.InnerText = dataStore.WorkOrder;
                xNode1.AppendChild(xNode2);
                xmlSaveFile.DocumentElement.AppendChild(resultSetNode);

                string fileName = string.Format("{0}-{1}.xml", Sn.Remove(10, 1), DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (!Directory.Exists(SaveXmlPath))
                {
                    Directory.CreateDirectory(SaveXmlPath);
                }
                string filePath = Path.Combine(SaveXmlPath, fileName);
                xmlSaveFile.Save(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static bool GetResult(float value, float lower, float upper)
        {
            return value >= lower && value <= upper;
        }

        private void CheckServer()
        {
            string cmdText = "SELECT *FROM dbo.Test Where SerialNumber='" + Sn + "'" + " And EndDateTime=(SELECT TOP 1 dbo.Test.EndDateTime From dbo.Test Where SerialNumber='"
                           + Sn + "'" + " And ItemTestname = 'IL_RL Test(1A)' ORDER BY EndDateTime DESC) ORDER BY ItemTestStartDateTime ";
            DataTable dt = DbHelperSql.SelectToDataTable(cmdText);
            int row = dt.Rows.Count;
            //存在已测数据
            if (row > 0)
            {
                lstReTestProCoreOrder.Clear();
                ObsHeader.Clear();
                lstProCoreOrder.ForEach(i => lstReTestProCoreOrder.Add(i));
                lstReTestProCoreOrder.RemoveAt(lstReTestProCoreOrder.Count - 1);
                lstReTestProCoreOrder.ForEach(p => ObsHeader.Add(p));
                SelHeader = lstReTestProCoreOrder[0];
                isTested = true;
                dataStore.StartDateTime = dt.Rows[0]["StartDateTime"].ToString();
                IsReTestCheck = false;
                if (row > 1)
                {
                    IsReTestEnable = true;
                }
                //单芯
                else
                {
                    IsReTestEnable = false;
                    ObsHeader.Clear();
                }
            }
            //不存在已测数据
            else
            {
                dataStore.StartDateTime = DateTime.Now.ToString("s");
                IsReTestCheck = false;
                IsSelHeaderEnable = false;
                ObsHeader.Clear();
                isTested = false;
                IsReTestEnable = false;
                return;
            }
            for (int i = 0; i < row; i++)
            {
                string itemTestName = dt.Rows[i]["ItemTestname"].ToString();
                dataStore.ItemTestname.Add(itemTestName);
                dataStore.ItemTestOutcome.Add(dt.Rows[i]["ItemTestOutcome"].ToString());
                dataStore.ItemTestStartDateTime.Add(dt.Rows[i]["ItemTestStartDateTime"].ToString());
                dataStore.ItemTestEndDateTime.Add(dt.Rows[i]["ItemTestEndDateTime"].ToString());
                for (int j = 0; j < 4; j++)
                {
                    string subItemTestname = dt.Rows[i][string.Concat("SubItemTestname", j + 1)].ToString();
                    string subItemTestOutcome = dt.Rows[i][string.Concat("SubItemTestOutcome", j + 1)].ToString();
                    string subItemTestDescription = dt.Rows[i][string.Concat("SubItemTestDescription", j + 1)].ToString();
                    string subItemTestcValue = dt.Rows[i][string.Concat("SubItemTestcValue", j + 1)].ToString();

                    dataStore.SubItemTestname.Add(subItemTestname);
                    dataStore.SubItemTestOutcome.Add(subItemTestOutcome);
                    dataStore.SubItemTestDescription.Add(subItemTestDescription);
                    dataStore.SubItemTestcValue.Add(subItemTestcValue);
                }

                int pos1 = itemTestName.IndexOf('(') + 1;
                int pos2 = itemTestName.IndexOf(')');
                testDataDetailModel = new TestDataDetailModel
                {
                    Connector = itemTestName.Substring(pos1, pos2 - pos1),
                    Date = dt.Rows[i]["ItemTestEndDateTime"].ToString().Split('T')[0],
                    Time = dt.Rows[i]["ItemTestEndDateTime"].ToString().Split('T')[1],
                    SerialNumber = dt.Rows[i]["SerialNumber"].ToString(),
                    Il1 = dt.Rows[i]["SubItemTestDescription1"].ToString(),
                    Rl1 = dt.Rows[i]["SubItemTestDescription2"].ToString(),
                    Il2 = dt.Rows[i]["SubItemTestDescription3"].ToString(),
                    Rl2 = dt.Rows[i]["SubItemTestDescription4"].ToString(),
                    Oper = dt.Rows[i]["opName"].ToString(),
                    WorkId = dt.Rows[i]["opNumber"].ToString(),
                    Result = dt.Rows[i]["ItemTestOutcome"].ToString()
                };
                TestDataDetail.Add(testDataDetailModel);
            }

            cmdText = "SELECT ItemTestname FROM dbo.Test Where SerialNumber='" + Sn + "'" + " And  EndDateTime=(SELECT TOP 1 dbo.Test.EndDateTime From dbo.Test Where SerialNumber='"
                          + Sn + "'" + " And ItemTestname = 'IL_RL Test(1A)' ORDER BY EndDateTime DESC) AND ItemTestOutcome='Failed' ";

            dt = DbHelperSql.SelectToDataTable(cmdText);
            //Fail行
            row = dt.Rows.Count;
            MessageBox.Show(row > 0 ? "器件测试不合格" : "器件测试合格");
        }

        private void Test()
        {
            if (curModel != "ILRL2")
            {
                MessageBox.Show("仪表模式设置错误，请设置为ILRL2模式!", "JW8307A", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            if (string.IsNullOrEmpty(Sn))
            {
                MessageBox.Show("请输入Sn", "JW8307A", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            if (string.IsNullOrEmpty(WorkOrder))
            {
                MessageBox.Show("请输入产品任务令");
                return;
            }

            if (!isWorkState)
            {
                return;
            }

            if (!IsSnInput())
            {
                isWorkState = false;
                return;
            }
            Result = GetTestResult();
            if (IsQuickTest)
            {
                QuickTest();
            }
        }

        private void InitDataTable()
        {
            dataTable.Columns.Add("StartDateTime", typeof(string));
            dataTable.Columns.Add("EndDateTime", typeof(string));
            dataTable.Columns.Add("ItemCode", typeof(string));
            dataTable.Columns.Add("WorkOrder", typeof(string));
            dataTable.Columns.Add("SerialNumber", typeof(string));
            dataTable.Columns.Add("AteName", typeof(string));
            dataTable.Columns.Add("OpName", typeof(string));
            dataTable.Columns.Add("Outcome", typeof(string));
            dataTable.Columns.Add("ItemTestname", typeof(string));
            dataTable.Columns.Add("ItemTestOutcome", typeof(string));
            dataTable.Columns.Add("ItemTestStartDateTime", typeof(string));
            dataTable.Columns.Add("ItemTestEndDateTime", typeof(string));
            for (int i = 1; i <= 12; i++)
            {
                dataTable.Columns.Add(string.Concat("SubItemTestname", i), typeof(string));
                dataTable.Columns.Add(string.Concat("SubItemTestOutcome", i), typeof(string));
                dataTable.Columns.Add(string.Concat("SubItemTestDescription", i), typeof(string));
                dataTable.Columns.Add(string.Concat("SubItemTestcValue", i), typeof(string));
            }
            dataTable.Columns.Add("ItemName", typeof(string));
            dataTable.Columns.Add("OperationSequence", typeof(int));
            dataTable.Columns.Add("SiteCode", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("Product", typeof(string));
            dataTable.Columns.Add("ProductLine", typeof(string));
            dataTable.Columns.Add("SystemOperator", typeof(string));
            dataTable.Columns.Add("AteVersion", typeof(string));
            dataTable.Columns.Add("cDefinitionname", typeof(string));
            dataTable.Columns.Add("cDefinitionversion", typeof(string));
            dataTable.Columns.Add("OpNumber", typeof(string));
            dataTable.Columns.Add("OpTeam", typeof(string));
            dataTable.Columns.Add("OpDepartment", typeof(string));
            dataTable.Columns.Add("Remark", typeof(string));
        }

        private void WriteToExcel()
        {
            if (!Directory.Exists(SaveExcelPath))
            {
                Directory.CreateDirectory(SaveExcelPath);
            }
            string dir = Path.Combine(SaveExcelPath, string.Format("{0}.xls", SaveExcelName));
            ExcelHelper.DataTabletoExcel(dataTable, dir);
        }

        private void WriteToServer()
        {
            dataStore.EndDateTime = DateTime.Now.ToString("s");
            dataStore.ItemCode = ItemCode;
            dataStore.WorkOrder = WorkOrder;
            dataStore.SerialNumber = Sn;
            dataStore.SiteCode = Person.SiteCode;
            dataStore.OperationSequence = Convert.ToInt32(Person.OperationSequence);
            dataStore.AteName = Person.AteName;
            dataStore.AteVersion = "V1.0";
            dataStore.Product = Person.Product;
            dataStore.ProductLine = Person.ProductLine;
            dataStore.Description = Person.Description;
            dataStore.SystemOperator = Person.LoginName;
            dataStore.ItemName = ItemName;
            dataStore.CDefinitionName = Person.TestSoftName;
            dataStore.CDefinitionVersion = "16.0125";
            dataStore.OpName = Person.OpName;
            dataStore.OpNumber = Person.OpWorkId;
            dataStore.OpTeam = Person.OpTeam;
            dataStore.OpDepartment = Person.Department;
            dataStore.Remark = string.Empty;
            if (dataStore.ItemTestOutcome.Contains("Failed"))
            {
                dataStore.Outcome = "Failed";
            }
            else
            {
                dataStore.Outcome = "Passed";
            }
            int row = testDataDetail.Count;
            for (int i = 0; i < row; i++)
            {
                DataRow newRow = dataTable.NewRow();
                newRow["StartDateTime"] = dataStore.StartDateTime;
                newRow["EndDateTime"] = dataStore.EndDateTime;
                newRow["ItemCode"] = dataStore.ItemCode;
                newRow["WorkOrder"] = dataStore.WorkOrder;
                newRow["SerialNumber"] = dataStore.SerialNumber;
                newRow["Outcome"] = dataStore.Outcome;

                if (isTested)
                {
                    newRow["ItemTestname"] = dataStore.ItemTestname[i];
                }
                else
                {
                    newRow["ItemTestname"] = string.Concat("IL_RL Test(", dataStore.ItemTestname[i], ")");
                }
                newRow["ItemTestOutcome"] = dataStore.ItemTestOutcome[i];
                newRow["ItemTestStartDateTime"] = dataStore.ItemTestStartDateTime[i];
                newRow["ItemTestEndDateTime"] = dataStore.ItemTestEndDateTime[i];
                //1310_IL
                newRow[string.Concat("SubItemTestname", 1)] = dataStore.SubItemTestname[4 * i];
                newRow[string.Concat("SubItemTestOutcome", 1)] = dataStore.SubItemTestOutcome[4 * i];
                newRow[string.Concat("SubItemTestDescription", 1)] = (dataStore.SubItemTestDescription[4 * i]);
                newRow[string.Concat("SubItemTestcValue", 1)] = dataStore.SubItemTestcValue[0];
                //1310_RL
                newRow[string.Concat("SubItemTestname", 2)] = dataStore.SubItemTestname[4 * i + 1];
                newRow[string.Concat("SubItemTestOutcome", 2)] = dataStore.SubItemTestOutcome[4 * i + 1];
                newRow[string.Concat("SubItemTestDescription", 2)] = (dataStore.SubItemTestDescription[4 * i + 1]);
                newRow[string.Concat("SubItemTestcValue", 2)] = dataStore.SubItemTestcValue[1];
                //1550_IL
                newRow[string.Concat("SubItemTestname", 3)] = dataStore.SubItemTestname[4 * i + 2];
                newRow[string.Concat("SubItemTestOutcome", 3)] = dataStore.SubItemTestOutcome[4 * i + 2];
                newRow[string.Concat("SubItemTestDescription", 3)] = (dataStore.SubItemTestDescription[4 * i + 2]);
                newRow[string.Concat("SubItemTestcValue", 3)] = dataStore.SubItemTestcValue[2];
                //1550_RL
                newRow[string.Concat("SubItemTestname", 4)] = dataStore.SubItemTestname[4 * i + 3];
                newRow[string.Concat("SubItemTestOutcome", 4)] = dataStore.SubItemTestOutcome[4 * i + 3];
                newRow[string.Concat("SubItemTestDescription", 4)] = (dataStore.SubItemTestDescription[4 * i + 3]);
                newRow[string.Concat("SubItemTestcValue", 4)] = dataStore.SubItemTestcValue[3];
                newRow["ItemName"] = dataStore.ItemName;
                newRow["OperationSequence"] = dataStore.OperationSequence;
                newRow["SiteCode"] = dataStore.SiteCode;
                newRow["Description"] = dataStore.Description;
                newRow["Product"] = dataStore.Product;
                newRow["ProductLine"] = dataStore.ProductLine;
                newRow["SystemOperator"] = dataStore.SystemOperator;
                newRow["AteName"] = dataStore.AteName;
                newRow["AteVersion"] = dataStore.AteVersion;
                newRow["cDefinitionname"] = dataStore.CDefinitionName;
                newRow["cDefinitionversion"] = dataStore.CDefinitionVersion;
                newRow["OpName"] = dataStore.OpName;
                newRow["OpNumber"] = dataStore.OpNumber;
                newRow["OpTeam"] = dataStore.OpTeam;
                newRow["OpDepartment"] = dataStore.OpDepartment;
                newRow["Remark"] = dataStore.Remark;
                dataTable.Rows.Add(newRow);
            }

            DbHelperSql.WriteToServer(dataTable, "dbo.Test");
        }

        public string DataTableToXml(DataTable dt)
        {
            StringBuilder strXml = new StringBuilder();
            strXml.AppendLine("<XmlTable>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strXml.AppendLine("<rows>");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    strXml.AppendLine("<" + dt.Columns[j].ColumnName + ">" + dt.Rows[i][j] + "</" + dt.Columns[j].ColumnName + ">");
                }
                strXml.AppendLine("</rows>");
            }
            strXml.AppendLine("</XmlTable>");
            return strXml.ToString();
        }

        private void StartTest(object obj)
        {
            Test();
        }

        private void SpHelper_ReadValueReceived(byte[] buf)
        {
            var modeNum = buf[0];
            var waveLen = BitConverter.ToInt32(buf, 1);
            var s = Encoding.ASCII.GetString(buf);
            CurModel = GetMode(modeNum);
            CurWave = string.Format("{0}nm", waveLen);
            var pos1 = s.IndexOf('#');
            var pos2 = s.IndexOf('#', pos1 + 1);
            var pos3 = s.IndexOf('#', pos2 + 1);
            var pos4 = s.IndexOf('#', pos3 + 1);
            var s1 = string.Empty;
            var s2 = string.Empty;
            var s3 = string.Empty;

            if (pos1 != -1 && pos2 != -1)
            {
                s1 = s.Substring(pos1 + 1, pos2 - (pos1 + 1));
            }
            if (pos2 != -1 && pos3 != -1)
            {
                s2 = s.Substring(pos2 + 1, pos3 - (pos2 + 1));
            }
            if (pos3 != -1 && pos4 != -1)
            {
                s3 = s.Substring(pos3 + 1, pos4 - (pos3 + 1));
            }

            if (modeNum == 0)
            {
                var unit = GetUnit(buf[buf.Length - 1]);
                CurDisplayData1 = string.Format("{0}{1}", s1, unit);
                CurDisplayData2 = string.Format("{0}  {1}{2}", "Pwr: ", s2, "dBm");
            }
            if (modeNum == 1)
            {
                var unit = GetUnit(buf[buf.Length - 1]);
                CurDisplayData1 = string.Format("{0}{1}", s1, unit);
                CurDisplayData2 = string.Format("{0}  {1}{2}", "Ref: ", s2, "dBm");
            }
            if (modeNum == 2)
            {
                CurDisplayData1 = string.Format("{0}{1}", s1, "dB");
                CurDisplayData2 = string.Format("{0}  {1}{2}", "L: ", s2, "m");
            }
            if (!Person.IsAddIl2Mode)
            {
                if (modeNum == 3)
                {
                    CurDisplayData1 = string.Format("{0}  {1}{2}", "IL:", s1, "dB");
                    CurDisplayData2 = string.Format("{0}  {1}{2}", "RL:", s2, "dB");
                }
                if (modeNum == 4)
                {
                    CurWave = string.Join(" ", Person.UsingWaves.Waves);
                    var il1 = s.Substring(1, pos1 - 1);
                    CurDisplayData1 = string.Format("{0}  {1}{2}  {3}{4}", "IL:", il1, "dB", s2, "dB");
                    CurDisplayData2 = string.Format("{0}  {1}{2}  {3}{4}", "RL:", s1, "dB", s3, "dB");
                    il1Value = il1;
                    il2Value = s2;
                    rl1Value = s1;
                    rl2Value = s3;
                }
            }
            else
            {
                if (modeNum == 3)
                {
                    var il1 = s.Substring(0, pos1);
                    CurDisplayData1 = string.Format("{0}  {1}{2}", Person.UsingWaves.Waves[0], il1, "dB");
                    CurDisplayData2 = string.Format("{0}  {1}{2}", Person.UsingWaves.Waves[1], s1, "dB");
                    CurWave = null;
                }
                if (modeNum == 4)
                {
                    CurDisplayData1 = string.Format("{0}  {1}{2}", "IL:", s1, "dB");
                    CurDisplayData2 = string.Format("{0}  {1}{2}", "RL:", s2, "dB");
                }
                if (modeNum == 5)
                {
                    CurWave = string.Join(" ", Person.UsingWaves.Waves);
                    var il1 = s.Substring(1, pos1 - 1);
                    CurDisplayData1 = string.Format("{0}  {1}{2}  {3}{4}", "IL:", il1, "dB", s2, "dB");
                    CurDisplayData2 = string.Format("{0}  {1}{2}  {3}{4}", "RL:", s1, "dB", s3, "dB");
                    il1Value = il1;
                    il2Value = s2;
                    rl1Value = s1;
                    rl2Value = s3;
                }
            }
        }

        private void SwitchWave(object obj)
        {
            SerialPortHelper.GCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.TxBytes, 0x22);
            SerialPortHelper.Ts = true;
        }

        private void SwitchMode(object obj)
        {
            SerialPortHelper.GCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.TxBytes, 0x26);
            SerialPortHelper.Ts = true;
        }

        private void Sign(object obj)
        {
            SerialPortHelper.GCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.TxBytes, 0x24);
            SerialPortHelper.Ts = true;
        }

        public string CurModel
        {
            get { return curModel; }
            set
            {
                curModel = value;
                RaisePropertyChanged("CurModel");
            }
        }

        public string CurWave
        {
            get { return curWave; }
            set
            {
                curWave = value;
                RaisePropertyChanged("CurWave");
            }
        }

        public string CurDisplayData1
        {
            get { return curDisplayData1; }
            set
            {
                curDisplayData1 = value;
                RaisePropertyChanged("CurDisplayData1");
            }
        }

        public string CurDisplayData2
        {
            get { return curDisplayData2; }
            set
            {
                curDisplayData2 = value;
                RaisePropertyChanged("CurDisplayData2");
            }
        }

        public string Sn
        {
            get { return sn; }
            set
            {
                sn = value;
                RaisePropertyChanged("Sn");
            }
        }

        public string ItemCode
        {
            get { return itemCode; }
            set
            {
                itemCode = value;
                RaisePropertyChanged("ItemCode");
            }
        }

        public string ConnSerial1
        {
            get { return connSerial1; }
            set
            {
                connSerial1 = value;
                RaisePropertyChanged("ConnSerial1");
            }
        }

        public string ConnType1
        {
            get { return connType1; }
            set
            {
                connType1 = value;
                RaisePropertyChanged("ConnType1");
            }
        }

        public string ConnSerial2
        {
            get { return connSerial2; }
            set
            {
                connSerial2 = value;
                RaisePropertyChanged("ConnSerial2");
            }
        }

        public string ConnType2
        {
            get { return connType2; }
            set
            {
                connType2 = value;
                RaisePropertyChanged("ConnType2");
            }
        }

        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                RaisePropertyChanged("Result");
            }
        }

        public Brush TestResColor
        {
            get { return testResColor; }
            set
            {
                testResColor = value;
                RaisePropertyChanged("TestResColor");
            }
        }

        public string WorkOrder
        {
            get { return workOrder; }
            set
            {
                workOrder = value;
                RaisePropertyChanged("workOrder");
            }
        }

        public string SavePath
        {
            get { return savePath; }
            set
            {
                savePath = value;
                RaisePropertyChanged("SavePath");
            }
        }

        public bool IsUserSetEnable
        {
            get { return isUserSetEnable; }
            set
            {
                isUserSetEnable = value;
                RaisePropertyChanged("IsUserSetEnable");
            }
        }

        public string SaveExcelPath
        {
            get { return saveExcelPath; }
            set
            {
                saveExcelPath = value;
                RaisePropertyChanged("SaveExcelPath");
            }
        }

        public string SaveXmlPath
        {
            get { return saveXmlPath; }
            set
            {
                saveXmlPath = value;
                RaisePropertyChanged("SaveXmlPath");
            }
        }

        public string SubSn
        {
            get { return subSn; }
            set
            {
                subSn = value;
                RaisePropertyChanged("SubSn");
            }
        }

        public bool IsTestComplete
        {
            get { return isTestComplete; }
            set
            {
                isTestComplete = value;
                RaisePropertyChanged("IsTestComplete");
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

        public bool IsCommonTest
        {
            get { return isCommonTest; }
            set
            {
                isCommonTest = value;
                RaisePropertyChanged("IsCommonTest");
            }
        }

        public bool IsQuickTest
        {
            get { return isQuickTest; }
            set
            {
                isQuickTest = value;
                RaisePropertyChanged("IsQuickTest");
            }
        }

        public ObservableCollection<string> ObsHeader
        {
            get { return obsHeader; }
            set
            {
                obsHeader = value;
                RaisePropertyChanged("ObsHeader");
            }
        }

        public string SelHeader
        {
            get { return selHeader; }
            set
            {
                selHeader = value;
                RaisePropertyChanged("SelHeader");
            }
        }

        public bool IsReTestCheck
        {
            get { return isReTestCheck; }
            set
            {
                isReTestCheck = value;
                RaisePropertyChanged("IsReTestCheck");
            }
        }

        public int SelHeaderIndex
        {
            get { return selHeaderIndex; }
            set
            {
                selHeaderIndex = value;
                RaisePropertyChanged("SelHeaderIndex");
            }
        }

        public string SaveExcelName
        {
            get { return saveExcelName; }
            set
            {
                saveExcelName = value;
                RaisePropertyChanged("SaveExcelName");
            }
        }

        public bool IsReTestEnable
        {
            get { return isReTestEnable; }
            set
            {
                isReTestEnable = value;
                RaisePropertyChanged("IsReTestEnable");
            }
        }

        public bool IsSelHeaderEnable
        {
            get { return isSelHeaderEnable; }
            set
            {
                isSelHeaderEnable = value;
                RaisePropertyChanged("IsSelHeaderEnable");
            }
        }

        public bool IsTestCompleteEnable
        {
            get { return isTestCompleteEnable; }
            set
            {
                isTestCompleteEnable = value;
                RaisePropertyChanged("IsTestCompleteEnable");
            }
        }

        public string ItemName
        {
            get { return itemName; }
            set
            {
                itemName = value;
                RaisePropertyChanged("itemName");
            }
        }

        public int ProCore
        {
            get { return proCore; }
            set
            {
                proCore = value;
                RaisePropertyChanged("ProCore");
            }
        }

        public bool IsSingleHead
        {
            get { return isSingleHead; }
            set
            {
                isSingleHead = value;
                RaisePropertyChanged("IsSingleHead");
            }
        }

        public string PinAconnType
        {
            get { return pinAconnType; }
            set
            {
                pinAconnType = value;
                RaisePropertyChanged("PinAconnType");
            }
        }

        public string PinBconnType
        {
            get { return pinBconnType; }
            set
            {
                pinBconnType = value;
                RaisePropertyChanged("PinBconnType");
            }
        }

        public bool IsDoubleHead
        {
            get { return isDoubleHead; }
            set
            {
                isDoubleHead = value;
                RaisePropertyChanged("IsDoubleHead");
            }
        }

        public bool IsCoresTest
        {
            get { return isCoresTest; }
            set
            {
                isCoresTest = value;
                RaisePropertyChanged("IsCoresTest");
            }
        }

        public bool IsPinsTest
        {
            get { return isPinsTest; }
            set
            {
                isPinsTest = value;
                RaisePropertyChanged("IsPinsTest");
            }
        }

        public ObservableCollection<TestItemModel> TestItemList
        {
            get { return testItemList; }
            set
            {
                testItemList = value;
                RaisePropertyChanged("TestItemList");
            }
        }

        public ObservableCollection<TestDataDetailModel> TestDataDetail
        {
            get { return testDataDetail; }
            set
            {
                testDataDetail = value;
                RaisePropertyChanged("TestDataDetail");
            }
        }

        public ObservableCollection<ThreSetModel> ThreSet
        {
            get { return threSet; }
            set
            {
                threSet = value;
                RaisePropertyChanged("ThreSet");
            }
        }
    }
}