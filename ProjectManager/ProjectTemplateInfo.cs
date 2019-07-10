using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSTEd.Core.IO.EnumerableFileSystem;
using Newtonsoft.Json;

namespace DSTEd.Core.ProjectManager
{
	/// <summary>
	/// Repersents a Project template
	/// </summary>
	[Serializable]
	public class ProjectTemplateInfo
	{
		/// <summary>
		/// Project template name
		/// </summary>
		[JsonRequired]
		public string Name { get; private set; }

		/// <summary>
		/// Project template location
		/// </summary>
		[JsonIgnore]
		public DirectoryInfo Location { get; private set; }

		[JsonRequired]
		private List<FileInfo> files = new List<FileInfo>(50);

		/// <summary>
		/// Static JSON serializer
		/// </summary>
		[JsonIgnore]
		protected static JsonSerializer serializer;

		static ProjectTemplateInfo()
		{
			serializer = new JsonSerializer();
			serializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
			serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;
			serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			serializer.Formatting = Formatting.Indented;
		}

		/// <summary>
		/// Defualt constructor, this should only be used by json deserializer.
		/// </summary>
		protected ProjectTemplateInfo()
		{
			Name = null;
			Location = null;
		}

		/// <summary>
		/// Automatually deserialize json to create it,
		/// Use this to get a <c>ProjectTemplateInfo</c> object
		/// </summary>
		/// <example>
		///	<code>
		///		ProjectTemplateInfo template = ProjectTemplateInfo.Deserialize(new DirectoryInfo("\template1\"))
		///	</code>
		/// </example>
		/// <param name="Location">Where the template locates</param>
		public static ProjectTemplateInfo Deserialize(DirectoryInfo Location)
		{
			string json_path = Location.FullName + "\\ProjectTemplate.json";
			var ret_value = serializer.Deserialize<ProjectTemplateInfo>(new JsonTextReader(new StreamReader(File.OpenRead(json_path))));
			ret_value.Location = Location;
			return ret_value;
		}

		 /// <summary>
		 /// Create a project and return a <c>ProjectInfo</c> object repersent it
		 /// </summary>
		 /// <param name="Name">Project Name</param>
		 /// <param name="ProjectLocation">Where the new project locates</param>
		 /// <returns><c>ProjectInfo</c> represent the created project</returns>
		virtual public ProjectInfo CreateFormThis(string Name, DirectoryInfo ProjectLocation)
		{
			RecursiveDirectoryIterator iter = FSUtil.CopyFilesToDirectory(files, Location, ProjectLocation);
			ProcessFiles(Name, ref iter);
			var ret_value = new ProjectInfo(Name, iter.OriginalDirectoryInfo, iter);
			ret_value.Save();
			return ret_value;
		}

		private void ProcessFiles(string NewName, ref RecursiveDirectoryIterator iter)
		{
			foreach (FileInfo file in iter)
			{
				if(file.Name.Contains("__NAME"))
				{
					string newpath = file.FullName.Replace("__NAME", NewName);
					try
					{
						file.MoveTo(newpath);
					}
					catch (DirectoryNotFoundException e)
					{
#if DEBUG
						System.Diagnostics.Debugger.Break();
#endif
						Console.WriteLine("Direcotry not found?\n" +
						newpath + '\n' +
						"HRESULT:\n" +
						e.HResult);
						Directory.CreateDirectory(Path.GetDirectoryName(newpath));
						file.MoveTo(newpath);
					}
					catch(FileNotFoundException e)
					{
						Console.WriteLine(
							"????????BUG???????\n" +
							"IS COPY FILE FAILED?\n" +
							"Check FSUtil.RecursiveDirectoryItertatior\n" +
							"Stack Traceback:\n{1}\n" +
							"Message:\n{2}\n" +
							"HRESULT:\n{3}\n", 
							e.StackTrace,e.Message,e.HResult);
					}
					catch(Exception e)
					{
						System.Diagnostics.Debug.WriteLine(e.Message + e.StackTrace);
					}

					//stage2: Change File Content
					try
					{
						byte[] buffier = File.ReadAllBytes(newpath);
						string content = Encoding.UTF8.GetString(buffier);
						content.Replace("<NAME>", NewName);
						buffier = Encoding.UTF8.GetBytes(content);
						file.OpenWrite().Write(buffier, 0, buffier.Length);
					}
					catch(FileNotFoundException e)
					{
#if DEBUG
						System.Diagnostics.Debugger.Break();
#endif
						Console.WriteLine("File Not Found?\n" +
							"newpath={0}\n" +
							"e:\n{1}\n" +
							"HRESULT={2}",
							newpath, e, e.HResult);
					}
					catch(DirectoryNotFoundException e)
					{
						Console.WriteLine("BUG???????\n" +
							"newpath={0}\n" +
							"e:\n{1}\n" +
							"HRESULT={2}",
							newpath, e, e.HResult);
					}
					catch(System.Security.SecurityException e)
					{
						//open a dialog?
						Console.WriteLine("Check permissions\n" + e.ToString());
					}
					catch(Exception e)
					{
#if DEBUG
						System.Diagnostics.Debugger.Break();
#endif
						Console.WriteLine(e.Message + e.HResult.ToString() + e.HResult);
					}
				}
			}
		}
	}
}
