using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TopLearn.Core.DTOs;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPictureService
    {
        Task<ImageViewModel> SaveImageAsync(IFormFile imageFile, string path);
    }
}
