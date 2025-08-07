using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Repositories.Extensions
{
    public static class RepositoryExtension
    {
        public static IQueryable<T> FilteredByDepartmentId<T>(
            this IQueryable<T> query,
            int? departmentId,
            Expression<Func<T, int?>> departmentSelector)
        {
            if (departmentId is null)
                return query;

            var parameter = departmentSelector.Parameters[0];
            var body = Expression.Equal(departmentSelector.Body, Expression.Constant(departmentId, typeof(int?)));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> FilteredBySearchTerm<T>(
            this IQueryable<T> query,
            string searchTerm,
            Expression<Func<T, string>> propertySelector)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var loweredTerm = searchTerm.ToLower();
            var parameter = propertySelector.Parameters[0];

            var toLowerCall = Expression.Call(propertySelector.Body, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);
            var containsCall = Expression.Call(toLowerCall, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, Expression.Constant(loweredTerm));
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> FilteredByStatus<T>(
            this IQueryable<T> query,
            string status,
            Expression<Func<T, string>> propertySelector)
        {
            if (string.IsNullOrWhiteSpace(status))
                return query;

            if (status != "OnWait" && status != "Interview" && status != "Denied" && status != "Approved" && status != "Read" && status != "NotRead")
                return query;

            var parameter = propertySelector.Parameters[0];

            var equalsCall = Expression.Equal(propertySelector.Body, Expression.Constant(status));
            var lambda = Expression.Lambda<Func<T, bool>>(equalsCall, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<Report> FilteredByType(this IQueryable<Report> report, string type)
        {
            if(type == "Daily")
            {
                return report
                    .Where(r => r.Work == null);
            }
            else if(type == "Work")
            {
                return report
                    .Where(r => r.Work != null);
            }
            else
            {
                return report;
            }
        }

        public static IQueryable<T> FilteredByDate<T>(
            this IQueryable<T> query,
            string startDate,
            string endDate,
            Expression<Func<T, DateTime>> propertySelector)
        {
            var parameter = propertySelector.Parameters[0];
            Expression? body = null;

            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var start))
            {
                var startExpr = Expression.GreaterThanOrEqual(propertySelector.Body, Expression.Constant(start));
                body = startExpr;
            }

            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var end))
            {
                var endExpr = Expression.LessThanOrEqual(propertySelector.Body, Expression.Constant(end));
                body = body != null ? Expression.AndAlso(body, endExpr) : endExpr;
            }

            if (body == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }

        public static IQueryable<T> ToPaginate<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize)
        {
            return source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        public static IQueryable<Account> Sort(this IQueryable<Account> accounts, string parameter)
        {
            var now = DateTime.Now;
            if (parameter == "PROGRESS_ASC")
            {
                return accounts
                    .OrderByDescending(u => u.InternshipStartDate <= now && u.InternshipEndDate >= now)
                    .ThenBy(u =>
                        (double)EF.Functions.DateDiffDay(u.InternshipStartDate, now) /
                        EF.Functions.DateDiffDay(u.InternshipStartDate, u.InternshipEndDate)
                    );
            }
            else if (parameter == "PROGRESS_DESC")
            {
                return accounts
                    .OrderByDescending(u => u.InternshipStartDate <= now && u.InternshipEndDate >= now)
                    .ThenByDescending(u =>
                        (double)EF.Functions.DateDiffDay(u.InternshipStartDate, now) /
                        EF.Functions.DateDiffDay(u.InternshipStartDate, u.InternshipEndDate)
                    );
            }
            else if (parameter == "FINISHED")
            {
                return accounts.OrderByDescending(u => u.InternshipEndDate <= DateTime.Now && u.InternshipStartDate <= DateTime.Now)
                    .ThenByDescending(u => u.InternshipStartDate >= DateTime.Now && u.InternshipEndDate >= DateTime.Now);
            }
            else if (parameter == "NOTSTARTED")
            {
                return accounts.OrderByDescending(u => u.InternshipStartDate >= DateTime.Now && u.InternshipEndDate >= DateTime.Now)
                    .ThenByDescending(u => u.InternshipEndDate <= DateTime.Now && u.InternshipStartDate <= DateTime.Now);
            }
            else
            {
                return accounts.OrderByDescending(u => u.InternshipStartDate <= DateTime.Now && u.InternshipEndDate >= DateTime.Now)
                .ThenBy(u => u.InternshipEndDate >= DateTime.Now ? u.InternshipEndDate : DateTime.MaxValue)
                .ThenBy(u => u.InternshipEndDate);
            }
        }

        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string propertyName, bool ascending = true)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return source;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression propertyAccess = parameter;

            foreach (var member in propertyName.Split('.'))
            {
                propertyAccess = Expression.PropertyOrField(propertyAccess, member);
            }

            var converted = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(converted, parameter);

            return ascending ? source.OrderBy(lambda) : source.OrderByDescending(lambda);
        }

        public static IQueryable<T> SortExtensionForApplications<T>(this IQueryable<T> query, string parameter)
        {
            if (typeof(T).Name == nameof(Application))
            {
                var applicationQuery = query as IQueryable<Application>;

                switch (parameter)
                {
                    case "ID_ASC":
                        return query.SortBy("ApplicationId", true);

                    case "ID_DESC":
                        return query.SortBy("ApplicationId", false);

                    case "NAME_ASC":
                        return query.SortBy("ApplicantFirstName", true);

                    case "NAME_DESC":
                        return query.SortBy("ApplicantFirstName", false);

                    case "DEPARTMENT_ASC":
                        return query.SortBy("Section.Department.DepartmentName", true);

                    case "DEPARTMENT_DESC":
                        return query.SortBy("Section.Department.DepartmentName", false);

                    case "DATE_ASC":
                        return query.SortBy("UpdatedDate", true);

                    case "DATE_DESC":
                        return query.SortBy("UpdatedDate", false);

                    case "APPROVED":
                        return (applicationQuery!
                            .Where(a => a.Status == "Approved")
                            .OrderByDescending(a => a.CreatedDate) as IQueryable<T>)!;

                    case "DENIED":
                        return (applicationQuery!
                            .Where(a => a.Status == "Denied")
                            .OrderByDescending(a => a.CreatedDate) as IQueryable<T>)!;

                    default:
                        return query;
                }
            }

            return query;
        }


        public static IQueryable<T> SortExtensionForInterns<T>(this IQueryable<T> query, string parameter)
        {
            var accountQuery = query as IQueryable<Account>;

            // Özel parametreler
            switch (parameter)
            {
                case "PROGRESS_ASC":
                case "PROGRESS_DESC":
                case "FINISHED":
                case "NOTSTARTED":
                    return (accountQuery!.Sort(parameter) as IQueryable<T>)!;
            }

            // Genel parametreler
            switch (parameter)
            {
                case "NAME_ASC":
                    return query.SortBy("FirstName", true);

                case "NAME_DESC":
                    return query.SortBy("FirstName", false);

                case "DEPARTMENT_ASC":
                    return query.SortBy("Section.Department.DepartmentName", true);

                case "DEPARTMENT_DESC":
                    return query.SortBy("Section.Department.DepartmentName", false);

                case "DATE_ASC":
                    return query.SortBy("InternshipStartDate", true);

                case "DATE_DESC":
                    return query.SortBy("InternshipStartDate", false);

                default:
                    return (accountQuery!.Sort("DEFAULT") as IQueryable<T>)!;
            }
        }

        public static IQueryable<T> SortExtensionForReports<T>(this IQueryable<T> query, string parameter)
        {
            if (typeof(T).Name == nameof(Report))
            {
                var reportQuery = query as IQueryable<Report>;

                switch (parameter)
                {

                    case "NAME_ASC":
                        return query.SortBy("Account.FirstName", true);

                    case "NAME_DESC":
                        return query.SortBy("Account.FirstName", false);

                    case "REPORT_ASC":
                        return query.SortBy("ReportTitle", true);

                    case "REPORT_DESC":
                        return query.SortBy("ReportTitle", false);

                    case "DATE_ASC":
                        return query.SortBy("CreatedAt", true);

                    case "DATE_DESC":
                        return query.SortBy("CreatedAt", false);

                    case "READ":
                        return (reportQuery!
                            .Where(r => r.Status == "Read")
                            .OrderByDescending(a => a.CreatedAt) as IQueryable<T>)!;

                    case "NOTREAD":
                        return (reportQuery!
                            .Where(a => a.Status == "NotRead")
                            .OrderByDescending(a => a.CreatedAt) as IQueryable<T>)!;
                    default:
                        return (reportQuery!
                            .OrderByDescending(a => a.CreatedAt) as IQueryable<T>)!;
                }
            }

            return query;
        }
    }
}
