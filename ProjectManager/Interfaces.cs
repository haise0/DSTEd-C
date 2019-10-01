using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DSTEd.Core.ProjectManager
{
	/// <summary>
	/// Repersents a mod file,code,animation,etc.
	/// <para/>when it serializes, it shall serialize FileTypeID,FileType and RelativePath only.
	/// </summary>
	public interface IDSTModFile : ISerializable
	{

		/// <summary>
		/// each implemention should have it's unique typeid.
		/// Code's type id is 0,
		/// Image's type id is 1
		/// </summary>
		int FileTypeID { get; }

		/// <summary>
		/// each implemention should have it's unique type string.
		/// such as "Code","Image"
		/// </summary>
		string FileTypeName { get; }

		/// <summary>
		/// Relative path
		/// </summary>
		string RelativePath { get; }

		/// <summary>
		/// Full path
		/// </summary>
		FileInfo Path { get; }

		/// <summary>
		/// Build this file
		/// </summary>
		void Build(DirectoryInfo target);
	}

	/// <summary>
	/// Mod file builder
	/// </summary>
	public interface IModFileBuilder
	{
		/// <summary>
		/// Defines which file type will it accept
		/// </summary>
		int AcceptedFileTypeID { get; }

		/// <summary>
		/// Build a bunch of files, also checks file type.
		/// </summary>
		/// <param name="filesToBuild">Input a collection which included those MOD files to be built</param>
		/// <param name="outPutDirectory">Output directory</param>
		void BatchBuild(IEnumerable<IDSTModFile> filesToBuild, DirectoryInfo outPutDirectory);
	}
}
