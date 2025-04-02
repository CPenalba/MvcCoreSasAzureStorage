using Azure.Data.Tables;
using MvcCoreSasAzureStorage.Models;
using Newtonsoft.Json.Linq;

namespace MvcCoreSasAzureStorage.Services
{
    public class ServiceAzureAlumnos
    {
        private TableClient tablaAlumnos;

        //NECESITAMOS LA URL DE ACCESO AL TOKEN
        private string UrlApi;

        public ServiceAzureAlumnos(IConfiguration configuration)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiAzureToken");
        }

        public async Task<string> GetTokenAsync(string curso)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "token/" + curso;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("token").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<Alumno>> GetAlumnosAsync(string curso)
        {
            string token = await this.GetTokenAsync(curso);

            //LO QUE NOS DEVUELVE EL TOKEN ES UNA URL
            Uri uriToken = new Uri(token);

            //PARA ACCEDER AL RECURSO, SIMPLEMENTE CREAMOS EL RECURSO MEDIANTE UN URI
            this.tablaAlumnos = new TableClient(uriToken);
            List<Alumno> alumnos = new List<Alumno>();
            var query = this.tablaAlumnos.QueryAsync<Alumno>(filter: "");
            await foreach (var item in query)
            {
                alumnos.Add(item);
            }
            return alumnos;
        }

        public async Task CreateAlumno(int idAlumno, string nombre, string apellidos, int nota)
        {
            string curso = "EN PROCESO";
            string token = await this.GetTokenAsync(curso);
            Alumno a = new Alumno();
            a.IdAlumno = idAlumno;
            //a.Curso = "AZURE";
            a.Curso = curso;
            a.Nombre = nombre;
            a.Apellidos = apellidos;
            a.Nota = nota;
            Uri uriToken = new Uri(token);
            this.tablaAlumnos = new TableClient(uriToken);
            await this.tablaAlumnos.AddEntityAsync<Alumno>(a);
        }
    }
}
