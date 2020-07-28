using System;
using System.Collections.Generic;
using System.IO;

namespace Navigator
{
    class FileSystemVisitor
    {
        //public event Action OnFileFound { add; remove; }
        //public event Action OnDirectoryFound { add; remove; }

        public List<string> GetFiles(Func<FileSystemInfo, bool> condition, FileSystemInfo dirFiles)
        {
            var result = new List<string>();
            //logic for looping through files
            try
            {
                //GetFiles(dirFiles => condition(dirFiles))
                foreach (string d in Directory.GetDirectories(dirFiles.ToString()))
                {
                    if (condition(new DirectoryInfo(d)))
                    {
                        result.Add(d);
                        //OnDirectoryFound();
                        Console.WriteLine(d);
                    }
                    GetFiles(d => condition(d), new DirectoryInfo(d));
                }
                foreach (string f in Directory.GetFiles(dirFiles.ToString()))
                {
                    if (condition(new FileInfo(f)))
                    {
                        result.Add(f);
                            //OnFileFound(); 
                        Console.WriteLine(f);
                    }
                }
                    
                
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return result;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var visitor = new FileSystemVisitor();
            //visitor.OnFileFound += () => Console.WriteLine("file found");
            var result = visitor.GetFiles(f => f.Name.Length > 5, new DirectoryInfo(args[0]));
        }
    }
}
