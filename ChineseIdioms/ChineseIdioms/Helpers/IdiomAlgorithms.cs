using ChineseIdioms.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseIdioms.Linkage
{
    public static class IdiomAlgorithms
    {
        public enum VisitResults
        {
            Continue,
            NoDeeper,
            Quit
        }

        public delegate VisitResults VisitDelegate(IReadOnlyCollection<string> chain,
            IReadOnlyCollection<IReadOnlyCollection<string>> children);

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

        public static string[] GetDeepestAsFirstChar(this IdiomsLookup lookup, char firstChar,
            int maxDepth = 30)
        {
            var maxChainLen = 0;
            string[] maxChain = null;
            lookup.TraverseDepthFirst(firstChar, (chain, childrenChain)=>
            {
                var chainLen = chain.Count;
                if (chainLen > maxChainLen)
                {
                    maxChainLen = chainLen;
                    maxChain = chain.ToArray();
                }
                return chainLen >= maxDepth ? VisitResults.Quit : VisitResults.Continue;
            });
            return maxChain;
        }

        public static string[] GetWidestPath(this IdiomsLookup lookup, char firstChar,
            int maxDepth = 20, HashSet<string> excluded = null)
        {
            var maxChainLen = 0;
            var maxAltCount = 0;
            string[] maxChain = null;
            lookup.TraverseDepthFirst(firstChar, (chain, childrenChain) =>
            {
                var chainLen = chain.Count;
                if (chainLen > maxChainLen)
                {
                    var altCount = childrenChain.Sum(x => x.Count);
                    if (altCount > maxAltCount)
                    {
                        maxAltCount = altCount;
                        maxChainLen = chain.Count;
                        maxChain = chain.ToArray();
                    }
                }
                return chainLen >= maxDepth ? VisitResults.NoDeeper : VisitResults.Continue;
            }, null, excluded);
            return maxChain;
        }

        public static bool TraverseDepthFirst(this IdiomsLookup lookup, char firstChar,
            VisitDelegate visit, LinkedList<string> chain = null, 
            HashSet<string> excluded = null, LinkedList<IReadOnlyCollection<string>> childrenList = null)
        {
            var children = lookup.IdiomsAsFirstChar(firstChar);
            if (children != null)
            {
                if (chain == null)
                {
                    chain = new LinkedList<string>();
                }
                if (excluded == null)
                {
                    excluded = new HashSet<string>();
                }
                if (childrenList == null)
                {
                    childrenList = new LinkedList<IReadOnlyCollection<string>>();
                }
                childrenList.AddLast(children);
                foreach (var child in children)
                {
                    if (excluded.Contains(child))
                    {
                        continue;
                    }
                    chain.AddLast(child);
                    excluded.Add(child);
                    var vr = visit(chain, childrenList);
                    if (vr == VisitResults.Quit)
                    {
                        return false;
                    }
                    if (vr == VisitResults.Continue)
                    {
                        if (!lookup.TraverseDepthFirst(child[child.Length - 1], visit, chain, excluded, childrenList))
                        {
                            return false;
                        }
                    }
                    chain.RemoveLast();
                    excluded.Remove(child);
                }
                childrenList.RemoveLast();
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
