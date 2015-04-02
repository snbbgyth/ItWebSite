using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ItWebSite.Core.Help
{
    public static  class UtilHelper
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

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
