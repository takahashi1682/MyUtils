using System.Collections.Generic;

namespace MyUtils.TalkUtils
{
    public class TalkData : List<LineData>
    {
        // public LineData GetLine(int id) => Find(x => x.Id == id);
        public LineData GetLine(string key) => Find(x => x.Key == key);
        public List<LineData> GetLineGroups(int group) => FindAll(x => x.Group == group);
    }
}