using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MyBookList.Infrastructure.Data;

public class OpenLibrarySeeder
{
    private const int BatchSize = 1_000_000;

    public static void Seed(ApplicationDbContext dbContext, IConfiguration configuration, ILogger logger)
    {
        if (!dbContext.Authors.Any())
        {
            logger.LogInformation("Initializing Authors database..");
            SeedAuthors(configuration, logger);
        }

        if (!dbContext.Books.Any())
        {
            logger.LogInformation("Initializing Books database..");
            SeedBooks(configuration, logger);
        }
    }

    private static void SeedAuthors(IConfiguration configuration, ILogger logger)
    {
        var connectionString = configuration.GetRequiredSection("ConnectionStrings")["DefaultConnection"];
        var authorsFile = configuration.GetRequiredSection("OpenLibraryDumps")["AuthorsFile"];

        using var fs = File.Open(authorsFile, FileMode.Open, FileAccess.Read);
        using var sr = new StreamReader(fs);

        var authorsTable = MakeTable(
            new DataColumn("Id", typeof(string)),
            new DataColumn("Name", typeof(string)),
            new DataColumn("PersonalName", typeof(string)),
            new DataColumn("BirthYear", typeof(int)),
            new DataColumn("DeathYear", typeof(int))
        );

        var authorAliasesTable = MakeTable(
            new DataColumn("AuthorId", typeof(string)),
            new DataColumn("Name", typeof(string))
        );


        while (!sr.EndOfStream)
        {
            var parts = sr.ReadLine()!.Split("\t");
            var json = parts[4];

            var olAuthor = JsonSerializer.Deserialize<OpenLibraryAuthor>(json);

            if (olAuthor.Name == null || olAuthor.Name.Length >= 450)
                continue;

            var authorRow = authorsTable.NewRow();
            authorRow["Id"] = olAuthor.Key[9..];
            authorRow["Name"] = olAuthor.Name;
            authorRow["PersonalName"] = olAuthor.PersonalName;
            authorRow["BirthYear"] = int.TryParse(olAuthor.BirthDate, out var year) ? year : DBNull.Value;
            authorRow["DeathYear"] = int.TryParse(olAuthor.DeathDate, out year) ? year : DBNull.Value;
            authorsTable.Rows.Add(authorRow);

            if (olAuthor.AlternateNames != null)
            {
                foreach (var name in olAuthor.AlternateNames)
                {
                    var aliasRow = authorAliasesTable.NewRow();
                    aliasRow["AuthorId"] = authorRow["Id"];
                    aliasRow["Name"] = name;
                }
            }

            if (authorsTable.Rows.Count >= BatchSize || sr.EndOfStream)
            {
                logger.LogInformation(
                    $"Inserting rows: Authors ({authorsTable.Rows.Count}), AuthorAliases ({authorAliasesTable.Rows.Count})");

                using var sqlConnection = new SqlConnection(connectionString);

                var authorsCopy = MakeBulkCopy(sqlConnection, authorsTable, "Authors");
                var authorAliasesCopy = MakeBulkCopy(sqlConnection, authorAliasesTable, "AuthorAliases");

                sqlConnection.Open();
                authorsCopy.WriteToServer(authorsTable);
                authorAliasesCopy.WriteToServer(authorAliasesTable);

                logger.LogInformation("Inserting finished");

                authorsTable.Clear();
                authorAliasesTable.Clear();
            }
        }
    }

