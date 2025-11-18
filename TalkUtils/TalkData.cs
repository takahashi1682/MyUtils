using System.Collections.Generic;

namespace MyUtils.TalkUtils
{
    [System.Serializable]
    public class TalkData : List<LineData>
    {
        public List<LineData> GetLines(string key) => FindAll(x => x.Key == key);
    }
}