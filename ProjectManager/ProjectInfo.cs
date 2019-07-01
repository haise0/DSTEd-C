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

		public IEnumerable<FileInfo> IncludedFiles { get; private set; }

		protected ProjectInfo()
		{
			Name = null;
			Location = null;
			IncludedFiles = null;
		}

		public ProjectInfo(string Name, DirectoryInfo Location,IEnumerable<FileInfo> IncludedFileEnumerator)
		{
			this.Name = Name;
			this.Location = Location;
			IncludedFiles = IncludedFileEnumerator;
		}

		public virtual void Build(DirectoryInfo OutPutPath = null)
		{
			if (OutPutPath == null)
			{
				string targetPath = Location.Parent.ToString() + "\\test_" + Name;
				OutPutPath = new DirectoryInfo(targetPath);
			}

			foreach (FileInfo fileinfo in IncludedFiles)
			{
				try
				{
					//IO.EnumerableFileSystem.FSUtil.
				}
				catch (Exception e)
				{
#if DEBUG
					System.Diagnostics.Debugger.Break();
					System.Diagnostics.Debug.WriteLine("msg:{0}\n" +
						"HRESULT:{1}" +
						"Stack Traceback:{2}",
						e.Message, e.HResult, e.StackTrace);
#else
					Console.WriteLine("msg:{0}\n" +
						"HRESULT:{1}" +
						"Stack Traceback:{2}",
						e.Message, e.HResult, e.StackTrace);
#endif
				}
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
