using ChineseIdioms.Io;
using ChineseIdioms.Linkage;
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ChineseIdiomsConsole
{
    class Program
    {
        static IdiomsLookup _idiomsLookup;

        static void InitIdiomRepo()
        {
            var loader = new SimpleIdiomsLoader();
            var idioms = loader.GetAllIdioms().Where(x => x.Length == 4);
            _idiomsLookup = new IdiomsLookup();
            _idiomsLookup.Load(idioms);
        }

        static void NextWithFirst(char first, TextWriter tw)
        {
            var sols = _idiomsLookup.IdiomsAsFirstChar(first);
            if (sols != null)
            {
                var num = 1;
                foreach (var sol in sols)
                {
                    tw.WriteLine($"解{num}: {sol}");
                    num++;
                }
            }
            else
            {
                tw.WriteLine($"“{first}”起首无解");
            }
        }

        static void FindWidest(char first, TextWriter tw, HashSet<string> excluded = null)
        {
            var sol = _idiomsLookup.GetWidestPath(first, 5, excluded);
            if (sol != null)
            {
                var notFirst = false;
                foreach (var idiom in sol)
                {
                    if (notFirst)
                    {
                        tw.Write("->");
                    }
                    else
                    {
                        notFirst = true;
                    }
                    tw.Write($"{idiom}");
                }
                tw.WriteLine();
            }
            else
            {
                tw.WriteLine($"“{first}”起首无解");
            }
        }

        static void Join(char start, char end, TextWriter tw)
        {
            var sols = _idiomsLookup.JoinEnds(start, end);
            var num = 1;
            foreach (var sol in sols)
            {
                tw.Write($"解{num}: ");
                var notFirst = false;
                foreach (var idiom in sol)
                {
                    if (notFirst)
                    {
                        tw.Write("->");
                    }
                    else
                    {
                        notFirst = true;
                    }
                    tw.Write(idiom);
                }
                tw.WriteLine();
                num++;
            }
            if (num == 1)
            {
                tw.WriteLine($"连接“{start}”和“{end}”无解");
            }
        }

        abstract class Request
        {
            public abstract void Action(TextWriter tw);
        }

        class JoinRequest : Request
        {
            public char FirstChar;
            public char LastChar;

            public override void Action(TextWriter tw)
            {
                Join(FirstChar, LastChar, tw);
            }
        }

        class InitialRequest: Request
        {
            public char FirstChar;

            public override void Action(TextWriter tw)
            {
                NextWithFirst(FirstChar, tw);
            }
        }

        class WidestPathRequest: Request
        {
            public char FirstChar;
            public HashSet<string> ExcludeSet;

            public override void Action(TextWriter tw)
            {
                FindWidest(FirstChar, tw, ExcludeSet);
            }
        }
        
        static Request ParseInput(TextReader sr)
        {
            var cmd = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(cmd)) return null;
            switch (cmd)
            {
                case "first":
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) return null;
                    return new InitialRequest
                    {
                        FirstChar = line.TrimStart()[0]
                    };
                }
                case "join":
                {
                    var line1 = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line1)) return null;
                    var line2 = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line2)) return null;
                    return new JoinRequest
                    {
                        FirstChar = line1.TrimStart()[0],
                        LastChar = line2.TrimStart()[0]
                    };
                }
                case "widest":
                {
                    var first = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(first)) return null;
                    var excludeSet = new HashSet<string>();
                    while (true)
                    {
                        var line = sr.ReadLine().Trim();
                        if (string.IsNullOrEmpty(line)) break;
                        excludeSet.Add(line.Trim());
                    }
                    return new WidestPathRequest
                    {
                        FirstChar = first.TrimStart()[0],
                        ExcludeSet = excludeSet
                    };
                }
                default:
                    return null;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                InitIdiomRepo();
                var inputFilePath = args[0];
                var outputFilePath = args[1];
                using (var inputFile = new FileStream(inputFilePath, FileMode.Open))
                using (var sr = new StreamReader(inputFile, Encoding.UTF8))
                using (var outputFile = new FileStream(outputFilePath, FileMode.Create))
                using (var sw = new StreamWriter(outputFile, Encoding.UTF8))
                {
                    var req = ParseInput(sr);
                    req?.Action(sw);
                }
                Process.Start("notepad.exe", outputFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"The appliction encountered an error:\n {e}");
            }
        }
    }
}
