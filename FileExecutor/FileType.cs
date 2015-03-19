using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TakamiChie.FileExecutor
{
    /// <summary>
    /// ファイルタイプ定義クラスの基本クラスです。
    /// </summary>
    public abstract class FileType
    {
        /// <summary>
        /// ファイルを実行します。
        /// </summary>
        /// <param name="stdout">標準出力を格納する変数</param>
        /// <param name="stderr">標準エラーを格納する変数</param>
        /// <param name="args">引数</param>
        /// <param name="input">標準入力の文字列</param>
        /// <returns>実行コード</returns>
        public virtual int Execute(out string stdout, out string stderr, string fileName, string args, string input)
        {
            return Execute(out stdout, out stderr, fileName + " " + args, input);
        }

        /// <summary>
        /// ファイルを実行します。
        /// </summary>
        /// <param name="stdout">標準出力を格納する変数</param>
        /// <param name="stderr">標準エラーを格納する変数</param>
        /// <param name="args">引数</param>
        /// <param name="input">標準入力の文字列</param>
        /// <returns>実行コード</returns>
        public abstract int Execute(out string stdout, out string stderr, string args, string input);
    }

    /// <summary>
    /// 実行ファイルの名前を指定し直接実行するだけの、ごく基本的なファイルタイプの基本クラスです。
    /// </summary>
    public abstract class BasicFileType: FileType
    {

        [DllImport("KERNEL32.DLL")]
        public static extern uint
          GetPrivateProfileString(string lpAppName,
          string lpKeyName, string lpDefault,
          StringBuilder lpReturnedString, uint nSize,
          string lpFileName);
        [DllImport("KERNEL32.DLL")]
        public static extern uint WritePrivateProfileString(
          string lpAppName,
          string lpKeyName,
          string lpString,
          string lpFileName);

        protected static string SETFILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.ini");

        /// <summary>
        /// 実行アプリケーションのファイル名を取得します。
        /// </summary>
        protected abstract string executor { get; }

        /// <summary>
        /// 実行アプリケーションのファイル名を取得します。
        /// もしファイルが発見できなかった場合、ファイルの選択ダイアログを表示します。
        /// </summary>
        /// <returns>ファイルパス</returns>
        protected virtual string getExecutor()
        {
            var r = executor;
            // 環境変数を探してファイルは存在するかどうかチェック
            var found = Environment.GetEnvironmentVariable("PATH").Split(';')
                 .Any(p => File.Exists(Path.Combine(p, r)) ||
                         Environment.GetEnvironmentVariable("PATHEXT").Split(';')
                     .Any(x => File.Exists(Path.Combine(p, Path.ChangeExtension(r, x)))));
            if (!found)
            {
                // ファイルが存在しなかったので探す
                var typeName = this.GetType().Name;
                StringBuilder sb = new StringBuilder(1024);
                GetPrivateProfileString("PATH", typeName, "", sb, Convert.ToUInt32(sb.Capacity), SETFILE);
                var fPath = sb.ToString();
                if (File.Exists(fPath))
                {
                    // INIファイルに設定があったらそれを使う
                    r = fPath;
                }
                else
                {
                    // ファイルを開くダイアログを表示
                    var ofn = new OpenFileDialog();
                    ofn.FileName = r;
                    ofn.Title = r + "が見つかりません。指定してください";
                    ofn.RestoreDirectory = true;
                    if (ofn.ShowDialog() == DialogResult.OK)
                    {
                        WritePrivateProfileString("PATH", typeName, ofn.FileName, SETFILE);
                        r = ofn.FileName;
                    }
                    else
                    {
                        r = null;
                    }
                }
            }
            return r;
        }

        public override int Execute(out string stdout, out string stderr, string args, string input)
        {
            var exitcode = -1;
            var exec = getExecutor();
            if (exec != null)
            {
                // 準備
                var pi = new ProcessStartInfo(exec, args);
                pi.UseShellExecute = false;
                pi.RedirectStandardInput = true;
                pi.RedirectStandardOutput = true;
                pi.RedirectStandardError = true;
                // 実行
                var proc = Process.Start(pi);
                if (input != null) proc.StandardInput.Write(input);
                proc.WaitForExit();
                stdout = proc.StandardOutput.ReadToEnd();
                stderr = proc.StandardError.ReadToEnd();
                exitcode = proc.ExitCode;
            }
            else
            {
                stdout = "";
                stderr = "実行ファイルが見つかりませんでした";
            }
            return exitcode;
        }

    }

    /// <summary>
    /// ファイルタイプの拡張子を示す属性です。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultFileExtAttribute : Attribute
    {
        /// <summary>
        /// 拡張子のリスト。
        /// </summary>
        public string[] Extensions;

        /// <summary>
        /// ファイルタイプの拡張子を指定します。
        /// </summary>
        /// <param name="extensions">拡張子(.から)</param>
        public DefaultFileExtAttribute(params string[] extensions) { this.Extensions = extensions; }
    }

    /// <summary>
    /// 別のファイルタイプ呼称を定義するための属性です。
    /// </summary>
    public class AlternateFileTypeAttribute : Attribute
    {
        /// <summary>
        /// ファイルタイプ名のリスト
        /// </summary>
        public string[] Type;

        /// <summary>
        /// ファイルタイプの湖沼を指定します。
        /// </summary>
        /// <param name="type">ファイルタイプ名称</param>
        public AlternateFileTypeAttribute(params string[] type) { this.Type = type; }
    }
}
