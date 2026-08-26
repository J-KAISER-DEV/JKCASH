import java.util.Scanner;

public class Leitura {
    private Scanner scan = new Scanner(System.in);

    private int lerInt() {
        for (int i = 0; i<10;){
            try{
                String entrada = scan.nextLine();
                return Integer.parseInt(entrada);
            } catch (RuntimeException e) {
                System.out.print("ENTRADA INVALIDA, DIGITE APENAS NUMEROS: ");
                i++;
                System.out.print("\n" +  i + "/10 TENTATIVAS");

            }
        }
        System.out.println("VAISEFUDER NE PORRA");
        return 0;
    }

    public int entradaValor(){

            return lerInt();
    }

    public String entradaTexto(){

        String entrada = scan.nextLine();
        return entrada;

    }
}
