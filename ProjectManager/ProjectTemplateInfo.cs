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
	[Serializable]
	public class ProjectTemplateInfo
	{
		public string Name { get; private set; }
		public DirectoryInfo Location { get; private set; }

		public ProjectInfo CreateProject(string Name, string FullPath)
		{
			RecursiveDirectoryIterator enumerator = FSUtil.CopyDirectory(Location, FullPath);
			ProcessFile(Name, ref enumerator);
			return new ProjectInfo(Name, enumerator.OriginalDirectoryInfo, enumerator);
		}

		private void ProcessFile(string NewName, ref RecursiveDirectoryIterator iter)
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
						System.Diagnostics.Debug.WriteLine("Direcotry not found?\n" +
							newpath + '\n' +
							"HRESULT:\n" +
							e.HResult);
						file.MoveTo(newpath);
					}
					catch(FileNotFoundException e)
					{
						Console.WriteLine(
							"????????BUG???????\n" +
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
				}
			}
		}
	}
}
