﻿using ChineseIdioms.Helpers;
using System.Collections.Generic;

namespace ChineseIdioms.Linkage
{
    public static class IdiomAlgorithms
    {
        public static IReadOnlyCollection<string> IdiomsAsFirstChar(this IdiomsLookup lookup, char firstChar)
        {
            if (lookup.FirstCharLookup.TryGetValue(firstChar, out var idioms))
            {
                return idioms;
            }
            return null;
        }

        public static IReadOnlyCollection<string> IdiomsAsLastChar(this IdiomsLookup lookup, char firstChar)
        {
            if (lookup.LastCharLookup.TryGetValue(firstChar, out var idioms))
            {
                return idioms;
            }
            return null;
        }

        public static IEnumerable<LinkedList<string>> JoinEnds(this IdiomsLookup lookup, 
            char firstChar, char lastChar, bool trimUsed = true)
        {
            var used = trimUsed ? new HashSet<string>() : null;
            var itree = new IdiomTree(firstChar);
            var q = new Queue<IHasChildren<IdiomTree.IdiomNode>>();
            q.Enqueue(itree);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                if (!lookup.FirstCharLookup.TryGetValue(n.LastChar, out var firstCharSet))
                {
                    continue;
                }
                if (used != null)
                {
                    firstCharSet.RemoveWhere(x => used.Contains(x));
                }
                if (firstCharSet.Count == 0)
                {
                    continue;
                }

                n.Children.LoadNodeList(firstCharSet);
                foreach (var c in n.Children)
                {
                    c.Parent = n;
                    used?.Add(c.Idiom);
                    if (c.LastChar == lastChar)
                    {
                        var l = new LinkedList<string>();
                        for (var p = c; p != null; p = p.Parent as IdiomTree.IdiomNode)
                        {
                            l.AddFirst(p.Idiom);
                        }
                        yield return l;
                    }
                    else
                    {
                        q.Enqueue(c);
                    }
                }
            }
        }
    }
}
