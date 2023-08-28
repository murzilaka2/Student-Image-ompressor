using ImageMagick;
using ImageСompressor.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

//https://github.com/dlemstra/Magick.NET/
//Nuget - Magick.NET-Q16-AnyCPU
namespace ImageСompressor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _appEnvironment;


        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                // путь к папке Files
                string path = "/Files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
                CompressImage(_appEnvironment.WebRootPath + path);
            }
            return RedirectToAction(nameof(GetVirtualFile), new { filename = uploadedFile.FileName });
        }

        public VirtualFileResult GetVirtualFile(string fileName)
        {
            var filepath = Path.Combine("~/Files", fileName);
            return File(filepath, "text/plain", fileName);
        }

        private void CompressImage(string imageSrc)
        {
            using (MagickImage image = new MagickImage(imageSrc))
            {
                image.Format = image.Format; //установка формата изображения
                //image.Resize(40, 40); // изменение размера изображения
                image.Quality = 70; // Уровень сжатия от 1 до 100.
                image.Write(imageSrc);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}