using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Log
{
    /// <summary>
    /// The log manager.
    /// </summary>
    [Export(typeof(ILog))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LogManager : ILog, IPartImportsSatisfiedNotification
    {
        /// <summary>
        /// danh sách các attack log 
        /// </summary>
        //[ImportMany(typeof(IAttackLog))]
        private readonly IList<IAttackLog> _attaclLogs = new List<IAttackLog>();

        /// <summary>
        /// Lấy lớp attack log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAttackLog<T>() where T : class, IAttackLog
        {
            return (T) _attaclLogs.FirstOrDefault(m => m.GetType() == typeof (T));
        }

        public LogType LogLevel { set; get; } = LogType.Verbose;

        public void InstallAttackLog(IAttackLog at)
        {
            _attaclLogs.Add(at);
        }

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
        public void Debug(string tag,string format, params object[] arg)
        {
            if (!this.LogLevel.HasFlag(LogType.Debug)) return;

            StackTrace stackTrace = new StackTrace(); 
            StackFrame[] stackFrames = stackTrace.GetFrames();

            StackFrame callingFrame = stackFrames[1];

            MethodInfo method = (MethodInfo)callingFrame.GetMethod();
            this.Writer(tag, LogType.Debug, $"{method.DeclaringType.Name}.{method.Name}", format, arg);
        }

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
        public void Info(string tag, string format, params object[] arg)
        {
            if (!this.LogLevel.HasFlag(LogType.Info)) return;

            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();

            StackFrame callingFrame = stackFrames[1];
            if (callingFrame.GetMethod() is MethodInfo)
            {
                MethodInfo method = (MethodInfo) callingFrame.GetMethod();
                this.Writer(tag, LogType.Info, $"{method.DeclaringType.Name}.{method.Name}", format, arg);
            }
            else
            {
                this.Writer(tag, LogType.Info, $"UK", format, arg);
            }
        }

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
        public void Success(string tag, string format, params object[] arg)
        {
            if (!this.LogLevel.HasFlag(LogType.Success)) return;

            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();

            StackFrame callingFrame = stackFrames[1];

            MethodInfo method = (MethodInfo)callingFrame.GetMethod();
            this.Writer(tag, LogType.Success, $"{method.DeclaringType.Name}.{method.Name}", format, arg);
        }

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
        public void Error(string tag, string format, params object[] arg)
        {
            if (!this.LogLevel.HasFlag(LogType.Error)) return;

            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();

            StackFrame callingFrame = stackFrames[1];

            MethodInfo method = (MethodInfo)callingFrame.GetMethod();
            this.Writer(tag, LogType.Error, $"{method.DeclaringType.Name}.{method.Name}", format, arg);
        }

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
        public void Warning(string tag, string format, params object[] arg)
        {
            if (!this.LogLevel.HasFlag(LogType.Warning)) return;

            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();

            StackFrame callingFrame = stackFrames[1];

            MethodInfo method = (MethodInfo)callingFrame.GetMethod();
            this.Writer(tag, LogType.Warning, $"{method.DeclaringType.Name}.{method.Name}", format, arg);
        }

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
        public void Fatal(string tag, string format, params object[] arg)
        {
            if (!this.LogLevel.HasFlag(LogType.Fatal)) return;

            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();

            StackFrame callingFrame = stackFrames[1];

            MethodInfo method = (MethodInfo)callingFrame.GetMethod();
            this.Writer(tag, LogType.Fatal, $"{method.DeclaringType.Name}.{method.Name}", format, arg);
        }

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
        public void Exception(string tag, Exception e, string format, params object[] arg)
        {
            if (!this.LogLevel.HasFlag(LogType.Exception)) return;

            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();

            StackFrame callingFrame = stackFrames[1];

            MethodInfo method = (MethodInfo)callingFrame.GetMethod();
            this.Writer(tag,
                LogType.Exception,
                $"{method.DeclaringType.Name}.{method.Name}",
                string.Format(string.Format(format, arg) + " :{0}", e.Message + "\r\n" + e.StackTrace));
        }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use. 
        /// </summary>
        public void OnImportsSatisfied()
        {
        }

        /// <summary>
        /// The writer.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="source">
        /// source call log method
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg">
        /// The arg.
        /// </param>
        private async void Writer(string tag,LogType type, string source , string format, params object[] arg)
        {
            try
            {
                if (!this.LogLevel.HasFlag(type)) return;

                var data = string.Format(
                    "{0} [{4}] [{1}][{2}]: {3}",
                    DateTime.Now.ToString("dd-MM HH:mm:ss"),
                    type.ToString().ToUpper(),
                    source,
                    string.Format(format, arg),tag);

                foreach (var attaclLog in this._attaclLogs)
                {
                    try
                    {
                        await attaclLog.Writer(tag, type, data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void InitLogConfig(string log)
        {
            if (String.IsNullOrWhiteSpace(log)) return;

            try
            {
                LogType ltype = LogType.None;
                foreach (var item in log.Split('|'))
                {
                    ltype |= (LogType)Enum.Parse(typeof(LogType), item.Trim());
                }
                this.LogLevel = ltype;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}