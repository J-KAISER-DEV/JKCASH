# Etapa 5 — Responsividade e tratamento de falhas

Entrega de 26/10/2026, referente às telas do Windows Forms.

## Comportamento

- Contas, Categorias e cadastro de Cartões consultam e gravam em segundo plano, como os fluxos de movimentação já faziam. Os valores dos controles são capturados na thread visual antes do trabalho com os repositórios.
- Consultas e gravações têm guarda de reentrada e desabilitam os controles correspondentes durante a execução. Um segundo clique na mesma operação não inicia outra gravação. A navegação principal continua disponível.
- Contas e Categorias exibem uma barra de situação e o botão Atualizar dados. Transações também oferece atualização das opções sem exigir reabrir a tela. Cartões reúne cadastro, limites, opções de compras e histórico no mesmo fluxo de atualização.
- Campos são preservados quando a gravação não é confirmada. São limpos após o retorno bem-sucedido da gravação, mesmo quando a consulta seguinte falha.
- Uma falha após salvar informa explicitamente que o registro foi salvo. Dados indisponíveis são retirados da tela e uma nova gravação depende da atualização. Quando a confirmação da gravação é desconhecida, a mensagem orienta consultar o histórico antes de tentar novamente.
- Conflitos conhecidos em compras, pagamentos, transações e transferências permitem revisar os dados atualizados. Não há repetição automática de movimentações.
- Falhas encapsuladas de infraestrutura não são apresentadas como validações de preenchimento. Os detalhes continuam em Debug.WriteLine; a interface mostra uma orientação para recuperação.
- Retornos assíncronos verificam se a tela foi descartada antes de alterar os controles. Sair de uma tela não cancela uma gravação já iniciada: ela pode concluir no banco, e o usuário deve consultar a operação ao retornar.
- Dashboard e extrato mantêm as consultas em segundo plano, bloqueio das ações correspondentes e limpeza de resultados em caso de erro.

Cada operação de repositório continua criando e descartando seu próprio contexto. Não foram adicionados pacotes, alteradas regras financeiras ou feitas migrações do banco.

## Verificação

A solução compilou sem erros. Permaneceu o aviso NU1900 por indisponibilidade da consulta de vulnerabilidades do NuGet.

Uma verificação temporária, fora da solução e dos arquivos versionados, executou os controles WinForms em uma thread STA com ciclo de mensagens e repositórios simulados. As 39 verificações passaram: consultas e gravações lentas, continuação da thread visual, bloqueio de reentrada, preservação e limpeza de campos, recuperação após falhas, distinção entre gravação e atualização, falhas encapsuladas, descarte durante consultas e disposição da barra de atualização sem sobrepor a tabela. Compras, pagamentos e transferências exercitaram os serviços reais contra esses repositórios. Nenhum banco real foi acessado e nenhum projeto de testes foi adicionado.

Essa verificação não substitui a conferência visual de tamanhos e acabamento nem a validação financeira com um histórico SQLite preenchido. As pendências da etapa 4 permanecem registradas no cronograma.
