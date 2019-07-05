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
	/// <summary>
	/// Repersents a project
	/// </summary>
	[Serializable]
	public class ProjectInfo : IEnumerable<FileInfo>
	{
		/// <summary>
		/// Project name
		/// </summary>
		[JsonRequired]
		public string Name { get; private set; }
		/// <summary>
		/// Project location
		/// </summary>
		[JsonIgnore]
		public DirectoryInfo Location { get; private set; }
		/// <summary>
		/// Files included by the project
		/// </summary>
		[JsonRequired]
		protected List<FileInfo> IncludedFiles;
		/// <summary>
		/// File count
		/// </summary>
		[JsonIgnore]
		public int Count => IncludedFiles.Count;
		/// <summary>
		/// Static JSON serializer
		/// </summary>
		[JsonIgnore]
		protected static JsonSerializer serializer = new JsonSerializer();

		/// <summary>
		/// Defualt constructor, this should only be used by json deserializer.
		/// </summary>
		public ProjectInfo()
		{
			Name = null;
			Location = null;
			IncludedFiles = null;
		}

		/// <summary>
		/// Initalize ProjectInfo by specified name,location,and included files
		/// </summary>
		/// <param name="Name">Project name</param>
		/// <param name="Location">Project Location</param>
		/// <param name="IncludedFiles"></param>
		public ProjectInfo(string Name, DirectoryInfo Location,IEnumerable<FileInfo> IncludedFiles)
		{
			this.Name = Name;
			this.Location = Location;
			this.IncludedFiles = new List<FileInfo>(IncludedFiles);
		}

		/// <summary>
		/// Automatually deserialize json to create it
		/// </summary>
		/// <param name="Location">Where the Project locates</param>
		/// <example>
		/// <code>
		/// ProjectList.Add(ProjectInfo.Deserialize(project_dir))
		/// </code>
		/// </example>
		public static ProjectInfo Deserialize(DirectoryInfo Location)
		{
			string json_path = Location.FullName + "\\Project.json";
			var ret_value = serializer.Deserialize<ProjectInfo>(new JsonTextReader(new StreamReader(File.OpenRead(json_path))));
			ret_value.Location = Location;
			return ret_value;
		}

		/// <summary>
		/// Save this ProjectInfo into Project.json
		/// </summary>
		public void Save()
		{
			string json_path = Location.FullName + "\\Project.json";
			using (FileStream json = new FileStream(json_path, FileMode.Truncate, FileAccess.Write))
				serializer.Serialize(new StreamWriter(json), this, typeof(ProjectInfo));
		}

		/// <summary>
		/// build the project
		/// </summary>
		/// <param name="OutPutPath">specifies where the output path,"..\test_{Name}" by default</param>
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

		/// <summary>
		/// Gets the enumerator
		/// </summary>
		/// <returns></returns>
		public IEnumerator<FileInfo> GetEnumerator()
		{
			return IncludedFiles.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return IncludedFiles.GetEnumerator();
		}

		/// <summary>
		/// Add an existing file into the project
		/// </summary>
		/// <param name="File"></param>
		public void AddFile(FileInfo File)
		{
			//param check
			if (!File.Exists)
				throw new FileNotFoundException(string.Format("Attempting to add a non-existing file into project \"{0}\"",Name),
					File.FullName);
			//check success,add
			IncludedFiles.Add(File);
		}

		/// <summary>
		/// Add some existing file into the project
		/// </summary>
		/// <param name="Files"></param>
		public void AddFiles(IEnumerable<FileInfo> Files)
		{
			//param check
			string not_exist = string.Empty;
			foreach (FileInfo file in Files)
				if (!file.Exists)
					not_exist += file.FullName + '\n';

			//check failed
			if (not_exist != string.Empty)
				throw new FileNotFoundException(string.Format("Attempting to add some non-existing file(s) into project \"{0}\"", Name)
					, not_exist);

			//check success,add
			IncludedFiles.AddRange(Files);
		}

		/// <summary>
		/// Ignore a specified file
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public bool IgnoreFile(FileInfo file)
		{
			return IncludedFiles.Remove(file);
		}

		/// <summary>
		/// Delete a specified file on the disk
		/// </summary>
		/// <param name="file"></param>
		public void DeleteFile(FileInfo file)
		{
			IncludedFiles.Remove(file);
			File.Delete(file.FullName);
		}
	}
}
