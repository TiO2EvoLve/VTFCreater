using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VTFCreater.Models;

namespace VTFCreater.Services;

//日志内容
public class LogService
{
    public ObservableCollection<LogEntry> Entries { get; } = [];

    public void Info(string message) => Add("INFO", message);

    public void Warn(string message) => Add("WARN", message);

    public void Error(string message) => Add("ERROR", message);

    public void Clear() => Entries.Clear();

    private void Add(string level, string message)
    {
        Entries.Add(new LogEntry
        {
            Level = level,
            Message = message,
            Timestamp = DateTime.Now.ToString("HH:mm:ss")
        });
    }
}
