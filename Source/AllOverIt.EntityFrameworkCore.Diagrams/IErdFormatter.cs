using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public interface IErdFormatter
    {
        string Generate(DbContext dbContext);
    }
}