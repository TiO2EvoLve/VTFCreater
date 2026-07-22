using System;
using System.Diagnostics;
using System.IO;
using VTFCreater.Enum;
using VTFCreater.Models;

namespace VTFCreater.Services;

public class VtfGenerator
{
    public void Generate(string vtfCmdPath, string sourceFilePath, string outputFilePath, Formats format)
    {
        if (!Directory.Exists(outputFilePath))
        {
            if (!string.IsNullOrEmpty(outputFilePath)) 
                Directory.CreateDirectory(outputFilePath);
        }

        var resolvedCmdPath = ResolvePath(vtfCmdPath);
        if (!File.Exists(resolvedCmdPath))
        {
            throw new FileNotFoundException($"找不到 VTFCmd.exe：{resolvedCmdPath}");
        }

        var cmd = new ProcessStartInfo
        {
            FileName = resolvedCmdPath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        cmd.ArgumentList.Add("-file");
        cmd.ArgumentList.Add(sourceFilePath);
        cmd.ArgumentList.Add("-output");
        cmd.ArgumentList.Add(outputFilePath);
        cmd.ArgumentList.Add("-format");
        cmd.ArgumentList.Add(format.ToString());

        using var process = Process.Start(cmd)
                              ?? throw new InvalidOperationException("无法启动 VTFCmd 进程。");

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var error = process.StandardError.ReadToEnd();
            throw new InvalidOperationException(
                $"VTF 转换失败（{Path.GetFileName(sourceFilePath)}，退出码 {process.ExitCode}）：{error}");
        }
    }

    private static string ResolvePath(string path)
    {
        return Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);
    }
}
