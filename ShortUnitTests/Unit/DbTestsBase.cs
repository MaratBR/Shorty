using Microsoft.EntityFrameworkCore;
using Shorty.Models;

namespace ShortUnitTests.Unit
{
    public abstract class DbTestsBase
    {
        protected readonly AppDbContext DbContext;

        protected DbTestsBase()
        {
            DbContext = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase("Shorty_Testing")
                    .Options
            );
        }
    }
}