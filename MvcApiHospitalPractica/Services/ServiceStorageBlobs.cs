using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MvcApiHospitalPractica.Models;
using Azure.Storage.Sas;

namespace MvcApiHospitalPractica.Services
{

    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        //metodo para mostrar todos los contenedores
        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (BlobContainerItem item in this.client.GetBlobContainersAsync())
            {
                containers.Add(item.Name);
            }
            return containers;
        }

        //metodo para crear contenedores
        public async Task CreateContainerAsync(string containerName)
        {
            //containerName = containerName.ToLower();
            //debemos indicar el nombre del contenedor y su tipo de acceso
            await this.client.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);

        }

        public async Task CreateContainerPrivateAsync(string containerName)
        {
            //containerName = containerName.ToLower();
            //debemos indicar el nombre del contenedor y su tipo de acceso
            await this.client.CreateBlobContainerAsync(containerName, PublicAccessType.None);

        }

        //metodo para eliminar contenedores
        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }

        //metodo para recuperar todos los blobs
        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            //recuperamos un client del container
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            List<BlobModel> blobModels = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                //necesitamos un blob client para visualizar mas caracteristacas delobjeto
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobModel model = new BlobModel();
                model.Nombre = item.Name;
                model.Contenedor = containerName;
                model.Url = blobClient.Uri.AbsoluteUri;
                blobModels.Add(model);
            }
            return blobModels;
        }

        //metodo para eliminar blobs
        public async Task DeleteBlobAsync
            (string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);

        }

        //metodo para subir un blob 
        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        //public async Task<string> FindBlobAsync(string containerName, string blobName)
        //{
        //    BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
        //   var blob =  containerClient.GetBlobClient(blobName);
        //    return blob.Uri.AbsoluteUri;

        //}
        public async Task<string> GetBlobUriAsync(string container, string blobName)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);



            var response = await containerClient.GetPropertiesAsync();
            var properties = response.Value;



            // Will be private if it's None
            if (properties.PublicAccess == Azure.Storage.Blobs.Models.PublicAccessType.None)
            {
                Uri imageUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddSeconds(3600));
                return imageUri.ToString();
            }



            return blobClient.Uri.AbsoluteUri.ToString();
        }
    }
}

