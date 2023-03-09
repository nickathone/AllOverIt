using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public interface IErdGenerator
    {
        string Generate(DbContext dbContext);
    }
}