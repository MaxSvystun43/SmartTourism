using System.Text.Json.Serialization;

namespace GeoApiService.Model.Responses;

/// <summary>
/// Represents a response from the Geo API containing a list of geographical features.
/// </summary>
public record GeoApiResponse
{
    /// <summary>
    /// Gets or sets the list of geographical features.
    /// </summary>
    public List<Feature> Features { get; set; }
}

/// <summary>
/// Represents a geographical feature with its type, properties, and geometry.
/// </summary>
public record Feature
{
    /// <summary>
    /// The type of the feature.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The properties of the feature.
    /// </summary>
    public Properties Properties { get; set; }

    /// <summary>
    /// The geometry of the feature.
    /// </summary>
    [JsonPropertyName("geometry")]
    public Point Point { get; set; }
}

/// <summary>
/// Represents the properties of a geographical feature.
/// </summary>
public record Properties
{
    /// <summary>
    /// The name of the geographical feature.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The country where the geographical feature is located.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// The city where the geographical feature is located.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// The district where the geographical feature is located.
    /// </summary>
    public string District { get; set; } // need to understand if needed

    /// <summary>
    /// The street where the geographical feature is located.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// The house number of the geographical feature.
    /// </summary>
    public string HouseNumber { get; set; }

    /// <summary>
    /// The longitude coordinate of the geographical feature.
    /// </summary>
    public double Lon { get; set; }

    /// <summary>
    /// The latitude coordinate of the geographical feature.
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// The categories of the geographical feature.
    /// </summary>
    public List<string> Categories { get; set; } // need to make enum 

    /// <summary>
    /// The details of the geographical feature.
    /// </summary>
    public List<string> Details { get; set; } // need to make enum 

    /// <summary>
    /// The data source from which the geographical feature was obtained.
    /// </summary>
    public Datasource Datasource { get; set; }

    /// <summary>
    /// The opening hours of the geographical feature.
    /// </summary>
    public string OpeningHours { get; set; }

    /// <summary>
    /// The contact information of the geographical feature.
    /// </summary>
    public Contact Contact { get; set; }

    /// <summary>
    /// The facilities available at the geographical feature.
    /// </summary>
    public Facilities Facilities { get; set; }

    /// <summary>
    /// The catering options available at the geographical feature.
    /// </summary>
    public Catering Catering { get; set; }

    /// <summary>
    /// The unique identifier of the geographical feature.
    /// </summary>
    public string PlaceId { get; set; }
}

/// <summary>
/// Represents the data source from which a geographical feature was obtained.
/// </summary>
public record Datasource
{
    /// <summary>
    /// The name of the data source.
    /// </summary>
    public string SourceName { get; set; }

    /// <summary>
    /// The attribution of the data source.
    /// </summary>
    public string Attribution { get; set; }

    /// <summary>
    /// The license of the data source.
    /// </summary>
    public string License { get; set; }

    /// <summary>
    /// The URL of the data source.
    /// </summary>
    public string Url { get; set; }
}

/// <summary>
/// Represents the contact information of a geographical feature.
/// </summary>
public record Contact
{
    /// <summary>
    /// The phone number of the geographical feature.
    /// </summary>
    public string Phone { get; set; }
}

/// <summary>
/// Represents the facilities available at a geographical feature.
/// </summary>
public record Facilities
{
    /// <summary>
    /// Indicates whether the geographical feature has internet access.
    /// </summary>
    public bool InternetAccess { get; set; }
}

/// <summary>
/// Represents the catering options available at a geographical feature.
/// </summary>
public record Catering
{
    /// <summary>
    /// The cuisine type of the geographical feature.
    /// </summary>
    public string Cuisine { get; set; }
}

/// <summary>
/// Represents a geographical point with its type and coordinates.
/// </summary>
public record Point
{
    /// <summary>
    /// The type of the geographical point.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The coordinates of the geographical point.
    /// </summary>
    public List<double> Coordinates { get; set; }
}