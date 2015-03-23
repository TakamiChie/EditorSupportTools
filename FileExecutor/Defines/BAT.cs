using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakamiChie.FileExecutor.Defines
{
    [DefaultFileExt(".bat")]
    class BAT : FileType
    {
        public override int Execute(out string stdout, out string stderr, string fileName, string args, string input)
        {
            var oldfn = fileName;
            var newfn = "";
            if (Path.GetExtension(oldfn) == ".tmp")
            {
                // 新規ファイル
                newfn = Path.ChangeExtension(fileName, ".bat");
                if (File.Exists(newfn)) File.Delete(newfn);
                File.Move(oldfn, newfn);
            }
            else
            {
                // ファイルを移動しない
                newfn = '"' + oldfn + '"';
            }

            var exitcode = -1;
            // 準備
            var pi = new ProcessStartInfo(newfn, args);
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

        public override int Execute(out string stdout, out string stderr, string args, string input)
        {
            throw new NotImplementedException();
        }
    }
}
