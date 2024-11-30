using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
	options.AddDocumentTransformer((document, _, _) =>
	{
		document.Info.Title = "Demo API v1";
		document.Info.Description = "An API application to demonstrate alternative to Swagger. Implemented Swagger & Scalar clients.";
		document.Info.Version = "v1";
		document.Info.Contact = new OpenApiContact()
		{
			Name = "Jiten Shahani",
			Email = "shahani.jiten@gmail.com",
			Url = new Uri("https://github.com/JitenShahani")
		};

		return Task.CompletedTask;
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	#region Implement Swagger - Use http launch settings

	app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Demo API v1"); });

	#endregion

	#region Implement Scalar - Use https lauch settings

	app.MapScalarApiReference(options =>
	{
		options
			.WithTitle("Demo API")
			.WithTheme(ScalarTheme.BluePlanet)
			.WithSidebar(true)
			.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
	});

	#endregion
}

app.UseHttpsRedirection();

new MyEndpoints().ConfigureMyEndpoints(app);

app.Run();

record DeveloperInfo(string FirstName, string LastName, string RequestHeaderValue);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}