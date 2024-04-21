using System.Text.Json.Serialization;

namespace MyBookList.Infrastructure.Data;

internal struct AuthorEntry
{
    // [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("author")]
    [JsonConverter(typeof(AuthorJsonConverter<string>))]
    public string? Author { get; set; }
}

// internal struct KeyValue
// {
//     [JsonPropertyName("key")] public string? Key { get; set; }
// }

internal struct Description
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("value")] public string Value { get; set; }
}

internal struct ExcerptEntry
{
    [JsonPropertyName("excerpt")] public object Excerpt { get; set; }

    [JsonPropertyName("page")] public string Page { get; set; }
}

internal struct FirstSentence
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("value")] public string Value { get; set; }
}

internal struct Link
{
    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("url")] public string Url { get; set; }

    [JsonPropertyName("type")] public Type Type { get; set; }
}

internal struct OpenLibraryBook
{
    [JsonPropertyName("first_publish_date")]
    public string FirstPublishDate { get; set; }

    [JsonPropertyName("last_modified")] public LastModified LastModified { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("created")] public Created Created { get; set; }

    [JsonPropertyName("covers")] public List<int?> Covers { get; set; }

    [JsonPropertyName("lc_classifications")]
    public List<string> LcClassifications { get; set; }

    [JsonPropertyName("latest_revision")] public int? LatestRevision { get; set; }

    [JsonPropertyName("key")] public string Key { get; set; }

    [JsonPropertyName("authors")] public List<AuthorEntry> Authors { get; set; }

    [JsonPropertyName("dewey_number")] public List<string> DeweyNumber { get; set; }

    [JsonPropertyName("type")] public Type Type { get; set; }

    [JsonPropertyName("subjects")] public List<string>? Subjects { get; set; }

    [JsonPropertyName("revision")] public int? Revision { get; set; }

    [JsonPropertyName("subject_places")] public List<string> SubjectPlaces { get; set; }

    [JsonPropertyName("subtitle")] public string Subtitle { get; set; }

    [JsonPropertyName("first_sentence")] public FirstSentence FirstSentence { get; set; }

    [JsonPropertyName("excerpts")] public List<ExcerptEntry> Excerpts { get; set; }

    [JsonPropertyName("subject_people")] public List<string> SubjectPeople { get; set; }

    [JsonPropertyName("description")]
    [JsonConverter(typeof(DescriptionJsonConverter<string>))]
    public string? Description { get; set; }

    [JsonPropertyName("links")] public List<Link> Links { get; set; }

    [JsonPropertyName("subject_times")] public List<string> SubjectTimes { get; set; }
}