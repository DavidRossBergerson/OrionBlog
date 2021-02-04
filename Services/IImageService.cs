using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Services
{
    public interface IImageService
    {
        //I need to declare a method like EncodeFileAsync
         Task<byte[]> EncodeFileAsync(IFormFile formFile);

        string DecodeFile(byte[] imageData, string contentType);

        string RecordContentType(IFormFile formFile);
    }
}
