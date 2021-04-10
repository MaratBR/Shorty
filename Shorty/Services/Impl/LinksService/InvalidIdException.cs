namespace Shorty.Services.Impl.LinksService
{
    public class InvalidIdException : LinkServiceException
    {
        public InvalidIdException(string id) : base($"{id} is not a valid base62 id!") {}
    }
}