package bancodedados;

import modelos.Movimentacao;

import java.util.ArrayList;
import java.util.List;

public class DBMovimentacoes {

    private static List<Movimentacao> historico = new ArrayList<>();

    public   static void salvar(Movimentacao novaMovimentacao){
        historico.add(novaMovimentacao);
    }
    public static List <Movimentacao> getHistorico(){
    return historico;

    }
}
