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
		public DirectoryInfo Loaction { get; private set; }

	}
}
