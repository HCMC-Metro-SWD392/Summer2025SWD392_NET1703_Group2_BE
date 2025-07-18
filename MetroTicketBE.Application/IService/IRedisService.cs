using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IRedisService
    {
        Task<bool> StoreKeyAsync(string key, string value, TimeSpan? expiry = null);
        Task<bool> DeleteStringAysnc(string key);
        Task<string?> RetrieveString(string key);
        Task<long> RemoveRangeByScoreAsync(string key, double start, double stop);
        Task<long> SortedSetLengthAsync(string key);
        Task<SortedSetEntry[]> GetSortedSetDescByScoreAsync(string key, bool isAccending, int take = 1);
        Task<bool> DeleteKeyAsync(string key);
        Task<bool> AddToSortedSetAsync(string key, string id, double score);
        Task<bool> ExpireKeyAsync(string key, TimeSpan expiry);
        //Task<bool> AddToSetAsync(string key, string value);
        //Task<bool> RemoveFromSetAsync(string key, string value);
        //Task<long> GetSetCountAsync(string key);
    }
}
