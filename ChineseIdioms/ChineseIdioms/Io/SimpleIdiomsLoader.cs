using System.Collections.Generic;
using System.IO;

namespace ChineseIdioms.Io
{
    /// <summary>
    ///  This is works with the specific dictionary in the current IdiomsData\chineseidioms.txt
    /// </summary>
    public class SimpleIdiomsLoader
    {
        public IEnumerable<string> GetAllIdioms()
        {
            using (var fs = new FileStream(@"IdiomsData\chineseidioms.txt", FileMode.Open))
            using (var sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Contains("Ａ　～　Ｂ "))
                    {
                        break;
                    }
                }
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Contains("我国历代公文体制、名称、用途简介"))
                    {
                        break;
                    }
                    var start = line.IndexOf('【');
                    var end = line.IndexOf('】', start + 1);
                    if (start >= 0 && end < line.Length)
                    {
                        yield return line.Substring(start + 1, end - start - 1);
                    }
                }
            }
        }
    }
}
