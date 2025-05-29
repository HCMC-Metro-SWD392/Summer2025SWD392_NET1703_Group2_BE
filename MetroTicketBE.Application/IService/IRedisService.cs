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
    }
}
