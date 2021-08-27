#region header
// /*********************************************************************************************/
// Project :ConfigFile
// FileName : ConfigManager.cs
// Time Create : 9:32 AM 01/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Log;

namespace ConfigFile
{
    [Export(typeof(IConfigManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConfigManager:IConfigManager
    {
        [Import] private ILog _log;
        #region Implementation of IConfigManager

        public T Read<T>(string path) where T : IConfigObject
        {
            try
            {
                if (!File.Exists(path))
                {
                    var t = Activator.CreateInstance<T>();
                    Write(t, path);
                    return t;
                }
                var fStream = File.Open(path, FileMode.Open);
                var reader = XmlReader.Create(fStream);
                var ser = new DataContractSerializer(typeof(T));
                var cfg = (T)ser.ReadObject(reader);
                reader.Dispose();
                fStream.Dispose();
                return cfg;
            }
            catch (Exception ex)
            {
                _log.Exception("", ex, "Đọc file cấu hình lỗi");
                
            }
            return default(T);
        }

        public bool Write<T>(T obj, string path) where T : IConfigObject
        {
            try
            {
                if (obj == null) return false;
                if (File.Exists(path))
                    File.Delete(path);
                obj.Fix();
                var fStream = File.Create(path);
                var settings = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "\t"
                };
                var writer = XmlWriter.Create(fStream, settings);
                var ser = new DataContractSerializer(obj.GetType());
                ser.WriteObject(writer, obj);
                writer.Flush();
                writer.Dispose();
                fStream.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                _log.Exception("", ex, "Lưu file lỗi");
            }
            return false;
        }

        #endregion
    }
}