using ChineseIdioms.Io;
using ChineseIdioms.Linkage;
using System;
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

        static void NextWithFirst(char first, StreamWriter sw)
        {
            var sols = _idiomsLookup.IdiomsAsFirstChar(first);
            if (sols != null)
            {
                var num = 1;
                foreach (var sol in sols)
                {
                    sw.WriteLine($"解{num}: {sol}");
                    num++;
                }
            }
            else
            {
                sw.WriteLine($"“{first}”起首无解");
            }
        }

        static void FindWidest(char first, StreamWriter sw, HashSet<string> excluded = null)
        {
            var sol = _idiomsLookup.GetWidestPath(first, 5, excluded);
            if (sol != null)
            {
                var notFirst = false;
                foreach (var idiom in sol)
                {
                    if (notFirst)
                    {
                        sw.Write("->");
                    }
                    else
                    {
                        notFirst = true;
                    }
                    sw.Write($"{idiom}");
                }
                sw.WriteLine();
            }
            else
            {
                sw.WriteLine($"“{first}”起首无解");
            }
        }

        static void Join(char start, char end, StreamWriter sw)
        {
            var sols = _idiomsLookup.JoinEnds(start, end);
            var num = 1;
            foreach (var sol in sols)
            {
                sw.Write($"解{num}: ");
                var notFirst = false;
                foreach (var idiom in sol)
                {
                    if (notFirst)
                    {
                        sw.Write("->");
                    }
                    else
                    {
                        notFirst = true;
                    }
                    sw.Write(idiom);
                }
                sw.WriteLine();
                num++;
            }
            if (num == 1)
            {
                sw.WriteLine($"连接“{start}”和“{end}”无解");
            }
        }

        static void Main(string[] args)
        {
            try
            {
                InitIdiomRepo();
                using (var sw = new StreamWriter(args[0]))
                {
                    var action = args[1];
                    if (action == "join")
                    {
                        Join(args[2][0], args[3][0], sw);
                    }
                    else if (action == "first")
                    {
                        NextWithFirst(args[2][0], sw);
                    }
                    else if (action == "widest")
                    {
                        HashSet<string> excludeSet = null;
                        if(args.Length > 3)
                        {
                            var excludes = args[3].Split(',');
                            excludeSet = new HashSet<string>();
                            foreach (var e in excludes)
                            {
                                excludeSet.Add(e);
                            }
                        }
                        FindWidest(args[2][0], sw, excludeSet);
                    }
                }
                Process.Start(args[0]);
                // TODO ...
            }
            catch (Exception e)
            {
                Console.WriteLine($"The appliction encountered an error:\n {e}");
            }
        }
    }
}
