namespace Shorty.Services.Impl.LinksService
{
    public class InvalidIdException : LinkServiceException
    {
        public InvalidIdException(string id, string description = null) : base($"{id} is not a valid id! {description ?? string.Empty}") {}
    }
}