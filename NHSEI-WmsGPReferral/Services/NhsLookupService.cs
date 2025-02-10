using System.Net.Http;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Services;

public interface INhsLookupService
{
  Task<string> Get(string odsCode);
}
public class NhsLookupService : INhsLookupService
{
  private HttpClient _httpClient;

  public NhsLookupService(HttpClient httpClient) => _httpClient = httpClient;

  public async Task<string> Get(string odsCode)
  {
    HttpResponseMessage response = await GetResponseMessage(odsCode);
    if (response.IsSuccessStatusCode)
    {
      return await GetPracticeDataAsync(response);
    }
    else
    {
      return "NotFound";
    }
  }

  private async Task<HttpResponseMessage> GetResponseMessage(string odsCode)
  {
    HttpResponseMessage response = await _httpClient.GetAsync(odsCode);
    return response;
  }

  private async Task<string> GetPracticeDataAsync(HttpResponseMessage response)
  {
    string responseContent = await response.Content.ReadAsStringAsync();
    return responseContent;
  }
}
