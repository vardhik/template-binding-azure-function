using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TemplateGenerate
{
    public class TemplateFunction
    {
        private readonly RazorViewToStringRenderer razorViewToStringRenderer;
        public TemplateFunction(RazorViewToStringRenderer razorViewToStringRenderer)
        {
            this.razorViewToStringRenderer = razorViewToStringRenderer;

        }
        [FunctionName("templategenerate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "templategenerate/{controller}/{action}/{viewmodel?}")]
            HttpRequest req,
            string controller,
            string action,
            string viewmodel,
            ILogger log)
        {
            //string resulttype = req.Query["resulttype"];
            log.LogInformation($"HTTP trigger {controller}/{action} function processed a request.");
            try
            {
                //get ViewModel type on basis of paramerter
                var modelType = ViewModelMap.GetModel[viewmodel];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var model = JsonConvert.DeserializeObject(requestBody, modelType);
                if (model == null)
                    return new BadRequestObjectResult("Invalid request body type");

                var htmlContent = await razorViewToStringRenderer.RenderViewToStringAsync(model, $"{action}.cshtml", $"/{controller}/");
                return new OkObjectResult(htmlContent);
            }
            catch (System.Exception ex)
            {
                log.LogInformation($"Error: \t {ex.Message}");
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
