if exists("b:current_syntax")
  finish
endif

syntax keyword primitiveTypes int float byte char
syntax keyword statements if else
syntax keyword compileMacro print
syntax keyword definition def nextgroup=functionIdentificator

syntax match definitionCall "::" nextgroup=functionIdentificator
syntax match definitionReturn "::="
syntax match valueAssignment "="

syntax match operator /+\|-\|*\|\/\|&\||\|\^\|<<\|>>/
syntax match relationalOperator /==\|!=\|>=\|>\|<=\|</

syntax match statementInvoker /->\|(\|)/
syntax match functionIdentificator "[^(]*" contained 

"Reduced match of python number syntax highlight.
syntax match numberValue /\<\%([1-9]\d*\|0\)[Ll]\=\>/

highlight link primitiveTypes Type
highlight link statements Operator

highlight link definition @keyword.function
highlight link definitionCall Operator
highlight link definitionReturn Operator

highlight link statementInvoker Operator
highlight link compileMacro Define

highlight link operator Operator
highlight link relationalOperator Boolean

highlight link valueVariableType Indentifier
highlight link functionIdentificator Function
highlight link valueAssignment Statement
highlight link numberValue Number

let b:current_syntax = "ath"
