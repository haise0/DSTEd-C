using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DSTEd.Core.ProjectManager
{
	[Serializable]
	public class ProjectInfo : IEnumerable<FileInfo>
	{
		public string Name { get; private set; }
		public DirectoryInfo Location { get; private set; }

		public List<FileInfo> IncludedFiles { get; private set; }

		public ProjectInfo(string Name, DirectoryInfo Location,IEnumerable<FileInfo> IncludedFileEnumerator)
		{
			this.Name = Name;
			this.Location = Location;
			IncludedFiles = new List<FileInfo>(IncludedFileEnumerator);
		}

		public void Build(DirectoryInfo OutPutPath = null)
		{
			if (OutPutPath == null)
			{
				string targetPath = Location.Parent.ToString() + "\\test_" + Name;
				OutPutPath = new DirectoryInfo(targetPath);
			}

			foreach (FileInfo fileinfo in IncludedFiles)
			{
				File.Copy(fileinfo.FullName, Path.Combine(OutPutPath.FullName, fileinfo.Name), true);
			}
		}

		public IEnumerator<FileInfo> GetEnumerator()
		{
			return IncludedFiles.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return IncludedFiles.GetEnumerator();
		}
	}
}
