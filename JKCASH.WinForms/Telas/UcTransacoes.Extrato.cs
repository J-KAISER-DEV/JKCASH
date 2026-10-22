using System.Diagnostics;
using System.Globalization;
using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.WinForms.Visual;

namespace JKCASH.WinForms.Telas;

public partial class UcTransacoes
{
    private bool _consultandoExtrato;
    private bool _contasExtratoCarregadas;
    private static readonly CultureInfo CulturaExtrato = CultureInfo.GetCultureInfo("pt-BR");

    // O Designer guarda os controles; este arquivo concentra o comportamento do extrato.
    private void ConfigurarExtrato()
    {
        dtpDataInicialExtrato.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpDataFinalExtrato.Value = DateTime.Today;
        clbContasExtrato.DisplayMember = nameof(OpcaoContaExtrato.Nome);
        btnConsultarExtrato.Enabled = false;

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
        AdicionarColunaExtrato(nameof(LinhaExtrato.Data), "Data", 85, 85, "dd/MM/yyyy");
        AdicionarColunaExtrato(nameof(LinhaExtrato.Conta), "Conta", 130, 110);
        AdicionarColunaExtrato(nameof(LinhaExtrato.Descricao), "Descrição", 170, 140);
        AdicionarColunaExtrato(nameof(LinhaExtrato.Categoria), "Categoria", 110, 100);
        AdicionarColunaExtrato(nameof(LinhaExtrato.Movimento), "Tipo", 140, 115);
        AdicionarColunaExtrato(nameof(LinhaExtrato.Valor), "Valor", 105, 100, "C2");
        dgvExtrato.Columns[nameof(LinhaExtrato.Valor)]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        chkTodasContasExtrato.CheckedChanged += (_, _) =>
            clbContasExtrato.Enabled = !chkTodasContasExtrato.Checked && !_consultandoExtrato;
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
            if (e.RowIndex < 0 || dgvExtrato.Rows[e.RowIndex].DataBoundItem is not LinhaExtrato linha) return;
            if (dgvExtrato.Columns[e.ColumnIndex].Name is not (nameof(LinhaExtrato.Valor) or nameof(LinhaExtrato.Movimento))) return;
            Color cor = linha.Transacao.TransferenciaId != null ? EstiloVisual.MenuSelecionado
                : linha.Transacao.Tipo == TipoTransacao.Entrada ? EstiloVisual.Verde : EstiloVisual.Vermelho;
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
        if (_contas is null || IsDisposed) return;
        DefinirConsultaEmAndamento(true);
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
            if (!IsDisposed) MostrarErroExtrato(erro);
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
        if (_contas is null || _categorias is null || _transacoes is null || _consultandoExtrato || IsDisposed) return;
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
            List<Guid> ids = selecionadas.Select(conta => conta.Id).Distinct().ToList();
            DefinirConsultaEmAndamento(true);
            MostrarStatusExtrato("Consultando movimentações...");

            // A consulta acontece fora da thread da interface, sem bloquear a navegação.
            var linhas = await Task.Run(() =>
            {
                var contas = _contas.ListarTodas().ToDictionary(conta => conta.Id);
                var categorias = _categorias.ListarTodas().ToDictionary(categoria => categoria.Id);
                return _transacoes.ListarPorPeriodo(ids, inicio, fim)
                    .OrderBy(transacao => transacao.Data).ThenBy(transacao => transacao.Id)
                    .Select(transacao => new LinhaExtrato(transacao,
                        contas.GetValueOrDefault(transacao.ContaId)?.Nome ?? "Conta não encontrada",
                        contas.GetValueOrDefault(transacao.ContaId)?.Banco ?? "Banco não encontrado",
                        categorias.GetValueOrDefault(transacao.CategoriaId)?.Nome ?? "Categoria não encontrada"))
                    .ToList();
            });
            if (IsDisposed) return;

            dgvExtrato.DataSource = linhas;
            AtualizarTotaisExtrato(linhas);
            MostrarStatusExtrato(linhas.Count == 0
                ? $"Nenhuma movimentação entre {inicio:dd/MM/yyyy} e {fim:dd/MM/yyyy} nas contas selecionadas."
                : $"{linhas.Count} movimentação(ões) | {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy} | {ids.Count} conta(s)");
            AtualizarSelecaoExtrato();
        }
        catch (Exception erro)
        {
            if (!IsDisposed) MostrarErroExtrato(erro);
        }
        finally
        {
            if (!IsDisposed) DefinirConsultaEmAndamento(false);
        }
    }

    private void AtualizarTotaisExtrato(List<LinhaExtrato> linhas)
    {
        var comuns = linhas.Where(linha => linha.Transacao.TransferenciaId == null).ToList();
        decimal entradas = comuns.Where(linha => linha.Transacao.Tipo == TipoTransacao.Entrada).Sum(linha => linha.Valor);
        decimal saidas = comuns.Where(linha => linha.Transacao.Tipo == TipoTransacao.Saida).Sum(linha => linha.Valor);
        decimal recebidas = linhas.Where(linha => linha.Transacao.TransferenciaId != null && linha.Transacao.Tipo == TipoTransacao.Entrada).Sum(linha => linha.Valor);
        decimal enviadas = linhas.Where(linha => linha.Transacao.TransferenciaId != null && linha.Transacao.Tipo == TipoTransacao.Saida).Sum(linha => linha.Valor);
        lblTotaisExtrato.Text = $"Entradas: {Moeda(entradas)}   |   Saídas: {Moeda(saidas)}   |   Resultado: {Moeda(entradas - saidas)}"
            + Environment.NewLine + $"Transferências recebidas: {Moeda(recebidas)}   |   Enviadas: {Moeda(enviadas)}"
            + Environment.NewLine + "Resultado = receitas − despesas do período; não representa o saldo da conta.";
    }

    private void AbrirDetalhesExtrato()
    {
        if (_consultandoExtrato || dgvExtrato.CurrentRow?.DataBoundItem is not LinhaExtrato) return;
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
        btnDetalhesExtrato.Enabled = !_consultandoExtrato && dgvExtrato.CurrentRow?.DataBoundItem is LinhaExtrato;
        if (splExtrato.Panel2Collapsed) return;
        if (dgvExtrato.CurrentRow?.DataBoundItem is not LinhaExtrato linha)
        {
            txtDetalhesExtrato.Clear();
            return;
        }
        Transacao transacao = linha.Transacao;
        txtDetalhesExtrato.Text = string.Join(Environment.NewLine,
            $"Conta: {linha.Conta}", $"Banco: {linha.Banco}",
            $"Data: {transacao.Data:dd/MM/yyyy HH:mm:ss}", $"Tipo: {linha.Movimento}",
            $"Valor: {Moeda(transacao.Valor)}", $"Categoria: {linha.Categoria}",
            $"Importância: {(transacao.Importancia == ImportanciaTransacao.Media ? "Média" : transacao.Importancia.ToString())}",
            $"Planejamento: {(transacao.Planejamento == PlanejamentoTransacao.NaoPlanejada ? "Não planejada" : transacao.Planejamento?.ToString() ?? "Não informado")}",
            $"Urgência: {(transacao.Urgencia == UrgenciaTransacao.SemUrgencia ? "Sem urgência" : transacao.Urgencia?.ToString() ?? "Não informada")}",
            "", "Descrição:", transacao.Descricao, "", "Justificativa:", transacao.Justificativa);
        txtDetalhesExtrato.SelectionStart = 0;
        txtDetalhesExtrato.SelectionLength = 0;
    }

    private void LimparResultadoExtrato()
    {
        dgvExtrato.DataSource = null;
        lblTotaisExtrato.Text = "";
        FecharDetalhesExtrato();
        btnDetalhesExtrato.Enabled = false;
    }

    private void DefinirConsultaEmAndamento(bool consultando)
    {
        _consultandoExtrato = consultando;
        btnConsultarExtrato.Enabled = !consultando && _contasExtratoCarregadas;
        dtpDataInicialExtrato.Enabled = !consultando;
        dtpDataFinalExtrato.Enabled = !consultando;
        chkTodasContasExtrato.Enabled = !consultando;
        clbContasExtrato.Enabled = !consultando && !chkTodasContasExtrato.Checked;
        AtualizarSelecaoExtrato();
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
    private sealed record LinhaExtrato(Transacao Transacao, string Conta, string Banco, string Categoria)
    {
        public DateTime Data => Transacao.Data;
        public string Descricao => Transacao.Descricao;
        public string Movimento => (Transacao.TransferenciaId != null ? "Transferência — " : "") + Transacao.Tipo;
        public decimal Valor => Transacao.Valor;
    }
}
