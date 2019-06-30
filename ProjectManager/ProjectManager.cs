using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DSTEd.Core.ProjectManager
{
	public class ProjectManager
	{
		public List<ProjectInfo> Projects { get; private set; }
		private DirectoryInfo Location;
		private List<ProjectTemplateInfo> Templates = new List<ProjectTemplateInfo>(10);
		public ProjectManager(string Path)
		{
			Location = new DirectoryInfo(Path);
			Projects = new List<ProjectInfo>();
			JsonSerializer serializer = new JsonSerializer();

			foreach (string template_path in Directory.EnumerateDirectories(".\\Project Templates\\"))
			{
				string template_config = template_path + "\\Project.json";
				//TODO: read json to deserialize it.
			}

			foreach (DirectoryInfo proj_dir in Location.EnumerateDirectories())
			{
				FileStream json = File.OpenRead(proj_dir.FullName + "\\Project.json");

				Projects.Add(serializer.Deserialize<ProjectInfo>(new JsonTextReader(new StreamReader(json))));
			}

			Projects = new List<ProjectInfo>();
		}

		public void New(ProjectTemplateInfo Template, string Name)
		{
			Projects.Add(Template.CreateProject(Name, Path.Combine(Location.FullName, Name)));
		}
	}
}
