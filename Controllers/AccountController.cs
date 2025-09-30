using Microsoft.AspNetCore.Mvc;
using LoginApp.Models;
using System.Text;

namespace LoginApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string soapEnvelope = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
  <soapenv:Header/>
  <soapenv:Body>
    <tem:Login>
      <tem:Username>{model.Username}</tem:Username>
      <tem:Password>{model.Password}</tem:Password>
    </tem:Login>
  </soapenv:Body>
</soapenv:Envelope>";

            using var client = new HttpClient();
            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
            client.DefaultRequestHeaders.Add("SOAPAction", "Login");

            try
            {
                var response = await client.PostAsync("http://isapi.mekashron.com/icu-tech/icutech-test.dll", content);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && result.Contains("Success"))
                {
                    ViewBag.Message = "✅ Login successful!";
                    ViewBag.AlertType = "success";
                }
                else
                {
                    ViewBag.Message = "❌ Login failed!";
                    ViewBag.AlertType = "danger";
                }
            }
            catch
            {
                ViewBag.Message = "⚠️ Error connecting to service.";
                ViewBag.AlertType = "danger";
            }

            return View(model);
        }
    }
}