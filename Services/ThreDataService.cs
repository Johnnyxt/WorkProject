using JW8307A.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace JW8307A.Services
{
    internal class ThreDataService : IThreDataService
    {
        public ObservableCollection<ThreItem> GetThreItems()
        {
            ObservableCollection<ThreItem> threItems = new ObservableCollection<ThreItem>();
            string cmdText = "SELECT * FROM TThresholdSet";
            DataSet ds = AccessHelper.DataSet(cmdText);
            ThreItem t = new ThreItem();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                t.ThreWave = Convert.ToString(ds.Tables[0].Rows[i].ItemArray[1]);
                t.IsIlEnable = Convert.ToBoolean(ds.Tables[0].Rows[i].ItemArray[2]);
                t.IsRlEnable = Convert.ToBoolean(ds.Tables[0].Rows[i].ItemArray[3]);
                t.ThreIlLowerLimit = Convert.ToSingle(ds.Tables[0].Rows[i].ItemArray[4]);
                t.ThreIlUpperLimit = Convert.ToSingle(ds.Tables[0].Rows[i].ItemArray[5]);
                t.ThreRlLowerLimit = Convert.ToSingle(ds.Tables[0].Rows[i].ItemArray[6]);
                t.ThreRlUpperLimit = Convert.ToSingle(ds.Tables[0].Rows[i].ItemArray[7]);
                threItems.Add(t);
            }
            return threItems;
        }
    }
}