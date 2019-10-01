using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DSTEd.Core.ProjectManager.FileTypes
{
	internal class ImageFile : IDSTModFile
	{
		public ImageFile(DirectoryInfo baseDirectory, FileInfo imageFileInfo)
		{
			if (imageFileInfo.Extension != ".png" || imageFileInfo.Extension != ".jpg")
				throw new ArgumentException("imageFileInfo", "ImageFile only supports png and jpg");

			RelativePath = IO.EnumerableFileSystem.FSUtil.Relative(baseDirectory.FullName, imageFileInfo.FullName);
			Path = imageFileInfo;
		}

		public int FileTypeID => 1;

		public string FileTypeName => "Image";

		public string RelativePath { get; private set; }

		public FileInfo Path { get; private set; }

		public void Build(DirectoryInfo target)
		{
			ModToolImageBuilder.BuildOne(this, target.FullName);
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
		public ImageFile(SerializationInfo info, StreamingContext context)
		{
			RelativePath = info.GetString("relativePath");
			Path = new FileInfo(RelativePath);
		}
	}
}
