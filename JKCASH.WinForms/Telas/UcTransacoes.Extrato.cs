using System.Diagnostics;
using System.Globalization;
using JKCASH.Application.Modelos;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcTransacoes
{
    private bool _consultandoExtrato;
    private bool _contasExtratoCarregadas;
    private FiltroExtrato? _filtroAplicado;
    private int _paginaExtrato, _totalPaginasExtrato;
    private Button _paginaAnterior = null!, _proximaPagina = null!;
    private Label _situacaoPagina = null!;
    private static readonly CultureInfo CulturaExtrato = CultureInfo.GetCultureInfo("pt-BR");

    // O Designer guarda os controles; este arquivo concentra o comportamento do extrato.
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
        AdicionarColunaExtrato(nameof(ExtratoItem.Descricao), "Descrição", 170, 140);
        AdicionarColunaExtrato(nameof(ExtratoItem.Categoria), "Categoria", 110, 100);
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
        tabTransacoes.SelectedIndexChanged += async (_, _) =>
        {
            // Voltar à aba também atualiza a consulta após registrar uma movimentação.
            if (tabTransacoes.SelectedTab == tabExtratoTransacoes && _contasExtratoCarregadas)
                await ConsultarExtratoAsync();
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
        if (!IsDisposed && _contasExtratoCarregadas && tabTransacoes.SelectedTab == tabExtratoTransacoes)
            await ConsultarExtratoAsync();
    }

    private async Task ConsultarExtratoAsync()
    {
        if (_extrato is null ||
            _consultandoExtrato || _registrando || IsDisposed || Disposing) return;
        LimparResultadoExtrato();
        try
        {
            DateTime inicio = dtpDataInicialExtrato.Value.Date;
            DateTime fim = dtpDataFinalExtrato.Value.Date;
            if (inicio > fim)
                throw new ArgumentException("A data inicial não pode ser posterior à data final.");

            var selecionadas = chkTodasContasExtrato.Checked
                ? clbContasExtrato.Items.Cast<OpcaoContaExtrato>().ToList()
                : clbContasExtrato.CheckedItems.Cast<OpcaoContaExtrato>().ToList();
            if (selecionadas.Count == 0)
                throw new ArgumentException("Marque pelo menos uma conta ou selecione Todas as contas.");
            Guid[] ids = selecionadas.Select(conta => conta.Id).Distinct().ToArray();
            await ConsultarPaginaExtratoAsync(new FiltroExtrato(ids, inicio, fim), 1);
        }
        catch (Exception erro)
        {
            if (!IsDisposed) MostrarErroExtrato(erro);
        }
    }

    private async Task ConsultarPaginaExtratoAsync(FiltroExtrato filtro, int pagina)
    {
        if (_extrato is null || _consultandoExtrato || _registrando || IsDisposed || Disposing) return;
        DefinirConsultaEmAndamento(true);
        LimparResultadoExtrato();
        MostrarStatusExtrato("Consultando movimentações e totais do período…");
        try
        {
            ExtratoPagina resultado = await Task.Run(() =>
                _extrato.Consultar(filtro.Contas, filtro.Inicio, filtro.Fim, pagina));
            if (IsDisposed || Disposing) return;
            _filtroAplicado = filtro;
            _paginaExtrato = resultado.Pagina;
            _totalPaginasExtrato = resultado.TotalPaginas;
            dgvExtrato.DataSource = resultado.Itens.ToList();
            ExtratoTotais totais = resultado.Totais;
            lblTotaisExtrato.Text = $"Entradas: {Moeda(totais.Entradas)}   |   Saídas: {Moeda(totais.Saidas)}   |   Resultado: {Moeda(totais.Entradas - totais.Saidas)}"
                + Environment.NewLine + $"Transferências recebidas: {Moeda(totais.TransferenciasRecebidas)}   |   Enviadas: {Moeda(totais.TransferenciasEnviadas)}"
                + Environment.NewLine + "Totais de todo o período, incluindo todas as páginas; não representam o saldo da conta.";
            _situacaoPagina.Text = resultado.QuantidadeTotal == 0 ? "Nenhuma movimentação."
                : $"Página {resultado.Pagina} de {resultado.TotalPaginas} • {resultado.QuantidadeTotal} movimentações";
            MostrarStatusExtrato($"{resultado.Itens.Count} exibida(s) | {filtro.Inicio:dd/MM/yyyy} a {filtro.Fim:dd/MM/yyyy} | {filtro.Contas.Length} conta(s)");
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
            $"Conta: {linha.Conta}", $"Banco: {linha.Banco}",
            $"Data: {linha.Data:dd/MM/yyyy HH:mm:ss}", $"Tipo: {linha.Movimento}",
            $"Valor: {Moeda(linha.Valor)}", $"Categoria: {linha.Categoria}")
            + Environment.NewLine + (avaliar ? string.Join(Environment.NewLine,
            $"Importância: {(linha.Importancia == ImportanciaTransacao.Media ? "Média" : linha.Importancia?.ToString() ?? "Não informada")}",
            $"Planejamento: {(linha.Planejamento == PlanejamentoTransacao.NaoPlanejada ? "Não planejada" : linha.Planejamento?.ToString() ?? "Não informado")}",
            $"Urgência: {(linha.Urgencia == UrgenciaTransacao.SemUrgencia ? "Sem urgência" : linha.Urgencia?.ToString() ?? "Não informada")}",
            "Justificativa:", linha.Justificativa) : "Avaliação: Não se aplica.")
            + Environment.NewLine + "Descrição:" + Environment.NewLine + linha.Descricao;
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
        btnConsultarExtrato.Enabled = !consultando && !_registrando && _contasExtratoCarregadas;
        dtpDataInicialExtrato.Enabled = !consultando;
        dtpDataFinalExtrato.Enabled = !consultando;
        chkTodasContasExtrato.Enabled = !consultando;
        clbContasExtrato.Enabled = !consultando && !chkTodasContasExtrato.Checked;
        AtualizarSelecaoExtrato();
        AtualizarEstadoRegistro();
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
    private sealed record FiltroExtrato(Guid[] Contas, DateTime Inicio, DateTime Fim);
}
