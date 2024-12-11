using GeoApiService;
using GeoApiService.Model;
using GeoApiService.Model.Requests;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using Serilog;
using SmartTourism.Endpoints.Models;
using SmartTourism.Extensions;
using SmartTourism.PathFinding.Service;
using SmartTourism.PathFinding.Service.Models;
using SmartTourism.RuleBase.Service.Models;
using SmartTourism.RuleBase.Service.Services;
using Location = SmartTourism.PathFinding.Service.Models.Location;

namespace SmartTourism.Services;

public class SmartTourismService
{
    private readonly IGeoapifyService _geoapifyService;

    public SmartTourismService(IGeoapifyService geoapifyService)
    {
        _geoapifyService = geoapifyService ?? throw new ArgumentNullException(nameof(geoapifyService));
    }

    public async Task<List<Point>?> Test(PathFindingRequest request)
    {
        // var points = new List<Point>
        // {
        //     new Point { Name = "A", Categories = "Start", Latitude = 40.7128, Longitude = -74.0060 },
        //     new Point { Name = "B", Categories = "Restaurant", Latitude = 40.7308, Longitude = -73.9973 },
        //     new Point { Name = "C", Categories = "Nature", Latitude = 40.7484, Longitude = -73.9857 },
        //     new Point { Name = "D", Categories = "Nature", Latitude = 40.7580, Longitude = -73.9855 },
        //     new Point { Name = "E", Categories = "End", Latitude = 40.7128, Longitude = -74.0059 },
        //     new Point { Name = "F", Categories = "Restaurant", Latitude = 40.7192, Longitude = -74.0021 },
        //     new Point { Name = "G", Categories = "Park", Latitude = 40.7407, Longitude = -73.9893 },
        //     new Point { Name = "H", Categories = "Cultural", Latitude = 40.7295, Longitude = -73.9965 },
        //     new Point { Name = "I", Categories = "Tourism", Latitude = 40.7411, Longitude = -73.9897 },
        //     new Point { Name = "J", Categories = "Shopping", Latitude = 40.7589, Longitude = -73.9851 }
        // };
        
        var results = await _geoapifyService.GetPlacesAsync(request.GeoApiRequest);

        var startLocation = request.Start.ToPoint("Start");
        var endLocation = request.End.ToPoint("End");
        var points = results.Where(x => x.Name != null).Select(x => x.ToPoint()).ToList();
        points.AddRange([startLocation, endLocation]);

        
        var routeFinder = new RouteFinder(points, startLocation, endLocation);
        
        var route = routeFinder.FindRouteUsingBidirectualAStar();
        
        return route;
    }


    private void SaveGraphVisualization(List<Location> locations, List<UpdatedEdge> edges, List<Location> path)
{
    var plotModel = new PlotModel { Title = "Smart Tourism Graph with Optimal Path" };

    // Add axes
    plotModel.Axes.Add(new LinearAxis
    {
        Position = AxisPosition.Bottom,
        Title = "Longitude",
        MajorGridlineStyle = LineStyle.Solid,
        MinorGridlineStyle = LineStyle.Dot
    });
    plotModel.Axes.Add(new LinearAxis
    {
        Position = AxisPosition.Left,
        Title = "Latitude",
        MajorGridlineStyle = LineStyle.Solid,
        MinorGridlineStyle = LineStyle.Dot
    });

    // Add edges as line series
    foreach (var edge in edges)
    {
        var lineSeries = new LineSeries { LineStyle = LineStyle.Solid, StrokeThickness = 1.0, Color = OxyColors.Gray };
        lineSeries.Points.Add(new DataPoint(edge.From.Longitude, edge.From.Latitude));
        lineSeries.Points.Add(new DataPoint(edge.To.Longitude, edge.To.Latitude));
        plotModel.Series.Add(lineSeries);
    }

    // Add locations as scatter points
    var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 4, MarkerFill = OxyColors.Blue };
    foreach (var location in locations)
    {
        scatterSeries.Points.Add(new ScatterPoint(location.Longitude, location.Latitude));
    }
    plotModel.Series.Add(scatterSeries);

    // Highlight the optimal path
    var pathSeries = new LineSeries { LineStyle = LineStyle.Solid, StrokeThickness = 2.0, Color = OxyColors.Red };
    foreach (var location in path)
    {
        pathSeries.Points.Add(new DataPoint(location.Longitude, location.Latitude));
    }
    plotModel.Series.Add(pathSeries);

