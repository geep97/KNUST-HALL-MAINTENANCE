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


    // =========================================================
    // CURRENT STUDENT
    // =========================================================

    private int CurrentUserId =>
        int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );


    private string CurrentStudentHall =>
        User.FindFirstValue("Hall") ?? string.Empty;


    private string CurrentStudentRoom =>
        User.FindFirstValue("RoomNumber") ?? string.Empty;


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
        var model = new ComplaintCreateViewModel
        {
            Hall = CurrentStudentHall,
            RoomNumber = CurrentStudentRoom
        };

        return View(model);
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
            // GET HALL AND ROOM FROM AUTHENTICATED STUDENT
            // -------------------------------------------------

            var hall = CurrentStudentHall;
            var roomNumber = CurrentStudentRoom;


            // -------------------------------------------------
            // IMPORTANT:
            // Never trust Hall or RoomNumber submitted by the
            // browser. The student's account is the source
            // of truth.
            // -------------------------------------------------

            model.Hall = hall;
            model.RoomNumber = roomNumber;


            // -------------------------------------------------
            // READ PHOTO
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
            // VALIDATE
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


            if (
                uploadedPhoto != null &&
                uploadedPhoto.Length > 0
            )
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

            var complaint = new Complaint
            {
                StudentId = CurrentUserId,

                Category = model.Category,

                Description = model.Description,

                // IMPORTANT:
                // These come from the authenticated student,
                // NOT from the submitted form.
                Hall = hall,

                RoomNumber = roomNumber,

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


            // Make sure the values are still available
            // if the form needs to be displayed again.
            model.Hall = CurrentStudentHall;
            model.RoomNumber = CurrentStudentRoom;


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