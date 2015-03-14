using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;
namespace TakamiChie.GitExecutor
{
    /// <summary>
    /// プログラムメイン。
    /// もしGitリポジトリのディレクトリおよびファイルパスを指定した場合、
    /// GitExtensionsを探して、コミットする
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var filepath = args.Length >= 1 ? args[0] : "";
            var command = args.Length >= 2 ? args[1] : "commit";
            var path_gitext = "";
            using(var key = Registry.CurrentUser.CreateSubKey(@"Software\GitExtensions")){
                // Git Extensionsのパス取得
                path_gitext = Path.Combine(key.GetValue("InstallDir", "").ToString(), "GitExtensions.exe");
            }
            if(File.Exists(path_gitext)){
                var dir = GetRepositoryDir(filepath);
                if (dir != null)
                {
                    Process.Start(path_gitext, string.Format("{0} \"{1}\"", command, dir));
                }else{
                    // リポジトリルートが見つからなかった
                    Console.WriteLine("GitExecutor directoryname(In Repositry of Git) [GitExtension Option]");
                }
            }else{
                // Git Extensions未インストール
                Console.Error.WriteLine("GitExtensionsが見つかりませんでした。このツールの実行にはGitExtensionsのインストールが必要です");
            }
        }

        /// <summary>
        /// リポジトリルートディレクトリを取得する
        /// </summary>
        /// <param name="filepath">検索対象のファイルまたはディレクトリパス</param>
        /// <returns>リポジトリルートディレクトリ。もしリポジトリで無かった場合、null</returns>
        public static object GetRepositoryDir(string filepath)
        {
            var dirname = "";
            if(File.Exists(filepath)){
                dirname = Path.GetDirectoryName(Path.GetFullPath(filepath));
            }else if(Directory.Exists(filepath)){
                dirname = Path.GetFullPath(filepath);
            }else{
                dirname = "";
            }
            var founded = false;
            if(Directory.Exists(dirname)){
                // 指定したディレクトリがある→リポジトリルート検索
                var root = Directory.GetDirectoryRoot(dirname);
                while ( root != dirname )
                {
                    if(Directory.Exists(Path.Combine(dirname, ".git")))
                    {
                        // パスを見つけた
                        founded = true;
                        break;
                    }
                    dirname = Directory.GetParent(dirname).FullName;
                }
            }
            return founded ? dirname : null;
        }
    }
}
