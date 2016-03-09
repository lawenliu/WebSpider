using System;
using System.IO;

namespace WebSpider.WebCrawl
{
    public class FileOperator
    {
        public static void WriteFile(string fileFolder, string fileName, string content)
        {
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }

            String filePath = fileFolder + "\\" + fileName;
            StreamWriter sw = new StreamWriter(filePath, true);
            sw.Write(content);
            sw.WriteLine();
            sw.Close();
        }
    }
}
