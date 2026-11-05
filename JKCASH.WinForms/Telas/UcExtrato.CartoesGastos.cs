using JKCASH.Application.Interfaces;

namespace JKCASH.WinForms.Telas;

public partial class UcExtrato
{
    private readonly ICartaoCreditoRepositorio? _cartoesGastos;
    private bool _cartoesGastosCarregados;
    private readonly ComboBox _origemGastos = new()
    {
        Name = "cmbOrigemGastos",
        AccessibleName = "Origem dos gastos",
        DropDownStyle = ComboBoxStyle.DropDownList,
        Location = new Point(20, 117),
        Size = new Size(240, 25)
    };
    private readonly CheckBox _todosCartoesGastos = new()
    {
        Name = "chkTodosCartoesGastos",
        AutoSize = true,
        Checked = true,
        Text = "Todos os cartões"
    };
    private readonly Label _tituloCartoesGastos = new()
    {
        AutoSize = true,
        Text = "Cartões (desmarque Todos para escolher)"
    };
    private readonly CheckedListBox _listaCartoesGastos = new()
    {
        Name = "clbCartoesGastos",
        CheckOnClick = true,
        IntegralHeight = false,
        DisplayMember = nameof(OpcaoContaExtrato.Nome)
    };

    private bool ConsultarComprasNoCartao =>
        _origemGastos.SelectedIndex == 1;

    private void ConfigurarSelecaoCartoesGastos()
    {
        _origemGastos.Items.AddRange(["Contas bancárias", "Compras no cartão"]);
        _origemGastos.SelectedIndex = 0;
        _todosCartoesGastos.Location = chkTodasContasExtrato.Location;
        _tituloCartoesGastos.Location = lblContasExtrato.Location;
        _listaCartoesGastos.Bounds = clbContasExtrato.Bounds;
        _listaCartoesGastos.Anchor = clbContasExtrato.Anchor;

        pnlFiltrosExtrato.Controls.AddRange([
            _origemGastos, _todosCartoesGastos, _tituloCartoesGastos, _listaCartoesGastos]);

        _origemGastos.SelectedIndexChanged += (_, _) =>
        {
            AtualizarOrigemGastos();
            InvalidarFiltroExtrato();
        };
        _todosCartoesGastos.CheckedChanged += (_, _) =>
        {
            AtualizarSelecaoCartoesGastos();
            InvalidarFiltroExtrato();
        };
        _listaCartoesGastos.ItemCheck += (_, _) => InvalidarFiltroExtrato();
        AtualizarOrigemGastos();
    }

    private void AtualizarOrigemGastos()
    {
        bool relatorio = tabExtrato.SelectedTab == tabRelatorioGastos;
        bool cartoes = ConsultarComprasNoCartao;
        dgvExtrato.Columns[nameof(JKCASH.Application.Modelos.ExtratoItem.Conta)]!.HeaderText =
            cartoes ? "Cartão" : "Conta";
        _todosCartoesGastos.Visible = cartoes;
        _tituloCartoesGastos.Visible = cartoes;
        _listaCartoesGastos.Visible = cartoes;
        chkTodasContasExtrato.Visible = !cartoes;
        lblContasExtrato.Visible = !cartoes;
        clbContasExtrato.Visible = !cartoes;
        btnConsultarExtrato.Text = relatorio ? "Consultar gastos" : "Consultar extrato";
        AtualizarSelecaoCartoesGastos();
    }

    private void AtualizarSelecaoCartoesGastos()
    {
        _origemGastos.Enabled = !_consultandoExtrato;
        _todosCartoesGastos.Enabled = !_consultandoExtrato;
        _listaCartoesGastos.Enabled = !_consultandoExtrato && !_todosCartoesGastos.Checked;
    }

    private async Task CarregarCartoesGastosAsync()
    {
        if (_cartoesGastosCarregados || _cartoesGastos is null) return;
        var cartoes = await Task.Run(() => _cartoesGastos.ListarTodos()
            .OrderBy(c => c.Nome).ThenBy(c => c.Id).ToList());
        if (IsDisposed || Disposing) return;

        _listaCartoesGastos.Items.Clear();
        foreach (var cartao in cartoes)
            _listaCartoesGastos.Items.Add(new OpcaoContaExtrato(cartao.Id, cartao.Nome));

        _cartoesGastosCarregados = true;
    }

    private Guid[] LerCartoesGastos()
    {
        var cartoes = _todosCartoesGastos.Checked
            ? _listaCartoesGastos.Items.Cast<OpcaoContaExtrato>()
            : _listaCartoesGastos.CheckedItems.Cast<OpcaoContaExtrato>();

        var ids = cartoes.Select(c => c.Id).ToArray();
        if (ids.Length == 0)
            throw new ArgumentException(_listaCartoesGastos.Items.Count == 0
                ? "Nenhum cartão cadastrado para consultar."
                : "Marque pelo menos um cartão ou selecione Todos os cartões.");

        return ids;
    }
}
