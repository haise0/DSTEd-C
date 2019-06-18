using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTEd.Core.ProjectManager
{
	[Serializable]
	public class ProjectInfo
	{
		public string Name { get; private set; }
		public DirectoryInfo Location { get; private set; }

		public string[] IncludePaths { get; private set; }
		public string[] ExcludeExtenions { get; private set; }
		public string[] ExcludeFiles { get; private set; }

		public ProjectInfo(FileInfo ProjectConfigJSON)
		{
			var deserializer = new JsonSerializer();
			ProjectInfo that = deserializer.Deserialize<ProjectInfo>(new JsonTextReader(new StreamReader(File.OpenRead(ProjectConfigJSON.FullName))));
			Name			 = that.Name;
			Location		 = that.Location;
			IncludePaths	 = that.IncludePaths;
			ExcludeExtenions = that.ExcludeExtenions;
			ExcludeFiles	 = that.ExcludeFiles;
		}

		public void Build(DirectoryInfo OutPutPath = null)
		{
			if (OutPutPath == null)
			{
				string targetPath = Location.Parent.ToString() + "\\test_" + Name;
				OutPutPath = new DirectoryInfo(targetPath);
			}

			foreach (string ThePath in IncludePaths)
			{
				if(Directory.Exists(ThePath))
				{
					
				}
				else if(File.Exists(ThePath))
				{

				}
				else
				{
					System.Diagnostics.Debug.WriteLine("Path not exists,or neither a file nor a directory.ThePath is: " + ThePath);
				}
			}
		}
	}
}
