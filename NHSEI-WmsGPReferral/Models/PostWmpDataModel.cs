namespace NHSEI_WmsGPReferral.Models;

public class PostWmpDataModel
{
  public string Email { get; set; }
  public string OdsCode { get; set; }
  public string Name { get; set; }
}

public class PostPracticeWmpDataModel : PostWmpDataModel
{
  public string SystemName { get; set; }
}

public class PostPharmacyWmpDataModel : PostWmpDataModel
{
  public string TemplateVersion { get; set; }
}