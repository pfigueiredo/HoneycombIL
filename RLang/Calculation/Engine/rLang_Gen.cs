﻿//Generated by the GOLD Parser Builder

using System.IO;
using System.Windows.Forms;

class MyParser
{
    private GOLD.Parser parser = new GOLD.Parser(); 

    private enum SymbolIndex
    {
        @Eof = 0,                                  // (EOF)
        @Error = 1,                                // (Error)
        @Comment = 2,                              // Comment
        @Newline = 3,                              // NewLine
        @Whitespace = 4,                           // Whitespace
        @Timesdiv = 5,                             // '*/'
        @Divtimes = 6,                             // '/*'
        @Divdiv = 7,                               // '//'
        @Minus = 8,                                // '-'
        @Dollar = 9,                               // '$'
        @Percent = 10,                             // '%'
        @Amp = 11,                                 // '&'
        @Lparen = 12,                              // '('
        @Rparen = 13,                              // ')'
        @Times = 14,                               // '*'
        @Comma = 15,                               // ','
        @Dot = 16,                                 // '.'
        @Div = 17,                                 // '/'
        @Colon = 18,                               // ':'
        @At = 19,                                  // '@'
        @Lbracket = 20,                            // '['
        @Rbracket = 21,                            // ']'
        @Caret = 22,                               // '^'
        @Lbrace = 23,                              // '{'
        @Rbrace = 24,                              // '}'
        @Plus = 25,                                // '+'
        @Lt = 26,                                  // '<'
        @Lteq = 27,                                // '<='
        @Ltgt = 28,                                // '<>'
        @Eq = 29,                                  // '='
        @Gt = 30,                                  // '>'
        @Gteq = 31,                                // '>='
        @And = 32,                                 // AND
        @Else = 33,                                // ELSE
        @End = 34,                                 // END
        @Identifier = 35,                          // Identifier
        @If = 36,                                  // IF
        @Not = 37,                                 // NOT
        @Number = 38,                              // Number
        @Or = 39,                                  // OR
        @Stringliteral = 40,                       // StringLiteral
        @Then = 41,                                // THEN
        @Addexp = 42,                              // <Add Exp>
        @Args = 43,                                // <Args>
        @Assignexp = 44,                           // <Assign Exp>
        @Assigncell = 45,                          // <AssignCell>
        @Assignlist = 46,                          // <AssignList>
        @Assingcolrange = 47,                      // <AssingColRange>
        @Boolexp = 48,                             // <Bool Exp>
        @Cellreference = 49,                       // <CellReference>
        @Colreference = 50,                        // <ColReference>
        @Expression = 51,                          // <Expression>
        @Globalreference = 52,                     // <GlobalReference>
        @Multexp = 53,                             // <Mult Exp>
        @Negateexp = 54,                           // <Negate Exp>
        @Percentexp = 55,                          // <Percent Exp>
        @Powexp = 56,                              // <Pow Exp>
        @Program = 57,                             // <Program>
        @Rangereference = 58,                      // <RangeReference>
        @Rowreference = 59,                        // <RowReference>
        @Singlecolrange = 60,                      // <SingleColRange>
        @Value = 61                                // <Value>
    }

