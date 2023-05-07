using Microsoft.AspNetCore.Mvc;
using MvcApiHospitalPractica.Filters;
using MvcApiHospitalPractica.Models;
using MvcApiHospitalPractica.Services;

namespace MvcApiHospitalPractica.Controllers
{
    public class AzureBlobsController : Controller
    {
        private ServiceStorageBlobs service;

        public AzureBlobsController(ServiceStorageBlobs service)
        {
            this.service = service;
        }
        [AuthorizeHospital]
        public async Task<IActionResult> ListContainers()
        {
            List<string> containers = await this.service.GetContainersAsync();
            return View(containers);
        }

        public IActionResult CreateContainer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            await this.service.CreateContainerAsync(containerName);
            return RedirectToAction("ListContainers");
        }

        public IActionResult CreateContainerPrivate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainerPrivate(string containerName)
        {
            await this.service.CreateContainerPrivateAsync(containerName);
            return RedirectToAction("ListContainers");
        }

        public async Task<IActionResult> DeleteContainer(string containerName)
        {
            await this.service.DeleteContainerAsync(containerName);
            return RedirectToAction("ListContainers");
        }

        public async Task<IActionResult> ListBlobs(string containerName)
        {
            List<BlobModel> blobModels =
            await this.service.GetBlobsAsync(containerName);



            foreach (BlobModel b in blobModels)
            {
                b.Url = await this.service.GetBlobUriAsync(containerName, b.Nombre);
            }



            return View(blobModels);
        }

        public async Task<IActionResult> DeleteBlob(string containerName, string blobName)
        {
            await this.service.DeleteBlobAsync(containerName, blobName);
            return RedirectToAction("ListBlob", new { containerName = containerName });
        }

        public IActionResult UploadBlob(string containerName)
        {
            ViewData["CONTAINER"] = containerName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBlob(string containerName, IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadBlobAsync(containerName, blobName, stream);
            }
            return RedirectToAction("ListBlob", new { containerName = containerName });
        }
    }
}
