using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using MetroTicketBE.Infrastructure.Context;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Repository;

public class LogRepository: Repository<Log>, ILogRepository
{
    private readonly ApplicationDBContext _context;
    public LogRepository(ApplicationDBContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<List<Log>> GetAllLogs(int pageNumber, int pageSize)
    {
        return  _context.Logs.Include(l => l.User)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
    
    public async Task<List<Log>> GetByCreatedAtRange(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10)
    {
        var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
        return await _context.Logs
            .Where(l => l.CreatedAt >= startDateUtc && l.CreatedAt <= endDateUtc)
            .Include(l => l.User)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<List<Log>> GetByUserIdAsync(string userId, int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Logs
            .Where(l => l.UserId == userId)
            .Include(l => l.User)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<List<Log>> GetByLogTypeAsync(LogType logType, int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Logs
            .Where(l => l.LogType == logType)
            .Include(l => l.User)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}