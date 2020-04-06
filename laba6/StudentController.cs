using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace laba6
{
    public class StudentController
    {
        public async Task<string> AddStudentAsync(HttpContext context)
        {
            string json = "";
            int rows = 0;

            using (StreamReader rdr = new StreamReader(context.Request.Body))
                json = await rdr.ReadToEndAsync();

            Student student = JsonSerializer.Deserialize<Student>(json);

            student.CreatedAt = DateTime.Now;

            using (ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>()))
            {
                rows = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO public.\"Students\" (\"FirstName\", \"LastName\", \"Group\", \"CreatedAt\", \"UpdatedAt\") VALUES ({student.FirstName}, {student.LastName}, {student.Group}, {student.CreatedAt}, {student.UpdatedAt})");
            }

            return rows == 0 ? await Task.FromResult("error") : await Task.FromResult("ok");
        }

        public async Task<string> GetStudentsAsync()
        {
            string res = "";

            using (ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>()))
            {
                List<Student> students = await db.Students.FromSqlInterpolated($"SELECT * FROM public.\"Students\";").ToListAsync();
                res = JsonSerializer.Serialize<List<Student>>(students);
            }

            return await Task.FromResult(JsonPrint(res));
        }

        public async Task<string> GetStudentAsync(HttpContext httpContext)
        {
            int id = Int32.Parse(httpContext.Request.Path.ToString().Substring(httpContext.Request.Path.ToString().LastIndexOf('/') + 1));

            string res = "";

            using (ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>()))
            {
                Student student = await db.Students.FromSqlInterpolated($"SELECT * FROM public.\"Students\" WHERE \"Id\"={id}").FirstOrDefaultAsync();
                res = JsonSerializer.Serialize<Student>(student);
            }

            return await Task.FromResult(JsonPrint(res));
        }

        public async Task<string> DeleteStudentAsync(HttpContext httpContext)
        {
            int id = Int32.Parse(httpContext.Request.Path.ToString().Substring(httpContext.Request.Path.ToString().LastIndexOf('/') + 1));

            int rows = 0;

            using (ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>()))
            {
                rows = await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM public.\"Students\" WHERE \"Id\"={id}");
            }

            return rows == 0 ? await Task.FromResult("error") : await Task.FromResult("ok");
        }

        public async Task<string> PutStudentAsync(HttpContext httpContext)
        {
            int id = Int32.Parse(httpContext.Request.Path.ToString().Substring(httpContext.Request.Path.ToString().LastIndexOf('/') + 1));

            int rows = 0;

            string json = "";

            using (StreamReader rdr = new StreamReader(httpContext.Request.Body))
            {
                json = await rdr.ReadToEndAsync();
            }

            Student student = JsonSerializer.Deserialize<Student>(json);

            string query = $"UPDATE public.\"Students\" SET \"UpdatedAt\"='{DateTime.Now}'";

            if (student.FirstName != null)
                query += $", \"FirstName\"='{student.FirstName}'";

            if (student.LastName != null)
                query += $", \"LastName\"='{student.LastName}'";

            if (student.Group != null)
                query += $", \"Group\"='{student.Group}'";

            query += $" WHERE \"Id\"={id}";

            using (ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>()))
            {
                rows = await db.Database.ExecuteSqlRawAsync(query);
            }

            return rows == 0 ? await Task.FromResult("error") : await Task.FromResult("ok");
        }

        public string JsonPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }
}