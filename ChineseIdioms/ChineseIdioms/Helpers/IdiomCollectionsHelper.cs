using System.Collections.Generic;
using System.Linq;
using static ChineseIdioms.Linkage.IdiomTree;

namespace ChineseIdioms.Helpers
{
    public static class IdiomCollectionsHelper
    {
        public static void LoadNodeList(this IList<IdiomNode> nodes, ICollection<string> idioms)
        {
            foreach (var n in idioms.Select(x => new IdiomNode(x)))
            {
                nodes.Add(n);
            }
        }
    }
}
