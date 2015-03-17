using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public abstract int Execute(out string stdout, out string stderr, string args, string input);
    }

    /// <summary>
    /// 実行ファイルの名前を指定し直接実行するだけの、ごく基本的なファイルタイプの基本クラスです。
    /// </summary>
    public abstract class BasicFileType: FileType
    {
        /// <summary>
        /// 実行アプリケーションのファイル名を取得します。
        /// </summary>
        protected abstract string executor { get; }

        public override int Execute(out string stdout, out string stderr, string args, string input)
        {
            // 準備
            var pi = new ProcessStartInfo(executor, args);
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
            return proc.ExitCode;
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
}
