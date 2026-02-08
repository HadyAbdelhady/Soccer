using Infra.Interface;
using Infra.DBContext;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Infra.Persistent
{
    public class UnitOfWork(SoccerDbContext context, IServiceProvider provider) : IUnitOfWork
    {
        private readonly SoccerDbContext _context = context;
        private readonly IServiceProvider _provider = provider;
        private IDbContextTransaction? _transaction;

        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public IRepository<T> Repository<T>() where T : class, IEntity
        {
            var type = typeof(T);
            // Try to get cached one
            if (_repositories.TryGetValue(type, out var repoObj))
                return (IRepository<T>)repoObj;

            // Resolve from DI (open generic registration will return Repository<T>)
            var repo = _provider.GetRequiredService<IRepository<T>>();
            _repositories[type] = repo!;
            return repo!;
        }

        public TRepo GetRepository<TRepo>() where TRepo : class
        {
            return _provider.GetRequiredService<TRepo>();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            // If a transaction is still open, roll it back to prevent connection locks
            if (_transaction != null)
            {
                try
                {
                    _transaction.Rollback();
                }
                catch
                {
                    // Ignore errors during rollback on dispose
                }
                _transaction.Dispose();
                _transaction = null;
            }
            // Do NOT dispose _context - it's managed by the DI container and will be disposed automatically
            // when the scope ends. Disposing it here causes double-dispose issues and connection pool problems.
        }
    }

}
