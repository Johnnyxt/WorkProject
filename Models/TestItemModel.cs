using JW8307A.ViewModels;
using NPOI.SS.Formula.Functions;

namespace JW8307A.Models
{
    internal class TestItemModel : NotificationObject
    {
        private string wave1Il;
        private string wave1Rl;
        private string wave2Il;
        private string wave2Rl;
        private string wave3Il;
        private string wave3Rl;
        private string wave4Il;
        private string wave4Rl;
        private string wave5Il;
        private string wave5Rl;
        private string wave6Il;
        private string wave6Rl;

        public string Wave1Il
        {
            get { return wave1Il; }
            set
            {
                wave1Il = value;
                RaisePropertyChanged("Wave1Il");
            }
        }

        public string Wave1Rl
        {
            get { return wave1Rl; }
            set
            {
                wave1Rl = value;
                RaisePropertyChanged("Wave1Rl");
            }
        }

        public string Wave2Il
        {
            get { return wave2Il; }
            set
            {
                wave2Il = value;
                RaisePropertyChanged("Wave2Il");
            }
        }

        public string Wave2Rl
        {
            get { return wave2Rl; }
            set
            {
                wave2Rl = value;
                RaisePropertyChanged("Wave2Rl");
            }
        }

        public string Wave3Il
        {
            get { return wave3Il; }
            set
            {
                wave3Il = value;
                RaisePropertyChanged("Wave3Il");
            }
        }

        public string Wave3Rl
        {
            get { return wave3Rl; }
            set
            {
                wave3Rl = value;
                RaisePropertyChanged("Wave3Rl");
            }
        }

        public string Wave4Il
        {
            get { return wave4Il; }
            set
            {
                wave4Il = value;
                RaisePropertyChanged("Wave4Il");
            }
        }

        public string Wave4Rl
        {
            get { return wave4Rl; }
            set
            {
                wave4Rl = value;
                RaisePropertyChanged("Wave4Rl");
            }
        }

        public string Wave5Il
        {
            get { return wave5Il; }
            set
            {
                wave5Il = value;
                RaisePropertyChanged("Wave5Il");
            }
        }

        public string Wave5Rl
        {
            get { return wave5Rl; }
            set
            {
                wave5Rl = value;
                RaisePropertyChanged("Wave5Rl");
            }
        }

        public string Wave6Il
        {
            get { return wave6Il; }
            set
            {
                wave6Il = value;
                RaisePropertyChanged("Wave6Il");
            }
        }

        public string Wave6Rl
        {
            get { return wave6Rl; }
            set
            {
                wave6Rl = value;
                RaisePropertyChanged("Wave6Rl");
            }
        }
    }
}