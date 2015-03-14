using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TakamiChie.GitExecutor;

namespace GitExecutorTest
{
    class Test
    {
        static void Main(string[] args)
        {
            var apppath = Application.ExecutablePath;
            Console.WriteLine("ファイルパスを指定した場合:" + apppath);
            Console.WriteLine(Program.GetRepositoryDir(apppath));

            Console.WriteLine("ディレクトリパスを指定した場合:" + Path.GetDirectoryName(apppath));
            Console.WriteLine(Program.GetRepositoryDir(Path.GetDirectoryName(apppath)));

            Console.WriteLine("リポジトリでないフォルダを指定した場合:" + @"C:\Windows");
            Console.WriteLine(Program.GetRepositoryDir(@"C:\Windows"));

            Console.WriteLine("リポジトリでないフォルダを指定した場合(パスの最後が\\):" + @"C:\Windows\");
            Console.WriteLine(Program.GetRepositoryDir(@"C:\Windows\"));

            Console.WriteLine("相対パスを指定した場合:" + @".");
            Console.WriteLine(Program.GetRepositoryDir(@"."));

            Console.WriteLine("なんでもない文字列を指定した場合:" + @"abcdefg");
            Console.WriteLine(Program.GetRepositoryDir(@"abcdefg"));

            Console.ReadLine();
        }
    }
}
