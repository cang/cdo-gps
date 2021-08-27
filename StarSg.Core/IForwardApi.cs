#region header
// /*********************************************************************************************/
// Project :Core
// FileName : IForwardApi.cs
// Time Create : 1:34 PM 23/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion
namespace StarSg.Core
{
    /// <summary>
    /// kết nối va truyền dữ liệu lên web api
    /// </summary>
    public interface IForwardApi
    {
        void AddHeader(string key, string value);
        T Post<T>(string url, object body = null);
        T Put<T>(string url, object body = null);
        T Del<T>(string url, object body = null);
        T Get<T>(string url);
        T Get<T>(string url, object body);
        T Get<T>(string url, int msTimeout);

    }
}