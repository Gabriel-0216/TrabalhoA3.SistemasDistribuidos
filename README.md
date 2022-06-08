https://drive.google.com/file/d/16W4CqnCxh7wxL9xVmg7TNdiBk_g6s4VM/view?usp=sharing
# Requisitos mínimos de software:
⦁	.NET 6
⦁	Linux Mint 20.3
⦁	Oracle VM VirtualBox


# MANUAL DE INSTALAÇÃO
## Passo 1º Configurar a rede no VirtualBox:
1. Clicar com o botão direito na opção "Ferramentas" na tela inicial ou simplesmente usar o atalho CTRL + G;
2. Clicar na opção "Rede"
Ao clicar na opção rede o resultado deve ser algo desse tipo:
(OBS: No seu caso provavelmente essa "RedeLeilao" não deve existir)
 

3. Clicar na opção para criar uma nova rede;
Ao clicar na opção de criar uma nova rede o VirtualBox cria uma com o nome "NatNetwork"
4. Selecionar a rede criada e clicar na opção "Editar", é o terceiro ícone ao lado direito
 

4. Para controle criaremos a rede com o nome "RedeLeilaoManual"; configure o CIDR com o seguinte endereço de ip: 192.168.100.0/24 e habilite o checkbox "Suporta DHCP" caso ele esteja desmarcado.
 
5. Clicar em Ok para confirmar a edição e depois em Ok novamente para retornar ao menu principal.

## Passo 2º Criar as máquinas virtuais;
[Opcional: No final do documento temos uma seção "Como criar máquinas virtuais"]
Precisamos criar três máquinas virtuais com o Linux Mint;
1. Criar uma máquina para o SERVIDOR;
⦁	Recomendo que o nome da máquina virtual seja "Servidor" para melhor controle
2. Criar uma máquina para o CLIENTE COMPRADOR;
⦁	Recomendo que o nome da máquina seja "Cliente comprador" para melhor controle
3. Criar uma máquina para o CLIENTE VENDEDOR;
⦁	Recomendo que o nome da máquina seja "Cliente vendedor" para melhor controle

## Passo 3º Registrar as máquinas virtuais na Rede
Precisamos colocar todas nossas máquinas virtuais na mesma rede para que elas comuniquem entre si.
1. Clicar com o botão direito na máquina no gerenciador do VirtualBox e clicar em "configurações", ou simplesmente usar o atalho CTRL+S
2. Na tela que abrir, clicar na opção "Rede" no menu lateral direito
3. Clicar na dropdown "Conectado a:" e selecionar a opção "Rede NAT"
4. Ao clicar na opção "Rede NAT" a dropdown "Nome:" é desbloqueada, clique nela e selecione a rede que criamos: "RedeLeilaoManual"
(OBS: Note que a "RedeLeilao" aparece no meu caso, não deve aparecer pra você e a rede criada deve ser a única escolha)
5. Clicar em OK para confirmar 
6. Verifique se a rede foi aplicada com sucesso rodando o comando "ifconfig" no terminal, a resposta deve ser algo como essa.
[OBS: Caso esteja configurando a máquina do SERVIDOR, salvar o IP para podermos utilizar nas próximas seções]
 



## Passo 3º Instalar o dotnet sdk nas máquinas (ESSE PROCESSO DEVE SER EFETUADO NAS TRÊS MÁQUINAS!)
1º Abrir o terminal e executar o comando: 

sudo rm /etc/apt/preferences.d/nosnap.pref

2º aplicar:

sudo apt update

3º Executar o segundo comando para instalar o gerenciador de pacotes que nos ajudará a instalar o dotnet sdk

sudo apt install snapd

4º Executar o terceiro comando para instalar o dotnet sdk:

sudo snap install dotnet-sdk --classic

5º após finalizar a instalação, executar o comando "dotnet --version" no terminal, a saída deve ser algo como:

 

## Passo 4º instalar a aplicação do servidor:
1º Na área de trabalho da máquina virtual destinada ao servidor criar uma pasta chamada "servidor"

2º Abrir a pasta via terminal e executar o comando: "dotnet new console"

3º Verifique que agora a pasta tem alguns arquivos, entre eles o arquivo Program.cs.

4º Acessar o arquivo Program.cs com o editor de texto padrão do Mint e colar o código:



## _____________#@!#$@___@#!$_______ não copie essa linha ___$@!_______$@%____


