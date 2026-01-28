using Infotecs2026.Share.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Infotecs2026.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ValueController : Controller
{
    private readonly ILogger<ValueController> _logger;

    public ValueController(ILogger<ValueController> logger)
    {
        _logger = logger;
    }

    [HttpPost("upload-csv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploudFile([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length <= 0)
                return BadRequest("Файл не выбран");

            //string FileName = file.FileName;

            //if (!Path.GetExtension(FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            //    return BadRequest("Поддерживаются только CSV файлы");

            return Ok(new { Message = "" });
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Неожиданное исключение при обработке события {EventId}", @event.Id);
            return StatusCode(500, new { Message = "Неожиданное исключение при попытке загрузить файл" });
        }
    }
}
