import java.time.LocalDate;

public class Despesa {
private String bancoUsado;
private LocalDate data;
private String recebedor;
private Necessidade grau;
private Categoria categoria;
private String descricao;
private double valor;

    public Despesa(String bancoUsado, String recebedor, Necessidade grau, Categoria categoria, String descricao, double valor) {
        this.bancoUsado = bancoUsado;
        this.recebedor = recebedor;
        this.grau = grau;
        this.categoria = categoria;
        this.descricao = descricao;
        this.valor = valor;
        this.data = LocalDate.now();
    }
public void exibirRecibo(){
    System.out.println("==============RECIBO================");
    System.out.println("CONTA: " + this.bancoUsado);
    System.out.println("Data: " + this.data);
    System.out.println("Cobrador: " + this.recebedor);
    System.out.println("Descrição: " + this.descricao);
    System.out.println("Categoria: " + this.categoria);
    System.out.println("Necessidade: " + this.grau);
    System.out.println("Valor: R$ " + this.valor);
    System.out.println("======================================");
}
public static void registrarSaida(){
        Leitura leitura = new Leitura();


    System.out.println("\n--------------REGISTRAR DESPESA------------------");
    System.out.print("Conta Usada: ");
    String banco = leitura.entradaTexto();
    Conta contaUsada = DBContas.buscarConta(banco);
    if(contaUsada == null){
        System.out.println("Conta Inexistente");
        return;
    }

    System.out.print("Local ou Entidade recebedora: ");
    String recebedor = leitura.entradaTexto();
    System.out.print("Descrição da Compra: ");
    String descricao = leitura.entradaTexto();
    System.out.println("Valor R$: ");
    double valor = leitura.lerDouble();
    if(contaUsada.getSaldo().descontar(valor)){
        Categoria catEscolhida = leitura.lerCategoria();
        Necessidade necEscolhida = leitura.lerNecessidade();

        Despesa novaDespesa = new Despesa(banco, recebedor, necEscolhida,catEscolhida, descricao, valor);
        DBDespesas.salvarDespesa(novaDespesa);
        System.out.println("Despesa Registrada!");
        novaDespesa.exibirRecibo();

    }else {
        System.out.println("Valor Insuficiente");
    }

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

    public String getRecebedor() {
        return recebedor;
    }

    public void setRecebedor(String recebedor) {
        this.recebedor = recebedor;
    }

    public Necessidade getGrau() {
        return grau;
    }

    public void setGrau(Necessidade grau) {
        this.grau = grau;
    }

    public Categoria getCategoria() {
        return categoria;
    }

    public void setCategoria(Categoria categoria) {
        this.categoria = categoria;
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
}
