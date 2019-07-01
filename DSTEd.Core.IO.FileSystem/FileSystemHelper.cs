using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DSTEd.Core.IO.EnumerableFileSystem
{
	/// <summary>
	/// Gets all files(also in the subdirectories) in a directory to itertate
	/// </summary>
	public class RecursiveDirectoryIterator : IEnumerable<FileInfo>, ICollection<FileInfo>, ICollection
	{
		List<FileInfo> internal_vector = new List<FileInfo>(50);
		public DirectoryInfo OriginalDirectoryInfo { get; private set; }

		public int Count => internal_vector.Count;

		public bool IsReadOnly => true;

		public object SyncRoot => ((ICollection)internal_vector).SyncRoot;

		public bool IsSynchronized => ((ICollection)internal_vector).IsSynchronized;

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

		public IEnumerator<FileInfo> GetEnumerator()
		{
			return internal_vector.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return internal_vector.GetEnumerator();
		}

		public void Add(FileInfo item)
		{
			((ICollection<FileInfo>)internal_vector).Add(item);
		}

		public void Clear()
		{
			internal_vector.Clear();
		}

		public bool Contains(FileInfo item)
		{
			return internal_vector.Contains(item);
		}

		public void CopyTo(FileInfo[] array, int arrayIndex)
		{
			internal_vector.CopyTo(array, arrayIndex);
		}

		public bool Remove(FileInfo item)
		{
			return internal_vector.Remove(item);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)internal_vector).CopyTo(array, index);
		}

		public FileInfo this[int i] => internal_vector[i];
	}

	public static class FSUtil
	{
		public static RecursiveDirectoryIterator CopyDirectory(DirectoryInfo Source,DirectoryInfo Destnation)
		{
			//Destnation = Path.GetFileName(Destnation);
			return CopyFilesToDirectory(new RecursiveDirectoryIterator(Source), Destnation);
		}

		public static RecursiveDirectoryIterator CopyFilesToDirectory(IEnumerable<FileInfo> Files, DirectoryInfo TargetDirectory)
		{
			foreach (FileInfo file in Files)
			{
				string filedest = TargetDirectory.FullName + '\\' + SimpleRelative(file.DirectoryName, file.FullName);
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
						"Stack Traceback:\n{1}\n" +
						"Message:\n{2}\n" +
						"HRESULT:\n{3}\n",
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

		public static string SimpleRelative(string Current, string Another)
		{
			return Another.Replace(Current, string.Empty);
			// HACK: 
			//it's buggy. if any paramitter does not a full path, it will return a wrong result.
		}
	}
}
