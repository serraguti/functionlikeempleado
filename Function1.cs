using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionLikeEmpleado
{
    public static class Function1
    {
        [FunctionName("functionlikeemp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post"
            , Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("App para dar Like a Empleados");

            string empno = req.Query["empno"];
            //SIRVE PARA RECUPERAR EL CONTENIDO DEL EMPLEADO
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            empno = empno ?? data?.name;
            if (empno == null)
            {
                return new BadRequestObjectResult("Necesitamos el parámetro de {empno}");
            }
            String cadenaconexion = @"Data Source=LOCALHOST;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Password=MCSD2020";
            return new OkObjectResult("");
        }
    }
}
