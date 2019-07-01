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

		public ProjectTemplateInfo()
		{
			Name = null;
			Location = null;
		}

		virtual public ProjectInfo CreateProject(string Name, string FullPath)
		{
			RecursiveDirectoryIterator enumerator = FSUtil.CopyDirectory(Location, new DirectoryInfo(FullPath));
			ProcessFiles(Name, ref enumerator);
			return new ProjectInfo(Name, enumerator.OriginalDirectoryInfo, enumerator);
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
