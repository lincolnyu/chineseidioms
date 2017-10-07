using ChineseIdioms.Io;
using ChineseIdioms.Linkage;
using System;
using System.IO;
using System.Text;

namespace ChineseIdiomsConsole
{
    class Program
    {
        static IdiomsLookup _idiomsLookup;

        static void InitIdiomRepo()
        {
            var loader = new SimpleIdiomsLoader();
            var idioms = loader.GetAllIdioms();
            _idiomsLookup = new IdiomsLookup();
            _idiomsLookup.Load(idioms);
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
                    else if (action == "next")
                    {

                    }
                }
                // TODO ...
            }
            catch (Exception e)
            {
                Console.WriteLine($"The appliction encountered an error:\n {e}");
            }
        }
    }
}
