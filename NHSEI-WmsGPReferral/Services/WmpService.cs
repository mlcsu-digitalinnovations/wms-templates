using Microsoft.Extensions.Configuration;
using NHSEI_WmsGPReferral.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Services;

public interface IWmpService
{
  Task<string> GetPractice(string odsCode, string orgType);
  Task<string> GetPharmacy(string odsCode, string orgType);
  Task<HttpResponseMessage> CreatePracticeItemAsync(PostPracticeWmpDataModel postWmpDataModel);
  Task<HttpResponseMessage> CreatePharmacyItemAsync(PostPharmacyWmpDataModel postWmpDataModel);
  Task<HttpResponseMessage> UpdatePharmacyItemAsync(PostPharmacyWmpDataModel postWmpDataModel);
}
public class WmpService : IWmpService
{
  private readonly HttpClient _httpClient;
  private readonly string _PharmacyAPIKey = string.Empty;
  private readonly string _PracticeAPIKey = string.Empty;
  public WmpService(HttpClient httpClient, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _PharmacyAPIKey = configuration["WmsTemplate:PharmacyApiKey"];
    _PracticeAPIKey = configuration["WmsTemplate:PracticeApiKey"];
  }

  public async Task<string> GetPractice(string odsCode, string orgType)
  {
    HttpResponseMessage response = await GetResponseMessage(odsCode, orgType);
    HttpStatusCode responseStatus = response.StatusCode;

    if (responseStatus == System.Net.HttpStatusCode.OK)
    {
      return await response.Content.ReadAsStringAsync();
    }
    else if (responseStatus == System.Net.HttpStatusCode.NoContent)
    {
      return "NoContent";
    }
    else
    {
      return "NotFound";
    }
  }

  public async Task<string> GetPharmacy(string odsCode, string orgType)
  {
    HttpResponseMessage response = await GetResponseMessage(odsCode, orgType);
    HttpStatusCode responseStatus = response.StatusCode;

    if (responseStatus == System.Net.HttpStatusCode.OK)
    {
      return await response.Content.ReadAsStringAsync();
    }
    else if (responseStatus == System.Net.HttpStatusCode.NoContent)
    {
      return "NoContent";
    }
    else
    {
      return "NotFound";
    }
  }

  public async Task<HttpResponseMessage> CreatePracticeItemAsync(PostPracticeWmpDataModel model)
  {
    StringContent modelJson =
      new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

    PreparePracticeClient();
    return await _httpClient.PostAsync($"Practice/system", modelJson);
  }
  public async Task<HttpResponseMessage> CreatePharmacyItemAsync(PostPharmacyWmpDataModel model)
  {
    StringContent modelJson =
      new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

    PreparePharmacyClient();
    return await _httpClient.PostAsync($"Pharmacy/Create", modelJson);
  }

  public async Task<HttpResponseMessage> UpdatePharmacyItemAsync(PostPharmacyWmpDataModel model)
  {
    StringContent modelJson =
      new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

    PreparePharmacyClient();
    return await _httpClient.PutAsync($"Pharmacy/Update/{model.OdsCode}", modelJson);
  }

  private async Task<HttpResponseMessage> GetResponseMessage(string odsCode, string orgType)
  {
    if (orgType == "Pharmacy")
    {
      PreparePharmacyClient();
      return await _httpClient.GetAsync($"{orgType}/GetOdsCode?odsCode={odsCode}");
    }

    PreparePracticeClient();
    return await _httpClient.GetAsync($"{orgType}/system/{odsCode}");
  }

  private void PreparePharmacyClient()
  {
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("X-API-KEY", _PharmacyAPIKey);
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
  }
  private void PreparePracticeClient()
  {
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("X-API-KEY", _PracticeAPIKey);
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
  }
}
