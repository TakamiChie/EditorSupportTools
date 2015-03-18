using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TakamiChie.FileExecutor.Defines;

namespace TakamiChie.FileExecutor.Test
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// 定義済みファイルタイプを拡張子で指定した場合、期待したファイルタイプが出力される？
        /// </summary>
        [TestMethod]
        public void IsExistFileTypeInExt()
        {
            Assert.AreEqual(
                typeof(Ruby),
                Program.FindFileType("null.rb", null).GetType());
        }

        /// <summary>
        /// 存在しないファイルタイプを拡張子で指定した場合、nullが出力される？
        /// </summary>
        [TestMethod]
        public void IsNonExistFileTypeInExt()
        {
            Assert.IsNull(
                Program.FindFileType("null.example", null));
        }

        /// <summary>
        /// 定義済みファイルタイプをクラス名で指定した場合、期待したファイルタイプが出力される？
        /// </summary>
        [TestMethod]
        public void IsExistFileTypeInClassName()
        {
            Assert.AreEqual(
                typeof(Ruby),
                Program.FindFileType("null.rb", "Ruby").GetType());
        }

        /// <summary>
        /// 定義済みファイルタイプをクラス名かつすべて小文字で指定した場合、期待したファイルタイプが出力される？
        /// </summary>
        [TestMethod]
        public void IsExistFileTypeInClassNameToLower()
        {
            Assert.AreEqual(
                typeof(Ruby),
                Program.FindFileType("null.rb", "ruby").GetType());
        }

        /// <summary>
        /// 存在しないファイルタイプをクラス名で指定した場合、nullが出力される？
        /// </summary>
        [TestMethod]
        public void IsNonExistFileTypeInClassName()
        {
            Assert.IsNull(
                Program.FindFileType("null.rb", "404"));
        }

        /// <summary>
        /// 定義済みファイルタイプを別名で指定した場合、期待したファイルタイプが出力される？
        /// </summary>
        [TestMethod]
        public void IsExistFileTypeInAlternateName()
        {
            Assert.AreEqual(
                typeof(Javascript),
                Program.FindFileType("null.rb", "JScript").GetType());
        }

        /// <summary>
        /// 定義済みファイルタイプを別名かつすべて小文字で指定した場合、期待したファイルタイプが出力される？
        /// </summary>
        [TestMethod]
        public void IsExistFileTypeInAlternateNameToLower()
        {
            Assert.AreEqual(
                typeof(Javascript),
                Program.FindFileType("null.rb", "jscript").GetType());
        }

        /// <summary>
        /// 存在しないファイルタイプを別名で指定した場合、nullが出力される？
        /// </summary>
        [TestMethod]
        public void IsNonExistFileTypeInAlternateName()
        {
            Assert.IsNull(
                Program.FindFileType("null.rb", "404"));
        }
    }
}
