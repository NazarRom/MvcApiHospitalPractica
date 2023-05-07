using Microsoft.AspNetCore.Mvc;
using MvcApiHospitalPractica.Filters;
using MvcApiHospitalPractica.Models;
using MvcApiHospitalPractica.Services;

namespace MvcApiHospitalPractica.Controllers
{
    public class HospitalController : Controller
    {
        private ServiceHospitales service;

        public HospitalController(ServiceHospitales service)
        {
            this.service = service;
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
            string token = HttpContext.Session.GetString("TOKEN");
            if (token == null)
            {
                ViewData["MENSAJE"] = "Debe realizar Login para visualizar datos";
                return View();
            }else
            {
                List<Hospital> hospitals = await this.service.GetHospitalesAsync(token);
                return View(hospitals);
            }
            
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
        public async Task<IActionResult> Create(Hospital hospital)
        {
            await this.service.InsertHospital(hospital.Hospital_cod, hospital.Nombre, hospital.Direccion, hospital.Telelfono, hospital.Num_cama);
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
            await this.service.UpdateHospital(hospital.Hospital_cod, hospital.Nombre, hospital.Direccion, hospital.Telelfono, hospital.Num_cama);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await this.service.DeleteHospital(id);
            return RedirectToAction("Index");
        }

    }
}