    private enum ProductionIndex
    {
        @Program = 0,                              // <Program> ::= <AssignList>
        @Program_Eq = 1,                           // <Program> ::= '=' <Bool Exp>
        @Assignlist = 2,                           // <AssignList> ::= <AssignList> <Assign Exp>
        @Assignlist2 = 3,                          // <AssignList> ::= <Assign Exp>
        @Assignexp_Identifier_Eq_Lbrace_Rbrace = 4,  // <Assign Exp> ::= Identifier '=' '{' <Bool Exp> '}'
        @Assignexp_Eq_Lbrace_Rbrace = 5,           // <Assign Exp> ::= <GlobalReference> '=' '{' <Bool Exp> '}'
        @Assignexp_Eq_Lbrace_Rbrace2 = 6,          // <Assign Exp> ::= <AssignCell> '=' '{' <Bool Exp> '}'
        @Assignexp_Eq_Lbrace_Rbrace3 = 7,          // <Assign Exp> ::= <AssingColRange> '=' '{' <Bool Exp> '}'
        @Args_Comma = 8,                           // <Args> ::= <Args> ',' <Bool Exp>
        @Args = 9,                                 // <Args> ::= <Bool Exp>
        @Boolexp_And = 10,                         // <Bool Exp> ::= <Bool Exp> AND <Expression>
        @Boolexp_Or = 11,                          // <Bool Exp> ::= <Bool Exp> OR <Expression>
        @Boolexp_And_Lparen_Rparen = 12,           // <Bool Exp> ::= AND '(' <Args> ')'
        @Boolexp_Or_Lparen_Rparen = 13,            // <Bool Exp> ::= OR '(' <Args> ')'
        @Boolexp_Not = 14,                         // <Bool Exp> ::= NOT <Expression>
        @Boolexp = 15,                             // <Bool Exp> ::= <Expression>
        @Expression_Gt = 16,                       // <Expression> ::= <Expression> '>' <Add Exp>
        @Expression_Lt = 17,                       // <Expression> ::= <Expression> '<' <Add Exp>
        @Expression_Lteq = 18,                     // <Expression> ::= <Expression> '<=' <Add Exp>
        @Expression_Gteq = 19,                     // <Expression> ::= <Expression> '>=' <Add Exp>
        @Expression_Eq = 20,                       // <Expression> ::= <Expression> '=' <Add Exp>
        @Expression_Ltgt = 21,                     // <Expression> ::= <Expression> '<>' <Add Exp>
        @Expression = 22,                          // <Expression> ::= <Add Exp>
        @Addexp_Plus = 23,                         // <Add Exp> ::= <Add Exp> '+' <Mult Exp>
        @Addexp_Minus = 24,                        // <Add Exp> ::= <Add Exp> '-' <Mult Exp>
        @Addexp_Amp = 25,                          // <Add Exp> ::= <Add Exp> '&' <Mult Exp>
        @Addexp = 26,                              // <Add Exp> ::= <Mult Exp>
        @Multexp_Times = 27,                       // <Mult Exp> ::= <Mult Exp> '*' <Pow Exp>
        @Multexp_Div = 28,                         // <Mult Exp> ::= <Mult Exp> '/' <Pow Exp>
        @Multexp = 29,                             // <Mult Exp> ::= <Pow Exp>
        @Powexp_Caret = 30,                        // <Pow Exp> ::= <Pow Exp> '^' <Percent Exp>
        @Powexp = 31,                              // <Pow Exp> ::= <Percent Exp>
        @Percentexp_Percent = 32,                  // <Percent Exp> ::= <Negate Exp> '%'
        @Percentexp = 33,                          // <Percent Exp> ::= <Negate Exp>
        @Negateexp_Minus = 34,                     // <Negate Exp> ::= '-' <Value>
        @Negateexp = 35,                           // <Negate Exp> ::= <Value>
        @Value_Number = 36,                        // <Value> ::= Number
        @Value_If_Lparen_Rparen_Then_End = 37,     // <Value> ::= IF '(' <Bool Exp> ')' THEN <Bool Exp> END
        @Value_If_Lparen_Rparen_Then_Else_End = 38,  // <Value> ::= IF '(' <Bool Exp> ')' THEN <Bool Exp> ELSE <Bool Exp> END
        @Value_If_Lparen_Rparen = 39,              // <Value> ::= IF '(' <Args> ')'
        @Value_Identifier_Lparen_Rparen = 40,      // <Value> ::= Identifier '(' <Args> ')'
        @Value_Identifier_Lparen_Rparen2 = 41,     // <Value> ::= Identifier '(' ')'
        @Value = 42,                               // <Value> ::= <RangeReference>
        @Value2 = 43,                              // <Value> ::= <CellReference>
        @Value3 = 44,                              // <Value> ::= <GlobalReference>
        @Value_Identifier = 45,                    // <Value> ::= Identifier
        @Value_Stringliteral = 46,                 // <Value> ::= StringLiteral
        @Value_Lparen_Rparen = 47,                 // <Value> ::= '(' <Bool Exp> ')'
        @Globalreference_At_Identifier_Dot_Identifier = 48,  // <GlobalReference> ::= '@' Identifier '.' Identifier
        @Globalreference_At_Identifier = 49,       // <GlobalReference> ::= '@' Identifier
        @Rangereference_Lbracket_Rbracket_Colon_Lbracket_Rbracket = 50,  // <RangeReference> ::= <ColReference> '[' <RowReference> ']' ':' <ColReference> '[' <RowReference> ']'
        @Rangereference_Colon = 51,                // <RangeReference> ::= <ColReference> ':' <ColReference>
        @Rangereference_Dollar_Number_Colon_Dollar_Number = 52,  // <RangeReference> ::= <ColReference> '$' Number ':' <ColReference> '$' Number
        @Rangereference = 53,                      // <RangeReference> ::= <SingleColRange>
        @Singlecolrange_Lbracket_Colon_Rbracket = 54,  // <SingleColRange> ::= <ColReference> '[' <RowReference> ':' <RowReference> ']'
        @Cellreference_Lbracket_Rbracket = 55,     // <CellReference> ::= <ColReference> '[' <RowReference> ']'
        @Cellreference_Dollar_Identifier = 56,     // <CellReference> ::= '$' Identifier
        @Assingcolrange_Lbracket_Colon_Rbracket = 57,  // <AssingColRange> ::= <ColReference> '[' <RowReference> ':' <RowReference> ']'
        @Assigncell_Lbracket_Rbracket = 58,        // <AssignCell> ::= <ColReference> '[' <RowReference> ']'
        @Assigncell_Dollar_Identifier = 59,        // <AssignCell> ::= '$' Identifier
        @Colreference_Identifier = 60,             // <ColReference> ::= Identifier
        @Colreference_Dollar_Identifier = 61,      // <ColReference> ::= '$' Identifier
        @Rowreference_Number = 62,                 // <RowReference> ::= Number
        @Rowreference_Minus_Number = 63,           // <RowReference> ::= '-' Number
        @Rowreference_Dollar_Number = 64           // <RowReference> ::= '$' Number
    }

