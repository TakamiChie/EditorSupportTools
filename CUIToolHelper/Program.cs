using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TakamiChie.CUIToolHelper
{
    /// <summary>
    /// CUIアプリケーションをGUIアプリから呼び出すためのヘルパーツール
    /// </summary>
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                ProcessStartInfo pi = new ProcessStartInfo(args[0]);
                if (args.Length > 1) { pi.Arguments = string.Join(" ", args.Skip(1)); }
                pi.RedirectStandardOutput = true;
                pi.RedirectStandardError = true;
                pi.UseShellExecute = false;
                var oe = false;
                var ee = false;
                var displaymsg = "";
                try
                {

                    // プロセス実行
                    var proc = Process.Start(pi);
                    proc.WaitForExit();
                    var output = proc.StandardOutput.ReadToEnd();
                    var errors = proc.StandardError.ReadToEnd();
                    oe = output != "";
                    ee = errors != "";
                    displaymsg = oe ?
                                        (ee ? output + "\nエラー：" + errors : output) :
                                        (ee ? errors : "");
                }
                catch (Exception e)
                {
                    displaymsg = e.Message;
                }
                if (displaymsg != "")
                {
                    MessageBox.Show(displaymsg, oe ? "情報" : "エラー", MessageBoxButtons.OK,
                        oe ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("CUIToolHelper ToolApp [ToolOption...]");
            }


        }
    }
}
