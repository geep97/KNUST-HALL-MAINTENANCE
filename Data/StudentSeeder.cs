using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;

namespace KnustHallMaintenance.Data;

public static class StudentSeeder
{
    public static void Seed(
        ApplicationDbContext context,
        IPasswordHasher<Student> passwordHasher)
    {
        // Don't seed again if students already exist
        if (context.Students.Any())
        {
            return;
        }

        var students = new List<Student>();

        var names = new[]
        {
            "Kwame Mensah",
            "Ama Owusu",
            "Kofi Asare",
            "Abena Boateng",
            "Yaw Osei",
            "Akosua Frimpong",
            "Kojo Antwi",
            "Adwoa Mensima",
            "Kwaku Addo",
            "Efua Sarpong",
            "Kwesi Appiah",
            "Maame Serwaa",
            "Daniel Owusu",
            "Michael Asante",
            "Emmanuel Boateng",
            "Samuel Ofori",
            "Joseph Mensah",
            "Richard Arthur",
            "David Amoah",
            "Isaac Opoku",
            "Esther Adjei",
            "Grace Asante",
            "Ruth Owusu",
            "Priscilla Mensah",
            "Nana Yaa Osei",
            "Linda Boateng",
            "Mavis Frimpong",
            "Belinda Addo",
            "Shirley Asare",
            "Patricia Owusu"
        };

        var programmes = new[]
        {
            new
            {
                Programme = "BSc Computer Science",
                Department = "Department of Computer Science",
                College = "College of Science"
            },

            new
            {
                Programme = "BSc Information Technology",
                Department = "Department of Computer Science",
                College = "College of Science"
            },

            new
            {
                Programme = "BSc Electrical Engineering",
                Department = "Department of Electrical and Electronic Engineering",
                College = "College of Engineering"
            },

            new
            {
                Programme = "BSc Civil Engineering",
                Department = "Department of Civil Engineering",
                College = "College of Engineering"
            },

            new
            {
                Programme = "BSc Mechanical Engineering",
                Department = "Department of Mechanical Engineering",
                College = "College of Engineering"
            },

            new
            {
                Programme = "BSc Accounting",
                Department = "Department of Accounting",
                College = "KNUST School of Business"
            },

            new
            {
                Programme = "BSc Economics",
                Department = "Department of Economics",
                College = "College of Humanities and Social Sciences"
            },

            new
            {
                Programme = "BA Communication Design",
                Department = "Department of Communication Design",
                College = "College of Art and Built Environment"
            },

            new
            {
                Programme = "BSc Architecture",
                Department = "Department of Architecture",
                College = "College of Art and Built Environment"
            },

            new
            {
                Programme = "BSc Biochemistry",
                Department = "Department of Biochemistry",
                College = "College of Science"
            }
        };

        var halls = new[]
        {
            "Independence Hall",
            "Unity Hall",
            "Katanga Hall",
            "Queens Hall",
            "Republic Hall",
            "Africa Hall"
        };

        var random = new Random(20260814);

        for (int i = 0; i < 100; i++)
        {
            var name = names[i % names.Length];

            var nameParts = name
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var firstName = nameParts[0];
            var lastName = nameParts[^1];

            var programme =
                programmes[random.Next(programmes.Length)];

            var hall =
                halls[i % halls.Length];

            var level =
                (random.Next(4) + 1) * 100;

            var studentId =
                $"2081{i + 1:0000}";

            var email =
                $"{firstName}.{lastName}{i + 1}@st.knust.edu.gh";

            var telephone =
                GenerateTelephoneNumber(random);

            var room =
                GenerateRoomNumber(random);

            var student = new Student
            {
                StudentId = studentId,

                FullName = name,

                Email = email,

                TelephoneNumber = telephone,

                Programme = programme.Programme,

                Department = programme.Department,

                College = programme.College,

                Level = level,

                Hall = hall,

                RoomNumber = room
            };

            student.PasswordHash =
                passwordHasher.HashPassword(
                    student,
                    "Password123!"
                );

            students.Add(student);
        }

        context.Students.AddRange(students);

        context.SaveChanges();
    }


    private static string GenerateTelephoneNumber(
        Random random)
    {
        var prefixes = new[]
        {
            "020",
            "024",
            "050",
            "054",
            "055",
            "059"
        };

        var prefix =
            prefixes[random.Next(prefixes.Length)];

        var number =
            random.Next(1000000, 9999999);

        return $"{prefix}{number}";
    }


    private static string GenerateRoomNumber(
        Random random)
    {
        var blocks = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "E"
        };

        var block =
            blocks[random.Next(blocks.Length)];

        var number =
            random.Next(1, 40);

        return $"{block}{number:00}";
    }
}

