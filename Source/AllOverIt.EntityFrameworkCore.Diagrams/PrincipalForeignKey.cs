namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public sealed class PrincipalForeignKey
    {
        public string EntityName { get; init; }
        public string ColumnName { get; init; }
        public bool IsOneToMany { get; init; }
    }
}