using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DSTEd.Core.IO.EnumerableFileSystem
{
	/// <summary>
	/// Gets all files(also in the subdirectories) in a directory to itertate
	/// </summary>
	public class RecursiveDirectoryIterator : IEnumerable<FileInfo>
	{
		List<FileInfo> internal_vector = new List<FileInfo>(50);

		/// <summary>
		/// 
		/// </summary>
		public DirectoryInfo OriginalDirectoryInfo { get; private set; }

		/// <summary>
		/// Get the number of founeded files
		/// </summary>
		public int Count => internal_vector.Count;
		
		/// <summary>
		/// Initalize iterator by DirectoryInfo object.
		/// </summary>
		/// <param name="directory"></param>
		public RecursiveDirectoryIterator(DirectoryInfo directory)
		{
			OriginalDirectoryInfo = directory;
			RecursiveAdd(directory);
		}

		/// <summary>
		/// Initalize iterator by path string.
		/// </summary>
		/// <param name="Path"></param>
		public RecursiveDirectoryIterator(string Path):this(new DirectoryInfo(Path))
		{

		}

		private void RecursiveAdd(DirectoryInfo dir)
		{
			internal_vector.AddRange(dir.EnumerateFiles());
			System.Threading.Tasks.Parallel.ForEach(dir.EnumerateDirectories(), 
				(DirectoryInfo file) => RecursiveAdd(file));
		}

		/// <summary>
		/// Get the enumerator
		/// </summary>
		/// <returns></returns>
		public IEnumerator<FileInfo> GetEnumerator()
		{
			return internal_vector.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return internal_vector.GetEnumerator();
		}

		/// <summary>
		/// </summary>
		/// <param name="file"></param>
		/// <returns>true for containd, false for not contained</returns>
		public bool Contains(FileInfo file)
		{
			foreach (FileInfo source in internal_vector)
			{
				if (source.FullName == file.FullName)
					return true;
			}
			return false;
		}

		/// <summary>
		/// indexer
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public FileInfo this[int i] => internal_vector[i];
	}

	/// <summary>
	/// File system utilities
	/// </summary>
	public static class FSUtil
	{
		/// <summary>
		/// Copy a directory into another directory,and keep the original directory structure
		/// </summary>
		/// <param name="Source">Source directory</param>
		/// <param name="Destnation">Destnation directory</param>
		/// <returns></returns>
		public static RecursiveDirectoryIterator CopyDirectory(DirectoryInfo Source,DirectoryInfo Destnation)
		{
			//Destnation = Path.GetFileName(Destnation);
			return CopyFilesToDirectory(new RecursiveDirectoryIterator(Source), Destnation);
		}

		/// <summary>
		/// Copy all specified files into a new directory
		/// </summary>
		/// <param name="Iterator">Source directory</param>
		/// <param name="TargetDirectory"></param>
		/// <returns></returns>
		public static RecursiveDirectoryIterator CopyFilesToDirectory(RecursiveDirectoryIterator Iterator,DirectoryInfo TargetDirectory)
		{
			return CopyFilesToDirectory(Iterator, Iterator.OriginalDirectoryInfo, TargetDirectory);
		}

		/// <summary>
		/// Copy all specified files into a new directory
		/// </summary>
		/// <param name="Files"></param>
		/// <param name="SourceDirectory"></param>
		/// <param name="TargetDirectory"></param>
		/// <returns></returns>
		public static RecursiveDirectoryIterator CopyFilesToDirectory(IEnumerable<FileInfo> Files, DirectoryInfo SourceDirectory , DirectoryInfo TargetDirectory)
		{
			foreach (FileInfo file in Files)
			{
				string filedest = TargetDirectory.FullName + '\\' + SimpleRelative(SourceDirectory.FullName, file.FullName);
				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(filedest));
					file.CopyTo(filedest);
				}
				catch (DirectoryNotFoundException e)
				{
					System.Diagnostics.Debug.WriteLine("Direcotry not found?\n" +
						filedest + '\n' +
						"HRESULT:\n" +
						e.HResult);
					Directory.CreateDirectory(Path.GetDirectoryName(filedest));
					file.CopyTo(filedest);
				}
				catch (FileNotFoundException e)
				{
					Console.WriteLine(
						"????????BUG???????\n" +
						"Check FSUtil.RecursiveDirectoryItertatior\n" +
						"Stack Traceback:\n{0}\n" +
						"Message:\n{1}\n" +
						"HRESULT:\n{2}\n",
						e.StackTrace, e.Message, e.HResult);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString() + e.HResult);
				}
#if DEBUG
				System.Diagnostics.Debug.WriteLine("Copy {0} to {1}", file.FullName, filedest);
#endif
			}
			return new RecursiveDirectoryIterator(TargetDirectory);
		}

		/// <summary>
		/// Get relative path from two full path strings. faster than Relative
		/// </summary>
		/// <param name="Parent">parent directory</param>
		/// <param name="File">file path</param>
		/// <returns>Relative path string</returns>
		public static string SimpleRelative(string Parent, string File)
		{
			return File.Replace(Parent, string.Empty);
		}

		/// <summary>
		/// Strict relative path
		/// </summary>
		/// <param name="basepath"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		//祖传代码
		public static string Relative(string basepath, string path)
		{
			string[] b = basepath.Replace('/', '\\').Split('\\');
			string[] p = path.Replace('/', '\\').Split('\\');
			string relative_path;

			IEnumerator<string>
			it_base = ((IEnumerable<string>)b).GetEnumerator(),
			it_path = ((IEnumerable<string>)p).GetEnumerator();
			{
				bool b_end = true, p_end = true;
				int diff = 0;

				//find same part by "foreach"
				do
				{
					b_end = it_base.MoveNext();
					p_end = it_path.MoveNext();

					//no difference until one collection end
					if (!b_end)
					{
						relative_path = "\\" + it_path.Current;
						while (it_path.MoveNext())
						{
							relative_path += '\\' + it_path.Current;
						}
						return relative_path;
					}
					if (!p_end)
					{
						relative_path = "\\..";
						while (it_base.MoveNext())
						{
							relative_path += "\\..";
						}
						return relative_path;
					}

					if (it_base.Current != it_path.Current && (it_base.Current != string.Empty)) break;

					diff++;
				} while (b_end && p_end);
			}
			return string.Empty;
		}

		/// <summary>
		/// Filter out some specified file in a FileInfo collection
		/// </summary>
		/// <param name="Files"></param>
		/// <param name="Extensions">like ".jpg",".lua"</param>
		/// <returns>A List{FileInfo} contains the filtered out files</returns>
		/// <example>ApplyFilter(files,".jpg",".png",".lua")</example>
		public static List<FileInfo> ApplyFilter(ICollection<FileInfo> Files,params string[] Extensions)
		{
			List<FileInfo> ret_value = new List<FileInfo>(Files.Count);
			foreach (FileInfo file in Files)
			{
				foreach (string ext in Extensions)
				{
					if (file.Extension == ext)
						ret_value.Add(file);
				}
			}
			return ret_value;
		}
	}
}
