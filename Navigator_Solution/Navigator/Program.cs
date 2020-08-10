using System;
using System.Collections.Generic;
using System.IO;

namespace Navigator
{
    class FileSystemVisitor
    {
        //public event Action OnFileFound { add; remove; }
        //public event Action OnDirectoryFound { add; remove; }
        public event Action<FileSystemInfo> OnFileFound;// { add; remove; }
        public event Action<FileSystemInfo> OnDirectiryFound;

        //public delegate void DoSomething(int a); ==Action<int>
        //public delegate int DoSomething(int a); ==Func<int, int>


        public List<string> GetFiles(Func<FileSystemInfo, bool> condition, FileSystemInfo dirFiles)
        {
            var result = new List<string>();
            if (stop) return result;
            //logic for looping through files
            try
            {
                //GetFiles(dirFiles => condition(dirFiles))
                foreach (string d in Directory.GetDirectories(dirFiles.ToString()))
                {
                    var dir = new DirectoryInfo(d);
                    if (condition(dir))
                    {
                        result.Add(d);
                        OnDirectiryFound?.Invoke(dir);
                    }
                    var r = GetFiles(d => condition(d), dir);
                    result.AddRange(r);
                }
                foreach (string f in Directory.GetFiles(dirFiles.ToString()))
                {
                    var fi = new FileInfo(f);
                    if (condition(fi))
                    {
                        result.Add(f);
                        OnFileFound(fi); 
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

            visitor.OnFileFound += (fi) => {
                Console.WriteLine();
                if (deep == 5)
                {
                    visitor.stop = true;
                }
                
            };
            var result = visitor.GetFiles(f => f.Name.Length > 5, new DirectoryInfo(args[0]));
        }

        static void Print(FileSystemInfo fi)
        {

        }
    }
}
