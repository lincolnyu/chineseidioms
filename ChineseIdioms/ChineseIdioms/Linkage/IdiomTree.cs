using System.Collections.Generic;
using System.Diagnostics;

namespace ChineseIdioms.Linkage
{
    public interface IHasChildren<TNode> where TNode : IHasChildren<TNode>
    {
        char LastChar { get; }
        List<TNode> Children { get; }
    }

    public class IdiomTree : IHasChildren<IdiomTree.IdiomNode>
    {
        public class IdiomNode : IHasChildren<IdiomNode>
        {
            public IdiomNode(string idiom)
            {
                Debug.Assert(idiom.Length >= 4);
                Idiom = idiom;
            }
            public string Idiom { get; }
            public char FirstChar => Idiom[0];
            public char LastChar => Idiom[Idiom.Length - 1];
            public IHasChildren<IdiomNode> Parent { get; set; }
            public List<IdiomNode> Children { get; } = new List<IdiomNode>();
        }

        public IdiomTree(char lastChar)
        {
            LastChar = lastChar;
        }
        public char LastChar { get; }
        public List<IdiomNode> Children { get; } = new List<IdiomNode>();
    }
}
