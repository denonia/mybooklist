using System.Text.Json.Serialization;

namespace MyBookList.Data;

internal struct Created
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("value")] public DateTime? Value { get; set; }
}

internal struct LastModified
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("value")] public DateTime? Value { get; set; }
}

internal struct RemoteIds
{
    [JsonPropertyName("viaf")] public string Viaf { get; set; }

    [JsonPropertyName("wikidata")] public string Wikidata { get; set; }

    [JsonPropertyName("isni")] public string Isni { get; set; }
}

internal struct OpenLibraryAuthor
{
    [JsonPropertyName("type")] public Type Type { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("key")] public string Key { get; set; }

    [JsonPropertyName("source_records")] public List<string> SourceRecords { get; set; }

    [JsonPropertyName("latest_revision")] public int? LatestRevision { get; set; }

    [JsonPropertyName("revision")] public int? Revision { get; set; }

    [JsonPropertyName("created")] public Created Created { get; set; }

    [JsonPropertyName("last_modified")] public LastModified LastModified { get; set; }

    [JsonPropertyName("personal_name")] public string PersonalName { get; set; }

    [JsonPropertyName("remote_ids")] public RemoteIds RemoteIds { get; set; }

    [JsonPropertyName("death_date")] public string DeathDate { get; set; }

    [JsonPropertyName("birth_date")] public string BirthDate { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("alternate_names")] public List<string>? AlternateNames { get; set; }
}

internal struct Type
{
    [JsonPropertyName("key")] public string Key { get; set; }
}