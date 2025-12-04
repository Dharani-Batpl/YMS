// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : EmployeeConfigurationController.cs
// Module      : Masters
// Author      : Kavin R
// Created On  : 2025-10-14
// Description : Controller for managing Employee master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Kavin R   | Initial creation based on Employee Master Model structure.
// 1.1     | 2025-11-28 | Dharani T | Modifications in UI and logics
// =================================================================================================

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;


namespace YardManagementApplication
{
    // Define the route for all actions in this controller
    [Route("[controller]/[action]")]
    public class EmployeeConfigurationController : Controller
    {
        // Injecting the API client for communication with the backend API
        private readonly v1Client _apiClient;

        private readonly ILogger<EmployeeConfigurationController> _logger;

        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<EmployeeModel> _validator;


        // Single constructor to inject both dependencies

        public EmployeeConfigurationController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<EmployeeModel> validator, ILogger<EmployeeConfigurationController> logger)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

            _logger = logger;

        }


        // Action for loading the Employee Configuration page
        public async Task<IActionResult> Index()
        {

            // Log action start
            string controller = nameof(EmployeeConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


            try
            {
                // Set the page title for the view
                ViewData["Title"] = "EmployeeConfiguration";


                // Fetching the list of all employees from the API
                var result = await _apiClient.GetAllEmployeesAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<EmployeeModel>
                           {
                               new EmployeeModel()
                           };
                }
                // Serializing the result to a JSON string to pass to the view
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Fetching dropdown data from the API (statuses, employee types, departments, etc.)
                var statuses = await _apiClient.UserStatusAsync();
                var employeeType = await _apiClient.EmployeeTypeAsync();
                var departmentName = await _apiClient.DepartmentAsync();
                var skillType = await _apiClient.EmployeeSkillAsync();
                var skillLevel = await _apiClient.EmployeeSkillLevelAsync();
                var certificateType = await _apiClient.CertificateTypeAsync();
              
                // Preparing the lists for dropdowns using a utility method
                ViewBag.StatusList = Utils.Utility.PrepareSelectList(statuses);
                ViewBag.EmployeeTypeList = Utils.Utility.PrepareSelectList(employeeType);
                ViewBag.DepartmentList = Utils.Utility.PrepareSelectList(departmentName);
                ViewBag.SkilllevelList = Utils.Utility.PrepareSelectList(skillLevel);
                ViewBag.SkillNameList = Utils.Utility.PrepareSelectList(skillType);
                ViewBag.CertificateTypeList = Utils.Utility.PrepareSelectList(certificateType);

                // Passing the employee data to the view in JSON format
                ViewData["EmployeeConfiguration"] = jsonResult;


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, jsonResult?.Count() ?? 0
                );


                // Returning the view for rendering
                return View();
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                // Handling any exceptions and returning an error response
                return StatusCode(500, ex.Message);
            }
        }

        // Action for creating a new employee record (POST method)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeModel model)
        {
            // Log action start
            string controller = nameof(EmployeeConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            try
            {
                _logger.LogInformation(
                    "[ACTION START] {controller}.{action} | User={user}",
                    controller, action, user
                );


                model.Created_by = HttpContext.Session.GetString("LoginUser");    

                // Inserting the new employee record using the API
                var result = await _apiClient.InsertEmployeeAsync(model);

                if (model.App_user == true)
                {
                    var userModel = new AppUserInsertModel
                    {
                        User_name = model.Employee_code, 
                        Email = model.Email,
                        Password = "test@123",
                        Is_deleted = false,
                        Created_by = HttpContext.Session.GetString("LoginUser"),
                        Created_at = DateTimeOffset.Now
                    };


                    await _apiClient.InsertAppUserAsync(userModel);
                }

                // Returning a success response with status and message
                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record Inserted successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        // Action for updating an existing employee record (PUT method)
        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] EmployeeUpdateModel model)
        {
            // Log action start
            string controller = nameof(EmployeeConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                model.Updated_at = DateTimeOffset.Now;

                // Updating the employee record using the API
                var result = await _apiClient.UpdateEmployeeAsync(model.Employee_id, model);

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | Updated={result}",
                    controller, action, result
                );


                // Returning a success response
                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );

                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        // Action for deleting an employee record (PUT method)
        [HttpPut()]
        public async Task<IActionResult> Delete([FromBody] EmployeeDeleteModel model)
        {
            // Log action start
            string controller = nameof(EmployeeConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            try
            {
                model.updated_by = HttpContext.Session.GetString("LoginUser");

                // Deleting the employee record using the API
                var result = await _apiClient.DeleteEmployeeAsync(model.Employee_id);

                _logger.LogInformation(
                       "[ACTION INFO] {controller}.{action} | Updated={result}",
                       controller, action, result
                   );



                // Returning a success response
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record Deleted successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );

                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        //-----------------------------------------------
        // UPLOAD TEMPLATE DATA VIA EXCEL
        // -----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Log action start
            string controller = nameof(EmployeeConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            try
            {
                // ---------------------------------------------------------
                // 1) Validate uploaded file
                // ---------------------------------------------------------
                if (file == null || file.Length == 0)
                    return BadRequest(new { status = "error", title = "No File", message = "No file uploaded." });

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".xlsx" && ext != ".xls")
                    return BadRequest(new { status = "error", title = "Invalid File", message = "Only Excel files allowed." });


                // ---------------------------------------------------------
                // 2) Extract excel data
                // ---------------------------------------------------------
                List<Dictionary<string, string>> excelRows = new();
                List<string> headers = new();

                using (var stream = file.OpenReadStream())
                {
                    IWorkbook wb = ext == ".xlsx" ? new XSSFWorkbook(stream) : new HSSFWorkbook(stream);
                    var sheet = wb.GetSheetAt(0);

                    var header = sheet.GetRow(0);
                    for (int i = 0; i < header.LastCellNum; i++)
                        headers.Add(header.GetCell(i)?.ToString()?.Trim() ?? $"Col{i}");

                    for (int r = 1; r <= sheet.LastRowNum; r++)
                    {
                        var row = sheet.GetRow(r);
                        if (row == null) continue;

                        var dict = new Dictionary<string, string>();
                        for (int c = 0; c < headers.Count; c++)
                            dict[headers[c]] = row.GetCell(c)?.ToString()?.Trim() ?? "";

                        excelRows.Add(dict);
                    }
                }

                if (excelRows.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "No Data",
                        message = "Excel template contains no data rows."
                    });
                }


                // ---------------------------------------------------------
                // 3) Normalize column names
                // ---------------------------------------------------------
                string norm(string x) =>
                    x.Trim().ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");

                
                string currentUser = HttpContext.Session.GetString("LoginUser") ?? "System";
                List<object> errorList = new();
                int successCount = 0;
                int rowIndex = 2;

                foreach (var row in excelRows)
                {

                    try
                    {

                    
                    var map = new Dictionary<string, string>();

                    foreach (var kv in row)
                    {
                        switch (norm(kv.Key))
                        {
                            case "employeecode": map["employee_code"] = kv.Value; break;
                            case "firstname": map["first_name"] = kv.Value; break;
                            case "middlename": map["middle_name"] = kv.Value; break;
                            case "lastname": map["last_name"] = kv.Value; break;
                            case "department": map["department_name"] = kv.Value; break;
                            case "reportingto": map["reporting_to_name"] = kv.Value; break;
                            case "countrycode": map["cc"] = kv.Value; break;
                            case "phonenumber": map["phone"] = kv.Value; break;
                            case "emergencycountrycode": map["ecc"] = kv.Value; break;
                            case "emergencycontactnumber": map["ephone"] = kv.Value; break;
                            case "email": map["email"] = kv.Value; break;
                            case "skilltype": map["skill_type"] = kv.Value; break;
                            case "skilllevel": map["skill_level"] = kv.Value; break;
                            case "certificatetype": map["certificate_type"] = kv.Value; break;
                            case "certificateissuedate": map["cert_issue"] = kv.Value; break;
                            case "certificateexpirydate": map["cert_expiry"] = kv.Value; break;
                            case "bloodgroup": map["blood_group"] = kv.Value; break;
                            case "employmenttype": map["employment_type"] = kv.Value; break;
                            case "status": map["status"] = kv.Value; break;
                            case "address": map["address"] = kv.Value; break;
                            case "isappuser(y/n)": map["app"] = kv.Value; break;
                        }
                    }

                    bool appuser = (map.GetValueOrDefault("app") ?? "").Trim().ToLower() == "y";

                    var model = new EmployeeModel
                    {
                        Employee_code = map.GetValueOrDefault("employee_code"),
                        First_name = map.GetValueOrDefault("first_name"),
                        Middle_name = map.GetValueOrDefault("middle_name"),
                        Last_name = map.GetValueOrDefault("last_name"),
                        Department_name = map.GetValueOrDefault("department_name"),
                        Reporting_to_name = map.GetValueOrDefault("reporting_to_name"),
                        Address = map.GetValueOrDefault("address"),
                        Contact_number = $"+{map.GetValueOrDefault("cc") ?? ""}-{map.GetValueOrDefault("phone") ?? ""}",
                        Emergency_contact_number = $"+{map.GetValueOrDefault("ecc") ?? ""}-{map.GetValueOrDefault("ephone") ?? ""}",
                        Skill_type = map.GetValueOrDefault("skill_type"),
                        Skill_level_name = map.GetValueOrDefault("skill_level"),
                        Certificate_name = map.GetValueOrDefault("certificate_type"),
                        Employee_type = map.GetValueOrDefault("employment_type"),
                        Status_name = map.GetValueOrDefault("status"),
                        Blood_group = map.GetValueOrDefault("blood_group"),
                        Email = map.GetValueOrDefault("email"),
                        App_user = appuser,
                        Created_by = currentUser,
                        Created_at = DateTime.Now
                    };

                    DateTime tmp;
                    if (DateTime.TryParse(map.GetValueOrDefault("cert_issue"), out tmp)) model.Certification_date = tmp;
                    if (DateTime.TryParse(map.GetValueOrDefault("cert_expiry"), out tmp)) model.Expiry_date = tmp;

                    
                        await _apiClient.UploadEmployeeAsync(model);
                        successCount++;
                    }
                    catch (ApiException<ResponseModel> ex)
                    {
                        errorList.Add(new { Row = rowIndex, Error = ex.Result?.Detail ?? ex.Message });
                    }
                    catch (ApiException<ProblemDetails> ex) when (ex.StatusCode == 409 || ex.StatusCode == 400)
                    {
                        errorList.Add(new { Row = rowIndex, Error = ex.Result?.Detail ?? "Duplicate / Missing Data" });
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(new { Row = rowIndex, Error = ex.Message });
                    }

                    rowIndex++;
                }


                // Generate CSV if there are errors
                string downloadUrl = null;
                if (errorList.Count > 0)
                {
                    var csvLines = new List<string> { "Row No,Error Message" };
                    csvLines.AddRange(errorList.Select(e =>
                        $"\"{e.GetType().GetProperty("Row").GetValue(e)}\",\"{e.GetType().GetProperty("Error").GetValue(e)}\""));

                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "ErrorReports");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string filename = $"Employee_ErrorReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    string fullPath = Path.Combine(folder, filename);
                    System.IO.File.WriteAllLines(fullPath, csvLines, Encoding.UTF8);

                    downloadUrl = Url.Content($"~/uploads/ErrorReports/{filename}");
                }

                // Return JSON response with link
                return Ok(new
                {
                    status = "success",
                    title = "Upload Completed",
                    message = $"Success: {successCount}, Failed: {errorList.Count}.",
                    downloadCsv = downloadUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ACTION ERROR] {controller}.{action}", controller, action);
                return BadRequest(new { status = "error", title = "Exception", message = ex.Message });
            }
        }


    }
}
