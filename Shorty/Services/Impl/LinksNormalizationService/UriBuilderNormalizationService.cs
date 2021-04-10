using System;

namespace Shorty.Services.Impl.LinksNormalizationService
{
    public class UriBuilderNormalizationService : ILinksNormalizationService
    {
        public Uri NormalizeLink(string link)
        {
            var builder = new UriBuilder(link);
            
            if (builder.Scheme != "https" && builder.Scheme != "http")
                throw new InvalidUrlException($"unexpected schema - {builder.Scheme}, only http/https is supported");

            return builder.Uri;
        }

        public string ConvertToString(Uri uri)
        {
            var port = uri.Port == 80 || uri.Port == 443 ? "" : $":{uri.Port}";
            return $"{uri.Scheme}://{uri.IdnHost}{port}{uri.PathAndQuery}{uri.Fragment}";
        }
    }
}