```
using System.Net.Sockets;
using System.Text;

const string INICIAR_LEILAO = "INICIAR_LEILAO";
const string FINALIZAR_LEILAO = "FINALIZAR_LEILAO";
const string LISTAR_PRODUTOS = "LISTAR_PRODUTOS";
const string DAR_LANCE = "DAR_LANCE";
const string CONSULTAR_ARREMATADOS = "CONSULTAR_ARREMATADOS";

var listaProdutosLeilao = new List<Produto>();
var listaProdutosArrematados = new List<Produto>();

var listener = new TcpListener(System.Net.IPAddress.Any, 1302);
listener.Start();

while (true)
{
    Console.WriteLine($"Esperando conexão. IP: {System.Net.IPAddress.Any}:1302");
    var client = listener.AcceptTcpClient();
    Console.WriteLine($"CLIENTE ACEITO E RECEBIDO IP: {client.Client.RemoteEndPoint}");

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
            streamWriter.WriteLine($"PRODUTO CADASTRADO COM SUCESSO, ID DO PRODUTO CRIADO: ${idCadastrado}$.");
        }
        else if(request.Contains(FINALIZAR_LEILAO))
        {
            Console.WriteLine("REQUISIÇÃO PARA FINALIZAR LEILÃO.");
            streamWriter.WriteLine(FinalizarLeilao(request));
        }
        else if (request.Contains(CONSULTAR_ARREMATADOS))
        {
            Console.WriteLine("CONSULTA PARA VERIFICAR PRODUTOS ARREMATADOS");
            RetornarProdutosArrematados(request, streamWriter);
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
        var mensagemRetornoFinalizacao = produto.FinalizarLeilao();
        listaProdutosLeilao.Remove(produto);
        if (produto.TeveLances)
        {
            listaProdutosArrematados.Add(produto);
        }
        return mensagemRetornoFinalizacao;
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
    produto.TeveLances = true;
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
    if(listaProdutosLeilao.Count == 0)
    {
        streamWriter.WriteLine("Nenhum produto em leilão");
        return;
    }
    foreach(var item in listaProdutosLeilao.Where(p=> p.Finalizado == false).ToList())
    {
        streamWriter.WriteLine(item.ToString());
    }

}

void RetornarProdutosArrematados(string request, StreamWriter streamWriter)
{
    int inicioIndex = 0, ultimoIndex = 0;
    inicioIndex = request.IndexOf("%");
    ultimoIndex = request.LastIndexOf("%");
    var emailComprador = request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);

    var listaArrematadosComprador = listaProdutosArrematados
        .Where(p => p.EmailClienteMelhorLance == emailComprador &&
                                            p.Finalizado == true &&
                                            p.TeveLances == true)
                                            .ToList();
    if(listaProdutosArrematados.Count == 0)
    {
        streamWriter.WriteLine("Você não arrematou nenhum leilão.");
        return;
    }
    var produtos = "LEILÃO FINALIZADO;";
    foreach(var item in listaProdutosArrematados)
    {
        produtos += item.RetornarArremate();
    }
    streamWriter.WriteLine(produtos);
}


public class Produto
{
    public int Id { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public string EmailVendedor { get; set; } = string.Empty;
    public decimal MelhorLance { get; set; }
    public string EmailClienteMelhorLance { get; set; } = string.Empty;
    public bool Finalizado { get; set; } = false;
    public bool TeveLances { get; set; } = false;

    public string FinalizarLeilao()
    {
        Finalizado = true;
        if (!TeveLances) return $"LEILÃO FINALIZADO, O produto de ID: {Id}, Nome :{NomeProduto}, NÃO TEVE NENHUM LANCE REGISTRADO.";

        return $"LEILÃO FINALIZADO, DADOS: Id: {Id}, Nome do produto: {NomeProduto}, Email do Comprador: {EmailClienteMelhorLance} Melhor lance: {MelhorLance}";

    }
    public string RetornarArremate()
    {
        return $"({NomeProduto}; Lance: {MelhorLance};)";
    }
    public override string ToString()
    {
        return $"Id: {Id}, Nome do produto: {NomeProduto}, Melhor lance: {MelhorLance}, já foi finalizado: {Finalizado}";
    }
}
  
  ```
________Fim do codigo nao cópiar ________________

Caso ocorra alguma dificuldade ao copiar-colar, o código está no github:

https://github.com/Gabriel-0216/TrabalhoA3.SistemasDistribuidos

5º Salvar o arquivo

6º Utilizando o comando: "dotnet run" é possível executar o código.

A saída deve ser algo dessa forma.

 

