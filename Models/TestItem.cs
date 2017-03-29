using JW8307A.ViewModels;

namespace JW8307A.Models
{
    public class TestItem : NotificationObject
    {
        public string IlTestWave { get; set; }
        public bool IlTestEanble { get; set; }
        public bool RlTestEnable { get; set; }
        public string IlTestValue { get; set; }
        public string RlTestWave { get; set; }
        public string RlTestValue { get; set; }
    }
}