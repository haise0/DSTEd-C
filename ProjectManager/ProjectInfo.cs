using System;
using System.Collections.Generic;
using System.IO;
using DSTEd.Core.IO.EnumerableFileSystem;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization;

namespace DSTEd.Core.ProjectManager
{
	/// <summary>
	/// Repersents a project
	/// </summary>
	[Serializable]
	public class ProjectInfo : IEnumerable<IDSTModFile>,ISerializable
	{
		/// <summary>
		/// Project name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Project location
		/// </summary>
		public DirectoryInfo Location { get; private set; }

		/// <summary>
		/// Code files included by the project
		/// </summary>
		protected List<IDSTModFile> IncludedFiles;

		/// <summary>
		/// File count
		/// </summary>
		public int Count => IncludedFiles.Count;

		/// <summary>
		/// Static JSON serializer
		/// </summary>
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
			this.IncludedFiles = new List<IDSTModFile>();

			foreach (FileInfo item in IncludedFiles)
			{

			}
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

			//Run their build command
			foreach(IDSTModFile file in IncludedFiles)
			{
				file.Build(OutPutPath);
			}
		}

		/// <summary>
		/// Gets the enumerator
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IDSTModFile> GetEnumerator()
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
		public void AddFile(IDSTModFile File)
		{
			IncludedFiles.Add(File);
		}

		/// <summary>
		/// Add some existing file into the project
		/// </summary>
		/// <param name="Files"></param>
		public void AddFiles(IEnumerable<IDSTModFile> Files)
		{
			IncludedFiles.AddRange(Files);
		}

		/// <summary>
		/// Ignore a specified file
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public bool IgnoreFile(IDSTModFile file)
		{
			return IncludedFiles.Remove(file);
		}

		/// <summary>
		/// Delete a specified file on the disk
		/// </summary>
		/// <param name="file"></param>
		public void DeleteFile(IDSTModFile file)
		{
			IncludedFiles.Remove(file);
			File.Delete(file.Path.FullName);
		}

		/// <summary>Serializer uses this</summary>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{

			throw new NotImplementedException();
		}
	}
}
