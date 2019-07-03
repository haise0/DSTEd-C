using System;
using System.Collections.Generic;
using System.IO;
using DSTEd.Core.IO.EnumerableFileSystem;
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
		[JsonRequired]
		public string Name { get; private set; }
		[JsonIgnore]
		public DirectoryInfo Location { get; private set; }
		[JsonRequired]
		protected List<FileInfo> IncludedFiles;
		[JsonIgnore]
		public int Count => IncludedFiles.Count;
		[JsonIgnore]
		public bool IsReadOnly => ((ICollection<FileInfo>)IncludedFiles).IsReadOnly;

		public ProjectInfo(string Name, DirectoryInfo Location,IEnumerable<FileInfo> IncludedFileEnumerator)
		{
			this.Name = Name;
			this.Location = Location;
			IncludedFiles = new List<FileInfo>(IncludedFileEnumerator);
		}

		/// <summary>
		/// Automatually deserialize json to create it
		/// </summary>
		/// <param name="Location">Where the Project locates</param>
		public static ProjectInfo Deserialize(DirectoryInfo Location)
		{
			string json_path = Location.FullName + "\\Project.json";
			JsonSerializer serializer = new JsonSerializer();
			var ret_value = serializer.Deserialize<ProjectInfo>(new JsonTextReader(new StreamReader(File.OpenRead(json_path))));
			ret_value.Location = Location;
			return ret_value;
		}

		public virtual void Build(DirectoryInfo OutPutPath = null)
		{
			if (OutPutPath == null)
			{
				string targetPath = Location.Parent.ToString() + "\\test_" + Name;
				OutPutPath = new DirectoryInfo(targetPath);
			}

			//Step 1:Copy
			foreach (FileInfo fileinfo in IncludedFiles)
			{
				try
				{

					IO.EnumerableFileSystem.FSUtil.CopyFilesToDirectory(IncludedFiles, Location, OutPutPath);
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

			//Step 2:Run autocompiler
			//TODO

			//Step 3:Delete unused files
			//TODO
		}

		public IEnumerator<FileInfo> GetEnumerator()
		{
			return IncludedFiles.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return IncludedFiles.GetEnumerator();
		}

		public void AddFile(FileInfo File)
		{
			IncludedFiles.Add(File);
		}

		public void AddFiles(IEnumerable<FileInfo> Files)
		{
			IncludedFiles.AddRange(Files);
		}

		public bool IgnoreFile(FileInfo file)
		{
			return IncludedFiles.Remove(file);
		}

		public void DeleteFile(FileInfo file)
		{
			IncludedFiles.Remove(file);
			File.Delete(file.FullName);
		}
	}
}
