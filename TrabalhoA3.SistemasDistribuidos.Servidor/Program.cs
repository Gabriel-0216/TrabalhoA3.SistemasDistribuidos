using System.Net.Sockets;
using System.Text;

const string INICIAR_LEILAO = "INICIAR_LEILAO";
const string FINALIZAR_LEILAO = "FINALIZAR_LEILAO";
const string LISTAR_PRODUTOS = "LISTAR_PRODUTOS";
const string DAR_LANCE = "DAR_LANCE";
var listaProdutosLeilao = new List<Produto>();

var listener = new TcpListener(System.Net.IPAddress.Any, 1302);
listener.Start();

while (true)
{
    Console.WriteLine($"Esperando conexão. IP: {System.Net.IPAddress.Any}:1302");
    var client = listener.AcceptTcpClient();
    Console.WriteLine("Cliente aceito");

    var stream = client.GetStream();
    var streamReader = new StreamReader(client.GetStream());
    var streamWriter = new StreamWriter(client.GetStream());
    try
    {
        var buffer = new byte[1024];
        stream.Read(buffer, 0, buffer.Length);
        int recv = 0;
        foreach(var b in buffer)
        {
            if (b != 0)
            {
                recv++;
            }
        }
        var request = Encoding.UTF8.GetString(buffer, 0, recv);
        if (request == LISTAR_PRODUTOS)
        {
            Console.WriteLine("REQUISIÇÃO PARA LISTAGEM DE PRODUTOS EM LEILÃO.");
            RetornarListaProdutos(streamWriter);
        }
        else if(request.Contains(DAR_LANCE))
        {
            Console.WriteLine("LANCE RECEBIDO.");
            var sucesso = DarLance(request, streamWriter);
            if (sucesso) streamWriter.WriteLine("SEU LANCE FOI COMPUTADO COM SUCESSO.");
            else streamWriter.WriteLine("OCORREU UM ERRO.");
            
        }
        else if(request.Contains(INICIAR_LEILAO))
        {
            Console.WriteLine("REQUISIÇÃO PARA CADASTRAR PRODUTO EM LEILÃO.");
            var idCadastrado = CadastrarProdutoLeilao(request);
            streamWriter.WriteLine($"Cadastrado com ID: ${idCadastrado}$.");
        }
        else if(request.Contains(FINALIZAR_LEILAO))
        {
            Console.WriteLine("REQUISIÇÃO PARA FINALIZAR LEILÃO.");
            streamWriter.WriteLine(FinalizarLeilao(request));
        }
        else
        {
            streamWriter.WriteLine("Não entendi a requisição");
        }
        streamWriter.Flush();
    }
    catch(Exception ex)
    {
        Console.WriteLine("Ruim");
        throw;
    }
}

string FinalizarLeilao(string request)
{
    int inicioIndex = 0, ultimoIndex = 0;
    inicioIndex = request.IndexOf("$");
    ultimoIndex = request.LastIndexOf("$");
    var idProduto = Convert.ToInt32(request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1));
    
    inicioIndex = request.IndexOf("&");
    ultimoIndex = request.LastIndexOf("&");
    var emailVendedor = request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);

    var produto = listaProdutosLeilao.FirstOrDefault(p => p.Id == idProduto);
    if(produto is null)
    {
        return "produto não existe";
    }

    if(produto.EmailVendedor == emailVendedor)
    {
        return produto.FinalizarLeilao();
    }
    return "Você não tem autorização para encerrar esse leilão.";

}
bool DarLance(string request, StreamWriter writer)
{
    int inicioIndex = 0, ultimoIndex = 0;
    inicioIndex = request.IndexOf("$");
    ultimoIndex = request.LastIndexOf("$");
    var idProduto = Convert.ToInt32(request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1));

    inicioIndex = request.IndexOf("%");
    ultimoIndex = request.LastIndexOf("%");
    var valorLance = Convert.ToDecimal(request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1));

    inicioIndex = request.IndexOf("&");
    ultimoIndex = request.LastIndexOf("&");
    var emailComprador = request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);

    var produto = listaProdutosLeilao.FirstOrDefault(p => p.Id == idProduto);
    if(produto is null)
    {
        writer.WriteLine("Produto não existe!");
        return false;
    }
    if(valorLance <= produto.MelhorLance)
    {
        writer.WriteLine("Seu lance é menor que o melhor lance atual do produto.");
        return false;
    }
    if(produto.Finalizado == true)
    {
        writer.WriteLine("O leilão desse produto já foi encerrado.");
    }

    produto.MelhorLance = valorLance;
    produto.EmailClienteMelhorLance = emailComprador;
    return true;
}
int CadastrarProdutoLeilao(string request)
{
    int inicioIndex = 0, ultimoIndex = 0;
    inicioIndex = request.IndexOf("$");
    ultimoIndex = request.LastIndexOf("$");
    var produtoNome = request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);

    inicioIndex = request.IndexOf("#");
    ultimoIndex = request.LastIndexOf("#");
    var emailVendedor = request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);

    inicioIndex = request.IndexOf("&");
    ultimoIndex = request.LastIndexOf("&");
    var lanceMinimo = request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);

    Console.WriteLine($"{produtoNome} {emailVendedor} {lanceMinimo}");

    var id = listaProdutosLeilao.Count;
    id++;
    var produto = new Produto() { Id = id, NomeProduto = produtoNome, EmailVendedor = emailVendedor, MelhorLance = Convert.ToDecimal(lanceMinimo) };
    listaProdutosLeilao.Add(produto);
    return id;
}

void RetornarListaProdutos(StreamWriter streamWriter)
{
    foreach(var item in listaProdutosLeilao.Where(p=> p.Finalizado == false))
    {
        streamWriter.WriteLine(item.ToString());
    }

}


public class Produto
{
    public int Id { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public string EmailVendedor { get; set; } = string.Empty;
    public decimal MelhorLance { get; set; }
    public string EmailClienteMelhorLance { get; set; } = string.Empty;
    public bool Finalizado { get; set; } = false;

    public string FinalizarLeilao()
    {
        Finalizado = true;
        return $"LEILÃO FINALIZADO, DADOS: Id: {Id}, Nome do produto: {NomeProduto}, Email do Comprador: {EmailClienteMelhorLance} Melhor lance: {MelhorLance}";

    }
    public override string ToString()
    {
        return $"Id: {Id}, Nome do produto: {NomeProduto}, Melhor lance: {MelhorLance}, já foi finalizado: {Finalizado}";
    }
}
