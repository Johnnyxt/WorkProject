using JW8307A.ViewModels;

namespace JW8307A.Models
{
    internal class ThreSetModel : NotificationObject
    {
        private bool isWaveEnable;
        private string wave;
        private bool isIlEnable;
        private float ilLower;
        private float ilUpper;
        private bool isRlEnable;
        private float rlLower;
        private float rlUpper;

        public bool IsWaveEnable
        {
            get { return isWaveEnable; }
            set
            {
                isWaveEnable = value;
                RaisePropertyChanged("IsWaveEnable");
            }
        }

        public string Wave
        {
            get { return wave; }
            set
            {
                wave = value;
                RaisePropertyChanged("Wave");
            }
        }

        public bool IsIlEnable
        {
            get { return isIlEnable; }
            set
            {
                isIlEnable = value;
                RaisePropertyChanged("IsIlEnable");
            }
        }

        public float IlLower
        {
            get { return ilLower; }
            set
            {
                ilLower = value;
                RaisePropertyChanged("IlLower");
            }
        }

        public float IlUpper
        {
            get { return ilUpper; }
            set
            {
                ilUpper = value;
                RaisePropertyChanged("IlUpper");
            }
        }

        public bool IsRlEnable
        {
            get { return isRlEnable; }
            set
            {
                isRlEnable = value;
                RaisePropertyChanged("IsRlEnable");
            }
        }

        public float RlLower
        {
            get { return rlLower; }
            set
            {
                rlLower = value;
                RaisePropertyChanged("RlLower");
            }
        }

        public float RlUpper
        {
            get { return rlUpper; }
            set
            {
                rlUpper = value;
                RaisePropertyChanged("RlUpper");
            }
        }
    }
}