using JW8307A.ViewModels;

namespace JW8307A.Models
{
    internal class ThreItem : NotificationObject
    {
        public string ThreWave { get; set; }
        public bool IsIlEnable { get; set; }
        public bool IsRlEnable { get; set; }
        public float ThreIlUpperLimit { get; set; }
        public float ThreIlLowerLimit { get; set; }
        public float ThreRlUpperLimit { get; set; }
        public float ThreRlLowerLimit { get; set; }
    }
}