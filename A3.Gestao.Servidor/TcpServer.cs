using A3.Gestao.Servidor.Models;
using A3.Gestao.Servidor.Models.Constantes;
using A3.Gestao.Servidor.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace A3.Gestao.Servidor
{
    public class TcpServer(LeilaoService leilaoService, int port = 1302) : IDisposable
    {
        private readonly TcpListener _listener = new(IPAddress.Any, port);
        private readonly LeilaoService _leilaoService = leilaoService;
        private bool _isRunning;

        public async Task StartAsync()
        {
            _isRunning = true;
            _listener.Start();
            Console.WriteLine($"Servidor iniciado na porta {((IPEndPoint)_listener.LocalEndpoint).Port}");

            try
            {
                while (_isRunning)
                {
                    var client = _listener.AcceptTcpClient();
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception ex) when (_isRunning)
            {
                Console.WriteLine($"Erro no servidor: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine($"Requisição recebida: {request}");
                    var response = ProcessRequest(request);

                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao tratar cliente: {ex.Message}");
            }
        }

        private string ProcessRequest(string request)
        {
            try
            {
                if (request == Comandos.LISTAR_PRODUTOS)
                {
                    return ProcessResponse(_leilaoService.RetornarProdutos());
                }
                else if (request.Contains(Comandos.DAR_LANCE))
                {
                    return ProcessResponse(_leilaoService.DarLance(request));
                }
                else if (request.Contains(Comandos.INICIAR_LEILAO))
                {
                    return ProcessResponse(_leilaoService.Cadastro(request));
                }
                else if (request.Contains(Comandos.FINALIZAR_LEILAO))
                {
                    return ProcessResponse(_leilaoService.FinalizarLeilao(request));
                }
                else if (request.Contains(Comandos.CONSULTAR_ARREMATADOS))
                {
                    return ProcessResponse(_leilaoService.ConsultaArrematados(request));
                }

                return "COMANDO_NÃO_RECONHECIDO";
            }
            catch (FormatException ex)
            {
                return $"ERRO_DE_FORMATO: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return "ERRO_INTERNO";
            }
        }

        private string ProcessResponse<T>(ResultadoOperacao<T> resultado) => resultado.FoiSucesso ? "Sucesso" : "Falha";
        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
        }

        public void Dispose() => Stop();
    }
}