    public object program;     //You might derive a specific object

    public void Setup()
    {
        //This procedure can be called to load the parse tables. The class can
        //read tables using a BinaryReader.
        
        parser.LoadTables(Path.Combine(Application.StartupPath, "grammar.egt"));
    }
    
    public bool Parse(TextReader reader)
    {
        //This procedure starts the GOLD Parser Engine and handles each of the
        //messages it returns. Each time a reduction is made, you can create new
        //custom object and reassign the .CurrentReduction property. Otherwise, 
        //the system will use the Reduction object that was returned.
        //
        //The resulting tree will be a pure representation of the language 
        //and will be ready to implement.

        GOLD.ParseMessage response; 
        bool done;                      //Controls when we leave the loop
        bool accepted = false;          //Was the parse successful?

        parser.Open(reader);
        parser.TrimReductions = false;  //Please read about this feature before enabling  

        done = false;
        while (!done)
        {
            response = parser.Parse();

            switch (response)
            {
                case GOLD.ParseMessage.LexicalError:
                    //Cannot recognize token
                    done = true;
                    break;

                case GOLD.ParseMessage.SyntaxError:
                    //Expecting a different token
                    done = true;
                    break;

                case GOLD.ParseMessage.Reduction:
                    //Create a customized object to store the reduction

                    parser.CurrentReduction = CreateNewObject(parser.CurrentReduction as GOLD.Reduction);
                    break;

                case GOLD.ParseMessage.Accept:
                    //Accepted!
                    //program = parser.CurrentReduction   //The root node!                 
                    done = true;
                    accepted = true;
                    break;

                case GOLD.ParseMessage.TokenRead:
                    //You don't have to do anything here.
                    break;

                case GOLD.ParseMessage.InternalError:
                    //INTERNAL ERROR! Something is horribly wrong.
                    done = true;
                    break;

                case GOLD.ParseMessage.NotLoadedError:
                    //This error occurs if the CGT was not loaded.                   
                    done = true;
                    break;

                case GOLD.ParseMessage.GroupError: 
                    //GROUP ERROR! Unexpected end of file
                    done = true;
                    break;
            } 
        } //while

        return accepted;
    }
    
    private object CreateNewObject(GOLD.Reduction r)
    { 
        object result = null;
        
