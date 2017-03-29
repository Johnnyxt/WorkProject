using JW8307A.ViewModels;

namespace JW8307A.Models
{
    internal class TestDataDetailModel : NotificationObject
    {
        private string date;
        private string time;
        private string _serialNumber;
        private string subSn;
        private string connector;
        private string il1;
        private string rl1;
        private string il2;
        private string rl2;
        private string il3;
        private string rl3;
        private string il4;
        private string rl4;
        private string il5;
        private string rl5;
        private string il6;
        private string rl6;
        private string result;
        private string oper;
        private string workId;

        public string Date
        {
            get { return date; }
            set
            {
                date = value;
                RaisePropertyChanged("Date");
            }
        }

        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                RaisePropertyChanged("Time");
            }
        }

        public string Connector
        {
            get { return connector; }
            set
            {
                connector = value;
                RaisePropertyChanged("Connector");
            }
        }

        public string Il1
        {
            get { return il1; }
            set
            {
                il1 = value;
                RaisePropertyChanged("Il1");
            }
        }

        public string Rl1
        {
            get { return rl1; }
            set
            {
                rl1 = value;
                RaisePropertyChanged("Rl1");
            }
        }

        public string Il2
        {
            get { return il2; }
            set
            {
                il2 = value;
                RaisePropertyChanged("Il2");
            }
        }

        public string Rl2
        {
            get { return rl2; }
            set
            {
                rl2 = value;
                RaisePropertyChanged("Rl2");
            }
        }

        public string Il3
        {
            get { return il3; }
            set
            {
                il3 = value;
                RaisePropertyChanged("Il3");
            }
        }

        public string Rl3
        {
            get { return rl3; }
            set
            {
                rl3 = value;
                RaisePropertyChanged("Rl3");
            }
        }

        public string Il4
        {
            get { return il4; }
            set
            {
                il4 = value;
                RaisePropertyChanged("Il4");
            }
        }

        public string Rl4
        {
            get { return rl4; }
            set
            {
                rl4 = value;
                RaisePropertyChanged("Rl4");
            }
        }

        public string Il5
        {
            get { return il5; }
            set
            {
                il5 = value;
                RaisePropertyChanged("Il5");
            }
        }

        public string Rl5
        {
            get { return rl5; }
            set
            {
                rl5 = value;
                RaisePropertyChanged("Rl5");
            }
        }

        public string Il6
        {
            get { return il6; }
            set
            {
                il6 = value;
                RaisePropertyChanged("Il6");
            }
        }

        public string Rl6
        {
            get { return rl6; }
            set
            {
                rl6 = value;
                RaisePropertyChanged("Rl6");
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

        public string Oper
        {
            get { return oper; }
            set
            {
                oper = value;
                RaisePropertyChanged("Oper");
            }
        }

        public string WorkId
        {
            get { return workId; }
            set
            {
                workId = value;
                RaisePropertyChanged("WorkId");
            }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                RaisePropertyChanged("SerialNumber");
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
    }
}