# JKCASH

## Abrir no Visual Studio

Depois de clonar o repositório, abra `JKCASH.sln`, na raiz, usando **Arquivo > Abrir > Projeto/Solução**. Essa solução reúne os projetos do JKCASH. Abrir apenas a pasta do repositório deixa o Visual Studio no modo de pastas.

Use o Visual Studio com suporte ao .NET 10 e o SDK do .NET 10 instalado. Para iniciar a aplicação, defina o projeto `JKCASH` como projeto de inicialização.

## Compilar

```powershell
dotnet build JKCASH.sln
```

O projeto de estudos `C#Aprendizado` tem uma solução própria em `C#Aprendizado/C#Aprendizado.slnx`.

## Interface Windows Forms — prévia visual

Defina **JKCASH.WinForms** como projeto de inicialização e execute com F5. O dashboard segue a referência visual aprovada: menu azul escuro, fundo claro, botões verdes, cartões de indicadores, barras por categoria e tabela de movimentações. Menu e atalhos permitem navegar entre as seis seções; fora do dashboard, as telas estão sinalizadas como em construção.

Esta etapa usa valores ilustrativos, identificados no rodapé, e não consulta nem altera o banco. Os filtros contêm somente o cenário demonstrativo. As funções financeiras continuam disponíveis no projeto Console enquanto conectamos as telas aos serviços existentes.

`FrmPrincipal.Designer.cs` mantém a estrutura dos painéis da janela. `FrmPrincipal.Visual.cs` configura o menu e a navegação em execução; abra esse arquivo como código. `Telas/UcDashboard.cs` organiza o conteúdo com `TableLayoutPanel`, `Panel`, `Label`, `ComboBox`, `Button` e `DataGridView`. `Visual/EstiloVisual.cs` centraliza a paleta, botões, ícones e desenho das bordas, usando somente Windows Forms e System.Drawing. Para conferir o acabamento completo, execute o projeto; o designer da janela mantém apenas a estrutura básica.

A referência e uma captura renderizada da interface ficam em `docs/design/`.

## Executar sem banco de dados

No Visual Studio, selecione o perfil **JKCASH (Memoria)** ao lado do botão de iniciar e pressione F5. Todos os cadastros e movimentações ficam em memória durante a execução; os dados são perdidos ao fechar o programa.

Pelo terminal, na raiz do repositório:

```powershell
dotnet run --project JKCASH/JKCASH.csproj --no-launch-profile -- --memoria
```

## Executar com SQL

No Visual Studio, selecione o perfil **JKCASH (SQL)**. Esse modo precisa de acesso ao banco configurado e das tabelas já criadas. Contas, categorias, transações, cartões, compras, parcelas, faturas e pagamentos são salvos no SQL.

```powershell
dotnet run --project JKCASH/JKCASH.csproj --no-launch-profile
```

Sem o argumento `--memoria`, a aplicação mantém o comportamento de usar SQL. Os perfis do Visual Studio também podem ser usados no terminal com `--launch-profile "JKCASH (Memoria)"` ou `--launch-profile "JKCASH (SQL)"`.

Para atualizar um banco existente com o suporte a transferências, execute `BancoDeDados/Scripts/001-transferencias.sql` no SQL Server. O script pode ser executado novamente: adiciona somente a coluna opcional `TransferenciaId` e seu índice, preservando os registros existentes. Essa atualização já foi aplicada no banco local usado no desenvolvimento.

## Navegação por submenus

O menu principal mostra somente os grupos. Dentro de cada submenu, **0 - Voltar** retorna ao principal; no principal, **0 - Sair** encerra o programa.

| Menu principal | Operações |
|---|---|
| 1 - Contas | Cadastrar, listar, excluir e transferir entre contas |
| 2 - Transações | Registrar, consultar, extrato e resumos por categoria, planejamento, importância e urgência |
| 3 - Cartões | Cadastrar e listar cartões, registrar e consultar compras |
| 4 - Faturas | Listar, consultar parcelas e pagamentos, registrar pagamento |
| 5 - Categorias | Cadastrar e listar categorias |

Cada classe de menu organiza suas operações. `NavegacaoConsole` reutiliza a exibição das opções, o retorno e o tratamento de erros. `Program.cs` cria os repositórios e serviços conforme o modo escolhido. As interfaces de compras e faturas recebem a mesma instância de repositório.

