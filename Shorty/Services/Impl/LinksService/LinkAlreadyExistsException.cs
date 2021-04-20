namespace Shorty.Services.Impl.LinksService
{
    public class LinkAlreadyExistsException : LinkServiceException
    {
        public LinkAlreadyExistsException(string id) : base($"link with id = {id} already exists") {}
    }
}