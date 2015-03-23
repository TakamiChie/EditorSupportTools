using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakamiChie.FileExecutor.Defines
{
    [DefaultFileExt(".lua")]
    class Lua : BasicFileType
    {
        protected override string executor
        {
            get { return "lua"; }
        }
    }
}
