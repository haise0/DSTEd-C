using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DSTEd.Core.IO.filesystem
{
	/// <summary>
	/// Gets all files(also in the subdirectories) in a directory to itertate
	/// </summary>
	public class RecursiveDirectoryIterator : IEnumerable<FileInfo>
	{
		List<FileInfo> internal_vector = new List<FileInfo>(50);
		public DirectoryInfo OriginalDirectoryInfo { get; private set; }
		public RecursiveDirectoryIterator(DirectoryInfo directory)
		{
			OriginalDirectoryInfo = directory;
			RecursiveAdd(directory);
		}

		public RecursiveDirectoryIterator(string Path):this(new DirectoryInfo(Path))
		{

		}

		private void RecursiveAdd(DirectoryInfo dir)
		{
			internal_vector.AddRange(dir.EnumerateFiles());
			System.Threading.Tasks.Parallel.ForEach(dir.EnumerateDirectories(), 
				(DirectoryInfo file) => RecursiveAdd(file));
		}
		IEnumerator<FileInfo> IEnumerable<FileInfo>.GetEnumerator()
		{
			return internal_vector.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return internal_vector.GetEnumerator();
		}
	}

	public static class FSUtil
	{
		public static RecursiveDirectoryIterator CopyDirectory(DirectoryInfo Source,string Destnation)
		{
			Destnation = Path.GetFileName(Destnation);
			foreach (FileInfo file in new RecursiveDirectoryIterator(Source))
			{
				string filedest = Destnation + '\\' + SimpleRelative(Source.FullName, file.FullName);
				try
				{
					file.CopyTo(filedest);
				}
				catch (DirectoryNotFoundException)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(filedest));
					file.CopyTo(filedest);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				#if DEBUG
				System.Diagnostics.Debug.WriteLine("Copy {0} to {1}", file.FullName, filedest); 
				#endif
			}
			return new RecursiveDirectoryIterator(Destnation);
		}

		public static string SimpleRelative(string Current, string Another)
		{
			return Another.Replace(Current, string.Empty);
			// HACK: 
			//it's buggy. if any paramitter does not a full path, it will return a wrong result.
		}
	}
}
