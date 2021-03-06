﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakamiChie.FileExecutor.Defines
{
    [DefaultFileExt(".js")]
    [AlternateFileType("JScript")]
    public class Javascript : BasicFileType
    {
        protected override string executor
        {
            get { return "wscript"; }
        }

        /// <summary>
        /// 拡張子をtmp→jsに置き換える処理を追加します
        /// </summary>
        public override int Execute(out string stdout, out string stderr, string args, string input)
        {
            var oldfn = filename;
            var newfn = "";
            if (Path.GetExtension(oldfn) == ".tmp")
            {
                // 新規ファイル
                newfn = Path.ChangeExtension(filename, ".js");
                if (File.Exists(newfn)) File.Delete(newfn);
                File.Move(oldfn, newfn);
            }
            else
            {
                // ファイルを移動しない
                newfn = oldfn;
            }
            this.filename = newfn;

            return base.Execute(out stdout, out stderr, args, input);
        }
    }
}
