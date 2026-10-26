using JKCASH.Application.Modelos;
using System.Diagnostics;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcCategorias : UserControl
{
    private readonly IConfirmacaoOperacao _confirmacao = ConfirmacaoOperacao.Padrao;
    private readonly ICategoriaRepositorio? _repositorio;
    private readonly BarraAtualizacao _consulta;
    private bool _ocupado, _consultaValida;

    public UcCategorias()
    {
        InitializeComponent();
        _consulta = new BarraAtualizacao(pnListaCategorias);
        _consulta.Atualizar.Click += async (_, _) => await AtualizarCategoriasAsync();
        AtualizarEstado();
    }

    public UcCategorias(ICategoriaRepositorio repositorio, IConfirmacaoOperacao? confirmacao = null) : this()
    {
        ArgumentNullException.ThrowIfNull(repositorio);
        _repositorio = repositorio;
        _confirmacao = confirmacao ?? ConfirmacaoOperacao.Padrao;
        Load += async (_, _) => await AtualizarCategoriasAsync();
        btnSalvarCategoria.Click += async (_, _) => await SalvarCategoriaAsync();
    }

    private async Task AtualizarCategoriasAsync()
    {
        if (_ocupado || _repositorio is null || IsDisposed || Disposing) return;
        Ocupar(true, "Consultando categorias…");
        try { await CarregarCoreAsync(); }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            InvalidarConsulta("Não foi possível consultar as categorias. Use Atualizar dados para tentar novamente.");
        }
        finally { Ocupar(false); }
    }

    private async Task CarregarCoreAsync()
    {
        var categorias = await Task.Run(() => _repositorio!.ListarTodas());
        if (IsDisposed || Disposing) return;
        dgvCategorias.DataSource = categorias;
        if (dgvCategorias.Columns[nameof(Categoria.Id)] is DataGridViewColumn colunaId)
            colunaId.Visible = false;
        _consultaValida = true;
        _consulta.Mostrar(categorias.Count == 0 ? "Nenhuma categoria cadastrada." : "Categorias atualizadas.");
    }

    private async Task SalvarCategoriaAsync()
    {
        if (_ocupado || !_consultaValida || _repositorio is null || IsDisposed || Disposing) return;
        bool gravando = false, salva = false;
        Ocupar(true, "Salvando categoria…");
        try
        {
            Categoria categoria = new(txtNomeCategoria.Text);
            if (!_confirmacao.Confirmar(this, "Cadastrar categoria", ResumoOperacao.Categoria(categoria)))
            { _consulta.Mostrar("Cadastro cancelado. Preenchimento preservado."); return; }
            gravando = true;
            await Task.Run(() => _repositorio.Adicionar(categoria));
            salva = true;
            if (IsDisposed || Disposing) return;
            txtNomeCategoria.Clear();
            await CarregarCoreAsync();
            if (IsDisposed || Disposing) return;
            _consulta.Mostrar("Categoria salva. Lista atualizada.");
            txtNomeCategoria.Focus();
        }
        catch (Exception erro)
        {
            Debug.WriteLine(erro);
            if (IsDisposed || Disposing) return;
            if (!gravando) _consulta.Mostrar(erro.Message, true);
            else InvalidarConsulta(salva
                ? "A categoria foi salva, mas a lista não pôde ser atualizada. Use Atualizar dados para conferir."
                : "Não foi possível confirmar a gravação. Atualize e confira a lista antes de cadastrar novamente.");
        }
        finally { Ocupar(false); }
    }

    private void InvalidarConsulta(string mensagem)
    {
        _consultaValida = false;
        dgvCategorias.DataSource = null;
        _consulta.Mostrar(mensagem, true);
    }

    private void Ocupar(bool ocupado, string? mensagem = null)
    {
        _ocupado = ocupado;
        if (IsDisposed || Disposing) return;
        if (mensagem is not null) _consulta.Mostrar(mensagem);
        AtualizarEstado();
    }

    private void AtualizarEstado()
    {
        if (IsDisposed || Disposing) return;
        UseWaitCursor = _ocupado;
        txtNomeCategoria.Enabled = !_ocupado;
        btnSalvarCategoria.Enabled = !_ocupado && _consultaValida;
        _consulta.Atualizar.Enabled = !_ocupado;
    }
}
