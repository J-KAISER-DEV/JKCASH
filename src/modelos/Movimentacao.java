package modelos;

import bancodedados.DBContas;
import bancodedados.DBMovimentacoes;
import utis.Leitura;

import java.time.LocalDate;

public class Movimentacao {
    private String tipo;
    private String bancoUsado;
    private LocalDate data;
    private String entidade;
    private Categoria categoria;
    private Necessidade necessidade;
    private String descricao;
    private double valor;

    public Movimentacao(String tipo, String bancoUsado, LocalDate data, Categoria categoria, String entidade, Necessidade necessidade, String descricao, double valor) {
        this.tipo = tipo;
        this.bancoUsado = bancoUsado;
        this.data = data;
        this.categoria = categoria;
        this.entidade = entidade;
        this.necessidade = necessidade;
        this.descricao = descricao;
        this.valor = valor;
    }

    public String getTipo() {
        return tipo;
    }

    public void setTipo(String tipo) {
        this.tipo = tipo;
    }

    public String getBancoUsado() {
        return bancoUsado;
    }

    public void setBancoUsado(String bancoUsado) {
        this.bancoUsado = bancoUsado;
    }

    public LocalDate getData() {
        return data;
    }

    public void setData(LocalDate data) {
        this.data = data;
    }

    public String getEntidade() {
        return entidade;
    }

    public void setEntidade(String entidade) {
        this.entidade = entidade;
    }

    public Categoria getCategoria() {
        return categoria;
    }

    public void setCategoria(Categoria categoria) {
        this.categoria = categoria;
    }

    public Necessidade getNecessidade() {
        return necessidade;
    }

    public void setNecessidade(Necessidade necessidade) {
        this.necessidade = necessidade;
    }

    public String getDescricao() {
        return descricao;
    }

    public void setDescricao(String descricao) {
        this.descricao = descricao;
    }

    public double getValor() {
        return valor;
    }

    public void setValor(double valor) {
        this.valor = valor;
    }

    public void exibirRecibo(){
        System.out.println("\n=======================");
        System.out.println("RECIBO DE " + this.tipo);
        System.out.println("Banco: " + this.bancoUsado.toUpperCase());

        if (this.tipo.equals("ENTRADA")){
            System.out.println("Origem: " + this.entidade);
        }else {
            System.out.println("Destino: " + this.entidade);
        }
        System.out.println("categoria: " + this.categoria);
        System.out.println("Descrição: " + this.descricao);
        System.out.println("Valor: R$ " + this.valor);
        System.out.println("==============================\n");
    }
public static void registrarEntrada(){
        Leitura leitura = new Leitura();
    System.out.println("\n💸---------------- REGISTRAR SAÍDA ----------------💸");
    System.out.print("TIPO DA CONTA: ");
    String tipo = leitura.entradaTexto();
    System.out.println("Banco: ");
    String banco = leitura.entradaTexto();

    Conta contaUsada = DBContas.verificaDBContas(tipo, banco);

    if (contaUsada == null){
        System.out.println("\"❌ Conta não encontrada! Operação cancelada.\"");
        return;
    }
    System.out.println("Origem da Receita: ");
    String entidade = leitura.entradaTexto();
    System.out.println("Descrição: ");
    String descricao = leitura.entradaTexto();
    System.out.println("Valor R$: ");
    double valor = leitura.entradaValor();
    Categoria categoria = leitura.lerCategoria();

    Necessidade necessidade = null;
    contaUsada.getSaldo().depositar(valor);

    Movimentacao novaEntrada = new Movimentacao("ENTRADA", banco, java.time.LocalDate.now(), categoria, entidade, necessidade, descricao, valor );
    DBMovimentacoes.salvar(novaEntrada);
    System.out.println("\n✅ Entrada registrada com sucesso!");
    novaEntrada.exibirRecibo();
}
public static void registrarSaida(){
        Leitura leitura = new Leitura();
    System.out.println("\n💸---------------- REGISTRAR SAÍDA ----------------💸");
    System.out.print("Tipo da Conta Usada: ");
    String tipo = leitura.entradaTexto();
    System.out.print("Banco Usado: ");
    String banco = leitura.entradaTexto();

    Conta contaUsada = DBContas.verificaDBContas(tipo, banco);

    if (contaUsada == null) {
        System.out.println("❌ Conta não encontrada! Operação cancelada.");
        return;
    }

    System.out.print("Local da Compra / Destino: ");
    String entidade = leitura.entradaTexto();
    System.out.print("Descrição da Compra: ");
    String descricao = leitura.entradaTexto();
    System.out.print("Valor R$: ");
    double valor = leitura.entradaValor();

    // Tenta descontar da conta. Se tiver saldo (true), continua!
    if (contaUsada.getSaldo().descontar(valor)) {

        Categoria categoria = leitura.lerCategoria();
        Necessidade necessidade = leitura.lerNecessidade();

        // OLHA A SUA LINHA AQUI!!! (Só mudei a variável 'bancoUsado' por 'banco')
        Movimentacao novaSaida = new Movimentacao("SAIDA", banco, java.time.LocalDate.now(), categoria, entidade, necessidade, descricao, valor);

        DBMovimentacoes.salvar(novaSaida);

        System.out.println("\n✅ Saída registrada com sucesso!");
        novaSaida.exibirRecibo();

    } else {
        System.out.println("❌ modelos.Saldo insuficiente!");
    }
}

}


