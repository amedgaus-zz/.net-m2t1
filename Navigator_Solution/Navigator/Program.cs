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
        public event Action<FileSystemInfo> OnDirectoryFound;
        public event Action<String> OnSearchStart;
        public event Action<String> OnSearchFinish;
        public bool Stop { get; set; } = false;
        public bool Exclude { get; set; } = false;
        //public delegate void DoSomething(int a); ==Action<int>
        //public delegate int DoSomething(int a); ==Func<int, int>

        //FileSystemVisitor(FileSystemBlaBla fs)
        //{
        //    this.fs = fs;
        //}

        public List<string> GetFiles(Func<FileSystemInfo, bool> condition, FileSystemInfo dirFiles)
        {
            var result = new List<string>();
            //logic for looping through files
            try
            {
                OnSearchStart?.Invoke("Search has started");
                foreach (string d in Directory.GetDirectories(dirFiles.ToString()))
                {
                    if (Stop) return result;
                    var dir = new DirectoryInfo(d);
                    if (condition(dir))
                    {
                        if (!Exclude)
                            result.Add(d);
                        Exclude = false;
                        OnDirectoryFound?.Invoke(dir);
                    }
                    var r = GetFiles(d => condition(d), dir);
                    result.AddRange(r);
                }
                foreach (string f in Directory.GetFiles(dirFiles.ToString()))
                {
                    if (Stop) return result;
                    var fi = new FileInfo(f);
                    if (condition(fi))
                    {
                        if (!Exclude)
                            result.Add(f);
                        OnFileFound?.Invoke(fi); 
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            OnSearchFinish?.Invoke("Search has finished");
            return result;
        }
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            var visitor = new FileSystemVisitor();
            var counter = 0;
            visitor.OnFileFound += (fi) => {
                counter++;
                Console.WriteLine($"File "+fi+" found");
                if (counter >= 2)
                {
                    //Console.WriteLine($"*** File stop condition: deep=" + visitor.Deep + " ***");
                    //visitor.Stop = true;
                    if (fi.Name.Contains("setup"))
                    {
                        visitor.Exclude = true;
                    }
                }
                
            };
            visitor.OnDirectoryFound += (dir) => {
                counter++;
                Console.WriteLine($"Directory " + dir + " found");
                if (counter >= 20)
                {
                    //Console.WriteLine($"*** Directory stop condition: deep=" + visitor.Deep + " ***");
                    visitor.Stop = true;
                    if (dir.Name == "Test")
                        visitor.Exclude = true;
                }

            };
            visitor.OnSearchStart += Visitor_OnSearchStart;
            visitor.OnSearchFinish += Visitor_OnSearchFinish;
            var result = visitor.GetFiles(f => f.Name.Length > 5, new DirectoryInfo(args[0]));
        }

        private static void Visitor_OnSearchStart(String str)
        {
            Console.WriteLine(str);
            //throw new NotImplementedException();
        }

        private static void Visitor_OnSearchFinish(String str)
        {
            Console.WriteLine(str);
            //throw new NotImplementedException();
        }
    }
}