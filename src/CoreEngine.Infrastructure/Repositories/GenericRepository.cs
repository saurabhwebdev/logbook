using CoreEngine.Domain.Common;
using CoreEngine.Domain.Interfaces;
using CoreEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.ToListAsync(ct);

    public IQueryable<T> Query() => _dbSet;

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _dbSet.AddAsync(entity, ct);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity)
    {
        // Soft delete is handled by DbContext.SaveChangesAsync override
        _context.Entry(entity).State = EntityState.Deleted;
    }
}
