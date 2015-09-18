using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TakamiChie.CommandRunner
{
  class Program
  {
    static void Main(string[] args)
    {
      /// コマンドラインオプション解析
      // http://neue.cc/2009/12/13_229.html        }
      var options = new HashSet<string> { "/file" };

      string key = null;
      var result = args
          .GroupBy(s => options.Contains(s) ? key = s : key)
          .ToDictionary(g => g.Key, g => g.Skip(1).FirstOrDefault());

      try
      {

        var path = String.Empty;
        if (result.TryGetValue("/file", out path))
        {
          if (!Path.IsPathRooted(path))
          {
            path = Path.Combine(System.Environment.CurrentDirectory, path);
          }
          var manager = new CommandManager(path);
          if (manager.hasKey("run"))
          {
            // プロセス起動準備
            var procinfo = new ProcessStartInfo(manager.getPath("run"));
            procinfo.Arguments = manager.hasKey("args") ? manager.getExtractVar("args") : "";
            procinfo.CreateNoWindow = true;
            procinfo.RedirectStandardOutput = true;
            procinfo.RedirectStandardError = true;
            procinfo.UseShellExecute = false;
            var stdout = "";
            var stderr = "";
            try
            {
              // 起動
              var proc = Process.Start(procinfo);
              stdout = proc.StandardOutput.ReadToEnd();
              stderr = proc.StandardError.ReadToEnd();

              // 結果表示
              switch (manager.hasKey("out") ? manager.get("out") : "console")
              {
                case "file":
                  // ファイル出力
                  if (manager.hasKey("outfile"))
                  {
                    using (var stream = new StreamWriter(manager.getExtractVar("outfile")))
                    {
                      stream.Write(stdout);
                    }
                  }
                  else
                  {
                    throw new InvalidDataException("outfileコマンドが見つかりません");
                  }
                  break;
                case "console":
                  Console.Out.Write(stdout);
                  break;
                case "dialog":
                  MessageBox.Show(stdout);
                  break;
              }
            }
            catch (Exception e)
            {
              stderr = e.Message;
            }
            if (stderr != "")
            {
              Console.Error.Write(stderr);
            }

            // メッセージ表示
            if (manager.hasKey("console"))
            {
              Console.Write(manager.getExtractVar("console"));
            }

            if (manager.hasKey("dialog"))
            {
              MessageBox.Show(manager.getExtractVar("dialog"));
            }
          }
        }
        else
        {
          throw new InvalidOperationException("Command Line Error:\nCommandRunner /file [FileName]");
        }
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e.Message);
      }
    }
  }
}
