using System;
using System.IO;

namespace ItWebSite.Core.Help
{
    public class UtilHelper
    {

        public static string SqliteFilePath
        {
            get
            {
                string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.ConfigFolderTags);
                if (!Directory.Exists(dataDirectory))
                    Directory.CreateDirectory(dataDirectory);
                return Path.Combine(dataDirectory, Constants.SqliteFileNameTags);
            }
        }
    }
}
