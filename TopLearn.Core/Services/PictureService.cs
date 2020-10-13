using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generators;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Core.Services
{
    public class PictureService : IPictureService
    {
        public void RemoveImage(string imageName, string path)
        {
            if (imageName == "default-avatar.png") return;
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(),
                $"wwwroot/images/{path}", imageName);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
            else
            {
                //TODO: Log error
                Console.WriteLine($"The image path cannot be found. Path = {imagePath}");
            }
        }

        public async Task<ImageViewModel> SaveImageAsync(IFormFile imageFile, string path)
        {
            var imageViewModel = new ImageViewModel
            {
                ImageName = null,
                LimitReached = false
            };

            if (imageFile is null)
            {
                return imageViewModel;
            }

            if (imageFile.Length > 500000)
            {
                imageViewModel.LimitReached = true;
                return imageViewModel;
            }

            try
            {
                imageViewModel.ImageName = Generator.GenerationUniqueName() + Path.GetExtension(imageFile.FileName);

                var savePath = Path.Combine(Directory.GetCurrentDirectory(),
                    $"wwwroot/images/{path}", imageViewModel.ImageName);

                await using var stream = new FileStream(savePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                imageViewModel.ImageName = null;
            }

            return imageViewModel;
        }
    }
}