using System.Diagnostics;
using System.Globalization;
using JKCASH.Application.Modelos;
using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcExtrato : UserControl
{
    private readonly IContaRepositorio? _contas;
    private readonly IExtratoConsulta? _extrato;
    private readonly IRelatorioGastosConsulta? _relatorioGastos;
    private readonly Label _statusGastos;
    private bool _gastosEmOrdemCrescente;

    public UcExtrato()
    {
        InitializeComponent();
        _statusGastos = new Label
        {
            Dock = DockStyle.Top,
            ForeColor = EstiloVisual.Secundario,
            Height = 48,
            Text = "Escolha os filtros e clique em Consultar gastos.",
            TextAlign = ContentAlignment.MiddleLeft
        };
        tabRelatorioGastos.Controls.Add(_statusGastos);
        _statusGastos.SendToBack();
        tabExtrato.SelectedIndexChanged += (_, _) => AtualizarOrigemGastos();
        ConfigurarExtrato();
        ConfigurarTabelaGastos();
        ConfigurarFiltrosGastos();
        ConfigurarGraficoGastos();
        ConfigurarSelecaoCartoesGastos();
    }

    public UcExtrato(IContaRepositorio contas, IExtratoConsulta extrato,
        IRelatorioGastosConsulta relatorioGastos, ICategoriaRepositorio categorias,
        ICartaoCreditoRepositorio cartoes) : this()
    {
        _contas = contas ?? throw new ArgumentNullException(nameof(contas));
        _extrato = extrato ?? throw new ArgumentNullException(nameof(extrato));
        _relatorioGastos = relatorioGastos ?? throw new ArgumentNullException(nameof(relatorioGastos));
        _categoriasGastos = categorias ?? throw new ArgumentNullException(nameof(categorias));
        _cartoesGastos = cartoes ?? throw new ArgumentNullException(nameof(cartoes));
        Load += async (_, _) => await CarregarContasDoExtratoAsync();
    }

    private bool _consultandoExtrato;
    private bool _contasExtratoCarregadas;
    private FiltroExtrato? _filtroAplicado;
    private int _paginaExtrato, _totalPaginasExtrato;
    private Button _paginaAnterior = null!, _proximaPagina = null!;
    private Label _situacaoPagina = null!;
    private static readonly CultureInfo CulturaExtrato = CultureInfo.GetCultureInfo("pt-BR");

    // O Designer guarda os controles. As consultas são executadas fora da thread da interface.

    private void ConfigurarTabelaGastos()
    {
        TabelaVisual.Configurar(
            dgvResumoGastos,
            (nameof(ResumoGasto.Nome), "Gasto", "", 26),
            (nameof(ResumoGasto.Categoria), "Categoria", "", 16),
            (nameof(ResumoGasto.Natureza), "Natureza", "", 13),
            (nameof(ResumoGasto.Importancia), "Importância", "", 12),
            (nameof(ResumoGasto.Necessidade), "Necessidade", "", 12),
            (nameof(ResumoGasto.Planejamento), "Planejamento", "", 15),
            (nameof(ResumoGasto.Frequencia), "Frequência", "", 12),
            (nameof(ResumoGasto.Quantidade), "Lançamentos", "N0", 10),
            (nameof(ResumoGasto.Total), "Total gasto", "C2", 14));

        dgvResumoGastos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        dgvResumoGastos.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex < 0) return;
            string coluna = dgvResumoGastos.Columns[e.ColumnIndex].Name;
            if (coluna is nameof(ResumoGasto.Natureza) or nameof(ResumoGasto.Importancia)
                or nameof(ResumoGasto.Necessidade) or nameof(ResumoGasto.Planejamento) or nameof(ResumoGasto.Frequencia))
                e.Value = NomeOpcao(e.Value);
        };
        DataGridViewColumn colunaTotal = dgvResumoGastos.Columns[nameof(ResumoGasto.Total)]!;
        colunaTotal.SortMode = DataGridViewColumnSortMode.Programmatic;
        colunaTotal.HeaderCell.ToolTipText = "Clique para alternar a ordem dos totais.";
        colunaTotal.HeaderCell.SortGlyphDirection = SortOrder.Descending;
        dgvResumoGastos.ColumnHeaderMouseClick += (_, e) =>
        {
            if (e.Button != MouseButtons.Left || e.ColumnIndex != colunaTotal.Index ||
                dgvResumoGastos.DataSource is not List<ResumoGasto> gastos)
                return;

            _gastosEmOrdemCrescente = !_gastosEmOrdemCrescente;
            ExibirGastosOrdenados(gastos);
        };
    }

    private void ExibirGastosOrdenados(IReadOnlyList<ResumoGasto> gastos)
    {
        var ordenados = _gastosEmOrdemCrescente
            ? gastos.OrderBy(g => g.Total)
            : gastos.OrderByDescending(g => g.Total);

        var resultado = ordenados.ThenBy(g => g.Nome).ThenBy(g => g.Categoria).ThenBy(g => g.CadastroId).ToList();
        dgvResumoGastos.DataSource = resultado;
        ExibirGraficoGastos(resultado);
        dgvResumoGastos.Columns[nameof(ResumoGasto.Total)]!.HeaderCell.SortGlyphDirection =
            _gastosEmOrdemCrescente ? SortOrder.Ascending : SortOrder.Descending;
    }

    private void ConfigurarExtrato()
    {
        dtpDataInicialExtrato.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpDataFinalExtrato.Value = DateTime.Today;
        clbContasExtrato.DisplayMember = nameof(OpcaoContaExtrato.Nome);
        btnConsultarExtrato.Enabled = false;
        ConfigurarPaginacao();

        // As colunas abaixo definem os dados visíveis; o objeto Transacao fica disponível para os detalhes.
        dgvExtrato.AutoGenerateColumns = false;
        dgvExtrato.Columns.Clear();
        dgvExtrato.EnableHeadersVisualStyles = false;
        dgvExtrato.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvExtrato.ColumnHeadersHeight = 36;
        dgvExtrato.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = EstiloVisual.Fundo, ForeColor = EstiloVisual.Texto,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        dgvExtrato.DefaultCellStyle = new DataGridViewCellStyle
        {
            ForeColor = EstiloVisual.Texto, BackColor = Color.White,
            SelectionBackColor = Color.FromArgb(232, 243, 239), SelectionForeColor = EstiloVisual.Texto
        };
        dgvExtrato.GridColor = EstiloVisual.Borda;
        dgvExtrato.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvExtrato.RowTemplate.Height = 34;
        AdicionarColunaExtrato(nameof(ExtratoItem.Data), "Data", 85, 85, "dd/MM/yyyy");
        AdicionarColunaExtrato(nameof(ExtratoItem.Conta), "Conta", 130, 110);
        AdicionarColunaExtrato(nameof(ExtratoItem.NomeGasto), "Gasto", 170, 140);
        AdicionarColunaExtrato(nameof(ExtratoItem.Descricao), "Descrição", 150, 130);
        AdicionarColunaExtrato(nameof(ExtratoItem.NaturezaGasto), "Natureza", 90, 90);
        AdicionarColunaExtrato(nameof(ExtratoItem.NecessidadeGasto), "Necessidade", 90, 90);
        AdicionarColunaExtrato(nameof(ExtratoItem.FrequenciaGasto), "Frequência", 90, 90);
        AdicionarColunaExtrato(nameof(ExtratoItem.Movimento), "Tipo", 140, 115);
        AdicionarColunaExtrato(nameof(ExtratoItem.Valor), "Valor", 105, 100, "C2");
        dgvExtrato.Columns[nameof(ExtratoItem.Valor)]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        chkTodasContasExtrato.CheckedChanged += (_, _) =>
            clbContasExtrato.Enabled = !chkTodasContasExtrato.Checked && !_consultandoExtrato;
        chkTodasContasExtrato.CheckedChanged += (_, _) => InvalidarFiltroExtrato();
        dtpDataInicialExtrato.ValueChanged += (_, _) => InvalidarFiltroExtrato();
        dtpDataFinalExtrato.ValueChanged += (_, _) => InvalidarFiltroExtrato();
        clbContasExtrato.ItemCheck += (_, _) => InvalidarFiltroExtrato();
        btnConsultarExtrato.Click += async (_, _) => await ConsultarExtratoAsync();
        btnDetalhesExtrato.Click += (_, _) => AbrirDetalhesExtrato();
        btnFecharDetalhesExtrato.Click += (_, _) => FecharDetalhesExtrato();
        dgvExtrato.SelectionChanged += (_, _) => AtualizarSelecaoExtrato();
        dgvExtrato.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) AbrirDetalhesExtrato();
        };
        dgvExtrato.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || dgvExtrato.Rows[e.RowIndex].DataBoundItem is not ExtratoItem linha) return;
            if (dgvExtrato.Columns[e.ColumnIndex].Name is not (nameof(ExtratoItem.Valor) or nameof(ExtratoItem.Movimento))) return;
            Color cor = linha.Transferencia ? EstiloVisual.MenuSelecionado
                : linha.Tipo == TipoTransacao.Entrada ? EstiloVisual.Verde : EstiloVisual.Vermelho;
            if (e.CellStyle is not null)
            {
                e.CellStyle.ForeColor = cor;
                e.CellStyle.SelectionForeColor = cor;
            }
        };
    }

    private void ConfigurarPaginacao()
    {
        var barra = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 42, WrapContents = false, AutoScroll = true
        };
        _paginaAnterior = EstiloVisual.Botao("Anterior");
        _proximaPagina = EstiloVisual.Botao("Próxima");
        _paginaAnterior.Name = "btnPaginaAnteriorExtrato";
        _proximaPagina.Name = "btnProximaPaginaExtrato";
        _paginaAnterior.Size = _proximaPagina.Size = new Size(100, 34);
        _situacaoPagina = new Label
        {
            AutoSize = true, Margin = new Padding(12, 10, 12, 0),
            Text = "Nenhuma consulta.", ForeColor = EstiloVisual.Secundario
        };
        barra.Controls.AddRange([_paginaAnterior, _proximaPagina, _situacaoPagina]);
        pnlResultadoExtrato.Controls.Add(barra);
        barra.SendToBack();
        _paginaAnterior.Click += async (_, _) => await MudarPaginaExtratoAsync(-1);
        _proximaPagina.Click += async (_, _) => await MudarPaginaExtratoAsync(1);
        AtualizarPaginacao();
    }

    private void InvalidarFiltroExtrato()
    {
        if (_consultandoExtrato || IsDisposed || Disposing) return;
        LimparResultadoExtrato();
        InvalidarFiltroGastos();
        MostrarStatusExtrato("Filtros alterados. Clique em Consultar extrato para aplicar.");
    }

    private Task MudarPaginaExtratoAsync(int direcao)
    {
        int pagina = _paginaExtrato + direcao;
        return _filtroAplicado is not null && pagina >= 1 && pagina <= _totalPaginasExtrato
            ? ConsultarPaginaExtratoAsync(_filtroAplicado, pagina) : Task.CompletedTask;
    }

    private void AdicionarColunaExtrato(string propriedade, string titulo, float largura, int minimo, string formato = "")
    {
        dgvExtrato.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = propriedade, DataPropertyName = propriedade, HeaderText = titulo,
            FillWeight = largura, MinimumWidth = minimo, SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle { Format = formato, FormatProvider = CulturaExtrato }
        });
    }

    private async Task CarregarContasDoExtratoAsync()
    {
        if (_contas is null || _consultandoExtrato || IsDisposed || Disposing) return;
        DefinirConsultaEmAndamento(true);
        LimparResultadoExtrato();
        try
        {
            // Inclui contas inativas: seu histórico continua disponível para consulta.
            var contas = await Task.Run(() => _contas.ListarTodas());
            if (IsDisposed) return;
            clbContasExtrato.Items.Clear();
            foreach (Conta conta in contas)
                clbContasExtrato.Items.Add(new OpcaoContaExtrato(conta.Id,
                    $"{conta.Nome} ({conta.Banco}){(conta.Ativa ? "" : " — inativa")}"));
            _contasExtratoCarregadas = contas.Count > 0;
            MostrarStatusExtrato(contas.Count == 0 ? "Nenhuma conta cadastrada para consultar."
                : "Escolha o período e clique em Consultar extrato.");
            try
            {
                await CarregarFiltrosGastosAsync();
                await CarregarCartoesGastosAsync();
            }
            catch (Exception erro)
            {
                Debug.WriteLine(erro);
                if (!IsDisposed && !Disposing)
                    MostrarStatusGastos("Não foi possível carregar todos os filtros. Clique em Consultar gastos para tentar novamente.", true);
            }
        }
        catch (Exception erro)
        {
            if (!IsDisposed)
            {
                _contasExtratoCarregadas = false;
                clbContasExtrato.Items.Clear();
                LimparResultadoExtrato();
                MostrarErroExtrato(erro);
            }
        }
        finally
        {
            if (!IsDisposed) DefinirConsultaEmAndamento(false);
        }
        if (!IsDisposed && _contasExtratoCarregadas)
            await ConsultarExtratoAsync();
    }

    private async Task ConsultarExtratoAsync()
    {
        if (_extrato is null ||
            _consultandoExtrato || IsDisposed || Disposing) return;
        bool exibirGastos = tabExtrato.SelectedTab == tabRelatorioGastos;
        try
        {
            DateTime inicio = dtpDataInicialExtrato.Value.Date;
            DateTime fim = dtpDataFinalExtrato.Value.Date;
            if (inicio > fim)
                throw new ArgumentException("A data inicial não pode ser posterior à data final.");

            if (ConsultarComprasNoCartao)
            {
                if (exibirGastos)
                    await ConsultarGastosAsync(inicio, fim, comprasNoCartao: true);
                else
                    await ConsultarPaginaExtratoAsync(new FiltroExtrato(LerCartoesGastos(), inicio, fim, true), 1);
                return;
            }

            var selecionadas = chkTodasContasExtrato.Checked
                ? clbContasExtrato.Items.Cast<OpcaoContaExtrato>().ToList()
                : clbContasExtrato.CheckedItems.Cast<OpcaoContaExtrato>().ToList();
            if (selecionadas.Count == 0)
                throw new ArgumentException("Marque pelo menos uma conta ou selecione Todas as contas.");
            Guid[] ids = selecionadas.Select(conta => conta.Id).Distinct().ToArray();
            var filtro = new FiltroExtrato(ids, inicio, fim);
            if (exibirGastos)
                await ConsultarGastosAsync(filtro.Inicio, filtro.Fim, comprasNoCartao: false, filtro.Origens);
            else
                await ConsultarPaginaExtratoAsync(filtro, 1);
        }
        catch (Exception erro)
        {
            if (IsDisposed || Disposing) return;
            if (exibirGastos)
            {
                Debug.WriteLine(erro);
                MostrarStatusGastos(erro is ArgumentException ? erro.Message
                    : "Não foi possível consultar os gastos. Tente novamente.", true);
            }
            else
            {
                MostrarErroExtrato(erro);
            }
        }
    }

    private async Task ConsultarGastosAsync(DateTime inicio, DateTime fim,
        bool comprasNoCartao, Guid[]? contas = null)
    {
        if (_relatorioGastos is null || _consultandoExtrato || IsDisposed || Disposing)
            return;

        DefinirConsultaEmAndamento(true);
        LimparGastos();
        MostrarStatusGastos("Consultando os gastos do período…");

        try
        {
            await CarregarFiltrosGastosAsync();
            if (comprasNoCartao && !IsDisposed && !Disposing) await CarregarCartoesGastosAsync();
            if (IsDisposed || Disposing) return;

            FiltrosRelatorioGastos filtrosGastos = LerFiltrosGastos();
            Guid[] ids = comprasNoCartao ? LerCartoesGastos() : contas!;
            var gastos = await Task.Run(() => comprasNoCartao
                ? _relatorioGastos.ConsultarComprasPorGasto(ids, inicio, fim, filtrosGastos)
                : _relatorioGastos.ConsultarPorGasto(ids, inicio, fim, filtrosGastos));
            if (IsDisposed || Disposing) return;

            ExibirGastosOrdenados(gastos);
            MostrarStatusGastos(gastos.Count == 0 ? "Nenhuma despesa corresponde aos filtros."
                : $"{gastos.Sum(g => g.Quantidade)} lançamento(s)  |  Total gasto: {Moeda(gastos.Sum(g => g.Total))}{Environment.NewLine}{(comprasNoCartao ? "Crédito: valor integral das compras" : "Débito: sem transferências e pagamentos de fatura")}");
        }
        finally
        {
            if (!IsDisposed && !Disposing) DefinirConsultaEmAndamento(false);
        }
    }

    private void MostrarStatusGastos(string mensagem, bool erro = false)
    {
        _statusGastos.Text = mensagem;
        _statusGastos.ForeColor = erro ? EstiloVisual.Vermelho : EstiloVisual.Secundario;
    }

    private async Task ConsultarPaginaExtratoAsync(FiltroExtrato filtro, int pagina)
    {
        if (_extrato is null || _consultandoExtrato || IsDisposed || Disposing) return;
        DefinirConsultaEmAndamento(true);
        LimparResultadoExtrato();
        MostrarStatusExtrato("Consultando movimentações e totais do período…");
        try
        {
            ExtratoPagina resultado = await Task.Run(() =>
                filtro.ComprasNoCartao
                    ? _extrato.ConsultarCompras(filtro.Origens, filtro.Inicio, filtro.Fim, pagina)
                    : _extrato.Consultar(filtro.Origens, filtro.Inicio, filtro.Fim, pagina));
            if (IsDisposed || Disposing) return;
            _filtroAplicado = filtro;
            _paginaExtrato = resultado.Pagina;
            _totalPaginasExtrato = resultado.TotalPaginas;
            dgvExtrato.DataSource = resultado.Itens.ToList();
            ExtratoTotais totais = resultado.Totais;
            lblTotaisExtrato.Text = filtro.ComprasNoCartao
                ? $"Total das compras: {Moeda(totais.Saidas)}"+ Environment.NewLine
                    + "O valor integral de cada compra entra no período da compra."
                : $"Entradas: {Moeda(totais.Entradas)}   |   Saídas: {Moeda(totais.Saidas)}   |   Resultado: {Moeda(totais.Entradas - totais.Saidas)}"
                + Environment.NewLine + $"Transferências recebidas: {Moeda(totais.TransferenciasRecebidas)}   |   Enviadas: {Moeda(totais.TransferenciasEnviadas)}"
                + Environment.NewLine + "Totais de todo o período, incluindo todas as páginas; não representam o saldo da conta.";
            _situacaoPagina.Text = resultado.QuantidadeTotal == 0 ? "Nenhuma movimentação."
                : $"Página {resultado.Pagina} de {resultado.TotalPaginas} • {resultado.QuantidadeTotal} movimentações";
            MostrarStatusExtrato($"{resultado.Itens.Count} exibida(s) | {filtro.Inicio:dd/MM/yyyy} a {filtro.Fim:dd/MM/yyyy} | {filtro.Origens.Length} {(filtro.ComprasNoCartao ? "cartão(ões)" : "conta(s)")}");
            AtualizarSelecaoExtrato();
        }
        catch (Exception erro)
        {
            if (IsDisposed || Disposing) return;
            LimparResultadoExtrato();
            MostrarErroExtrato(erro);
        }
        finally
        {
            if (!IsDisposed && !Disposing) DefinirConsultaEmAndamento(false);
        }
    }

    private void AbrirDetalhesExtrato()
    {
        if (_consultandoExtrato || dgvExtrato.CurrentRow?.DataBoundItem is not ExtratoItem) return;
        splExtrato.Panel2Collapsed = false;
        splExtrato.SplitterDistance = Math.Max(splExtrato.Panel1MinSize, splExtrato.Width - 320);
        btnFecharDetalhesExtrato.Visible = true;
        AtualizarSelecaoExtrato();
    }

    private void FecharDetalhesExtrato()
    {
        splExtrato.Panel2Collapsed = true;
        btnFecharDetalhesExtrato.Visible = false;
        txtDetalhesExtrato.Clear();
    }

    private void AtualizarSelecaoExtrato()
    {
        btnDetalhesExtrato.Enabled = !_consultandoExtrato && dgvExtrato.CurrentRow?.DataBoundItem is ExtratoItem;
        if (splExtrato.Panel2Collapsed) return;
        if (dgvExtrato.CurrentRow?.DataBoundItem is not ExtratoItem linha)
        {
            txtDetalhesExtrato.Clear();
            return;
        }
        bool avaliar = linha.Tipo == TipoTransacao.Saida || linha.Transferencia;
        txtDetalhesExtrato.Text = string.Join(Environment.NewLine,
            linha.QuantidadeParcelas is int parcelas
                ? $"Cartão: {linha.Conta}{Environment.NewLine}Parcelas: {parcelas}"
                : $"Conta: {linha.Conta}{Environment.NewLine}Banco: {linha.Banco}",
            $"Data: {linha.Data:dd/MM/yyyy HH:mm:ss}", $"Tipo: {linha.Movimento}",
            $"Valor: {Moeda(linha.Valor)}", $"Gasto: {linha.NomeGasto}", $"Categoria: {linha.Categoria}")
            + Environment.NewLine + (avaliar ? string.Join(Environment.NewLine,
            $"Importância: {(linha.Importancia == ImportanciaTransacao.Media ? "Média" : linha.Importancia?.ToString() ?? "Não informada")}",
            $"Planejamento: {(linha.Planejamento == PlanejamentoTransacao.NaoPlanejada ? "Não planejada" : linha.Planejamento?.ToString() ?? "Não informado")}",
            linha.Gasto is null ? $"Urgência (registro anterior): {(linha.Urgencia == UrgenciaTransacao.SemUrgencia ? "Sem urgência" : linha.Urgencia?.ToString() ?? "Não informada")}" : $"Necessidade: {NomeOpcao(linha.Gasto.Necessidade)}",
            "Motivo do gasto:", string.IsNullOrWhiteSpace(linha.Justificativa) ? "Não informado (opcional)." : linha.Justificativa) : "Avaliação: Não se aplica.")
            + Environment.NewLine + "Descrição:" + Environment.NewLine + linha.Descricao;
        if (linha.Gasto is { } gasto)
        {
            txtDetalhesExtrato.AppendText(Environment.NewLine + string.Join(Environment.NewLine,
                $"Natureza: {NomeOpcao(gasto.Natureza)}", $"Frequência: {gasto.Frequencia}",
                $"Cadastro reutilizável: {(gasto.CadastroId.HasValue ? "Sim" : "Não, somente neste lançamento")}"));
        }
        if (linha.Parcelas.Count > 0)
        {
            txtDetalhesExtrato.AppendText(Environment.NewLine + Environment.NewLine + "Parcelas:" + Environment.NewLine);
            txtDetalhesExtrato.AppendText(string.Join(Environment.NewLine, linha.Parcelas.Select(p =>
                $"{p.Numero}/{p.Quantidade} • {Moeda(p.Valor)} • Fechamento: {p.Fechamento:dd/MM/yyyy} • Vencimento: {p.Vencimento:dd/MM/yyyy}")));
        }

        txtDetalhesExtrato.SelectionStart = 0;
        txtDetalhesExtrato.SelectionLength = 0;
    }

    private void LimparResultadoExtrato()
    {
        _filtroAplicado = null;
        _paginaExtrato = _totalPaginasExtrato = 0;
        _situacaoPagina.Text = "Nenhuma consulta.";
        dgvExtrato.DataSource = null;
        lblTotaisExtrato.Text = "";
        FecharDetalhesExtrato();
        btnDetalhesExtrato.Enabled = false;
        AtualizarPaginacao();
    }

    private void DefinirConsultaEmAndamento(bool consultando)
    {
        _consultandoExtrato = consultando;
        _painelFiltrosGastos.Enabled = !consultando;
        AtualizarSelecaoCartoesGastos();
        btnConsultarExtrato.Enabled = !consultando && _contasExtratoCarregadas;
        dtpDataInicialExtrato.Enabled = !consultando;
        dtpDataFinalExtrato.Enabled = !consultando;
        chkTodasContasExtrato.Enabled = !consultando;
        clbContasExtrato.Enabled = !consultando && !chkTodasContasExtrato.Checked;
        AtualizarSelecaoExtrato();
        AtualizarPaginacao();
    }

    private void AtualizarPaginacao()
    {
        _paginaAnterior.Enabled = !_consultandoExtrato && _filtroAplicado is not null && _paginaExtrato > 1;
        _proximaPagina.Enabled = !_consultandoExtrato && _filtroAplicado is not null && _paginaExtrato < _totalPaginasExtrato;
    }

    private void MostrarStatusExtrato(string mensagem, bool erro = false)
    {
        lblStatusExtrato.Text = mensagem;
        lblStatusExtrato.ForeColor = erro ? EstiloVisual.Vermelho : EstiloVisual.Secundario;
    }

    private void MostrarErroExtrato(Exception erro)
    {
        Debug.WriteLine(erro);
        MostrarStatusExtrato(erro is ArgumentException ? erro.Message
            : "Não foi possível consultar o banco. Verifique a conexão e tente novamente.", true);
    }

    private static string Moeda(decimal valor) => valor.ToString("C2", CulturaExtrato);
    private sealed record OpcaoContaExtrato(Guid Id, string Nome);
    private sealed record FiltroExtrato(Guid[] Origens, DateTime Inicio, DateTime Fim, bool ComprasNoCartao = false);
}
