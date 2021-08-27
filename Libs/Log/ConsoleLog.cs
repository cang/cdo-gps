#region header

// /*********************************************************************************************/
// Project :Log
// FileName : Class1.cs
// Time Create : 8:38 AM 13/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Log
{
    //[Export(typeof(IAttackLog))]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public class ConsoleLog : IAttackLog
    {
        /// <summary>
        ///     The _cancel token.
        /// </summary>
        private readonly CancellationTokenSource _cancelToken;

        /// <summary>
        ///     The data.
        /// </summary>
        private readonly Queue<Tuple<LogType, string>> _data = new Queue<Tuple<LogType, string>>();

        /// <summary>
        ///     The datalog.
        /// </summary>
        private readonly object _datalog = new object();

        public ConsoleLog()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Title = AppDomain.CurrentDomain.FriendlyName;


            _cancelToken = new CancellationTokenSource();
            Task.Factory.StartNew(async () => await Display());
        }


        public void Clear()
        {
            lock (_datalog)
            {
                _data.Clear();
                Console.Clear();
            }
        }


        /// <summary>
        ///     The display.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        private async Task<bool> Display()
        {
            while (true)
            {
                try
                {
                    if (_cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (_data.Count > 0)
                    {
                        Tuple<LogType, string> log;
                        lock (_datalog)
                        {
                            log = _data.Dequeue();
                            if (_data.Count > 1000)
                                _data.Clear();
                        }

                        if (log != null)
                        {
                            PrintLog(log.Item1, log.Item2);
                        }
                    }


                    await Task.Delay(10, _cancelToken.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return true;
        }

        /// <summary>
        ///     The print log.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        private void PrintLog(LogType type, string data)
        {
            var backupForceColor = Console.ForegroundColor;

            switch (type)
            {
                case LogType.Debug:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Exception:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogType.Fatal:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogType.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            Console.WriteLine(data);

            Console.ForegroundColor = backupForceColor;
        }

        #region Implementation of IAttackLog

        /// <summary>
        ///     hàm nhận thông tin log từ trình quản lý
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="log">
        ///     log data
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
#pragma warning disable 1998
        public async Task<bool> Writer(string tag, LogType type, string log)
#pragma warning restore 1998
        {
            lock (_datalog)
            {
                _data.Enqueue(new Tuple<LogType, string>(type, log));
            }
            return true;
        }

        /// <summary>
        ///     cài đặt đường dẫn cần lưu log
        /// </summary>
        /// <param name="path"></param>
        public void SetPath(string path)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}