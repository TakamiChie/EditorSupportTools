using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakamiChie.FileExecutor.Defines
{
    /// <summary>
    /// Rubyの実行クラス
    /// </summary>
    /// <remarks>
    /// テストで使うためPublicになってます。他のクラスはPublicにしなくても良いです
    /// </remarks>
    [DefaultFileExt(".rb")]
    public class Ruby : BasicFileType
    {
        protected override string executor
        {
            get { return "ruby"; }
        }
    }
}
