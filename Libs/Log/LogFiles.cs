using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Log
{
    public class LogFiles : IAttackLog,IDisposable
    {
        private readonly ConcurrentQueue<Tuple<LogType, string>> _queue = new ConcurrentQueue<Tuple<LogType, string>>();
        private readonly CancellationTokenSource _cancelTaskHandle = new CancellationTokenSource();
        private string _path = HostingEnvironment.MapPath("~");
        private string GetFileNameFromType(LogType type)
        {
            var timePRefix = $"log{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}";

            var path =_path  + "\\Logs";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var fileName = string.Format("{2}\\{0}  {1}.txt", timePRefix, Enum.GetName(typeof (LogType), type), path);
            if (!File.Exists(fileName))
                File.Create(fileName).Close();
            return fileName;
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> Writer(string tag,LogType type, string log)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _queue.Enqueue(new Tuple<LogType, string>(type, log));
            return true;
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public LogFiles()
        {
            _path = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);
            Task.Factory.StartNew(HandleLog);
        }

        private async void HandleLog()
        {
            while (true)
            {
                if (!_queue.IsEmpty)
                {
                    //add to hash by log type 
                    Dictionary<LogType, List<String>> slog = new Dictionary<LogType, List<String>>();
                    Tuple<LogType, string> data;
                    for (int i = 0; i < 100; i++)//ghi mỗi lần 100 dòng nếu có đủ
                    {
                        if (_queue.IsEmpty) break;
                        if (_queue.TryDequeue(out data))
                        {
                            if (slog.ContainsKey(data.Item1)) slog[data.Item1].Add(data.Item2);
                            else
                                slog.Add(data.Item1, new List<String>{data.Item2});
                        }
                    }

                    //write to file
                    foreach (var logtype in slog.Keys)
                    {
                        try
                        {
                            var file = GetFileNameFromType(logtype);
                            var f = new StreamWriter(file, true);
                            foreach (var logline in slog[logtype])
                                f.WriteLine(logline);
                            f.Flush();
                            f.Dispose();
                        }
                        catch
                        {
                        }
                    }

                    slog.Clear(); slog = null;
                }

                if(_cancelTaskHandle.IsCancellationRequested)
                    return;

                await Task.Delay(1);
            }
        }

        public void Dispose()
        {
            _cancelTaskHandle.Cancel();
        }

    }
}