(OPCIONAL: Em caso de erros)

7º Caso o terminal retorne algum erro executar os seguintes comandos:

"dotnet clean"

"dotnet build"

8. Executar "dotnet run" novamente
9. 

Passo 5º Instalar a aplicação do Cliente (VENDEDOR)

1. Na máquina virtual destinada ao cliente vendedor criar uma pasta na área de trabalho chamada "vendedor"
2. Abrir a pasta no terminal e executar o comando: "dotnet new console"  
3. Abrir o arquivo Program.cs que foi criado dentro da pasta 
4. Colar o código abaixo:

_2@_________________$#__$@__não copie essa linha___$@_____$@!_______________


  ```
using System.Net.Sockets;
using System.Text;

var finalizar = false;
var listaProdutosDoVendedor = new List<Produto>();
const string IP_SERVIDOR = "";
const string INICIAR_LEILAO = "INICIAR_LEILAO";
const string FINALIZAR_LEILAO = "FINALIZAR_LEILAO";
Console.WriteLine("CLIENTE: VENDEDOR");

Console.WriteLine("Digite o seu nome:");
var nome = Console.ReadLine();
Console.WriteLine("Digite o seu email:");
var email = Console.ReadLine();

var vendedor = new Vendedor() { Email = email, Nome = nome };


while(finalizar is not true)
{
    Console.WriteLine("----------------------------");
    Console.WriteLine("Digite 1 para cadastrar um novo produto.");
    Console.WriteLine("Digite 2 para finalizar um leilão.");
    Console.WriteLine("Digite 3 para listar meus produtos em leilão");
    Console.WriteLine("Digite 0 para finalizar a aplicação.");
    Console.WriteLine("-----------------------------------------");

    try
    {
        var opcao = Convert.ToInt32(Console.ReadLine());
        switch (opcao)
        {
            case 1:
                CadastrarNovoProduto();
                Console.WriteLine("Clicar em algo para continuar.");
                Console.ReadKey();
                Console.Clear();
                break;
            case 2:
                FinalizarLeilao();
                Console.WriteLine("Clicar em algo para continuar.");
                Console.ReadKey();
                Console.Clear();
                break;
            case 3:
                Console.WriteLine("Listagem dos produtos em leilão.");
                ListarProdutosEmLeilao();
                Console.WriteLine("Clicar em algo para continuar.");
                Console.ReadKey();
                Console.Clear();
                break;
            case 0:
                finalizar = true;
                break;
            default:
                Console.WriteLine("Opção inexistente.");
                break;
        }

    }
    catch(Exception ex)
    {
        Console.WriteLine("Digite um número para continuar.");
    }
}


void ListarProdutosEmLeilao()
{
    if(listaProdutosDoVendedor.Count == 0)
    {
        Console.WriteLine("Nenhum produto em leilão.");
        return;
    }
    foreach(var item in listaProdutosDoVendedor)
    {
        Console.WriteLine(item.RetornarProduto());
    }
}


void CadastrarNovoProduto()
{
    var continuar = false;
    var valor = 0;
    Console.WriteLine("Digite o nome do produto.");
    var nome = Console.ReadLine();
    Console.WriteLine("Digite o lance mínimo do produto.");
    while(continuar is not true)
    {
        try
        {
            valor = Convert.ToInt32(Console.ReadLine());
            continuar = true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Digite um valor válido.");
        }
    }
    var produto = new Produto() { NomeProduto = nome, EmailVendedor = vendedor.Email, LanceMinimo = valor };

    var client = CriarClienteTcp();
    var mensagem = INICIAR_LEILAO + "\n" + produto.ToString();
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando requisição ao servidor.");

    var streamReader = new StreamReader(stream);
    var resposta = streamReader.ReadLine();
    if(resposta is not null)
    {
        int inicioIndex = 0, ultimoIndex = 0;

        inicioIndex = resposta.IndexOf("$");
        ultimoIndex = resposta.LastIndexOf("$");
        var idCadastrado = resposta.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);
        produto.Id = Convert.ToInt32(idCadastrado);
        stream.Close();
        client.Close();
        listaProdutosDoVendedor.Add(produto);
        Console.WriteLine(resposta);
    }
    else
    {
        Console.WriteLine("Não tivemos resposta do servidor!");
    }

}
void FinalizarLeilao()
{
    if (listaProdutosDoVendedor.Count == 0)
    {
        Console.WriteLine("Você não tem nenhum produto em leilão!");
        return;
    }
    var continuar = false;
    var idProduto = 0;
    var client = CriarClienteTcp();
    ListarProdutosEmLeilao();
    Console.WriteLine("Digite o ID do produto a ser finalizado o leilão?");
    while(continuar is not true)
    {
        try
        {
            idProduto = Convert.ToInt32(Console.ReadLine());
            var produto = listaProdutosDoVendedor.FirstOrDefault(p => p.Id == idProduto);
            if (produto is null) Console.WriteLine("Esse produto não é valido, tente novamente.");

            continuar = true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Digite um número válido!");
        }
    }
    var mensagem = @$"{FINALIZAR_LEILAO} \n Id: ${idProduto}$, EMAIL_COMPRADOR: &{vendedor.Email}&";
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine($"Enviando requisição para finalizar leilão do produto de id: {idProduto}.");

    var streamReader = new StreamReader(stream);
    var resposta = streamReader.ReadLine();
    if(resposta is not null && resposta.Contains("LEILÃO FINALIZADO"))
    {
        Console.WriteLine(resposta);
        var produto = listaProdutosDoVendedor.FirstOrDefault(p => p.Id == idProduto);
        if(produto is not null)
        {
            listaProdutosDoVendedor.Remove(produto);
            Console.WriteLine("Leilão finalizado com sucesso!");
        }
    }
    else
    {
        Console.WriteLine("Ocorreu um erro");
    }
}



TcpClient CriarClienteTcp()
{
    return new TcpClient(IP_SERVIDOR, 1302);
}

public class Vendedor
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
public class Produto
{
    public int Id { get; set; } = 0;
    public string NomeProduto { get; set; } = string.Empty;
    public string EmailVendedor { get; set; } = string.Empty;
    public int LanceMinimo { get; set; }

    public string RetornarProduto()
    {
        return $"Id do produto: {Id}; Nome do produto: {NomeProduto}";
    }
    public override string ToString()
    {
        return $@"NOME_PRODUTO: ${NomeProduto}$
                  EMAIL_VENDEDOR: #{EmailVendedor}#
                  LANCE_MINIMO: &{LanceMinimo}&";
    }

}
  
  ```
