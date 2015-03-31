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
        [DllImport("KERNEL32.DLL")]
        private static extern uint
          GetPrivateProfileString(string lpAppName,
          string lpKeyName, string lpDefault,
          StringBuilder lpReturnedString, uint nSize,
          string lpFileName);
        [DllImport("KERNEL32.DLL")]
        private static extern uint WritePrivateProfileString(
          string lpAppName,
          string lpKeyName,
          string lpString,
          string lpFileName);

        private static string SETFILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.ini");
        protected string filename { get; set; }
        protected string arguments { get; set; }

        public void Init(string filename, string fileoptions)
        {
            this.filename = filename;
            this.arguments = fileoptions;
        }

        #region オーバーライド用メソッド

        /// <summary>
        /// ファイルを実行します。
        /// </summary>
        /// <param name="stdout">標準出力を格納する変数</param>
        /// <param name="stderr">標準エラーを格納する変数</param>
        /// <param name="args">引数</param>
        /// <param name="input">標準入力の文字列</param>
        /// <returns>実行コード</returns>
        public abstract int Execute(out string stdout, out string stderr, string args, string input);

        #endregion

        #region ユーティリティメソッド

        /// <summary>
        /// 指定した実行ファイルを環境変数PATHに定義したパス内から検索し、見つかればそのパスを返します。
        /// 見つからなかった場合、INIファイルに既にパスが記録されていればそれを、なければファイルを開くダイアログを表示して、その結果を返します。
        /// </summary>
        /// <param name="iniSection">INIファイルのセクション</param>
        /// <param name="iniKey">INIファイルのキー</param>
        /// <param name="executableName">実行ファイルの名称</param>
        /// <returns>実行ファイルのパス。ダイアログがキャンセルされたなどの要因により、ファイルパスが見つからなかった場合はnull</returns>
        protected string findExecutablePath(string iniSection, string iniKey, string executableName)
        {
            var r = executableName;
            // 環境変数を探してファイルは存在するかどうかチェック
            var found = Environment.GetEnvironmentVariable("PATH").Split(';')
                .Select(x => Path.Combine(x, r))
                .SelectMany(_ => Environment.GetEnvironmentVariable("PATHEXT").Split(';')
                    .Concat(new String[] { Path.GetExtension(r) }),
                        (p, ext) => Path.ChangeExtension(p, ext))
                .Where(File.Exists).FirstOrDefault();
            if (found == null)
            {
                // ファイルが存在しなかったので探す
                var typeName = this.GetType().Name;
                var fPath = getSetting("PATH", typeName, "");
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
                        setSetting("PATH", typeName, ofn.FileName);
                        r = ofn.FileName;
                    }
                    else
                    {
                        r = null;
                    }
                }
            }
            else
            {
                r = found;
            }
            return r;

        }

        /// <summary>
        /// 設定をファイルに書き込みます。
        /// </summary>
        /// <param name="section">設定のセクション</param>
        /// <param name="key">設定キー</param>
        /// <param name="defaultValue"デフォルト値></param>
        /// <returns>読み込まれた値もしくは、デフォルト値</returns>
        protected string getSetting(string section, string key, string defaultValue)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, defaultValue, sb, Convert.ToUInt32(sb.Capacity), SETFILE);
            return sb.ToString();
        }

        /// <summary>
        /// 設定をファイルから読み込みます
        /// </summary>
        /// <param name="section">設定のセクション</param>
        /// <param name="key">設定キー</param>
        /// <param name="value">設定する値</param>
        protected void setSetting(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, SETFILE);
        }

        /// <summary>
        /// 任意のプロセスを実行し、結果を返却します。
        /// </summary>
        /// <param name="stdout">標準出力が格納される文字列変数</param>
        /// <param name="stderr">標準エラーが格納される文字列変数</param>
        /// <param name="executable">実行ファイル</param>
        /// <param name="args">引数</param>
        /// <param name="input">標準入力</param>
        /// <returns>終了コード</returns>
        protected virtual int processExecute(out string stdout, out string stderr, string executable, string args, string input)
        {
            var exitcode = -1;
            // 準備
            var pi = new ProcessStartInfo(executable, args);
            pi.UseShellExecute = false;
            pi.RedirectStandardInput = true;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;
            // 実行
            try
            {
                var proc = Process.Start(pi);
                if (input != null) proc.StandardInput.Write(input);
                proc.WaitForExit();
                stdout = proc.StandardOutput.ReadToEnd();
                stderr = proc.StandardError.ReadToEnd();
                exitcode = proc.ExitCode;
            }
            catch (Exception e)
            {
                stdout = "";
                stderr = e.Message;
                exitcode = -1;
            }
            return exitcode;
        }

        #endregion
    }

    /// <summary>
    /// 実行ファイルの名前を指定し直接実行するだけの、ごく基本的なファイルタイプの基本クラスです。
    /// </summary>
    public abstract class BasicFileType: FileType
    {

        /// <summary>
        /// 実行アプリケーションのファイル名を取得・設定します。
        /// </summary>
        protected abstract string executor { get; }

        public override int Execute(out string stdout, out string stderr, string args, string input)
        {
            var exitcode = -1;
            var exec = executor == null ? this.filename : findExecutablePath("PATH", this.GetType().Name, executor);
            if (exec != null)
            {
                var argument = executor == null ? args : string.Format("\"{0}\" {1}", filename, args);
                processExecute(out stdout, out stderr, exec, argument, input);
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
