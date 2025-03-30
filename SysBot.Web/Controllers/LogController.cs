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
        var standardLogPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), "SysBotLog.txt");
        
        // Wenn diese nicht existiert, versuche die Port-spezifische Log-Datei zu lesen
        if (!System.IO.File.Exists(standardLogPath))
        {
            var currentPort = WebApiIntegration.GetCurrentPort();
            var portLogPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), $"SysBotLog_Port{currentPort}.txt");
            
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
        var logPath = WebApiIntegration.GetLogDirectoryPath();
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
        var logPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), fileName);
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
        var portLogPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), $"SysBotLog_Port{currentPort}.txt");
        
        if (!System.IO.File.Exists(portLogPath))
            return NotFound(new { message = $"Port-spezifische Log-Datei nicht gefunden: {portLogPath}" });
        
        var logLines = System.IO.File.ReadLines(portLogPath)
                         .TakeLast(lines)
                         .ToList();
        return Ok(logLines);
    }
    
    [HttpPost("setpath")]
    public IActionResult SetLogPath([FromBody] LogPathModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(model.Path))
                return BadRequest(new { success = false, message = "Der Pfad darf nicht leer sein." });
            
            if (!Directory.Exists(model.Path))
            {
                // Versuche auch, den Pfad mit hinzugefügtem /logs zu prüfen
                var logsPath = Path.Combine(model.Path, "logs");
                if (Directory.Exists(logsPath))
                {
                    WebApiIntegration.SetCustomLogPath(logsPath);
                    return Ok(new { success = true, message = $"Log-Pfad auf {logsPath} gesetzt" });
                }
                
                return BadRequest(new { success = false, message = $"Das Verzeichnis {model.Path} existiert nicht." });
            }
            
            WebApiIntegration.SetCustomLogPath(model.Path);
            return Ok(new { success = true, message = $"Log-Pfad auf {model.Path} gesetzt" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Fehler beim Setzen des Log-Pfads: {ex.Message}" });
        }
    }
}

public class LogPathModel
{
    public string Path { get; set; } = string.Empty;
} 