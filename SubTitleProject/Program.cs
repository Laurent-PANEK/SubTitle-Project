using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubTitleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            SubtitleFile f = new SubtitleFile();

            f.ReadFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\srt.txt");


            f.GetSubTitle();

            Console.Read();
        }
    }
}
