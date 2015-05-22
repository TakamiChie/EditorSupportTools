using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakamiChie.FileExecutor
{
    /// <summary>
    /// コンパイラを使用するファイルタイプの基底クラスです。
    /// </summary>
    abstract class CompiledFileType : BasicFileType
    {
        /// <summary>
        /// コンパイラファイル名
        /// </summary>
        protected abstract string compiler { get; }

        protected override string executor
        {
            get { return null; }
        }

        public override int Execute(out string stdout, out string stderr, string args, string input)
        {
            var exitcode = -1;
            string outputfile;
            if (Compile(out stdout, out stderr, out outputfile, args, input))
            {
                this.filename = outputfile;
                string tstdout;
                string tstderr;
                tstdout = stdout;
                tstderr = stderr;
                exitcode = base.Execute(out stdout, out stderr, args, input);
                if (tstdout != "") stdout = tstdout + "\n" + stdout;
                if (tstderr != "") stderr = tstderr + "\n" + stderr;
            }
            return exitcode;
        }

        /// <summary>
        /// コンパイル処理を実行します。
        /// </summary>
        /// <param name="stdout">標準出力が格納される文字列変数</param>
        /// <param name="stderr">標準エラーが格納される文字列変数</param>
        /// <param name="executable">出力された実行ファイルパスを格納する文字列変数。本メソッド内では値を変更しません。</param>
        /// <param name="args">引数</param>
        /// <param name="input">標準入力</param>
        /// <returns>処理が成功したかどうか</returns>
        protected virtual bool Compile(out string stdout, out string stderr, out string executable, string args, string input)
        {
            var compiler = findExecutablePath("COMPILER", this.GetType().Name, this.compiler);
            executable = compiler;
            var success = false;
            if (compiler != null)
            {
                success = processExecute(out stdout, out stderr, compiler, args, input) == 0;
            }
            else
            {
                stdout = "";
                stderr = "コンパイラが見つかりませんでした";
            }
            return success;
        }

    }
}