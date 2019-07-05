using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DSTEd.Core.ProjectManager
{
	/// <summary>
	/// thrown when the specified project template not found
	/// </summary>
	public class TemplateNotFoundException : Exception
	{
		/// <summary>
		/// The specified template name
		/// </summary>
		public string TemplateName { get; protected set; }
		/// <summary>
		/// Get the message, included the template name (and a hint message)
		/// </summary>
		public override string Message
		{
			get
			{
				if(base.Message == null)
					return string.Format(
							"Specified project template not found.\n" +
							"\tproject template name: {0}\n",
							TemplateName);
				else
					return string.Format(
							"Specified project template not found.\n" +
							"\tproject template name: {0}\n" +
							"\tother message: {1}\n",
							TemplateName, 
							base.Message);
			}
		}

		/// <summary>
		/// Initializes a new instance of the TemplateNotFoundException class with a specified project template name.
		/// </summary>
		/// <param name="templateName">the specified project template name</param>
		public TemplateNotFoundException(string templateName):base(null)
		{
			TemplateName = templateName;
		}

		/// <summary>
		/// Initializes a new instance of the TemplateNotFoundException class with specified project template name and error message.
		/// </summary>
		/// <param name="templateName">The specified project template name</param>
		/// <param name="message">Error message</param>
		public TemplateNotFoundException(string templateName,string message) : base(message)
		{
			TemplateName = templateName;
		}
	}

	/// <summary>
	/// Manages the projects
	/// </summary>
	public class ProjectManager
	{
		private List<ProjectInfo> Projects;
		private DirectoryInfo ProjectsLocation;
		private List<ProjectTemplateInfo> Templates = new List<ProjectTemplateInfo>(10);

		/// <summary>
		/// Initalizes a new instance of ProjectManger class by a specified directory which palced projects in
		/// </summary>
		/// <param name="directory">The directory which placed projects in</param>
		public ProjectManager(string directory)
		{
			ProjectsLocation = new DirectoryInfo(directory);
			Projects = new List<ProjectInfo>(20);
			Templates = new List<ProjectTemplateInfo>(10);

			foreach (DirectoryInfo template_dir in new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Project Templates").EnumerateDirectories())
			{
				Templates.Add(ProjectTemplateInfo.Deserialize(template_dir));
			}

			Templates.TrimExcess();

			foreach (DirectoryInfo project_dir in ProjectsLocation.EnumerateDirectories())
			{
				Projects.Add(ProjectInfo.Deserialize(project_dir));
			}

		}

		/// <summary>
		/// Create a new project and add it into projects list
		/// </summary>
		/// <param name="TemplateName">
		/// Name of template
		/// </param>
		/// <param name="ProjectName">
		/// Name of the new project
		/// </param>
		/// <exception cref="TemplateNotFoundException"></exception>
		public void NewFormTemplate(string TemplateName, string ProjectName)
		{
			foreach (ProjectTemplateInfo template in Templates)
			{
				if(template.Name == TemplateName)
				{
					Projects.Add(template.CreateFormThis(ProjectName, Directory.CreateDirectory(ProjectsLocation.FullName + '\\' + ProjectName)));
					return;
				}
			}
			throw new TemplateNotFoundException(TemplateName);
		}
	}
}
