using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DSTEd.Core.ProjectManager.FileTypes
{
	[Serializable]
	internal class CodeFile : IDSTModFile
	{
		public CodeFile(DirectoryInfo basedir,FileInfo codeFileInfo)
		{
			if (codeFileInfo.Extension != ".lua" || !codeFileInfo.Exists)
				throw new ArgumentException(codeFileInfo.FullName + " isn't a code file");
			Path = codeFileInfo;
			RelativePath = IO.EnumerableFileSystem.FSUtil.Relative(basedir.FullName, codeFileInfo.FullName);
		}


		public int FileTypeID => 0;

		public string FileTypeName => "Code";

		public string RelativePath { get; private set; }

		public FileInfo Path { get; private set; }

		public void Build(DirectoryInfo Target)
		{
			File.Copy(Path.FullName, Target.FullName + RelativePath, true);
		}

		/// <summary>
		/// serialize,system uses
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("typeId", 0, typeof(int));
			info.AddValue("typeName", "Code", typeof(string));
			info.AddValue("relativePath", RelativePath, typeof(string));
		}

		/// <summary>
		/// deserialize,system uses
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public CodeFile(SerializationInfo info, StreamingContext context)
		{
			RelativePath = info.GetString("relativePath");
			Path = new FileInfo(RelativePath);
		}

	}
}