_____________$@___Fim código não copiar essa linha___________$@!____#$@%______

5. ATENÇÃO: No código acima procure a seguinte linha de código:
const string IP_SERVIDOR = "";

Colocar o IP DO SERVIDOR ENTRE ASPAS.

Exemplo:

const string IP_SERVIDOR = "192.168.100.4";


(Opcional: caso não tenha salvo o IP do servidor como foi solicitado, basta executar o comando "ifconfig" no terminal)

6. rodar a aplicação com "dotnet run"

7. Testar o envio ao servidor:
 


## Passo 6º - Criar a máquina do cliente comprador
1. Na máquina virtual destinada ao cliente comprador crie uma pasta na área de trabalho chamada "comprador" 
2. Acessar a pasta via terminal e executar o comando: "dotnet new console"
3. Abrir o arquivo Program.cs que foi criado dentro da pasta.
4º colar o seguinte código:


_________%@!______inicio do código não copiar___________%@#$@%¨_____________

  ```
using System.Net.Sockets;
using System.Text;

var finalizar = false;
const string IP_SERVIDOR = "";
const string LISTAR_PRODUTOS = "LISTAR_PRODUTOS";
const string DAR_LANCE = "DAR_LANCE";
const string CONSULTAR_ARREMATADOS = "CONSULTAR_ARREMATADOS";


Console.WriteLine("CLIENTE: COMPRADOR");

Console.WriteLine("Digite seu nome.");
var nome = Console.ReadLine();
Console.WriteLine("Digite seu email.");
var email = Console.ReadLine();

var cliente = new Comprador() { Email = email, Nome = nome };

while(finalizar is not true)
{
    var opcao = 0;
    Console.WriteLine("-----------------------------------------");
    Console.WriteLine("Digite 1 para requisitar todos os produtos.");
    Console.WriteLine("Digite 2 para dar um lance em um produto.");
    Console.WriteLine("Digite 3 para verificar seus arremates.");
    Console.WriteLine("Digite 0 para finalizar o leilão.");
    Console.WriteLine("-----------------------------------------");


    try
    {
        opcao = Convert.ToInt32(Console.ReadLine());
        switch (opcao)
        {
            case 1:
                var b = RequisitarProdutos();
                break;
            case 2:
                DarLance();
                Console.WriteLine("Digite algo para continuar");
                Console.ReadKey();
                Console.Clear();
                break;
            case 3:
                VerificarArrematados();
                break;
            case 0:
                finalizar = true;
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }

    }catch(Exception ex)
    {
        Console.WriteLine("Digite um número válido.");
    }
}


void VerificarArrematados()
{

    var client = CriarClienteTcp();
    var mensagem = $"CONSULTAR_ARREMATADOS: %{cliente.Email}%";
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando ao servidor.");
    var streamReader = new StreamReader(stream);
    var resposta = streamReader.ReadLine();
    if(resposta is not null)
    {
        Console.WriteLine("SEUS ARREMATES:");
        Console.WriteLine(resposta);
        Console.WriteLine("---------------------------");
    }
    stream.Close();
    client.Close();
}

void DarLance()
{
    if (!RequisitarProdutos())
    {
        return;
    }
    else
    {
        var client = CriarClienteTcp();
        var continuar = false;
        var idProduto = 0;
        var lance = 0;
        while (continuar is not true)
        {
            try
            {
                Console.WriteLine("Digite o ID do produto a dar o lance.");

                idProduto = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Digite o valor do lance: ");
                lance = Convert.ToInt32(Console.ReadLine());
                continuar = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Valor inválido, continuar");
            }
        }

        var mensagem = $"{DAR_LANCE}, Id_Produto: ${idProduto}$, Lance: %{lance}%, Email_Comprador: &{cliente.Email}&";
        var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
        var envioDados = Encoding.ASCII.GetBytes(mensagem);
        var stream = client.GetStream();
        stream.Write(envioDados, 0, envioDados.Length);
        Console.WriteLine("Enviando ao servidor.");
        var streamReader = new StreamReader(stream);
        var resposta = streamReader.ReadLine();
        Console.WriteLine(resposta);
        stream.Close();
        client.Close();
    }
  
}
bool RequisitarProdutos()
{
    var operacaoSucesso = false;
    var client = CriarClienteTcp();
    var mensagem = LISTAR_PRODUTOS;
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando ao servidor.");
    var streamReader = new StreamReader(stream);
    if(streamReader is not null)
    {
        var resposta = streamReader.ReadLine();
        if (resposta is not null)
        {
            Console.WriteLine(resposta);
            if(resposta.Contains("Nenhum produto em leilão"))
            {
                operacaoSucesso = false;
            }
            else
            {
                operacaoSucesso = true;
            }
        }
        else
        {
            operacaoSucesso = false;
        }
    }    
    else
    {
        Console.WriteLine("Falha ao receber resposta do servidor.");
        operacaoSucesso = false;
    }
    stream.Close();
    client.Close();
    return operacaoSucesso;
}

TcpClient CriarClienteTcp()
{
    return new TcpClient(IP_SERVIDOR, 1302);
}

public class Comprador
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
  
  ```
