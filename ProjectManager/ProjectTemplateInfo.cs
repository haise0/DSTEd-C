using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DSTEd.Core.ProjectManager
{
	[Serializable]
	public class ProjectTemplateInfo
	{
		public string Name { get; private set; }
		public DirectoryInfo Location { get; private set; }

		public ProjectInfo CreateProject(string Name, string FullPath)
		{
			IO.filesystem.RecursiveDirectoryIterator enumerator = IO.filesystem.FSUtil.CopyDirectory(Location, FullPath);
			ProcessFile(Name, ref enumerator);
			return new ProjectInfo(Name, enumerator.OriginalDirectoryInfo, enumerator);
		}

		private void ProcessFile(string NewName, ref IO.filesystem.RecursiveDirectoryIterator iter)
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
					catch (DirectoryNotFoundException)
					{
						Directory.CreateDirectory(file.DirectoryName);
						file.MoveTo(newpath);
					}
					catch(FileNotFoundException e)
					{
						Console.WriteLine("Exception:{0}\n????????BUG???????\nCheck FSUtil,RecursiveDirectoryItertatior", e);
					}
					catch(Exception e)
					{
						System.Diagnostics.Debug.WriteLine(e);
					}
				}
			}
		}
	}
}
