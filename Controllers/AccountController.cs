using Microsoft.AspNetCore.Mvc;
using LoginApp.Models;
using LoginApp.Services;
using System.Text.Json;

namespace LoginApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginModel
            {
                IPs = HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
            };
            return View(model);
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            Console.WriteLine("=== Login POST called ===");

            if (!ModelState.IsValid)
    {
        Console.WriteLine("ModelState invalid:");
        foreach (var kvp in ModelState)
        {
            foreach (var err in kvp.Value.Errors)
                Console.WriteLine($" - {kvp.Key}: {err.ErrorMessage}");
        }
        return View(model);
    }

            model.IPs = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var xmlBody = BuildLoginEnvelope(model);

            // Логування запиту
            string loginRequestFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"soap_request_login_{DateTime.Now:yyyyMMddHHmmss}.xml");
            await System.IO.File.WriteAllTextAsync(loginRequestFilePath, xmlBody);
            Console.WriteLine("=== SOAP CALL finished ===");

            // Виклик SOAP-сервісу
            var xmlResponse = await SoapService.CallAsync("Login", xmlBody);

            // Логування відповіді
            string loginResponseFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"soap_response_login_{DateTime.Now:yyyyMMddHHmmss}.xml");
            await System.IO.File.WriteAllTextAsync(loginResponseFilePath, xmlResponse);

            // Витягування значення з XML-відповіді
            var returnValue = SoapService.ExtractReturnValue(xmlResponse);

            // Обробка помилок
            if (IsErrorResponse(returnValue))
                return ShowMessage(returnValue, false, model, xmlResponse, returnValue);

            // Обробка JSON-відповіді
            if (IsJson(returnValue))
            {
                try
                {
                    var result = JsonSerializer.Deserialize<LoginResult>(returnValue);
                    if (result == null)
                        return ShowMessage("Deserialization returned null", false, model, xmlResponse, returnValue);

                    bool success = result.ResultCode == 0;
                    string message = BuildMessage(result.ResultMessage, success, "Login");

                    if (success)
                    {
                        TempData["LoginSuccessMessage"] = message;
                        return RedirectToAction("Index", "Home");
                    }
                    return ShowMessage(message, false, model, xmlResponse, returnValue);
                }
                catch (JsonException je)
                {
                    return ShowMessage($"JSON parsing error: {je.Message}", false, model, xmlResponse, returnValue);
                }
            }
            return ShowMessage(returnValue, returnValue.Contains("success"), model, xmlResponse, returnValue);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterModel
            {
                SignupIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
            };
            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.SignupIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var xmlBody = BuildRegisterEnvelope(model);

            // Логування запиту
            string registerRequestFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"soap_request_register_{DateTime.Now:yyyyMMddHHmmss}.xml");
            await System.IO.File.WriteAllTextAsync(registerRequestFilePath, xmlBody);

            // Виклик SOAP-сервісу
            var xmlResponse = await SoapService.CallAsync("RegisterNewCustomer", xmlBody);

            // Логування відповіді
            string registerResponseFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"soap_response_register_{DateTime.Now:yyyyMMddHHmmss}.xml");
            await System.IO.File.WriteAllTextAsync(registerResponseFilePath, xmlResponse);

            // Витягування значення з XML-відповіді
            var returnValue = SoapService.ExtractReturnValue(xmlResponse);

            // Обробка помилок
            if (IsErrorResponse(returnValue))
                return ShowMessage(returnValue, false, model, xmlResponse, returnValue);

            // Обробка JSON-відповіді
            if (IsJson(returnValue))
            {
                try
                {
                    var result = JsonSerializer.Deserialize<RegisterResult>(returnValue);
                    if (result == null)
                        return ShowMessage("Deserialization returned null", false, model, xmlResponse, returnValue);

                    bool success = result.ResultCode == 0 || result.EntityId > 0;
                    string message = BuildMessage(result.ResultMessage, success, "Registration", result.EntityId);

                    return ShowMessage(message, success, model, xmlResponse, returnValue);
                }
                catch (JsonException je)
                {
                    return ShowMessage($"JSON parsing error: {je.Message}", false, model, xmlResponse, returnValue);
                }
            }
            return ShowMessage(returnValue, returnValue.Contains("success"), model, xmlResponse, returnValue);
        }

        // Метод для формування повідомлень
        private static string BuildMessage(string resultMessage, bool success, string operation, int entityId = 0)
        {
            if (!string.IsNullOrWhiteSpace(resultMessage))
                return resultMessage;

            if (success && operation == "Registration")
                return $"Registration successful! EntityId: {entityId}";

            return success ? $"{operation} successful." : $"{operation} failed. No message returned.";
        }

        // Метод для перевірки помилок
        private static bool IsErrorResponse(string value) =>
            string.IsNullOrWhiteSpace(value) ||
            value.StartsWith("Invalid XML response") ||
            value.StartsWith("XML parsing error") ||
            value.StartsWith("HTTP Request Error") ||
            value.StartsWith("Error");

        // Метод для перевірки JSON
        private static bool IsJson(string value) =>
            !string.IsNullOrWhiteSpace(value) && value.TrimStart().StartsWith("{");

        // Метод для відображення повідомлень
        private IActionResult ShowMessage(string message, bool success, object model, string rawXml, string rawReturn)
        {
            ViewBag.Message = message;
            ViewBag.AlertType = success ? "success" : "danger";
            ViewBag.RawXml = rawXml;
            ViewBag.RawReturn = rawReturn;
            return View(model);
        }

        // Метод для формування XML-запиту для логіну
        private static string BuildLoginEnvelope(LoginModel m) => $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <Login xmlns=""urn:ICUTech.Intf-IICUTech"">
      <UserName>{m.Username}</UserName>
      <Password>{m.Password}</Password>
      <IPs>{m.IPs}</IPs>
    </Login>
  </soap:Body>
</soap:Envelope>";

        // Метод для формування XML-запиту для реєстрації
        private static string BuildRegisterEnvelope(RegisterModel m) => $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <RegisterNewCustomer xmlns=""urn:ICUTech.Intf-IICUTech"">
      <Email>{m.Email}</Email>
      <Password>{m.Password}</Password>
      <FirstName>{m.FirstName}</FirstName>
      <LastName>{m.LastName}</LastName>
      <Mobile>{m.MobileNo}</Mobile>
      <CountryID>{m.CountryID}</CountryID>
      <aID>0</aID>
      <SignupIP>{m.SignupIP}</SignupIP>
    </RegisterNewCustomer>
  </soap:Body>
</soap:Envelope>";
    }
}
