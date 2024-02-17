"Relative tokens, which are containing syntax highlite.
"
"TokenIndentificator.Int = "int"
"TokenIndentificator.Float, "float"
"TokenIndentificator.Byte, "byte"
"TokenIndentificator.Char, "char"
"TokenIndentificator.If, "if"
"TokenIndentificator.Else, "else"
"TokenIndentificator.EqualLogical, "=="
"TokenIndentificator.NotEqual, "!="
"TokenIndentificator.GreaterEqual, ">="
"TokenIndentificator.GreaterThan, ">"
"TokenIndentificator.LessEqual, "<="
"TokenIndentificator.LessThan, "<"
"TokenIndentificator.Invoker, "->"
"TokenIndentificator.Print, "print"
"TokenIndentificator.Definition, "def"
"TokenIndentificator.DefinitionCall, "::"
"TokenIndentificator.DefinitionReturn, "::="

"Operator tokens: TokenIndentificator.Add("+"), "TokenIndentificator.Sub("-")
"     TokenIndentificator.Mul("*"), TokenIndentificator.Div("/").

"TokenIndentificator.LogicalAnd, "&"
"TokenIndentificator.LogicalOr, "|"
"TokenIndentificator.LogicalXor, "^"
"TokenIndentificator.LogicalLshift, "<<"
"TokenIndentificator.LogicalRshift, ">>"

autocmd BufRead, BufNewFile *.ath set filetype=ath

"Syntax keywords
syntax keyword primitiveTypes int float byte char
syntax keyword Types int float byte char




