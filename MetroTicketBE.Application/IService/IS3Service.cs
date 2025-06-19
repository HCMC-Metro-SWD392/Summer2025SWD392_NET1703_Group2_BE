using MetroTicketBE.Domain.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Application.IService
{
    public interface IS3Service
    {
        ResponseDTO GenerateUploadUrl(string objectKey, string contentType);
        ResponseDTO GenerateDownloadUrl(string objectKey);
    }
}
