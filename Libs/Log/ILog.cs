using System;

namespace Log
{
    public interface ILog
    {
        /// <summary>
        /// Lấy lớp attack log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetAttackLog<T>() where T : class, IAttackLog;

        /// <summary>
        /// Cấu hình log từ string Verbose = Debug | Info | Fatal | Warning | Error | Success | Exception
        /// </summary>
        /// <param name="log"></param>
        void InitLogConfig(String log);

        /// <summary>
        /// cài đặt mức level ghi log
        /// </summary>
        LogType  LogLevel { set; get; }
        void InstallAttackLog(IAttackLog at);
        /// <summary>
        /// debug log
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format">
        /// format
        /// </param>
        /// <param name="arg">
        /// argument
        /// </param>
        void Debug(string tag,string format, params object[] arg);

        /// <summary>
        /// info log
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format">
        /// format
        /// </param>
        /// <param name="arg">
        /// argument
        /// </param>
        void Info(string tag, string format, params object[] arg);

        /// <summary>
        /// success log
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format">
        /// format
        /// </param>
        /// <param name="arg">
        /// argument
        /// </param>
        void Success(string tag, string format, params object[] arg);

        /// <summary>
        /// Error log
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format">
        /// format
        /// </param>
        /// <param name="arg">
        /// argument
        /// </param>
        void Error(string tag, string format, params object[] arg);

        /// <summary>
        /// Warning log
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format">
        /// format
        /// </param>
        /// <param name="arg">
        /// argument
        /// </param>
        void Warning(string tag, string format, params object[] arg);

        /// <summary>
        /// Fatal log
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format">
        /// format
        /// </param>
        /// <param name="arg">
        /// argument
        /// </param>
        void Fatal(string tag, string format, params object[] arg);

        /// <summary>
        /// Exception log
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="e">
        /// Exception
        /// </param>
        /// <param name="format">
        /// format
        /// </param>
        /// <param name="arg">
        /// argument
        /// </param>
        void Exception(string tag, Exception e, string format, params object[] arg); 
    }
}