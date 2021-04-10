using System;

namespace Shorty.Services
{
    public interface ILinksNormalizationService
    {
        Uri NormalizeLink(string link);

        string ConvertToString(Uri uri);
    }
}