Em **Contas > Excluir conta**, selecione uma conta: a tela informa quantas transações serão apagadas junto com ela. Digite **EXCLUIR** para confirmar a exclusão definitiva da conta e de suas transações; qualquer outra resposta cancela. As categorias são preservadas. Contas com cartões ou pagamentos de fatura vinculados continuam bloqueadas, inclusive quando pagam a fatura de um cartão de outra conta. No SQL, um único `SaveChanges` remove as transações e a conta em uma transação do banco: se uma exclusão falhar, tudo é desfeito. O serviço mantém a opção de excluir sem histórico bloqueada por padrão; a remoção conjunta exige a opção explícita `excluirTransacoes: true`.

Para verificar a exclusão em memória e no SQL com dados temporários e rollback: `dotnet run --project JKCASH.TestesManuais -- --exclusao-contas`.

## Transferências entre contas

Em **Contas > 4 - Transferir entre contas**, escolha a origem, informe o valor e selecione o destino. O destino deve ser outra conta ativa. O valor deve ser positivo, ter no máximo duas casas decimais e caber no saldo e cheque especial da origem. O menu verifica o valor antes de pedir os demais dados. Selecione a categoria, importância, planejamento, urgência, descrição e justificativa. A data e o horário são os do registro da transferência.

`TransferenciaServico` coordena o débito e o crédito. A entidade `Transferencia` cria dois registros `Transacao` ligados pelo mesmo `TransferenciaId`: saída na origem e entrada no destino. O repositório SQL grava os dois saldos e os dois registros na mesma transação; uma falha desfaz toda a operação. Leituras e atualizações da transferência usam isolamento serializável, e saldos desatualizados são recusados. Em memória, os registros são preparados em uma cópia e o serviço restaura os saldos se o armazenamento falhar.

O extrato identifica as transferências e apresenta os totais enviados e recebidos separadamente das entradas e saídas comuns. Os resumos de gastos por categoria, planejamento, importância e urgência não incluem transferências internas, mesmo quando apenas uma das contas é selecionada. Transferências não modificam os saldos iniciais e não representam compras no cartão nem pagamentos de fatura.

Contas que participam de transferências não podem ser excluídas nesta versão: remover apenas um lado apagaria parte do histórico de outra conta. Estorno e exclusão conjunta de transferências ficam para uma funcionalidade própria.

## Cartões de crédito

Uma conta pode ter nenhum, um ou vários cartões. Cada cartão pertence a uma conta, e o banco do cartão é o banco dessa conta.

Cadastre uma conta em **Contas > Cadastrar conta**. Em **Cartões > Cadastrar cartão**, selecione a conta e informe os dados do cartão. **Cartões > Listar cartões e limites** mostra os cartões com a conta e o banco correspondentes. Cadastrar ou comprar no cartão não debita automaticamente o saldo da conta.

Cartões, compras, parcelas, faturas e pagamentos usam memória no perfil **JKCASH (Memoria)** e persistência no banco no perfil **JKCASH (SQL)**. Pagamentos de faturas estão disponíveis nos dois modos. No SQL, compras, parcelas, faturas e limite são gravados juntos; cada pagamento grava seu histórico, a saída da conta, o saldo e o limite na mesma transação do banco.

## Compras, parcelas e faturas

| Caminho | Operação |
|---|---|
| Cartões > Registrar compra à vista ou parcelada | Criar compra e suas parcelas |
| Cartões > Listar compras de um cartão | Consultar compras e parcelas |
| Cartões > Buscar compra por Id | Localizar uma compra |
| Faturas > Listar faturas de um cartão | Consultar totais por ciclo |
| Faturas > Consultar parcelas e pagamentos | Consultar o histórico da fatura |
| Faturas > Registrar pagamento total ou parcial | Registrar um pagamento confirmado |

Ao cadastrar cada cartão, escolha se compras feitas no próprio dia de fechamento entram na fatura atual ou na próxima. Quando a escolha é a fatura atual, ela permanece aberta durante esse dia e encerra no dia seguinte. Na outra opção, o ciclo encerra no início do dia de fechamento.