    private static void SeedBooks(IConfiguration configuration, ILogger logger)
    {
        var connectionString = configuration.GetRequiredSection("ConnectionStrings")["DefaultConnection"];
        var worksFile = configuration.GetRequiredSection("OpenLibraryDumps")["WorksFile"];

        using var fs = File.Open(worksFile, FileMode.Open, FileAccess.Read);
        using var sr = new StreamReader(fs);

        var booksTable = MakeTable(
            new DataColumn("Id", typeof(string)),
            new DataColumn("Title", typeof(string)),
            new DataColumn("Description", typeof(string)),
            new DataColumn("CoverId", typeof(int)));

        var authorBookTable = MakeTable(
            new DataColumn("AuthorsId", typeof(string)),
            new DataColumn("BooksId", typeof(string)));

        var subjectsTable = MakeTable(
            new DataColumn("Id", typeof(Guid)),
            new DataColumn("Title", typeof(string)));

        var bookSubjectTable = MakeTable(
            new DataColumn("BooksId", typeof(string)),
            new DataColumn("SubjectsId", typeof(Guid))
        );

        var subjectIdCache = new Dictionary<string, Guid>();

        while (!sr.EndOfStream)
        {
            var parts = sr.ReadLine()!.Split("\t");
            var json = parts[4];

            var olBook = JsonSerializer.Deserialize<OpenLibraryBook>(json);

            if (olBook.Title == null || olBook.Authors == null || olBook.Title.Length >= 450)
                continue;

            var bookRow = booksTable.NewRow();
            bookRow["Id"] = olBook.Key[7..];
            bookRow["Title"] = olBook.Title;
            bookRow["Description"] = olBook.Description;
            booksTable.Rows.Add(bookRow);

            if (olBook.Covers != null)
            {
                bookRow["CoverId"] = olBook.Covers.FirstOrDefault();
            }

            if (olBook.Subjects != null)
            {
                foreach (var subject in olBook.Subjects.Distinct().Where(s => s.Length < 450))
                {
                    if (!subjectIdCache.ContainsKey(subject))
                    {
                        var subjectRow = subjectsTable.NewRow();
                        var newId = Guid.NewGuid();
                        subjectRow["Id"] = newId;
                        subjectRow["Title"] = subject;
                        subjectsTable.Rows.Add(subjectRow);

                        subjectIdCache[subject] = newId;
                    }

                    var bookSubjectRow = bookSubjectTable.NewRow();
                    bookSubjectRow["BooksId"] = bookRow["Id"];
                    bookSubjectRow["SubjectsId"] = subjectIdCache[subject];
                    bookSubjectTable.Rows.Add(bookSubjectRow);
                }
            }

            foreach (var author in olBook.Authors.DistinctBy(x => x.Author))
            {
                if (author.Author is null)
                    continue;

                var authorBookRow = authorBookTable.NewRow();
                authorBookRow["AuthorsId"] = author.Author[9..];
                authorBookRow["BooksId"] = bookRow["Id"];
                authorBookTable.Rows.Add(authorBookRow);
            }

            if (booksTable.Rows.Count >= BatchSize || sr.EndOfStream)
            {
                logger.LogInformation(
                    $"Inserting rows: Books ({booksTable.Rows.Count}), AuthorBook ({authorBookTable.Rows.Count}), Subjects ({subjectsTable.Rows.Count}), BookSubject ({bookSubjectTable.Rows.Count})");

                using var sqlConnection = new SqlConnection(connectionString);

                var booksCopy = MakeBulkCopy(sqlConnection, booksTable, "Books");
                var authorBookCopy = MakeBulkCopy(sqlConnection, authorBookTable, "AuthorBook");
                var subjectsCopy = MakeBulkCopy(sqlConnection, subjectsTable, "Subjects");
                var bookSubjectCopy = MakeBulkCopy(sqlConnection, bookSubjectTable, "BookSubject");

                sqlConnection.Open();
                booksCopy.WriteToServer(booksTable);
                authorBookCopy.WriteToServer(authorBookTable);
                subjectsCopy.WriteToServer(subjectsTable);
                bookSubjectCopy.WriteToServer(bookSubjectTable);

                logger.LogInformation("Inserting finished");

                booksTable.Clear();
                authorBookTable.Clear();
                subjectsTable.Clear();
                bookSubjectTable.Clear();
            }
        }
    }

    private static DataTable MakeTable(params DataColumn[] columns)
    {
        var table = new DataTable();
        table.Columns.AddRange(columns);
        return table;
    }

    private static SqlBulkCopy MakeBulkCopy(SqlConnection connection, DataTable table, string tableName)
    {
        var bulkCopy = new SqlBulkCopy(connection);
        bulkCopy.DestinationTableName = tableName;
        bulkCopy.BulkCopyTimeout = 180;

        foreach (DataColumn column in table.Columns)
        {
            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
        }

        return bulkCopy;
    }
}