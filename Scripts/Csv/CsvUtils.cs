using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyUtils.Csv
{
    public static class CsvUtils<T> where T : AbstractCsvData, new()
    {
        public static List<T> Parse(TextAsset csvFile, string delimiter = ",", int linesToSkip = 2)
        {
            var result = new List<T>();
            var reader = new StringReader(csvFile.text);

            // 最初の2行を無視する
            for (int i = 0; i < linesToSkip; i++)
            {
                if (reader.Peek() != -1) { reader.ReadLine(); }
            }

            // 1行ずつ読み込み、オブジェクトに変換する
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();

                var data = new T();
                data.SetParameter(line?.Split(delimiter));
                result.Add(data);
            }

            reader.Dispose();
            return result;
        }
    }
}