_________#@!_________fim código não copiar___$@_______________________%@___

5. ATENÇÃO: No código acima procure a seguinte linha de código:
const string IP_SERVIDOR = "";

Colocar o IP DO SERVIDOR ENTRE ASPAS.

Exemplo:

const string IP_SERVIDOR = "192.168.100.4";

(Opcional: caso não tenha salvo o IP do servidor como foi solicitado, basta executar o comando "ifconfig" no terminal)

6. rodar a aplicação com "dotnet run"
7. Testar 
 


# Apêndice
CRIAÇÃO DAS MÁQUINAS VIRTUAIS
Nome e sistema operacional: ATENÇÃO A VERSÃO (UBUNTU 64-BIT)
Memória 
Disco rígido

FINALIZAÇÃO
 
A máquina deve ser algo dessa forma:

Iniciando a máquina virtual
Ao inicializar a máquina, selecionar a ISO do Linux Mint 20.3 e confirmar,  a máquina virtual deve mostrar essa tela: Selecionar "Start Linux Mint"

Ao completar o boot a tela deve ser algo como essa:


Como descobrir o IP da máquina:
1. Basta executar o comando ifconfig no termina


# Seção de soluções:
Referência 1. Como fazer duas máquinas virtuais se comunicarem com o VirtualBox:
https://youtu.be/vReAkOq-59I
Referência 2. Download do Linux Mint
https://linuxmint.com/edition.php?id=292
Referência 3. Instalação do dotnet no Linux
https://snapcraft.io/install/dotnet-sdk/mint
Referência 4. Repósitorio do projeto:
https://github.com/Gabriel-0216/TrabalhoA3.SistemasDistribuidos
