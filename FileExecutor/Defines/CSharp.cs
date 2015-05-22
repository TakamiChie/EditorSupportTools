using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakamiChie.FileExecutor.Defines
{
    [DefaultFileExt(".cs")]
    [AlternateFileType("C#")]
    class CSharp : CompiledFileType
    {
        protected override string compiler
        {
            get { return "csc"; }
        }

        protected override bool Compile(out string stdout, out string stderr, out string executable, string args, string input)
        {
            executable = Path.ChangeExtension(this.filename, ".exe");
            String unused;
            return base.Compile(out stdout, out stderr, out unused,
                string.Format("/t:exe /nologo /out:\"{0}\" {1}", executable, this.filename), input);
        }
    }
}