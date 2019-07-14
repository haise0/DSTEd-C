using DSTEd.Core.IO.EnumerableFileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace DSTEd.Test.IO.EnumerableFileSystem
{
	[TestClass]
	public class FSUtilTest
	{
		[TestMethod]
		public void GetFileList()
		{
			RecursiveDirectoryIterator iter = new RecursiveDirectoryIterator(@".\FileSystemTest");
			Debug.WriteLine(iter.OriginalDirectoryInfo.FullName);
			FileInfo[] except = 
			{
				new FileInfo(@".\FileSystemTest\File1.txt"),
				new FileInfo(@".\FileSystemTest\File2.txt"),
				new FileInfo(@".\FileSystemTest\Directory1\File1.txt"),
				new FileInfo(@".\FileSystemTest\Directory1\File2.txt"),
				new FileInfo(@".\FileSystemTest\Directory1\.DotFolderTest\File1.txt"),
				new FileInfo(@".\FileSystemTest\Directory1\.DotFolderTest\File2.txt")
			};
			//CollectionAssert.AreEquivalent(except, iter, "iter not equivent to excepted collection");
			for(int i = 0; i<6; i++)
			{
				string except_fullpath = except[i].FullName;
				string actual_fullpath = iter[i].FullName;
				Assert.AreEqual(except_fullpath, actual_fullpath, "\nexcxept:{0}\nActual{1}", except_fullpath, actual_fullpath);
			}
		}

		[TestMethod]
		public void CopyDirectoryTest()
		{
			FileInfo[] except =
			{
				new FileInfo(@".\FileSystemCopyTest\File1.txt"),
				new FileInfo(@".\FileSystemCopyTest\File2.txt"),
				new FileInfo(@".\FileSystemCopyTest\Directory1\File1.txt"),
				new FileInfo(@".\FileSystemCopyTest\Directory1\File2.txt"),
				new FileInfo(@".\FileSystemCopyTest\Directory1\.DotFolderTest\File1.txt"),
				new FileInfo(@".\FileSystemCopyTest\Directory1\.DotFolderTest\File2.txt")
			};
			Directory.Delete(@".\FileSystemCopyTest", true);
			RecursiveDirectoryIterator actual = FSUtil.CopyDirectory(new DirectoryInfo(@".\FileSystemTest\"),
				new DirectoryInfo(@".\FileSystemCopyTest\"));

			for (int i = 0; i < 6; i++)
			{
				string except_fullpath = except[i].FullName;
				string actual_fullpath = actual[i].FullName;
				Assert.AreEqual(except_fullpath, actual_fullpath, "\nexcxept:{0}\nActual{1}", except_fullpath, actual_fullpath);
			}
		}

		[TestMethod]
		public void RelativeTest()
		{
			string _base = "\\dir\\base";
			string path1 = "\\dir\\base\\file";
			string path2 = "\\dir\\file";
			string path3 = "\\file";

			string path4 = "\\dir\\base\\sub\\directory\\file";

			//common relative
			Assert.AreEqual("\\file", FSUtil.Relative(_base, path1),"base->path1");
			Assert.AreEqual("\\sub\\directory\\file", FSUtil.Relative(_base, path4), "base->path4");

			//a little rare?
			Assert.AreEqual("\\..", FSUtil.Relative(path1, _base),"path1->base");
			Assert.AreEqual("\\..\\..\\..", FSUtil.Relative(path4, _base), "path4->base");

			//more rare
			Assert.AreEqual("..\\file", FSUtil.Relative(_base, path2), "base->path2");
			Assert.AreEqual("..\\..\\file", FSUtil.Relative(_base, path3), "base->path3");
		}
	}
}
