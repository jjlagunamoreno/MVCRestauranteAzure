using NugetApiModelsRestauranteJJLM;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

public class ServiceApiRestaurante
{
    private readonly string UrlApi;
    private readonly HttpClient client;

    public ServiceApiRestaurante()
    {
        this.UrlApi = "https://apirestaurante.azurewebsites.net/api/carta";
        this.client = new HttpClient();
    }

    // MÉTODO PARA CAMBIAR EL ESTADO DEL PLATO
    public async Task CambiarEstadoPlatoAsync(int id)
    {
        string request = $"/carta/{id}/estado";
        await this.client.PutAsync(this.UrlApi + request, null);
    }

    // MÉTODO PARA CREAR UN PLATO
    public async Task CrearPlatoAsync(Carta plato, IFormFile imagen)
    {
        using (var content = new MultipartFormDataContent())
        {
            content.Add(new StringContent(plato.Nombre), "Nombre");
            content.Add(new StringContent(plato.Descripcion), "Descripcion");
            content.Add(new StringContent(plato.Precio.ToString()), "Precio");
            content.Add(new StringContent(plato.TipoPlato), "TipoPlato");

            if (imagen != null)
            {
                var streamContent = new StreamContent(imagen.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(imagen.ContentType);
                content.Add(streamContent, "Imagen", imagen.FileName);
            }

            await this.client.PostAsync(this.UrlApi + "/carta", content);
        }
    }

    // MÉTODO PARA EDITAR UN PLATO
    public async Task EditarPlatoAsync(Carta plato, IFormFile imagen)
    {
        using (var content = new MultipartFormDataContent())
        {
            content.Add(new StringContent(plato.IdPlato.ToString()), "IdPlato");
            content.Add(new StringContent(plato.Nombre), "Nombre");
            content.Add(new StringContent(plato.Descripcion), "Descripcion");
            content.Add(new StringContent(plato.Precio.ToString()), "Precio");
            content.Add(new StringContent(plato.TipoPlato), "TipoPlato");

            if (imagen != null)
            {
                var streamContent = new StreamContent(imagen.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(imagen.ContentType);
                content.Add(streamContent, "Imagen", imagen.FileName);
            }

            await this.client.PutAsync(this.UrlApi + "/carta", content);
        }
    }
}
