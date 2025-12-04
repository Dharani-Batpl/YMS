using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace YardManagementApplication.Controllers
{
    [ApiController]
    [Route("Common")]
    public class CommonController : Controller
    {
        [HttpGet("GetCountryCodes")]
        public IActionResult GetCountryCodes()
        {
            try
            {
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "assets",
                    "country-codes.csv");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("country-codes.csv not found in wwwroot/assets.");
                }

                var list = new List<CountryCodeModel>();
                var lines = System.IO.File.ReadAllLines(filePath);

                foreach (var line in lines.Skip(1)) // skip header
                {
                    var parts = line.Split(',');

                    if (parts.Length < 2) continue;

                    list.Add(new CountryCodeModel
                    {
                        Code = parts[0].Trim(),
                        Dial = parts[1].Trim()
                    });
                }

                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }

    public class CountryCodeModel
    {
        public string Code { get; set; } = "";
        public string Dial { get; set; } = "";

        public string Name { get; set; }
    }
}
