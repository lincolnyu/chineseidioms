using System.Collections.Generic;
using System.Linq;

namespace ChineseIdioms.Linkage
{
    public class IdiomsLookup
    {
        public Dictionary<char, HashSet<string>> FirstCharLookup { get; } = new Dictionary<char, HashSet<string>>();
        public Dictionary<char, HashSet<string>> LastCharLookup { get; } = new Dictionary<char, HashSet<string>>();

        public void Clear()
        {
            FirstCharLookup.Clear();
            LastCharLookup.Clear();
        }

        public void Load(IEnumerable<string> idioms)
        {
            foreach (var idiom in idioms.Where(x=>x.Length >= 4))
            {
                var firstChar = idiom[0];
                var lastChar = idiom[idiom.Length - 1];
                if (!FirstCharLookup.TryGetValue(firstChar, out var firstCharSet))
                {
                    firstCharSet = new HashSet<string>();
                    FirstCharLookup[firstChar] = firstCharSet;
                }
                if (!LastCharLookup.TryGetValue(lastChar, out var lastCharSet))
                {
                    lastCharSet = new HashSet<string>();
                    LastCharLookup[lastChar] = lastCharSet;
                }
                firstCharSet.Add(idiom);
                lastCharSet.Add(idiom);
            }
        }
       
        public void ReLoad(IEnumerable<string> idioms)
        {
            Clear();
            Load(idioms);
        }
    }
}
