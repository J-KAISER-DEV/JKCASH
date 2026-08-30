import java.util.Scanner;

public class Leitura {
    private Scanner scan = new Scanner(System.in);

    private int lerInt() {
        for (int i = 0; i < 10; ) {
            try {
                String entrada = scan.nextLine();
                return Integer.parseInt(entrada);
            } catch (RuntimeException e) {
                System.out.print("ENTRADA INVALIDA, DIGITE APENAS NUMEROS: ");
                i++;
                System.out.print("\n" + i + "/10 TENTATIVAS");

            }
        }
        System.out.println("VAISEFUDER NE PORRA");
        return 0;
    }

    public int entradaValor() {

        return lerInt();
    }

    public String entradaTexto() {

        String entrada = scan.nextLine();
        return entrada;

    }

    public Categoria lerCategoria() {
    while(true){
        System.out.println("Selecione a Categoria: ");
        System.out.println("1 - ALIMENTAÇÃO");
        System.out.println("2 - SAUDE E HIGIENE");
        System.out.println("3 - LOCOMOÇÃO");
        System.out.println("4 - LAZER");
        System.out.println("5 - ESTUDO");
        System.out.println("6 - ASSINATURAS");
        System.out.println("7 - OUTROS");
        System.out.print("SELECIONE: ");

        int opcao = lerInt();

        switch (opcao){
            case 1: return Categoria.ALIMENTACAO;
            case 2: return Categoria.SAUDE_HIGIENE;
            case 3: return Categoria.LOCOMOCAO;
            case 4: return Categoria.LAZER;
            case 5: return Categoria.ESTUDO;
            case 6: return Categoria.ASSINATURAS;
            case 7: return Categoria.OUTROS;
            default:
                System.out.println("OPÇÃO INVALIDA,TENTE NOVAMENTE");
        }
    }
    }

    public Necessidade lerNecessidade(){
        while (true){
            System.out.println("Selecione o grau da necessidade");
            System.out.println("1 - URGENTE");
            System.out.println("2 - ESSENCIAL");
            System.out.println("3 - IMPORTANTE");
            System.out.println("4 - LUXO");
            System.out.println("5 - DESNECESSARIO");
            System.out.print("SELECIONE: ");

            int opcao = lerInt();
            switch (opcao){
                case 1: return Necessidade.URGENTE;
                case 2: return Necessidade.ESSENCIAL;
                case 3: return Necessidade.IMPORTANTE;
                case 4: return Necessidade.DESNECESSARIO;
                default:
                    System.out.println("OPÇÃO INVALIDA, TENTE NOVAMENTE");
            }
        }

    }


public double lerDouble(){
        while (true){
            try{
                String entrada = scan.nextLine();
                entrada = entrada.replace(",", ".");
                return Double.parseDouble(entrada);
            }catch (NumberFormatException e ){
                System.out.print("Valor Invalido, Tente Novamente: ");
            }

        }
}


}
