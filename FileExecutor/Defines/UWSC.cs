using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakamiChie.FileExecutor.Defines
{
    [DefaultFileExt(".uws")]
    class UWSC : BasicFileType
    {
        protected override string executor
        {
            get { return "UWSC"; }
        }
    }
}
