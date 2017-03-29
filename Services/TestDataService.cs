using JW8307A.Models;
using System.Collections.ObjectModel;

namespace JW8307A.Services
{
    internal class TestDataService : ITestDataService
    {
        public ObservableCollection<TestItem> GetTestItems()
        {
            ObservableCollection<TestItem> testItems = new ObservableCollection<TestItem>();
            string[] ilWaves = { "1310nm", "1490nm", "1550nm", "1625nm", "850nm", "1300nm" };
            string[] rlWaves = { "1310nm", "1490nm", "1550nm", "1625nm", "850nm", "1300nm" };
            string[] ilValues = new string[6];
            string[] rlValues = new string[6];
            for (int i = 0; i < 6; i++)
            {
                TestItem t = new TestItem
                {
                    IlTestWave = Data.IlWave[i],
                    IlTestValue = Data.IlValue[i].ToString(),
                    RlTestWave = Data.RlWave[i],
                    RlTestValue = Data.RlValue[i].ToString()
                };
                //TestItem t = new TestItem
                //{
                //    IlTestWave = Data.IlWave[i],

                //    IlTestValue = Data.RlValue[i].ToString(),
                //    RlTestWave = Data.RlWave[i],
                //    RlTestValue = Data.RlValue[i].ToString()
                //};
                testItems.Add(t);
            }
            return testItems;
        }
    }
}