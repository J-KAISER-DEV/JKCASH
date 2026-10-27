namespace JKCASH.WinForms;

public partial class FrmInicial : Form
{
    private readonly JKCASH.Infrastructure.Integracoes.Api.ConexaoApiCliente? _clienteApi;
    private CancellationTokenSource? _verificacaoAtual;
    private bool _verificando, _apiDisponivel;

    public FrmInicial()
    {
        InitializeComponent();
    }

    public FrmInicial(JKCASH.Infrastructure.Integracoes.Api.ConexaoApiCliente clienteApi) : this()
    {
        ArgumentNullException.ThrowIfNull(clienteApi);
        _clienteApi = clienteApi;
        btnVerificarConexao.Enabled = true;
        btnVerificarConexao.Click += async (_, _) => await VerificarConexaoAsync();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        // Confere somente a existência do arquivo, sem abrir ou modificar o banco.
        lblEstadoLocal.Text = File.Exists(
            JKCASH.Infrastructure.Persistencia.ConexaoLocal.CaminhoBanco)
            ? "Arquivo local encontrado"
            : "Será preparado ao abrir";

        await VerificarConexaoAsync();
    }

    private async Task VerificarConexaoAsync()
    {
        if (_clienteApi is null || _verificando || _operandoBackup || IsDisposed || Disposing) return;

        _verificando = true;
        _apiDisponivel = false;
        _armazenamentoDisponivel = false;
        AtualizarAcoesBackup();
        btnVerificarConexao.Enabled = false;
        btnVerificarConexao.Text = "Verificando…";
        lblEstadoServidor.Text = "Verificando conexão…";
        lblEstadoServidor.ForeColor = Visual.EstiloVisual.Secundario;

        // Cada tentativa tem seu próprio prazo. Abrir o app continua disponível.
        using var cancelamento = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        _verificacaoAtual = cancelamento;
        try
        {
            bool disponivel = await _clienteApi.VerificarAsync(cancelamento.Token);
            if (IsDisposed || Disposing || cancelamento.IsCancellationRequested) return;
            MostrarEstadoServidor(disponivel,
                disponivel ? "API disponível" : "Resposta da API não reconhecida");
            if (disponivel && _backupsApi is not null)
            {
                try
                {
                    await _backupsApi.ListarAsync(cancelamento.Token);
                    if (IsDisposed || Disposing) return;
                    _armazenamentoDisponivel = true;
                    lblEstadoServidor.Text = "API e armazenamento disponíveis";
                    lblOrientacao.Text = "Você pode enviar ou restaurar um backup.\r\nAbrir aplicativo usa sempre o banco local.";
                }
                catch (Exception erro)
                {
                    System.Diagnostics.Debug.WriteLine(erro);
                    if (IsDisposed || Disposing) return;
                    lblEstadoServidor.Text = "API online; backups indisponíveis";
                    lblOrientacao.Text = "Confira o SQL Server e verifique a conexão novamente.\r\nO aplicativo pode continuar usando os dados locais.";
                }
            }
        }
        catch (OperationCanceledException)
        {
            if (!IsDisposed && !Disposing)
                MostrarEstadoServidor(false, "Sem resposta em até 5 segundos");
        }
        catch (Exception erro)
        {
            System.Diagnostics.Debug.WriteLine(erro);
            if (!IsDisposed && !Disposing)
                MostrarEstadoServidor(false, "API indisponível — modo offline");
        }
        finally
        {
            _verificacaoAtual = null;
            _verificando = false;
            if (!IsDisposed && !Disposing)
            {
                btnVerificarConexao.Text = "Verificar conexão";
                btnVerificarConexao.Enabled = true;
                AtualizarAcoesBackup();
            }
        }
    }

    private void MostrarEstadoServidor(bool disponivel, string mensagem)
    {
        _apiDisponivel = disponivel;
        lblEstadoServidor.Text = mensagem;
        lblEstadoServidor.ForeColor = disponivel
            ? Visual.EstiloVisual.Verde : Visual.EstiloVisual.Secundario;
        lblOrientacao.Text = disponivel
            ? "API respondeu. O aplicativo continua usando o SQLite local.\r\nConferindo o armazenamento de backups…"
            : "Você pode abrir e usar os dados locais normalmente.\r\nConfira se a API está em execução e tente verificar novamente.";
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _verificacaoAtual?.Cancel();
        base.OnFormClosed(e);
    }
}
