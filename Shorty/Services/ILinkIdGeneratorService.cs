using Shorty.Models;

namespace Shorty.Services
{
    public interface ILinkIdGeneratorService
    {
        string GenerateId(string normalizedUrl);
    }
}