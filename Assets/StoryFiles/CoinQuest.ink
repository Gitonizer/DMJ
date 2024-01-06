->inicio
=== inicio ===
Olá, Feiticeiro, tudo bem?
+ [Sim]
->engraxar
+ [Não]
(vamos tentar isto outra vez...)
->inicio

=== engraxar ===
Boa!
Fico feliz por si!
Eu sei que não me perguntou nada, mas comigo as coisas não estão bem:
->problema

=== problema ===
Perdi a minha moeda!
+ [Que chatice]
->pedido
+ [De facto não perguntei nada]
->chateado

=== chateado ===
Olhe, se vai ser mal educado, então gostaria que não me dirigisse mais a palavra!
+ [Peço desculpa]
Como estava a dizer...
->problema
+ [Não mandas em mim]
->chateado

=== pedido ===
Acha que poderia ir procurar a minha moeda?
Ela está perdida algures numa destas salas
+ [Sim]
->aceite
+ [Não]
->chateado

=== aceite ===
Excelente! Aguardarei pacientemente! #needsCoin:true

Incrível! A minha moeda está de volta!
Encontrei esta chave, acredito que lhe poderá ser útil
->END