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
    class BAT : BasicFileType
    {
        protected override string executor
        {
            get { return null; }
        }
        public override int Execute(out string stdout, out string stderr, string args, string input)
        {
            var oldfn = filename;
            var newfn = "";
            if (Path.GetExtension(oldfn) == ".tmp")
            {
                // 新規ファイル
                newfn = Path.ChangeExtension(filename, ".bat");
                if (File.Exists(newfn)) File.Delete(newfn);
                File.Move(oldfn, newfn);
            }
            else
            {
                // ファイルを移動しない
                newfn = '"' + oldfn + '"';
            }

            return base.Execute(out stdout, out stderr, args, input);
        }
    }
}
