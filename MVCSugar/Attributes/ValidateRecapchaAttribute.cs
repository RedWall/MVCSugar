using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RedWall.MVCSugar.Attributes
{
    public class ValidateReCaptchaAttribute : ActionFilterAttribute
    {
        public static string RecaptchaSecret { get; set; }

        private class ReCaptchaResult
        {
            public bool Success { get; set; }
            public DateTime Challenge_TS { get; set; }
            public string Hostname { get; set; }
            public List<string> Error_Codes { get; set; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrWhiteSpace(RecaptchaSecret))
                throw new NullReferenceException($"The static {nameof(RecaptchaSecret)} property must be set prior to validation");

            var recaptchaResponse = filterContext.RequestContext.HttpContext.Request["g-recaptcha-response"];

            if (string.IsNullOrWhiteSpace(recaptchaResponse))
            {
                AddModelError(filterContext, "ReCaptcha is required");
                return;
            }

            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={RecaptchaSecret}&response={recaptchaResponse}").Result;

                response.EnsureSuccessStatusCode();

                var captchaResponse = response.Content.ReadAsAsync<ReCaptchaResult>().Result;

                if (!captchaResponse.Success)
                {
                    string errorMessage = "Error occured. Please try again";

                    if (captchaResponse.Error_Codes.Any())
                    {
                        var error = captchaResponse.Error_Codes[0].ToLower();

                        switch (error)
                        {
                            case ("missing-input-secret"):
                                errorMessage = "The secret parameter is missing.";
                                break;
                            case ("invalid-input-secret"):
                                errorMessage = "The secret parameter is invalid or malformed.";
                                break;

                            case ("missing-input-response"):
                                errorMessage = "The response parameter is missing.";
                                break;
                            case ("invalid-input-response"):
                                errorMessage = "The response parameter is invalid or malformed.";
                                break;
                        }
                    }

                    AddModelError(filterContext, errorMessage);
                }
            }
        }

        private void AddModelError(ActionExecutingContext filterContext, string errorMessage)
        {
            ((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", errorMessage);
        }
    }

}
