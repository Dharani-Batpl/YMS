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
using OfficeOpenXml;
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



        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<EmployeeModel> _validator;


        // Single constructor to inject both dependencies

        public EmployeeConfigurationController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<EmployeeModel> validator)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

        }


        // Action for loading the Employee Configuration page
        public async Task<IActionResult> Index()
        {
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
                //var userGroupName = await _apiClient.UserGroupAsync();
                //var UserName = await _apiClient.UserAsync();

                // Preparing the lists for dropdowns using a utility method
                ViewBag.StatusList = Utils.Utility.PrepareSelectList(statuses);
                ViewBag.EmployeeTypeList = Utils.Utility.PrepareSelectList(employeeType);
                ViewBag.DepartmentList = Utils.Utility.PrepareSelectList(departmentName);
                ViewBag.SkilllevelList = Utils.Utility.PrepareSelectList(skillLevel);
                ViewBag.SkillNameList = Utils.Utility.PrepareSelectList(skillType);
                ViewBag.CertificateTypeList = Utils.Utility.PrepareSelectList(certificateType);
                //ViewBag.UserGroupNameList = Utils.Utility.PrepareSelectList(userGroupName);
                //ViewBag.UserNameList = Utils.Utility.PrepareSelectList(UserName);

                // Passing the employee data to the view in JSON format
                ViewData["EmployeeConfiguration"] = jsonResult;

                // Returning the view for rendering
                return View();
            }
            catch (Exception ex)
            {
                // Handling any exceptions and returning an error response
                return StatusCode(500, ex.Message);
            }
        }

        // Action for creating a new employee record (POST method)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");

                // Inserting the new employee record using the API
                var result = await _apiClient.InsertEmployeeAsync(model);

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
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                model.Updated_at = DateTimeOffset.Now;

                // Updating the employee record using the API
                var result = await _apiClient.UpdateEmployeeAsync(model.Employee_id, model);

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
            try
            {
                model.updated_by = HttpContext.Session.GetString("LoginUser");

                // Deleting the employee record using the API
                var result = await _apiClient.DeleteEmployeeAsync(model.Employee_id);

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

        // Action for downloading an Excel template for user configuration
        public IActionResult DownloadUserConfigurationTemplate()
        {
            // Creating an in-memory stream to store the Excel file
            var stream = new MemoryStream();

            // Using OfficeOpenXml to create the Excel file
            using (var package = new ExcelPackage(stream))
            {
                // Adding a worksheet for the template
                var worksheet = package.Workbook.Worksheets.Add("EmployeeConfigurationTemplate");

                // Header columns to be added in the first row
                string[] headers = new string[]
                {
                    "Employee_id", "Employee_code", "First_name", "Middle_name",
                    "Last_name", "Department_id", "Department_name", "Reporting_to_id", "Reporting_to_name",
                    "Contact_number", "Email", "Employee_type", "User_id",
                    "User_name", "User_group_id", "User_group_name", "Skill_id", "Skill_type", "Skill_level_id",
                    "Skill_level_name", "Certification_date", "Expiry_date", "Status_id", "Status_name", "Is_deleted",
                    "Country_code","Emergency_country_code","Certificate_type_id","Certificate_type_name",
                    "Created_by", "Created_at", "Updated_by", "Updated_at"
                };

                // Adding headers to the worksheet and auto-fitting columns
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Column(i + 1).AutoFit();
                }

                // Saving the Excel file to the memory stream
                package.Save();
            }

            // Resetting the stream position and returning the file as a download
            stream.Position = 0;
            string excelName = $"EmployeeConfigurationTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        excelName);
        }

        // Utility method to prepare a SelectList from a list of DropdownModel items
        private List<SelectListItem> PrepareSelectList(IEnumerable<DropdownModel> dropdownModels)
        {
            return dropdownModels.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.Name
            }).ToList();
        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                // 0) Guards
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");
                var nameOk = Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);
                var type = (file.ContentType ?? "").ToLowerInvariant();
                var typeOk = type.Contains("csv") || type == "application/vnd.ms-excel";
                if (!nameOk && !typeOk)
                    return BadRequest("Only CSV files are allowed.");
                // 1) Parse + validate CSV
                var res = _csvUploadService.ProcessCsvFile<EmployeeModel>(file, _validator);
                // 2) Insert valid rows; collect API failures (no row numbers)
                var apiFailures = new List<(EmployeeModel Record, string Error)>();
                var successCount = 0;
                foreach (var item in res.ValidItems)
                {
                    try
                    {
                        await _apiClient.UploadEmployeeAsync(item);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        var msg = ex is ApiException<ResponseModel> apiEx && apiEx.Result != null
                            ? (apiEx.Result.Detail ?? apiEx.Message)
                            : ex.Message;
                        apiFailures.Add((item, msg ?? "API insert error"));
                    }
                }
                // 3) Build ONE CSV for all failures (validation + API),
                //    but include ONLY the columns that were uploaded (plus ErrorMessages)
                string invalidCsv = _csvUploadService.CreateUnifiedInvalidCsv(
                    res.InvalidItems,
                    apiFailures,
                    res.UploadedHeaders // << key: only uploaded columns are exported
                );
                // Encode to Base64 only if we have any failures
                string invalidBase64 = string.IsNullOrWhiteSpace(invalidCsv)
                    ? null
                    : Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidCsv));
                // 4) Prepare response
                var status = successCount > 0 ? "success" : "error";
                var title = successCount > 0 ? "Success" : "No Records";
                var message = successCount > 0
                    ? $"{successCount} records added successfully"
                    : "No valid records to insert.";
                return Ok(new
                {
                    status,
                    title,
                    message,
                    successCount,
                    validationFailedCount = res.InvalidCount,
                    apiFailedCount = apiFailures.Count,
                    invalidRecords = invalidBase64 // unified CSV (ErrorMessages + only uploaded columns)
                });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new
                {
                    status = "error",
                    title = "Client Closed Request",
                    message = "The upload was cancelled."
                });
            }
            //catch (ApiException<ProblemDetails> ex)
            //{
            //    var problem = ex.Result;
            //    return StatusCode(problem.Status ?? ex.StatusCode, new
            //    {
            //        status = problem.Status ?? ex.StatusCode,
            //        title = problem.Title ?? "Error",
            //        message = problem.Detail ?? "An unexpected error occurred."
            //    });
            //}

            catch (Exception ex)
            {
                if (ex is ApiException<ResponseModel> apiEx && apiEx.Result != null)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "Error",
                        message = apiEx.Result.Detail // <- Pass Detail here
                    });
                }

                return BadRequest(new
                {
                    status = "error",
                    title = "Error",
                    message = ex.Message
                });
            }


        }



    }
}