    // Save the plot as an image
    var exporter = new OxyPlot.SkiaSharp.PngExporter { Width = 800, Height = 600 };
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "OptimalPathVisualization.png");
    using (var stream = File.Create(filePath))
    {
        exporter.Export(plotModel, stream);
    }

    Log.Information("Graph visualization with optimal path saved at: {FilePath}", filePath);
}
    
    /*private void SaveGraphVisualization(List<Location> locations, List<UpdatedEdge> edges, List<Location> path)
    {
        var plotModel = new PlotModel { Title = "Smart Tourism Graph with Optimal Path and Distances" };

        // Add axes
        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Longitude",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });
        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Latitude",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        // Add edges as line series and annotate distances
        foreach (var edge in edges)
        {
            // Draw the edge
            var lineSeries = new LineSeries { LineStyle = LineStyle.Solid, StrokeThickness = 1.0, Color = OxyColors.Gray };
            lineSeries.Points.Add(new DataPoint(edge.From.Longitude, edge.From.Latitude));
            lineSeries.Points.Add(new DataPoint(edge.To.Longitude, edge.To.Latitude));
            plotModel.Series.Add(lineSeries);

            // Annotate the distance
            var midpointLongitude = (edge.From.Longitude + edge.To.Longitude) / 2;
            var midpointLatitude = (edge.From.Latitude + edge.To.Latitude) / 2;

            var distanceAnnotation = new TextAnnotation
            {
                TextPosition = new DataPoint(midpointLongitude, midpointLatitude),
                Text = $"{edge.Distance:F2} km",
                FontSize = 10,
                Stroke = OxyColors.Transparent,
                TextColor = OxyColors.Black
            };
            plotModel.Annotations.Add(distanceAnnotation);
        }

        // Add locations as scatter points
        var scatterSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 4,
            MarkerFill = OxyColors.Blue
        };
        foreach (var location in locations)
        {
            scatterSeries.Points.Add(new ScatterPoint(location.Longitude, location.Latitude));
        }
        plotModel.Series.Add(scatterSeries);

        // Highlight the start and end points
        var startLocation = path.First();
        var endLocation = path.Last();

        var startSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Star,
            MarkerSize = 8,
            MarkerFill = OxyColors.Green,
            Title = "Start"
        };
        startSeries.Points.Add(new ScatterPoint(startLocation.Longitude, startLocation.Latitude));
        plotModel.Series.Add(startSeries);

        var endSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Triangle,
            MarkerSize = 8,
            MarkerFill = OxyColors.Red,
            Title = "End"
        };
        endSeries.Points.Add(new ScatterPoint(endLocation.Longitude, endLocation.Latitude));
        plotModel.Series.Add(endSeries);

        // Highlight the optimal path
        var pathSeries = new LineSeries
        {
            LineStyle = LineStyle.Solid,
            StrokeThickness = 2.0,
            Color = OxyColors.Red,
            Title = "Optimal Path"
        };
        foreach (var location in path)
        {
            pathSeries.Points.Add(new DataPoint(location.Longitude, location.Latitude));
        }
        plotModel.Series.Add(pathSeries);

        // Save the plot as an image
        var exporter = new OxyPlot.SkiaSharp.PngExporter { Width = 800, Height = 600 };
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "OptimalPathWithDistances.png");
        using (var stream = File.Create(filePath))
        {
            exporter.Export(plotModel, stream);
        }

        Log.Information("Graph visualization with optimal path, distances, start, and end locations saved at: {FilePath}", filePath);
    }*/
    
    
    public static double CalculateDistance(Location p1, Location p2)
    {
        return Math.Sqrt(Math.Pow(p1.Longitude - p2.Longitude, 2) + Math.Pow(p1.Latitude - p2.Latitude, 2));
    }
    
    private (double Latitude, double Longitude) GenerateRandomPoint(double lat, double lon, double radiusKm)
    {
        Random random = new Random();
        double radiusEarthKm = 6371.0; // Earth's radius in kilometers

        // Convert radius from kilometers to degrees
        double radiusInDegrees = radiusKm / radiusEarthKm * (180 / Math.PI);

        double u = random.NextDouble();
        double v = random.NextDouble();
        double w = radiusInDegrees * Math.Sqrt(u);
        double t = 2 * Math.PI * v;
        double x = w * Math.Cos(t);
        double y = w * Math.Sin(t);

        // Adjust the x-coordinate for the shrinking of the east-west distances
        double new_x = x / Math.Cos(lat * Math.PI / 180);

        double foundLongitude = lon + new_x;
        double foundLatitude = lat + y;

        return (foundLatitude, foundLongitude);
    }

    private List<Location> GenerateLocations()
    {
        double centralLatitude = 50.61762454932047 ;
        double centralLongitude = 26.234978325243198;

// Define the number of locations
        int numberOfLocations = 20;

// Define the radius in kilometers within which to generate random locations
        double radiusInKm = 5.0;

// Create a random number generator
        Random random = new Random();

// Generate locations
        var locations = new List<Location>();
        for (int i = 0; i < numberOfLocations; i++)
        {
            // Generate a random point within the radius
            var randomPoint = GenerateRandomPoint(centralLatitude, centralLongitude, radiusInKm);

            var location = new Location
            {
                Id = Guid.NewGuid(),
                Name = $"Location_{i}",
                Latitude = randomPoint.Latitude,
                Longitude = randomPoint.Longitude,
            };
            locations.Add(location);
        }

        return locations;
    }
    
}