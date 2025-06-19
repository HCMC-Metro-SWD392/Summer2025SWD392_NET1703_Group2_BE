using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.DTO.FormRequest
{
    public class PreSignedUploadDTO
    {
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
    }
}
