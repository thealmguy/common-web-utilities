using System.Text.RegularExpressions;

namespace CMCS.Common.WebUtilities.Helpers
{
    public class UrlHelpers
    {
        public static string Encode(string value)
        {
            string workingValue = value;
            workingValue = workingValue.Trim();
            Regex replaceRegEx = new Regex("[^A-Za-z0-9-]");
            workingValue = replaceRegEx.Replace(workingValue, "-");

            return workingValue.ToLower();
        }
    }
}