        switch( (ProductionIndex) r.Parent.TableIndex)
        {
            case ProductionIndex.Program:                 
                // <Program> ::= <AssignList>
                break;

            case ProductionIndex.Program_Eq:                 
                // <Program> ::= '=' <Bool Exp>
                break;

            case ProductionIndex.Assignlist:                 
                // <AssignList> ::= <AssignList> <Assign Exp>
                break;

            case ProductionIndex.Assignlist2:                 
                // <AssignList> ::= <Assign Exp>
                break;

            case ProductionIndex.Assignexp_Identifier_Eq_Lbrace_Rbrace:                 
                // <Assign Exp> ::= Identifier '=' '{' <Bool Exp> '}'
                break;

            case ProductionIndex.Assignexp_Eq_Lbrace_Rbrace:                 
                // <Assign Exp> ::= <GlobalReference> '=' '{' <Bool Exp> '}'
                break;

            case ProductionIndex.Assignexp_Eq_Lbrace_Rbrace2:                 
                // <Assign Exp> ::= <AssignCell> '=' '{' <Bool Exp> '}'
                break;

            case ProductionIndex.Assignexp_Eq_Lbrace_Rbrace3:                 
                // <Assign Exp> ::= <AssingColRange> '=' '{' <Bool Exp> '}'
                break;

            case ProductionIndex.Args_Comma:                 
                // <Args> ::= <Args> ',' <Bool Exp>
                break;

            case ProductionIndex.Args:                 
                // <Args> ::= <Bool Exp>
                break;

            case ProductionIndex.Boolexp_And:                 
                // <Bool Exp> ::= <Bool Exp> AND <Expression>
                break;

            case ProductionIndex.Boolexp_Or:                 
                // <Bool Exp> ::= <Bool Exp> OR <Expression>
                break;

            case ProductionIndex.Boolexp_And_Lparen_Rparen:                 
                // <Bool Exp> ::= AND '(' <Args> ')'
                break;

            case ProductionIndex.Boolexp_Or_Lparen_Rparen:                 
                // <Bool Exp> ::= OR '(' <Args> ')'
                break;

            case ProductionIndex.Boolexp_Not:                 
                // <Bool Exp> ::= NOT <Expression>
                break;

            case ProductionIndex.Boolexp:                 
                // <Bool Exp> ::= <Expression>
                break;

            case ProductionIndex.Expression_Gt:                 
                // <Expression> ::= <Expression> '>' <Add Exp>
                break;

            case ProductionIndex.Expression_Lt:                 
                // <Expression> ::= <Expression> '<' <Add Exp>
                break;

            case ProductionIndex.Expression_Lteq:                 
                // <Expression> ::= <Expression> '<=' <Add Exp>
                break;

            case ProductionIndex.Expression_Gteq:                 
                // <Expression> ::= <Expression> '>=' <Add Exp>
                break;

            case ProductionIndex.Expression_Eq:                 
                // <Expression> ::= <Expression> '=' <Add Exp>
                break;

            case ProductionIndex.Expression_Ltgt:                 
                // <Expression> ::= <Expression> '<>' <Add Exp>
                break;

            case ProductionIndex.Expression:                 
                // <Expression> ::= <Add Exp>
                break;

            case ProductionIndex.Addexp_Plus:                 
                // <Add Exp> ::= <Add Exp> '+' <Mult Exp>
                break;

            case ProductionIndex.Addexp_Minus:                 
                // <Add Exp> ::= <Add Exp> '-' <Mult Exp>
                break;

            case ProductionIndex.Addexp_Amp:                 
                // <Add Exp> ::= <Add Exp> '&' <Mult Exp>
                break;

            case ProductionIndex.Addexp:                 
                // <Add Exp> ::= <Mult Exp>
                break;

            case ProductionIndex.Multexp_Times:                 
                // <Mult Exp> ::= <Mult Exp> '*' <Pow Exp>
                break;

            case ProductionIndex.Multexp_Div:                 
                // <Mult Exp> ::= <Mult Exp> '/' <Pow Exp>
                break;

            case ProductionIndex.Multexp:                 
                // <Mult Exp> ::= <Pow Exp>
                break;

            case ProductionIndex.Powexp_Caret:                 
                // <Pow Exp> ::= <Pow Exp> '^' <Percent Exp>
                break;

            case ProductionIndex.Powexp:                 
                // <Pow Exp> ::= <Percent Exp>
                break;

            case ProductionIndex.Percentexp_Percent:                 
                // <Percent Exp> ::= <Negate Exp> '%'
                break;

            case ProductionIndex.Percentexp:                 
                // <Percent Exp> ::= <Negate Exp>
                break;

            case ProductionIndex.Negateexp_Minus:                 
                // <Negate Exp> ::= '-' <Value>
                break;

            case ProductionIndex.Negateexp:                 
                // <Negate Exp> ::= <Value>
                break;

            case ProductionIndex.Value_Number:                 
                // <Value> ::= Number
                break;

            case ProductionIndex.Value_If_Lparen_Rparen_Then_End:                 
                // <Value> ::= IF '(' <Bool Exp> ')' THEN <Bool Exp> END
                break;

            case ProductionIndex.Value_If_Lparen_Rparen_Then_Else_End:                 
                // <Value> ::= IF '(' <Bool Exp> ')' THEN <Bool Exp> ELSE <Bool Exp> END
                break;

            case ProductionIndex.Value_If_Lparen_Rparen:                 
                // <Value> ::= IF '(' <Args> ')'
                break;

            case ProductionIndex.Value_Identifier_Lparen_Rparen:                 
                // <Value> ::= Identifier '(' <Args> ')'
                break;

            case ProductionIndex.Value_Identifier_Lparen_Rparen2:                 
                // <Value> ::= Identifier '(' ')'
                break;

            case ProductionIndex.Value:                 
                // <Value> ::= <RangeReference>
                break;

            case ProductionIndex.Value2:                 
                // <Value> ::= <CellReference>
                break;

            case ProductionIndex.Value3:                 
                // <Value> ::= <GlobalReference>
                break;

            case ProductionIndex.Value_Identifier:                 
                // <Value> ::= Identifier
                break;

            case ProductionIndex.Value_Stringliteral:                 
                // <Value> ::= StringLiteral
                break;

            case ProductionIndex.Value_Lparen_Rparen:                 
                // <Value> ::= '(' <Bool Exp> ')'
                break;

            case ProductionIndex.Globalreference_At_Identifier_Dot_Identifier:                 
                // <GlobalReference> ::= '@' Identifier '.' Identifier
                break;

            case ProductionIndex.Globalreference_At_Identifier:                 
                // <GlobalReference> ::= '@' Identifier
                break;

            case ProductionIndex.Rangereference_Lbracket_Rbracket_Colon_Lbracket_Rbracket:                 
                // <RangeReference> ::= <ColReference> '[' <RowReference> ']' ':' <ColReference> '[' <RowReference> ']'
                break;

            case ProductionIndex.Rangereference_Colon:                 
                // <RangeReference> ::= <ColReference> ':' <ColReference>
                break;

            case ProductionIndex.Rangereference_Dollar_Number_Colon_Dollar_Number:                 
                // <RangeReference> ::= <ColReference> '$' Number ':' <ColReference> '$' Number
                break;

            case ProductionIndex.Rangereference:                 
                // <RangeReference> ::= <SingleColRange>
                break;

            case ProductionIndex.Singlecolrange_Lbracket_Colon_Rbracket:                 
                // <SingleColRange> ::= <ColReference> '[' <RowReference> ':' <RowReference> ']'
                break;

            case ProductionIndex.Cellreference_Lbracket_Rbracket:                 
                // <CellReference> ::= <ColReference> '[' <RowReference> ']'
                break;

            case ProductionIndex.Cellreference_Dollar_Identifier:                 
                // <CellReference> ::= '$' Identifier
                break;

            case ProductionIndex.Assingcolrange_Lbracket_Colon_Rbracket:                 
                // <AssingColRange> ::= <ColReference> '[' <RowReference> ':' <RowReference> ']'
                break;

            case ProductionIndex.Assigncell_Lbracket_Rbracket:                 
                // <AssignCell> ::= <ColReference> '[' <RowReference> ']'
                break;

            case ProductionIndex.Assigncell_Dollar_Identifier:                 
                // <AssignCell> ::= '$' Identifier
                break;

            case ProductionIndex.Colreference_Identifier:                 
                // <ColReference> ::= Identifier
                break;

            case ProductionIndex.Colreference_Dollar_Identifier:                 
                // <ColReference> ::= '$' Identifier
                break;

            case ProductionIndex.Rowreference_Number:                 
                // <RowReference> ::= Number
                break;

            case ProductionIndex.Rowreference_Minus_Number:                 
                // <RowReference> ::= '-' Number
                break;

            case ProductionIndex.Rowreference_Dollar_Number:                 
                // <RowReference> ::= '$' Number
                break;

        }  //switch

        return result;
    }
    
}; //MyParser
