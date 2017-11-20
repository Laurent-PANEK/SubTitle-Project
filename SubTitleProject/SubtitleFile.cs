using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubTitleProject
{
    public class SubtitleFile
    {
        public enum StatusLine { NumberLine, TimerLine, SubTitleLine, EmptyLine };
        public List<string> allLineOfFile;
        public List<SubTitle> allSubTitle;
        public string path;

        public SubtitleFile()
        {
        }

        public void ReadFile(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    allLineOfFile = new List<string>();

                    string line;

                    while ((line = sr.ReadLine()) != null)
                        allLineOfFile.Add(line);

                    ParseFile();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void ParseFile()
        {
            int nbSub = -1;
            string Timer = "";
            string Subs = "";
            StatusLine statusL = StatusLine.NumberLine;
            allSubTitle = new List<SubTitle>();

            foreach (string line in allLineOfFile)
            {
                if (line == "")
                {
                    statusL = StatusLine.EmptyLine;
                }

                switch (statusL)
                {
                    case StatusLine.NumberLine:
                        nbSub = Int32.Parse(line);
                        statusL++;
                        break;
                    case StatusLine.TimerLine:
                        Timer = line;
                        statusL++;
                        break;
                    case StatusLine.SubTitleLine:
                        Subs += line+"\n";
                        break;
                    case StatusLine.EmptyLine:
                        SubTitle sub = new SubTitle(nbSub, Timer, Subs);
                        if (!allSubTitle.Contains(sub))
                            allSubTitle.Add(sub);
                        statusL = StatusLine.NumberLine;
                        Subs = "";
                        break;
                }
            }
        }

        public void GetSubTitle()
        {
            foreach (SubTitle sub in allSubTitle)
            {
                Task r = sub.AddSubTitle();
            }
        }
        
    }
}
