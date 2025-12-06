using Serilog;
using System;

namespace AutomateClickerBrielina.Servico
{
    public class ServicoComSerilog
    {
        public ServicoComSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File($@"logs\log{DateTime.Now.ToString("ddMMyyyy")}.txt")
                .CreateLogger();

            Log.Information($"Aplicação Iniciada.");
            //Log.Information("Hello, Serilog!");

            //Log.CloseAndFlush();
        }
    }
}
