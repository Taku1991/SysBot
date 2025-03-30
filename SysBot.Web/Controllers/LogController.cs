using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System;

namespace SysBot.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    [HttpGet]
    public IActionResult GetLogs(int lines = 100)
    {
        // Versuche zuerst, die Standard-Log-Datei zu lesen
        var standardLogPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "logs", "SysBotLog.txt");
        
        // Wenn diese nicht existiert, versuche die Port-spezifische Log-Datei zu lesen
        if (!System.IO.File.Exists(standardLogPath))
        {
            var currentPort = WebApiIntegration.GetCurrentPort();
            var portLogPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "logs", $"SysBotLog_Port{currentPort}.txt");
            
            if (!System.IO.File.Exists(portLogPath))
                return NotFound(new { message = $"Keine Logs verfügbar. Log-Datei nicht gefunden: {standardLogPath} oder {portLogPath}" });
            
            var portLogLines = System.IO.File.ReadLines(portLogPath)
                             .TakeLast(lines)
                             .ToList();
            return Ok(portLogLines);
        }
        
        var logLines = System.IO.File.ReadLines(standardLogPath)
                         .TakeLast(lines)
                         .ToList();
        return Ok(logLines);
    }

    [HttpGet("files")]
    public IActionResult GetLogFiles()
    {
        var logPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "logs");
        if (!Directory.Exists(logPath))
            return NotFound(new { message = "Log-Verzeichnis nicht gefunden: " + logPath });

        var files = Directory.GetFiles(logPath, "*.txt")
                    .Select(f => new
                    {
                        Name = Path.GetFileName(f),
                        Size = new FileInfo(f).Length,
                        LastModified = new FileInfo(f).LastWriteTime
                    })
                    .OrderByDescending(f => f.LastModified)
                    .ToList();

        return Ok(files);
    }

    [HttpGet("file/{fileName}")]
    public IActionResult GetLogFile(string fileName, int lines = 100)
    {
        var logPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "logs", fileName);
        if (!System.IO.File.Exists(logPath))
            return NotFound(new { message = $"Log-Datei {fileName} nicht gefunden: " + logPath });

        var logLines = System.IO.File.ReadLines(logPath)
                         .TakeLast(lines)
                         .ToList();
        return Ok(logLines);
    }
    
    [HttpGet("port")]
    public IActionResult GetPortLogs(int lines = 100)
    {
        var currentPort = WebApiIntegration.GetCurrentPort();
        var portLogPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "logs", $"SysBotLog_Port{currentPort}.txt");
        
        if (!System.IO.File.Exists(portLogPath))
            return NotFound(new { message = $"Port-spezifische Log-Datei nicht gefunden: {portLogPath}" });
        
        var logLines = System.IO.File.ReadLines(portLogPath)
                         .TakeLast(lines)
                         .ToList();
        return Ok(logLines);
    }
} 