using MvcApiHospitalPractica.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcApiHospitalPractica.Services
{
    public class ServiceHospitales
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;

        public ServiceHospitales(IConfiguration configuration)
        {
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.UrlApi = configuration.GetValue<string>("ApiUrl:ApiHospital");

        }
        public async Task<string> GetTokenAsync
        (string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>
        (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }else
                {
                    return default;
                }
            }
        }

        public async Task<List<Hospital>> GetHospitalesAsync(string token)
        {
            string request = "api/Hospital";
            List<Hospital> hospitals = await this.CallApiAsync<List<Hospital>>(request, token);
            return hospitals;
        }

        public async Task<Hospital> FindHospital(int id)
        {
            string request = "api/Hospital/" + id;
            Hospital hospital = await this.CallApiAsync<Hospital>(request);
            return hospital;
        }
        //insert
        public async Task InsertHospital(int hos_cod, string nombre, string direccion, string telefono, int num_cama, string imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Hospital";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                //tenemos que enviar un objeto JSON
                //nos creamos un objeto de la clase Hospital
                Hospital hospital = new Hospital
                {
                    Hospital_cod = hos_cod,
                    Nombre = nombre,
                    Direccion = direccion,
                    Telelfono = telefono,
                    Num_cama = num_cama,
                    Imagen = imagen
                };
                //convertimos el objeto a json
                string json = JsonConvert.SerializeObject(hospital);
                //para enviar datos al servicio se utiliza 
                //la clase SytringContent, donde debemos indicar
                //los datos, de ending  y su tipo
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content); 
            }
        }
        //update

        public async Task UpdateHospital(int hos_cod, string nombre, string direccion, string telefono, int num_cama, string imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Hospital";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Hospital hospital = new Hospital
                {
                    Hospital_cod = hos_cod,
                    Nombre = nombre,
                    Direccion = direccion,
                    Telelfono = telefono,
                    Num_cama = num_cama, 
                    Imagen = imagen
                };
                //convierto a json el objeto
                string json = JsonConvert.SerializeObject(hospital);
                //para enviar datos al servicio se utiliza 
                //la clase SytringContent, donde debemos indicar
                //los datos, de ending  y su tipo
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);

            }
        }

        //delete 

        public async Task DeleteHospital(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/hospital/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        //perfil

        //METODO PROTEGIDO PARA RECUPERAR EL PERFIL
        public async Task<Hospital> GetPerfilEmpleadoAsync
            (string token)
        {
            string request = "api/hospital/perfilhospital";
            Hospital empleado = await
                this.CallApiAsync<Hospital>(request, token);
            return empleado;
        }

    }
}