Uma compra compromete o valor total no limite imediatamente. Uma compra de R$ 300 em 3 vezes gera três parcelas de R$ 100 em faturas consecutivas. Uma compra à vista gera uma parcela. Nesta versão, são aceitas de 1 a 120 parcelas, cada uma de pelo menos um centavo. Diferenças de arredondamento ficam na última parcela: R$ 100 em 3 vezes vira R$ 33,33 + R$ 33,33 + R$ 33,34.

Parcelas do mesmo cartão e ciclo são agrupadas em uma fatura. As datas usam o dia configurado ou o último dia do mês, se necessário; em março, por exemplo, o dia 31 volta a ser usado depois de fevereiro. O vencimento é a primeira ocorrência do dia configurado após o fechamento. Não há ajuste por feriados ou dias úteis.

O fechamento acontece automaticamente conforme a data, sem precisar deixar o programa aberto. Uma fatura fechada não recebe novas compras. O cadastro aceita compras até hoje cujo primeiro ciclo ainda esteja aberto; importação de compras históricas de ciclos já fechados não faz parte desta versão.

Uma fatura aberta pode receber pagamento antecipado e continua aberta para novas compras até o fechamento. A tela mostra separadamente a situação do ciclo (aberta, fechada ou paga), o total pago e o saldo pendente. Após o vencimento, o saldo restante é sinalizado como atrasado. Sem juros, multas, rotativo ou parcelamento do saldo da fatura. O saldo pendente permanece na fatura original.

## Pagamentos confirmados

Em **Faturas > Registrar pagamento total ou parcial**, registre um pagamento já confirmado, na data de hoje. Selecione a fatura, a conta pagadora (que pode ser de outro banco), a categoria da saída e o valor. Valores precisam ser positivos, ter até duas casas decimais e não exceder o saldo pendente. A conta deve estar ativa e ter saldo ou cheque especial disponível.

Cada pagamento gera uma saída na conta e um registro no histórico da fatura, ligados pelo Id da transação. O limite do cartão é liberado somente pelo valor pago. É possível pagar R$ 200, depois R$ 100 e depois R$ 200 para quitar uma fatura de R$ 500. As parcelas de outros meses permanecem comprometendo limite até seus próprios pagamentos.

O serviço desfaz a movimentação do saldo e do limite se o armazenamento falhar. No repositório em memória, compras e suas faturas são preparadas antes da confirmação; pagamentos e saídas também respeitam o contrato de salvar juntos. As compras não geram saídas na conta: a saída é registrada no pagamento, evitando duplicar a movimentação do saldo.

### Roteiro para experimentar

1. Escolha o perfil de execução e cadastre uma conta com saldo em **Contas > Cadastrar conta**.
2. Cadastre uma categoria em **Categorias** e um cartão em **Cartões**.
3. Em **Cartões > Registrar compra à vista ou parcelada**, registre uma compra de hoje, por exemplo R$ 300 em 3 vezes.
4. Confira o limite e as compras em **Cartões** e os três ciclos em **Faturas > Listar faturas de um cartão**.
5. Em **Faturas > Registrar pagamento total ou parcial**, pague R$ 40 da primeira fatura. Ela ficará com R$ 60 pendentes e o limite disponível aumentará R$ 40.
6. Consulte o histórico em **Faturas > Consultar parcelas e pagamentos** e a saída em **Transações > Listar todas as transações**.
7. Pague os R$ 60 restantes e confira que os R$ 200 das parcelas futuras continuam comprometendo o limite.
8. No perfil SQL, feche e abra novamente para confirmar a permanência dos dados. No perfil em memória, os dados são descartados ao encerrar.

## Testes

Para executar os cenários do projeto de testes:

```powershell
dotnet run --project JKCASH.TestesManuais/JKCASH.TestesManuais.csproj
```

Além dos cenários anteriores, há 19 cenários automáticos para parcelas, calendário, opções de fechamento, agrupamento de faturas, pagamentos parciais, conta pagadora diferente, validações e recuperação em caso de falha. Esses cenários encerram o processo com erro quando uma verificação falha.

O executor também chama `TesteCartaoCreditoSql`, que precisa do SQL Server configurado e de uma conta existente. Ele testa a inclusão e leitura de um cartão dentro de um `TransactionScope` sem confirmação, desfazendo a inclusão ao terminar.
