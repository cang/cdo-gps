#region header

// /*********************************************************************************************/
// Project :ConfigFile
// FileName : IConfigManager.cs
// Time Create : 8:54 AM 23/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace ConfigFile
{
    public interface IConfigManager
    {
        T Read<T>(string path) where T : IConfigObject;
        bool Write<T>(T obj, string path) where T : IConfigObject;
    }

    public interface IConfigObject
    {
        /// <summary>
        ///     cấu hình các thông tin mặc định
        /// </summary>
        void Fix();
    }
}