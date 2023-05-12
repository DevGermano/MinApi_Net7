internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();


        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Adiciona filtro antes de enviar a resposta da requisição. 
        app.MapGet("inviteCode", (string inviteCode) =>
        {
            return "Muito bom, você tem o invite!";
        })
        .AddEndpointFilter(async (context, next) =>
        {
            if (!context.HttpContext.Request.QueryString.Value.Contains("invite123"))
            {
                return Results.BadRequest("Você não possui o invite!");
            }
            return await next(context);
        });

        // Adiciona uma rota que chama uma classe chamada ShortCircuit
        app.MapGet("/testeLogico", () => "").AddEndpointFilter<ShortCircuit>();


        // Adiciona rota de upload que salva arquivo na raiz
        app.MapPost("/upload", async (IFormFile arquivo) =>
        {
            await arquivo.CopyToAsync(File.OpenWrite($@"{DateTime.Now.Ticks}.txt"));
        });


        // Adiciona rota de upload que salva arquivo na raiz
        app.MapPost("/uploadArquivo", async (IFormFile arquivo) =>
        {
            string arquivoTemp = CriaCaminhoArquivoTemp();
            using var stream = File.OpenWrite(arquivoTemp);
            await arquivo.CopyToAsync(stream);
            
            return Results.Ok("Arquivo enviado com sucesso!");
        });

        app.MapPost("/uploadArquivos", async (IFormFileCollection arquivos) =>
        {
            foreach (var arquivo in arquivos)
            {
                string arquivoTemp = CriaCaminhoArquivoTemp();
                using var stream = File.OpenWrite(arquivoTemp);
                await arquivo.CopyToAsync(stream);
    
            }
            return Results.Ok("Arquivos enviados com sucesso!");
        });




        app.UseHttpsRedirection();
        app.Run();

        static string CriaCaminhoArquivoTemp()
        {
            var nomeArquivo = $@"{DateTime.Now.Ticks}.tmp";
            var directoryPath = Path.Combine("temp", "uploads");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return Path.Combine(directoryPath, nomeArquivo);
        }
    }
}
public class ShortCircuit : IEndpointFilter
{
    public ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {

        return new ValueTask<object>(Results.Json(new { Curto = "circuito" }));
    }
}

