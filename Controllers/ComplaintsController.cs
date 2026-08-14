using System.Security.Claims;
using KnustHallMaintenance.Models;
using KnustHallMaintenance.Models.ViewModels;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;

[Authorize(Roles = "Student")]
public class ComplaintsController : Controller
{
    private readonly IComplaintService _service;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private const long MaxPhotoSize = 5 * 1024 * 1024;


    public ComplaintsController(
        IComplaintService service,
        IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }


    private int CurrentUserId =>
        int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );


    // =========================================================
    // MY COMPLAINTS
    // =========================================================

    public async Task<IActionResult> MyComplaints()
    {
        var complaints =
            await _service.GetForStudentAsync(CurrentUserId);

        return View(complaints);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ComplaintCreateViewModel());
    }


    // =========================================================
    // CREATE - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ComplaintCreateViewModel model)
    {
        Console.WriteLine("================================");
        Console.WriteLine("CREATE REQUEST RECEIVED");
        Console.WriteLine("================================");


        try
        {
            // -------------------------------------------------
            // IMPORTANT:
            // We are reading the uploaded file directly from
            // Request.Form instead of relying on IFormFile
            // model binding.
            // -------------------------------------------------

            IFormFile? uploadedPhoto = null;


            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();

                uploadedPhoto = form.Files["Photo"];

                Console.WriteLine(
                    $"Files received: {form.Files.Count}"
                );
            }


            // -------------------------------------------------
            // Validate normal form
            // -------------------------------------------------

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid.");

                return View(model);
            }


            // -------------------------------------------------
            // PHOTO
            // -------------------------------------------------

            string? photoPath = null;


            if (uploadedPhoto != null &&
                uploadedPhoto.Length > 0)
            {
                Console.WriteLine(
                    $"Photo received: {uploadedPhoto.FileName}"
                );

                Console.WriteLine(
                    $"Photo size: {uploadedPhoto.Length} bytes"
                );


                // ---------------------------------------------
                // SIZE
                // ---------------------------------------------

                if (uploadedPhoto.Length > MaxPhotoSize)
                {
                    ModelState.AddModelError(
                        "Photo",
                        "The photo must be 5 MB or smaller."
                    );

                    return View(model);
                }


                // ---------------------------------------------
                // EXTENSION
                // ---------------------------------------------

                var extension =
                    Path.GetExtension(
                        uploadedPhoto.FileName
                    ).ToLowerInvariant();


                Console.WriteLine(
                    $"Extension: {extension}"
                );


                if (!AllowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "Photo",
                        "Only JPG, JPEG, PNG and WEBP images are allowed."
                    );

                    return View(model);
                }


                // ---------------------------------------------
                // UPLOAD DIRECTORY
                // ---------------------------------------------

                var uploadsDirectory =
                    Path.Combine(
                        _env.WebRootPath,
                        "uploads"
                    );


                Directory.CreateDirectory(
                    uploadsDirectory
                );


                Console.WriteLine(
                    $"Upload directory: {uploadsDirectory}"
                );


                // ---------------------------------------------
                // SAFE FILE NAME
                // ---------------------------------------------

                var fileName =
                    $"{Guid.NewGuid():N}{extension}";


                var filePath =
                    Path.Combine(
                        uploadsDirectory,
                        fileName
                    );


                Console.WriteLine(
                    $"Saving to: {filePath}"
                );


                // ---------------------------------------------
                // SAVE FILE
                // ---------------------------------------------

                await using (
                    var stream = new FileStream(
                        filePath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None
                    ))
                {
                    await uploadedPhoto.CopyToAsync(stream);
                }


                photoPath =
                    $"/uploads/{fileName}";


                Console.WriteLine(
                    $"Photo saved successfully: {photoPath}"
                );
            }
            else
            {
                Console.WriteLine(
                    "No photo uploaded."
                );
            }


            // -------------------------------------------------
            // CREATE COMPLAINT
            // -------------------------------------------------

            Console.WriteLine(
                "Creating complaint..."
            );


            var complaint = new Complaint
            {
                StudentId = CurrentUserId,

                Category = model.Category,

                Description = model.Description,

                Hall = model.Hall,

                RoomNumber = model.RoomNumber,

                PhotoPath = photoPath
            };


            // -------------------------------------------------
            // SAVE COMPLAINT
            // -------------------------------------------------

            Console.WriteLine(
                "Saving complaint..."
            );


            await _service.CreateAsync(
                complaint
            );


            Console.WriteLine(
                $"Complaint saved. ID: {complaint.Id}"
            );


            return RedirectToAction(
                nameof(MyComplaints)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine("");
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("UPLOAD / COMPLAINT ERROR");
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!");


            ModelState.AddModelError(
                "",
                "There was a problem submitting the request."
            );


            return View(model);
        }
    }


    // =========================================================
    // DETAILS
    // =========================================================

    public async Task<IActionResult> Details(int id)
    {
        var complaint =
            await _service.GetByIdAsync(id);


        if (
            complaint == null ||
            complaint.StudentId != CurrentUserId
        )
        {
            return NotFound();
        }


        return View(complaint);
    }
}