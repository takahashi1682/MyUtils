using System;
using System.Collections.Generic;

namespace MyUtils.HTTPUtils
{
    /// <summary>
    /// APIから配列でデータを取得する際に使用するラッパークラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SchemaList<T>
    {
        public List<T> list;

        public static string ToFormat(string x) => "{\"list\": " + x + "}";
    }
}