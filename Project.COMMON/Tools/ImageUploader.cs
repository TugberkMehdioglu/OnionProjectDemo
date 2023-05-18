﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.COMMON.Tools
{
    public static class ImageUploader
    {
        public static async Task<string> UploadImageAsync(IFormFile picture, IFileProvider fileProvider, string entityImagePath)
        {
            if (picture == null) return "To be upload picture is empty";

            string extension = Path.GetExtension(picture.FileName);

            if (extension == "jpg" || extension == "gif" || extension == "png" || extension == "jpeg")
            {
                IDirectoryContents wwwroot = fileProvider.GetDirectoryContents("wwwroot");
                IFileInfo userPictures = wwwroot.First(x => x.Name == "userPictures");

                string randomFileName = $"{Guid.NewGuid()}{extension}";
                string newPicturePath = Path.Combine(userPictures.PhysicalPath!, randomFileName);

                using FileStream stream = new FileStream(newPicturePath, FileMode.Create);
                await picture.CopyToAsync(stream);

                entityImagePath = randomFileName;
                return newPicturePath;
            }
            else return "Selected file is not a picture !";
        }
    }
}
