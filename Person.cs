using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace JW8307A
{
    internal class Person
    {
        public static class UsingWaves
        {
            public static List<string> Waves { get; set; }
        }

        public static string ServerName { get; set; }//服务器名称
        public static string DbLoginName { get; set; } //登录名
        public static string DbLoginPsd { get; set; }//登录密码
        public static string DbName { get; set; } //数据库名称
        public static List<float> IlUpperThre = new List<float>();
        public static List<float> IlLowerThre = new List<float>();
        public static List<float> RlUpperThre = new List<float>();
        public static List<float> RlLowerThre = new List<float>();
        public static bool IsAddIl2Mode;
        public static DbHelperSql SqlHelper;
        public static SerialPortHelper SpHelper = new SerialPortHelper();
        public static bool IsBasicInfoViewEnable;
        public static bool IsUserSetViewEnable;
        public static string WorkOrder;
        public static string LoginName;
        public static string LoginPsd;
        public static string OpName;
        public static string Description;
        public static string OpWorkId;
        public static string Product;
        public static string OpTeam;
        public static string ProductLine;
        public static string Department;
        public static string AteName;
        public static string OperationSequence;
        public static string AteVersion;
        public static string SiteCode;
        public static string TestSoftName;
        public static string SaveExcelPath;
        public static string SaveExcelName;
        public static string[] TestWave = { "1310nm", "1550nm", "850nm", "1300nm", "1450nm", "1625nm" };
        private static readonly string CompilePath = Process.GetCurrentProcess().MainModule.FileName;
        public static DateTime CompileTime = File.GetLastWriteTime(CompilePath);

        //public static string Vision = string.Format("JW8307A V{0}.{1}.{2}", CompileTime.Year, CompileTime.Month, CompileTime.Day);
        public static string Vision = string.Format("JW8307A V{0}.{1}.{2}", 2017, 3, 24);

        public const string XmlPath = @".//Config.xml";
        public static XmlDocument XmlDoc = new XmlDocument();
    }
}