using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

internal static class Extensors
{
    internal static string RemoveDiacritics(this string text, bool lowerCase = true)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        string formD = text.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char ch in formD)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }
        return lowerCase ?  sb.ToString().Normalize(NormalizationForm.FormC).ToLower() 
            : sb.ToString().Normalize(NormalizationForm.FormC); 
    }
}