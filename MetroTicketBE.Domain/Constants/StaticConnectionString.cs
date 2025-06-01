using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Constants
{
    public static class StaticConnectionString
    {
        public const string POSTGRE_DefaultConnection = "DefaultConnection"; // Kết nối tới localhost
        public const string REDIS_ConnectionString = "ConnectionString";
    }
}
