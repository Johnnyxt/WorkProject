using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace JW8307A
{
    internal class SerialPortHelper
    {
        private readonly byte[] rxBytes = new byte[1024];
        private byte[] data;
        private int timeCnt;
        private int gCmd;
        private static readonly SerialPort Sp = new SerialPort();
        public static readonly Protocol ProtocolPars = new Protocol();
        public static byte[] TxBytes = new byte[1024];
        public static bool Ts;
        public static int GCnt;

        public static readonly DispatcherTimer DTimer = new DispatcherTimer();

        public static event SerialPortEventArgs.ReadValue ReadValueReceived;

        public static event SerialPortEventArgs.GetConnect ConnectReceived;

        public SerialPortHelper()
        {
            Sp.BaudRate = 115200;
            Sp.DataReceived += Sp_DataReceived;
            InitTimer();
        }

        private void InitTimer()
        {
            DTimer.Interval = TimeSpan.FromMilliseconds(200);
            DTimer.Tick += DTimer_Tick;
        }

        public void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(100);
                int num = Sp.BytesToRead;
                Sp.Read(rxBytes, 0, num);
                for (int i = 0; i < num; i++)
                {
                    int rxovert = ProtocolPars.Protcol_Parser_P(rxBytes[i]);
                    if (rxovert != 0)
                    {
                        gCmd = rxovert;
                        data = ProtocolPars.Protocol_Convert();
                        com1_pro_decode(gCmd, data);
                        Array.Clear(rxBytes, 0, rxBytes.Length);
                    }
                }
            }
            catch (System.InvalidOperationException ioException)
            {
                MessageBox.Show(ioException.Message);
            }
        }

        private static void com1_pro_decode(int cmd, byte[] data)
        {
            switch (cmd)
            {
                case 0x41:
                    if (ConnectReceived != null)
                    {
                        ConnectReceived(data);
                    }
                    break;

                case 0x4d:
                    if (ReadValueReceived != null)
                    {
                        ReadValueReceived(data);
                    }
                    break;
            }
        }

        private void DTimer_Tick(object sender, EventArgs e)
        {
            if (Ts)
            {
                SerialPortWrite(TxBytes, GCnt);
                Array.Clear(TxBytes, 0, TxBytes.Length);
                timeCnt = 0;
                Ts = false;
            }
            else
            {
                timeCnt++;
                if (timeCnt >= 3)
                {
                    GCnt = ProtocolPars.Protocol_wr(TxBytes, 0x4c);
                    SerialPortWrite(TxBytes, GCnt);
                    Array.Clear(TxBytes, 0, TxBytes.Length);
                    timeCnt = 0;
                }
            }
        }

        public int Baud
        {
            get { return Sp.BaudRate; }
            set { Sp.BaudRate = value; }
        }

        public string PortName
        {
            get { return Sp.PortName; }
            set
            {
                Sp.PortName = value;
            }
        }

        public bool IsOpen { get; set; }

        public void SerialPortWrite(byte[] buffer, int cnt)
        {
            if (!Sp.IsOpen) return;
            try
            {
                Sp.Write(buffer, 0, cnt);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClosePort()
        {
            if (Sp.IsOpen)
            {
                Sp.Close();
            }

            IsOpen = false;
        }

        public void OpenPort()
        {
            if (Sp.IsOpen)
            {
                Sp.Close();
            }
            try
            {
                Sp.Open();
            }
            catch (Exception)
            {
                IsOpen = false;
            }
            IsOpen = true;
        }
    }
}