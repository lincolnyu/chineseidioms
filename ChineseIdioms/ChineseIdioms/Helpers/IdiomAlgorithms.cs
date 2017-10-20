using ChineseIdioms.Helpers;
using System;
using System.Collections.Generic;

namespace ChineseIdioms.Linkage
{
    public static class IdiomAlgorithms
    {
        public delegate bool VisitDelegate(LinkedList<string> chain);

        public static IReadOnlyCollection<string> IdiomsAsFirstChar(this IdiomsLookup lookup, char firstChar)
        {
            if (lookup.FirstCharLookup.TryGetValue(firstChar, out var idioms))
            {
                return idioms;
            }
            return null;
        }

        public static IReadOnlyCollection<string> IdiomsAsLastChar(this IdiomsLookup lookup, char lastChar)
        {
            if (lookup.LastCharLookup.TryGetValue(lastChar, out var idioms))
            {
                return idioms;
            }
            return null;
        }

        public static string[] GetDeepestAsFirstChar(this IdiomsLookup lookup, char firstChar)
        {
            var maxChainLen = 0;
            string[] maxChain = null;
            lookup.TraverseDepthFirst(firstChar, chain=>
            {
                if (chain.Count > maxChainLen)
                {
                    Console.WriteLine($"Got len: {chain.Count}");
                    maxChainLen = chain.Count;
                    maxChain = new string[maxChainLen];
                    chain.CopyTo(maxChain, 0);
                    if (maxChainLen >= 20)
                    {
                        return false;
                    }
                }
                return true;
            });
            return maxChain;
        }

        public static bool TraverseDepthFirst(this IdiomsLookup lookup, char firstChar,
            VisitDelegate visit, LinkedList<string> chain = null, HashSet<string> used = null)
        {
            var children = lookup.IdiomsAsFirstChar(firstChar);
            if (children != null)
            {
                if (chain == null)
                {
                    chain = new LinkedList<string>();
                }
                if (used == null)
                {
                    used = new HashSet<string>();
                }
                foreach (var child in children)
                {
                    if (used.Contains(child))
                    {
                        continue;
                    }
                    chain.AddLast(child);
                    used.Add(child);
                    if (!visit(chain))
                    {
                        return false;
                    }
                    if (!lookup.TraverseDepthFirst(child[child.Length-1], visit, chain, used))
                    {
                        return false;
                    }
                    chain.RemoveLast();
                    used.Remove(child);
                }
            }
            return true;
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
