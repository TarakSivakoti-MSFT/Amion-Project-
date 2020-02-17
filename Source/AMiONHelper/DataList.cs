using AMiON.Helper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public sealed class DataList<T>
    {
        private List<T> dataList { get; set; }

        public DataList()
        {
            dataList = new List<T>();
        }

        public void Add(T t)
        {
            if(dataList == null)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new ArgumentException(Constant.DataListEmptyMessage);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }

            if(t is Logger)
            {
                dataList.Add(t);
            }
        }

        public List<T> GetAllDataList()
        {
            return dataList;
        }
    }
}
