using CRUDImgs.Class;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using System.IO;

namespace CRUDImgs.Controllers
{
   [Route("api/images/")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public  AppContextt context;

        private const string ImagesFolder = "images";
        private const int MaxFileSize = 100 * 1024 * 1024; // 5MB


        public ImageController(AppContextt dbContext)
        {
            context = dbContext;
            //var images = context.Images.ToList();
        }

        [HttpPost]
        [Route("upload-by-url")]
        public async Task<IActionResult> UploadImageByUrl([FromBody] ImageUrlRequest request)
        {
            try
            {
                var image = new Images();

                // download pic by URL
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(request.Url);

                    if (imageBytes != null)
                    {
                        if (imageBytes.Length < MaxFileSize)
                        {
                            // Generate a unique filename for the image
                            //string uniqueFileName = Guid.NewGuid().ToString() + ".png";

                            
                            // Save the image to the specified path
                            string fileName = Guid.NewGuid().ToString() + ".jpeg";
                            string filePath = Path.Combine("Images", fileName); // Шлях до папки "Images" і ім'я файлу

                            using (var fs = new FileStream(filePath, FileMode.Create))
                            {
                                await fs.WriteAsync(imageBytes, 0, imageBytes.Length);
                            }
                            image.Id = Guid.NewGuid().ToString();
                            image.ImageData = filePath;

                            await context.Images.AddAsync(image);
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            return BadRequest("File size exceeds the maximum limit.");
                        }
                    }
                    else
                    {
                        return BadRequest("No file uploaded.");
                    }
                }

                return Ok(new { url = $"http://localhost/PATH_TO_IMAGE/{image.Id}" });
            }
            catch (Exception exc)
            {
                return BadRequest("Помилка під час завантаження та збереження зображення.");
            }
        }


        [HttpGet("get-url/{id}")]
        public IActionResult GetImageUrl(string id)
        {
            try
            {
                var image = context.Images.FirstOrDefault(i => i.Id.Equals(id));
                if (image == null)
                {
                    return NotFound("Зображення не знайдено");
                }

                // Отримуємо шлях до зображення з бази даних
                string imagePath = image.ImageData;

                // Формуємо повний URL на основі базової адреси та шляху до зображення
                string imageUrl = $"http://localhost/PATH_TO_IMAGE/{imagePath}";

                return Ok(new { url = imageUrl });
            }
            catch (Exception exc)
            {
                return BadRequest("Помилка під час отримання URL зображення.");
            }
        }

        [HttpGet("get-url/{id}/size/{size}")]
        public IActionResult GetImagePreviewUrl(string id, int size)
        {
            try
            {
                var image = context.Images.FirstOrDefault(i => i.Id.Equals(id));
                if (image == null)
                {
                    return NotFound("Зображення не знайдено");
                }

                // Отримуємо шлях до зображення з бази даних
                string imagePath = image.ImageData;

                // Формуємо шлях до прев'ю зображення на основі базової адреси, шляху до зображення та розміру
                string previewPath = $"Images/Preview/{imagePath}/{size}";

                // Формуємо повний URL на основі базової адреси та шляху до прев'ю зображення
                string previewUrl = $"http://localhost/{previewPath}";

                return Ok(new { url = previewUrl });
            }
            catch (Exception exc)
            {
                return BadRequest("Помилка під час отримання URL прев'ю зображення.");
            }
        }


        [HttpDelete("remove/{id}")]
        public IActionResult RemoveImage(string id)
        {
            try
            {
                var image = context.Images.FirstOrDefault(i => i.Id.Equals(id));
                if (image == null)
                {
                    return NotFound("Зображення не знайдено");
                }

                // delete image file from server
                if (System.IO.File.Exists(image.ImageData))
                {
                    System.IO.File.Delete(image.ImageData);
                }

                // delete image from DB
                context.Images.Remove(image);
                context.SaveChanges();

                return Ok("Зображення успішно видалено");
            }
            catch (Exception exc)
            {
                return BadRequest("Помилка під час видалення зображення.");
            }
        }

    }

}
