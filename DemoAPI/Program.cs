var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	#region Implement Swagger - Use http launch settings

	app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openApi/v1.json", "Demo API v1"); });

	#endregion

	#region Implement Scalar - Use https launch settings

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