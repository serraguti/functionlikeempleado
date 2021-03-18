using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

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
            empno = empno ?? data?.empno;
            if (empno == null)
            {
                return new BadRequestObjectResult("Necesitamos el parámetro de {empno}");
            }
            String cadenaconexion = @"Data Source=LOCALHOST;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Password=MCSD2020";
            using (SqlConnection cn = new SqlConnection(cadenaconexion))
            {
                String sqlselect = "select * from emp where emp_no=" + empno;
                SqlCommand com = new SqlCommand();
                com.Connection = cn;
                com.CommandType = System.Data.CommandType.Text;
                com.CommandText = sqlselect;
                cn.Open();
                SqlDataReader reader = com.ExecuteReader();
                if (reader.Read())
                {
                    int salario = int.Parse(reader["SALARIO"].ToString()) + 1;
                    String mensaje = "El empleado " + reader["APELLIDO"]
                        + " ha sido gratificado con 1€ por su labor!!!" 
                        + " Su salario se queda en " + salario;
                    reader.Close();
                    String sqlupdate = "update emp set salario = salario + 1 "
                        + "where emp_no=" + empno;
                    com.CommandText = sqlupdate;
                    com.ExecuteNonQuery();
                    cn.Close();
                    return new OkObjectResult(mensaje);
                }
                else
                {
                    reader.Close();
                    cn.Close();
                    return new BadRequestObjectResult("No existe el empleado " + empno);
                }
            }
        }
    }
}
