using JW8307A.Models;

namespace JW8307A.ViewModels
{
    internal class ThreItemViewModel : NotificationObject
    {
        public ThreItem ThreItem { get; set; }

        public bool IsIlSelected
        {
            get { return isIlSelected; }
            set
            {
                isIlSelected = value;
                RaisePropertyChanged("IsIlSelected");
            }
        }

        public bool IsRlSelected
        {
            get { return isRlSelected; }
            set
            {
                isRlSelected = value;
                RaisePropertyChanged("IsRlSelected");
            }
        }

        private bool isIlSelected;
        private bool isRlSelected;
    }
}