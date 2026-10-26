# Extrato paginado

Entrega de 26/10/2026, correspondente à otimização do extrato na etapa 4.

## Organização do código

- Application define IExtratoConsulta e os resultados ExtratoPagina, ExtratoTotais e ExtratoItem. O resultado transporta dados de leitura, sem expor entidades rastreadas ou contextos.
- Infrastructure implementa ExtratoConsulta. Cada chamada cria e descarta um contexto, calcula quantidade e totais agrupados no banco e consulta uma página. A página padrão tem 50 registros; o contrato aceita tamanhos entre 1 e 200.
- FabricaTelas injeta a consulta SQLite em UcTransacoes. O serviço de gravação continua usando o repositório de transações.
- UcTransacoes.Extrato.cs captura os filtros na thread visual, consulta em Task.Run e exibe os dados. Anterior e Próxima navegam sobre os filtros aplicados. Alterar um filtro limpa a consulta e exige Consultar extrato novamente.

## Filtros e paginação

O início é inclusivo e o fim é o início do dia seguinte, exclusivo:

```csharp
t.Data >= inicio && t.Data < fimExclusivo
```

Assim, o último dia inteiro continua incluído sem aplicar Data.Date sobre cada registro da coluna. Quando o último dia é 31/12/9999, a condição superior é dispensada para evitar overflow. O método ListarPorPeriodo do repositório, usado também pelo Console, recebeu o mesmo ajuste.

A consulta ordena por Data e depois por Id, aplica Skip e Take antes de materializar os registros e associa os nomes de conta, banco e categoria apenas aos itens da página. O painel de detalhes mantém descrição, justificativa e avaliações.

Os totais abrangem o período inteiro. Transferências enviadas e recebidas ficam separadas de entradas e saídas comuns. No SQLite, a soma usa os centavos INTEGER armazenados no banco e só converte o resultado para decimal em reais; não usa double.

Quantidade, totais e itens de uma chamada são lidos na mesma transação. São duas consultas SELECT: agrupamento de totais/quantidade e página com os detalhes. A navegação seguinte faz uma nova leitura: alterações no histórico entre páginas podem mudar quantidades, totais e posição dos registros. Uma página que deixou de existir é ajustada para a última disponível.

## Verificação e limites

- Solução compilada sem erros. Permanece o aviso NU1900 referente à indisponibilidade da consulta de vulnerabilidades do NuGet.
- 27 verificações concluídas com 3.146 movimentações sintéticas em um SQLite temporário, removido ao terminar, e controles WinForms executados em uma thread STA.
- Foram conferidos centavos, totais de todas as páginas, ordenação com datas iguais, ausência de duplicação entre páginas com histórico estável, limites das datas, conta inativa, múltiplas contas, transferências, consulta vazia, página fora do limite e parâmetros inválidos.
- Um interceptor confirmou duas consultas SELECT por página e o limite no SQL gerado. A interface foi conferida quanto a navegação, totais, detalhes, alteração de filtro, falha de consulta, clique repetido e descarte durante carregamento.
- A consulta do Console foi comparada com os mesmos registros esperados, inclusive no limite máximo de DateTime.
- Nenhum banco real foi acessado e nenhum projeto de testes foi adicionado. A verificação temporária permanece em arquivos ignorados dentro de obj/bin.
- SQL Server foi compilado, mas não executado nesta entrega. Não há benchmark de latência/memória nem alteração de índices, esquema ou migrações. Paginação por deslocamento pode custar mais nas últimas páginas de históricos muito grandes; essa avaliação permanece para as medições da etapa 4.
