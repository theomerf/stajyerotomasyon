using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using System.Text;

namespace Stajyeryotom.Infrastructure.Services;

public class CsvImporter
{
    private readonly RepositoryContext _context;
    private readonly UserManager<Account> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public CsvImporter(
        RepositoryContext context,
        UserManager<Account> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task ImportAsync(string departmentsPath, string sectionsPath, string accountsPath, string applicationsPath, string reportsPath)
    {
        var departmentLines = File.ReadAllLines(departmentsPath, Encoding.UTF8);
        var departments = new List<Department>();
        for (int i = 1; i < departmentLines.Length; i++)
        {
            var columns = departmentLines[i].Split(';');
            departments.Add(new Department
            {
                DepartmentName = columns[1].Trim()
            });
        }

        await _context.Departments.AddRangeAsync(departments);
        await _context.SaveChangesAsync();

        var sectionLines = File.ReadAllLines(sectionsPath, Encoding.UTF8);
        var sections = new List<Section>();
        for (int i = 1; i < sectionLines.Length; i++)
        {
            var columns = sectionLines[i].Split(';');
            sections.Add(new Section
            {
                SectionName = columns[1].Trim(),
                DepartmentId = int.Parse(columns[2].Trim())
            });
        }

        await _context.Sections.AddRangeAsync(sections);
        await _context.SaveChangesAsync();

        var accountLines = File.ReadAllLines(accountsPath, Encoding.UTF8);
        for (int i = 1; i < accountLines.Length; i++)
        {
            var columns = accountLines[i].Split(';');

            var account = new Account
            {
                UserName = columns[0].Trim(),
                Email = columns[1].Trim(),
                PhoneNumber = columns[2].Trim(),
                FirstName = columns[3].Trim(),
                LastName = columns[4].Trim(),
                InternshipStartDate = DateTime.Parse(columns[5].Trim()),
                InternshipEndDate = DateTime.Parse(columns[6].Trim()),
                SectionId = int.Parse(columns[7].Trim()),
            };

            var result = await _userManager.CreateAsync(account, "deneme123_");
            if (!result.Succeeded)
                throw new Exception($"Kullanıcı oluşturulamadı: {account.FirstName}");

            var roleName = columns[9].Trim();

            var roleResult = await _userManager.AddToRoleAsync(account, roleName);
            if (!roleResult.Succeeded)
                throw new Exception($"Rol atanamadı: {account.FirstName}");
        }

        var applicationsLines = File.ReadAllLines(applicationsPath, Encoding.UTF8);
        var applications = new List<Application>();
        for (int i = 1; i < applicationsLines.Length; i++)
        {
            var columns = applicationsLines[i].Split(';');

            string statusText = columns[0].Trim();
            ApplicationStatus status = Enum.Parse<ApplicationStatus>(statusText);

            applications.Add(new Application
            {
                Status = status,
                Title = columns[1].Trim(),
                Description = columns[2].Trim(),
                ApplicantFirstName = columns[3].Trim(),
                ApplicantLastName = columns[4].Trim(),
                CvUrl = columns[5].Trim(),
                ApplicantEmail = columns[6].Trim(),
                CreatedDate = DateTime.Parse(columns[7].Trim()),
                UpdatedDate = DateTime.Parse(columns[8].Trim()),
                SectionId = int.Parse(columns[9].Trim())
            });
        }

        await _context.Applications.AddRangeAsync(applications);
        await _context.SaveChangesAsync();

        Random rnd = new Random();

        List<string> accountIds = await _context.Users.Select(a => a.Id).ToListAsync();

        var reportsLines = File.ReadAllLines(reportsPath, Encoding.UTF8);
        var reports = new List<Report>();
        for (int i = 1; i < reportsLines.Length; i++)
        {
            var columns = reportsLines[i].Split(';');
            reports.Add(new Report
            {
                ReportTitle = columns[0].Trim(),
                ReportContent = columns[1].Trim(),
                ImageUrls = columns[2]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(url => url.Trim())
                    .ToList(),
                AccountId = accountIds[rnd.Next(accountIds.Count)]
            });
        }

        await _context.Reports.AddRangeAsync(reports);
        await _context.SaveChangesAsync();
    }
}
