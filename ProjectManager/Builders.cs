using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTEd.Core.ProjectManager
{
	internal class ModToolImageBuilder : IModFileBuilder
	{
		//accepts Image
		public int AcceptedFileTypeID => 1;

		public void BatchBuild(IEnumerable<IDSTModFile> filesToBuild, DirectoryInfo outPutDirectory)
		{
			foreach (IDSTModFile item in filesToBuild)
			{
				if (item.FileTypeID != 1)
					continue;

				string outfile = outPutDirectory.FullName + item.RelativePath;
				File.Copy(item.Path.FullName, outfile, true);

				default_si.Arguments = string.Format("\"{0}\"", outfile);
				//wait for 10 seconds
				Process.Start(default_si).WaitForExit(10000);
			}
		}

		public static string BuildOne(IDSTModFile file, string outdir)
		{
			if (file.FileTypeID != 1)
				return string.Empty;

			string outfile = outdir + file.RelativePath;
			File.Copy(file.Path.FullName, outfile, true);

			default_si.Arguments = default_si.Arguments = string.Format("\"{0}\"", outfile);
			Process.Start(default_si).WaitForExit();

			return outfile;
		}

		//we don't have to recreate it when we need to start a png compiler.
		//when we need to use it,we just need to change "Arguments" property
		private static ProcessStartInfo default_si = new ProcessStartInfo
		{
			CreateNoWindow = true,
			FileName = Builders.ModToolsBinDir + "png.exe",
			RedirectStandardError = true,
			RedirectStandardOutput = true,
			StandardErrorEncoding = Encoding.UTF8,
			StandardOutputEncoding = Encoding.UTF8,
			UseShellExecute = false,
		};
	}

	internal class CodeBuilder : IModFileBuilder
	{
		public int AcceptedFileTypeID => 0;

		public void BatchBuild(IEnumerable<IDSTModFile> filesToBuild, DirectoryInfo outPutDirectory)
		{
			foreach (IDSTModFile file in filesToBuild)
			{
				if (file.FileTypeID != 0)
					continue;

				string target = outPutDirectory.FullName + file.RelativePath;
				File.Copy(file.Path.FullName, target, true);
			}
		}
	}

	/// <summary>
	/// get or add builders here
	/// </summary>
	public static class Builders
	{
		static Builders()
		{
			builders = new List<IModFileBuilder>(24)
			{
				new CodeBuilder(),
				new ModToolImageBuilder()
			};
		}

		/// <summary>
		/// This shall set once.
		/// </summary>
		public static readonly string ModToolsBinDir;

		/// <summary>
		/// Add a builder into builders
		/// </summary>
		/// <param name="builder">custom builder,can't be null</param>
		/// <exception cref="ArgumentNullException">throws when builder is null</exception>
		public static void AddBuilder(IModFileBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder", "builder can't be null");
			builders.Add(builder);
		}

		internal static void RunBuilders(IEnumerable<IDSTModFile> files,DirectoryInfo outPutDirectory)
		{
			foreach (IModFileBuilder builder in builders)
			{
				if (builder == null)
					continue;

				builder.BatchBuild(files, outPutDirectory);
			}
		}

		private static List<IModFileBuilder> builders;
	}
}
