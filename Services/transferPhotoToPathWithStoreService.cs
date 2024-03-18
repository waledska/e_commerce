using Microsoft.Extensions.Hosting;

namespace e_commerce.Services
{
    public class transferPhotoToPathWithStoreService : ITransferPhotosToPathWithStoreService
    {
        // uploading photos!

        // for upload only one image
        public string GetPhotoPath(IFormFile model)
        {
            if (model == null || model.Length == 0)
            {
                // Handle the case where no file is provided
                return "error, IFormFile model can't be empty";
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var maxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB

            var imagesFolderName = "Images"; // Path relative to the project root

            // Validate file type
            var fileExtension = Path.GetExtension(model.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                // Handle invalid file type
                return "error, file format should be only { \".jpg\", \".jpeg\", \".png\", \".gif\" }";
            }

            // Validate file size
            if (model.Length > maxFileSizeInBytes)
            {
                // Handle oversized file
                return "error, image size can't be bigger than 10MB";
            }

            string uniquePhotoName = Guid.NewGuid() + fileExtension;
            // Construct the full path
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), imagesFolderName, uniquePhotoName);

            try
            {
                // Save the file
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }

                return fullPath;
            }
            catch (Exception ex)
            {
                // Handle exceptions, log or rethrow based on your requirements
                // You might want to consider returning an error message or throwing a custom exception
                Console.WriteLine($"Error saving file: {ex.Message}");
                return "error, some thing went wrong!";
            }
        }

        // for uploading more than one image
        public List<string> GetPhotosPath(List<IFormFile> model)
        {
            var resultPaths = new List<string>();

            if (model == null || !model.Any())
            {
                // Handle the case where no files are provided
                resultPaths.Add("error, List<IFormFile> model can't be empty");
                return resultPaths;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var maxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
            var imagesFolderName = "Images"; // Path relative to the project root

            foreach (var file in model)
            {
                if (file == null || file.Length == 0)
                {
                    // Handle the case where no file is provided
                    resultPaths.Add("error, IFormFile model can't be empty");
                    return resultPaths;
                }

                // Validate file type
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    // Handle invalid file type
                    resultPaths.Add("error, file format should be only { \".jpg\", \".jpeg\", \".png\", \".gif\" }");
                    return resultPaths;
                }

                // Validate file size
                if (file.Length > maxFileSizeInBytes)
                {
                    // Handle oversized file
                    resultPaths.Add("error, image size can't be bigger than 10MB");
                    return resultPaths;
                }

                string uniquePhotoName = Guid.NewGuid() + fileExtension;
                // Construct the full path
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), imagesFolderName, uniquePhotoName);

                try
                {
                    // Save the file
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    // Add the path to the result list
                    resultPaths.Add(fullPath);
                }
                catch (Exception ex)
                {
                    // Handle exceptions, log or rethrow based on your requirements
                    // You might want to consider returning an error message or throwing a custom exception
                    Console.WriteLine($"Error saving file: {ex.Message}");
                    resultPaths.Add("error, something went wrong!");
                }
            }

            // This point is reached only if there are no errors and all files are processed successfully
            resultPaths.Add("success");
            return resultPaths;
        }

        // delete un needed images 
        public bool DeleteFile(string path)
        {
            // Check if file exists with its full path    
            if (File.Exists(path))
            {
                // If file found, delete it    
                File.Delete(path);
                Console.WriteLine("File deleted.");
                return true;
            }
            else
            {
                Console.WriteLine("File not found");
                return false;
            }
        }


    }
}
