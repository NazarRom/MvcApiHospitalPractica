using Microsoft.AspNetCore.Mvc;
using MvcApiHospitalPractica.Filters;
using MvcApiHospitalPractica.Models;
using MvcApiHospitalPractica.Services;

namespace MvcApiHospitalPractica.Controllers
{
    public class HospitalController : Controller
    {
        private ServiceHospitales service;
        private ServiceStorageBlobs serviceBlobs;

        public HospitalController(ServiceHospitales service, ServiceStorageBlobs serviceBlobs)
        {
            this.service = service;
            this.serviceBlobs = serviceBlobs;
        }
        [AuthorizeHospital]
        public async Task<IActionResult> Perfil()
        {
            string token =
                HttpContext.Session.GetString("TOKEN");
            Hospital hospital = await
                this.service.GetPerfilEmpleadoAsync(token);
            return View(hospital);
        }

        [AuthorizeHospital]
        public async Task<IActionResult> Index()
        {
            List<Hospital> hospitales = new();
            string? token = HttpContext.Session.GetString("TOKEN");

            if (token == null)
            {
                ViewData["MENSAJE"] = "Debe realizar login para visualizar datos";
            }
            else
            {
                hospitales = await this.service.GetHospitalesAsync(token);
                foreach (var hospital in hospitales)
                {
                    if (hospital.Imagen != null)
                    {
                        hospital.Imagen = await this.serviceBlobs.GetBlobUriAsync("privado", hospital.Imagen);
                    }
                }
            }
            return View(hospitales);
        }

        public async Task<IActionResult> Details(int id)
        {
            Hospital hospital = await this.service.FindHospital(id);
            return View(hospital);
        }
        public IActionResult Create()
        {
            return View();
        }
  
        [HttpPost]
        
        public async Task<IActionResult> Create(CreateHospital hospital)
        {
            string image = hospital.Nombre.ToLower();

            using (Stream stream = hospital.Imagen.OpenReadStream())
            {
                await this.serviceBlobs.UploadBlobAsync("privado", image, stream);
            }

            await this.service.InsertHospital(hospital.Hospital_cod, hospital.Nombre, hospital.Direccion, hospital.Telelfono, hospital.Num_cama, image);



            return RedirectToAction("Index");
        }

        public async Task<IActionResult>Edit(int id)
        {
            Hospital hospital = await this.service.FindHospital(id);
            return View(hospital);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Hospital hospital)
        {
            await this.service.UpdateHospital(hospital.Hospital_cod, hospital.Nombre, hospital.Direccion, hospital.Telelfono, hospital.Num_cama, hospital.Imagen);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Hospital hospital = await this.service.FindHospital(id);
            await this.serviceBlobs.DeleteBlobAsync("blobimages", hospital.Imagen);
            await this.service.DeleteHospital(id);
            return RedirectToAction("Index");
        }

    }
}
