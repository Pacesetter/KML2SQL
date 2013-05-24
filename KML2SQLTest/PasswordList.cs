using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KML2SQLTest
{
    public class PasswordList : List<string>
    {
        public PasswordList()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            StreamReader reader = new StreamReader(dir + "\\passwords.txt");
            string currentLine;
            while (!reader.EndOfStream)
            {
                currentLine = reader.ReadLine();
                this.Add(currentLine);
            }
        }
    }
}
