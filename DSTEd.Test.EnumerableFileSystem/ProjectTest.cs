using System;
using System.Collections.Generic;
using System.Reflection;
using DSTEd.Core.ProjectManager;
using System.IO;
using DSTEd.Core.IO.EnumerableFileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DSTEd.Test.ProjectManager
{
	[TestClass]
	public class ProjectTest
	{
		//private static Core.ProjectManager.ProjectManager projmgr = new Core.ProjectManager.ProjectManager(".\\Projects");
		/*[TestMethod]
		public void FromTemplate()
		{

		}*/

		[TestMethod]
		public void ReadTemplate()
		{
			ProjectTemplateInfo test_template = ProjectTemplateInfo.Deserialize(new DirectoryInfo(".\\Project Templates\\Test template"));
			Assert.AreEqual(test_template.Name, "TestTemplate");
			//read private member by reflection
			List<FileInfo> files = 
				typeof(ProjectTemplateInfo).GetField("files",
				BindingFlags.Instance | BindingFlags.NonPublic).GetValue(test_template) as List<FileInfo>;
			foreach (FileInfo file in files)
			{
				//Assert
			}
		}

		/*[TestMethod]
		public void ReadProject()
		{

		}*/
	}
}
