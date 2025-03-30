using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System;

namespace SysBot.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(int lines = 100)
    {
        try
        {
            // Versuche zuerst, die Standard-Log-Datei zu lesen
            var standardLogPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), "SysBotLog.txt");
            
            // Wenn diese nicht existiert, versuche die Port-spezifische Log-Datei zu lesen
            if (!System.IO.File.Exists(standardLogPath))
            {
                var currentPort = WebApiIntegration.GetCurrentPort();
                var portLogPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), $"SysBotLog_Port{currentPort}.txt");
                
                if (!System.IO.File.Exists(portLogPath))
                    return NotFound(new { success = false, message = $"Keine Logs verfügbar. Log-Datei nicht gefunden: {standardLogPath} oder {portLogPath}" });
                
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
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Fehler beim Abrufen der Logs: {ex.Message}" });
        }
    }

    [HttpGet("latest")]
    public IActionResult GetLatest(int count = 10)
    {
        try
        {
            // Versuche zuerst, die Standard-Log-Datei zu lesen
            var standardLogPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), "SysBotLog.txt");
            
            // Wenn diese nicht existiert, versuche die Port-spezifische Log-Datei zu lesen
            if (!System.IO.File.Exists(standardLogPath))
            {
                var currentPort = WebApiIntegration.GetCurrentPort();
                var portLogPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), $"SysBotLog_Port{currentPort}.txt");
                
                if (!System.IO.File.Exists(portLogPath))
                    return NotFound(new { success = false, message = $"Keine Logs verfügbar. Log-Datei nicht gefunden: {standardLogPath} oder {portLogPath}" });
                
                var portLogLines = System.IO.File.ReadLines(portLogPath)
                                 .TakeLast(count)
                                 .ToList();
                return Ok(portLogLines);
            }
            
            var logLines = System.IO.File.ReadLines(standardLogPath)
                            .TakeLast(count)
                            .ToList();
            return Ok(logLines);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Fehler beim Abrufen der Logs: {ex.Message}" });
        }
    }
    
    [HttpGet("port")]
    public IActionResult GetPortLogs(int lines = 100)
    {
        try
        {
            var currentPort = WebApiIntegration.GetCurrentPort();
            var portLogPath = Path.Combine(WebApiIntegration.GetLogDirectoryPath(), $"SysBotLog_Port{currentPort}.txt");
            
            if (!System.IO.File.Exists(portLogPath))
                return NotFound(new { success = false, message = $"Port-spezifische Log-Datei nicht gefunden: {portLogPath}" });
            
            var logLines = System.IO.File.ReadLines(portLogPath)
                            .TakeLast(lines)
                            .ToList();
            return Ok(logLines);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Fehler beim Abrufen der Logs: {ex.Message}" });
        }
    }
} 