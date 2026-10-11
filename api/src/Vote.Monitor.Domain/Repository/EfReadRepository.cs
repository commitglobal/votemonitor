using System.Linq.Expressions;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Vote.Monitor.Core.Specifications;

namespace Vote.Monitor.Domain.Repository;

/// <summary>
/// Read-only repository: every query runs with AsNoTracking, so loaded entities are not attached to the DbContext.
/// Use <see cref="IRepository{T}"/> when the loaded entity is modified or referenced by an entity being saved.
/// </summary>
public class EfReadRepository<T> : RepositoryBase<T>, IReadRepository<T> where T : class, IAggregateRoot
{
    private readonly VoteMonitorContext _dbContext;

    public EfReadRepository(VoteMonitorContext dbContext) : base(dbContext, CustomSpecificationEvaluator.Instance)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<T> ApplySpecification(ISpecification<T> specification,
        bool evaluateCriteriaOnly = false)
    {
        return CustomSpecificationEvaluator.Instance.GetQuery(_dbContext.Set<T>().AsNoTracking(), specification,
            evaluateCriteriaOnly);
    }

    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
    {
        return CustomSpecificationEvaluator.Instance.GetQuery(_dbContext.Set<T>().AsNoTracking(), specification);
    }

    public override async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        // FindAsync would return a tracked instance, so filter by the primary key instead: x => x.<Key> == id
        var keyName = _dbContext.Model.FindEntityType(typeof(T))!.FindPrimaryKey()!.Properties.Single().Name;
        var entity = Expression.Parameter(typeof(T), "x");
        var predicate = Expression.Lambda<Func<T, bool>>(
            Expression.Equal(
                Expression.Property(entity, keyName),
                // reading the id from a captured object makes EF send it as a query parameter
                Expression.Property(Expression.Constant(new KeyValue<TId>(id)), nameof(KeyValue<TId>.Value))),
            entity);

        return await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public override async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    private sealed record KeyValue<TId>(TId Value);
}
