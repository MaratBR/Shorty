using System;

namespace Shorty.Services.Impl.LinksNormalizationService
{
    public class UriBuilderNormalizationService : ILinksNormalizationService
    {
        public Uri NormalizeLink(string link)
        {
            try
            {
                var builder = new UriBuilder(link);
            
                if (builder.Scheme != "https" && builder.Scheme != "http")
                    throw new InvalidUrlException($"unexpected schema - {builder.Scheme}, only http/https is supported");

                return builder.Uri;
            }
            catch (UriFormatException e)
            {
                throw new InvalidUrlException(e);
            }
        }

        public string ConvertToString(Uri uri)
        {
            string portStr;
            if (uri.Port == 80 && uri.Scheme == "http" ||
                uri.Port == 443 && uri.Scheme == "https")
            {
                portStr = string.Empty;
            }
            else
            {
                portStr = ":" + uri.Port;
            }
            return $"{uri.Scheme}://{uri.IdnHost}{portStr}{uri.PathAndQuery}{uri.Fragment}";
        }
    }
}