using System.Text;
using System.Xml;

namespace LoginApp.Services
{
    public static class SoapService
    {
        private const string Endpoint = "http://isapi.mekashron.com/icu-tech/icutech-test.dll/soap/IICUTech";

        public static async Task<string> CallAsync(string methodName, string xmlBody)
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
            {
                Content = new StringContent(xmlBody, Encoding.UTF8, "text/xml")
            };

            // Встановлення правильного SOAPAction
            request.Headers.Add("SOAPAction", $"urn:ICUTech.Intf-IICUTech#{methodName}");

            try
            {
                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Логування відповіді
                string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"soap_raw_{methodName}_{DateTime.Now:yyyyMMddHHmmss}.xml");
                await File.WriteAllTextAsync(logFilePath, responseContent);

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                return $"HTTP Request Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static string ExtractReturnValue(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return "Empty response";

            if (xml.Contains("<!DOCTYPE HTML>") || xml.Contains("<html>") || xml.Contains("<body>"))
                return "Invalid XML response: HTML page received";

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                var returnNode = xmlDoc.GetElementsByTagName("return");
                if (returnNode == null || returnNode.Count == 0)
                    return "<return> tag not found";

                var returnValue = returnNode[0]?.InnerText.Trim() ?? string.Empty;
                return returnValue;
            }
            catch (XmlException ex)
            {
                return "XML parsing error: " + ex.Message;
            }
        }
    }
}
