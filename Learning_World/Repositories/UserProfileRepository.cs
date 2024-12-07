using Learning_World.Data;
using Learning_World.Models;
using Learning_World.ViewModels;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Learning_World.Repositories
{
    public class UserProfileRepository
    {
        private readonly ElearningPlatformContext _context;

        public UserProfileRepository(ElearningPlatformContext context)
        {
            _context = context;
        }

        public async Task<UserProfileViewModel> GetUserProfile(int userId)
        {
            var dbUser = await _context.Users
                .Include(u => u.Courses)
                .Include(u => u.Certificates)
                    .ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (dbUser == null)
            {
                return null;
            }

            return new UserProfileViewModel
            {
                Id = dbUser.Id,
                Name = dbUser.UserName,
                Email = dbUser.Email,
                Image = dbUser.Image,
                Certificates = dbUser.Certificates.Select(c => new CertificateViewModel
                {
                    CourseName = c.Course.Title,
                    IssueDate = c.IssueDate
                }).ToList()
            };
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users
                .Include(u => u.Certificates)
                    .ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> UpdateUser(User user, IFormFile imageFile, string webRootPath)
        {
            if (imageFile != null)
            {
                var imagePath = System.IO.Path.Combine(webRootPath, "images");
                Directory.CreateDirectory(imagePath);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + System.IO.Path.GetFileName(imageFile.FileName);
                var filePath = System.IO.Path.Combine(imagePath, uniqueFileName);

                using (var image = Image.Load(imageFile.OpenReadStream()))
                {
                    image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Size = new Size(230, 230),
                            Mode = ResizeMode.Crop
                        })
                        .ApplyRoundedCorners(115)
                    );

                    await image.SaveAsync(filePath, new PngEncoder());
                }

                if (!string.IsNullOrEmpty(user.Image))
                {
                    var oldImagePath = System.IO.Path.Combine(imagePath, user.Image);
                    if (File.Exists(oldImagePath) && user.Image != "default.jpg")
                    {
                        File.Delete(oldImagePath);
                    }
                }

                user.Image = uniqueFileName;
            }

            _context.Update(user);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }

    public static class ImageProcessingExtensions
    {
        public static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            Size size = ctx.GetCurrentSize();
            IPathCollection corners = BuildCorners(size.Width, size.Height, cornerRadius);

            ctx.SetGraphicsOptions(new GraphicsOptions()
            {
                Antialias = true,
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut
            });

            foreach (var c in corners)
            {
                ctx = ctx.Fill(Color.Red, c);
            }
            return ctx;
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // Create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            var cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(imageWidth - cornerRadius, 0);
            var cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, imageHeight - cornerRadius);
            var cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(imageWidth - cornerRadius, imageHeight - cornerRadius);

            return new PathCollection(cornerTopLeft, cornerTopRight, cornerBottomLeft, cornerBottomRight);
        }
    }
}

