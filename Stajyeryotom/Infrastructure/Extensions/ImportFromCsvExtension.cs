using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Stajyeryotom.Infrastructure.Services;

namespace Stajyeryotom.Infrastructure.Extensions;

public static class ImportFromCsvExtension
{
    public static async Task ImportDataFromCsvAsync(
        this IApplicationBuilder app,
        string departmentsPath,
        string sectionsPath,
        string accountsPath,
        string applicationsPath,
        string reportsPath)
    {
        bool imported = true;

        if (!imported)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var importer = scope.ServiceProvider.GetRequiredService<CsvImporter>();
            await importer.ImportAsync(departmentsPath, sectionsPath, accountsPath, applicationsPath, reportsPath);
            imported = true;
        }

    }
}
