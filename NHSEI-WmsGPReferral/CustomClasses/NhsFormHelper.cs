namespace NHSEI_WmsGPReferral.CustomClasses;

public class NhsFormHelper
{
  public static string GetFormElementClasses(string element, bool error)
  {
    string cssClasses = "nhsuk-input";

    switch (element)
    {
      case "text":
        break;
      case "form-group":
        cssClasses = "nhsuk-form-group";
        break;
    }

    if (error)
    {
      cssClasses += " " + cssClasses + "--error";
    }

    cssClasses += " ";

    return cssClasses;
  }
}
