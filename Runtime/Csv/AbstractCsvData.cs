using System;

namespace MyUtils.Csv
{
    [Serializable]
    public abstract class AbstractCsvData
    {
        public abstract void SetParameter(string[] parameter);
    }
}