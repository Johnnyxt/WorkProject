using JW8307A.Models;
using System.Collections.ObjectModel;

namespace JW8307A.Services
{
    internal interface ITestDataService
    {
        ObservableCollection<TestItem> GetTestItems();
    }
}