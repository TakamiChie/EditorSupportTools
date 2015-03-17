using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TakamiChie.FileExecutor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /// コマンドラインオプション解析
            // http://neue.cc/2009/12/13_229.html        }
            var options = new HashSet<string> { "/file", "/type", "/opt" };

            string key = null;
            var result = args
                .GroupBy(s => options.Contains(s) ? key = s : key) // 副作用！
                .ToDictionary(g => g.Key, g => g.Skip(1).FirstOrDefault());

            var filename = result.ContainsKey("/file") ? result["/file"] : "";
            var filetype = result.ContainsKey("/type") ? result["/type"] : null;
            var fileoptions = result.ContainsKey("/opt") ? filename + " " + result["/opt"] : filename;
            
            if (File.Exists(filename))
            {
                var type = FindFileType(filename, filetype);
                if (type != null)
                {
                    string stdout;
                    string stderr;
                    type.Execute(out stdout, out stderr, fileoptions, null);
                    Console.Out.Write(stdout);
                    Console.Error.Write(stderr);
                }
            }
            else
            {
                // 存在するファイルが指定されていない
                Console.WriteLine("FileExecutor /file Filename [/opt options] [/type filetype]");
            }
        }

        /// <summary>
        /// 拡張子及びファイルタイプから、合致するファイルタイプを検索し、返す。
        /// </summary>
        /// <param name="filename">ファイル名</param>
        /// <param name="filetype">ファイルタイプ</param>
        /// <returns>ファイルタイプを示すクラス。無い場合はnull</returns>
        public static FileType FindFileType(string filename, string filetype)
        {
            string fileext = Path.GetExtension(filename);
            var asm = Assembly.GetExecutingAssembly();
            // 将来的にファイルタイプの検索方法を増やす可能性があるため、一律でSelect
            var t = asm.GetTypes().Where(tp =>
            {
                var found = false;
                if (filetype != null)
                {
                    // ファイルタイプが指定されている＝ファイルタイプ名に合致するファイルタイプを探す
                    if (tp.FullName == "TakamiChie.FileExecutor.Defines." + filetype)
                    {
                        found = true;
                    }
                }
                else
                {
                    // ファイルタイプが指定されていない＝拡張子を検索
                    var attr = tp.GetCustomAttribute<DefaultFileExtAttribute>();
                    if (attr != null && attr.Extensions.Contains(fileext))
                    {
                        found = true;
                    }
                }
                return found;
            });

            var ret = t.FirstOrDefault();
            return ret == null ? null : Activator.CreateInstance(ret) as FileType;
        }
    }
}
