// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<String> equation;
        private Func<string, string> norm;
        private Func<string, bool> valid;
        private HashSet<String> variables;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {



        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            equation = new List<String>();
            equation = GetTokens(formula).ToList();
            norm = normalize;
            valid = isValid;
            variables = new HashSet<String>();

            if (!ValidFormula(equation, out String message))
            {
                throw new FormulaFormatException(message);
            }

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            //TODO: Bring in PS1 Evaluate
            Stack<char> OperatorStack = new Stack<char>();
            Stack<double> ValueStack = new Stack<double>();


            //for each loop to extract each value

            foreach (String s in equation)
            {

                if (Double.TryParse(s, out double number))
                {
                    ValueStack.Push(number);

                    if (OperatorStack.IsOnTop('*'))
                    {

                        ValueStack.Push(Opperation('*', OperatorStack, ValueStack));

                        // formula error used in evaluate method never in the constructor 
                        //in constructor only in ffexception invalid vars in const

                    }
                    else if (OperatorStack.IsOnTop('/'))
                    {

                        if (ValueStack.Peek() == 0)
                        {
                            return new FormulaError("cannot divide by zero");
                        }

                        ValueStack.Push(Opperation('/', OperatorStack, ValueStack));
                    }

                }
                //variable lookup
                else if (IsValidVariable(norm(s)) && valid(norm(s)))
                {
                    //try catch used to see if variable is valid token or not
                    try
                    {
 
                        if(ReferenceEquals(lookup(s), null))
                        {
                            return new FormulaError("variable does not have a valid value");
                        }

                        ValueStack.Push(lookup(s));


                        if (OperatorStack.IsOnTop('*'))
                        {
                            ValueStack.Push(Opperation('*', OperatorStack, ValueStack));
                        }
                        else if (OperatorStack.IsOnTop('/'))
                        {
                            if (ValueStack.Peek() == 0)
                            {
                                return new FormulaError("cannot divide by zero");
                            }

                            ValueStack.Push(Opperation('/', OperatorStack, ValueStack));
                        }

                    }
                    catch
                    {
                        //This is okay
                        return new FormulaError();
                    }

                }
                else
                {

                    //Switch takes string and determines which token it is
                    switch (s)
                    {
                        case "(":
                            OperatorStack.Push('(');
                            break;

                        case ")":
                            // while loop can be removed
                            if (OperatorStack.IsOnTop('*'))
                            {
                                ValueStack.Push(Opperation('*', OperatorStack, ValueStack));
                            }
                            else if (OperatorStack.IsOnTop('/'))
                            {
                                if (ValueStack.Peek() == 0)
                                {
                                    return new FormulaError("cannot divide by zero");
                                }
                                ValueStack.Push(Opperation('/', OperatorStack, ValueStack));
                            }
                            else if (OperatorStack.IsOnTop('+'))
                            {
                                ValueStack.Push(Opperation('+', OperatorStack, ValueStack));
                            }

                            else if (OperatorStack.IsOnTop('-'))
                            {
                                ValueStack.Push(Opperation('-', OperatorStack, ValueStack));
                            }

                            OperatorStack.Pop();

                            if (OperatorStack.IsOnTop('*'))
                            {
                                ValueStack.Push(Opperation('*', OperatorStack, ValueStack));
                            }
                            else if (OperatorStack.IsOnTop('/'))
                            {
                                if (ValueStack.Peek() == 0)
                                {
                                    return new FormulaError("cannot divide by zero");
                                }
                                ValueStack.Push(Opperation('/', OperatorStack, ValueStack));
                            }

                            break;

                        case "+":
                            if (OperatorStack.IsOnTop('+'))
                            {
                                ValueStack.Push(Opperation('+', OperatorStack, ValueStack));

                            }
                            else if (OperatorStack.IsOnTop('-'))
                            {
                                ValueStack.Push(Opperation('-', OperatorStack, ValueStack));

                            }
                            OperatorStack.Push('+');
                            break;

                        case "-":
                            if (OperatorStack.IsOnTop('+'))
                            {
                                ValueStack.Push(Opperation('+', OperatorStack, ValueStack));

                            }
                            else if (OperatorStack.IsOnTop('-'))
                            {
                                ValueStack.Push(Opperation('-', OperatorStack, ValueStack));
                            }
                            OperatorStack.Push('-');
                            break;

                        case "*":
                            OperatorStack.Push('*');
                            break;

                        case "/":
                            OperatorStack.Push('/');
                            break;

                        default:
                            return new FormulaError("token is not valid");
                    }
                }

            }

            double ResultValue = 0;
            if (OperatorStack.Count == 0)
            {
                //if (ValueStack.Count == 0)
                //{
                //    //TODO: fix this in the constructor
                //    throw new ArgumentException("empty stack");
                //}
                ResultValue = ValueStack.Pop();
            }
            else if (OperatorStack.IsOnTop('+'))
            {
                //if (ValueStack.Count <= 1 || ValueStack.Count > 2)
                //{
                //    //TODO: these should fix in constructor
                //    throw new ArgumentException("stack error");
                //}
                ResultValue = Opperation('+', OperatorStack, ValueStack);
            }
            else if (OperatorStack.IsOnTop('-'))
            {
                //if (ValueStack.Count <= 1 || ValueStack.Count > 2)
                //{
                //    //TODO: fix in constructor
                //    throw new ArgumentException("stack error");
                //}

                ResultValue = Opperation('-', OperatorStack, ValueStack);
            }

            return ResultValue;

        }

        /// <summary>
        /// helper method that determines if formula is valid
        /// </summary>
        private bool ValidFormula(List<String> eq, out String message)
        {
            message = "";
            
            //one length rule
            if(eq.Count == 0)
            {
                message = "Invalid Formula. Must be at least one token long.";
                return false;
            }
            // first token rule
            if(!(Double.TryParse(eq[0], out double x)) && !(char.Equals(eq[0], "(")) && !(IsValidVariable(eq[0])))
            {
                message = "Invalid Formula, expression must start with number variable or opening parethesis";
                return false;
            }

            //final token rule
            if (!(Double.TryParse(eq[eq.Count - 1], out double z)) && !(char.Equals(eq[eq.Count - 1], ")")) && !(IsValidVariable(eq[eq.Count - 1])))
            {
                message = "Invalid Formula, expression must end with number variable or closing parethesis";
                return false;
            }

            int count1 = 0;
            int count2 = 0;
            String prev = "";
            for (int i = 0; i < eq.Count; i++)
            {
                String s = eq[i];
                s = norm(s);
                if(IsValidVariable(s) && valid(s))
                {
                    if (!variables.Contains(s))
                    {
                        variables.Add(s);
                    }
                }
                else if(!valid(s) && IsValidVariable(s))
                {
                    throw new FormulaFormatException("Invalid Variable");
                }
                // check that each opening '(' has a closing ')'
                if (s.Equals("("))
                {
                    count1++;
                }
                if (s.Equals(")"))
                {
                    count2++;
                }

                //following rule
                if ( prev.Equals("(") || prev.Equals("*") || prev.Equals("/") || prev.Equals("+") || prev.Equals("-"))
                {
                    if(!(Double.TryParse(s, out double y)) && !(s.Equals("(")) && !(IsValidVariable(s)))
                    {
                        message = "invalid expression format";
                        return false;
                    }
                }

                //extra following rule
                if ((Double.TryParse(prev, out double a)) || (prev.Equals(")")) || (IsValidVariable(prev)))
                {
                    if (!(s.Equals(")")) && !(s.Equals("*")) && !(s.Equals("/")) && !(s.Equals("+")) && !(s.Equals("-")))
                    {
                        message = "invalid expression format";
                        return false;
                    }
                }
                prev = s;
            }
            // parenthesis rule
            if(count1 != count2)
            {
                message = "error each opening parenthesis needs a complimenting closing parenthesis";
                return false;
            }
            //every option exhausted
            return true;
        }

        /// <summary>
        /// helper method that checks to make sure that variable is valid.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsValidVariable(String s)
        {
            if(s == "")
            {
                return false;
            }

            if(char.IsLetter(s,0) || s.StartsWith("_"))
            {
                // variable already passed first requirement
 
                foreach (char c in s)
                {
                    //checks every character to make sure it follows the rules
                    if (!(c.Equals('_')) && !(char.IsLetter(c)) && !(char.IsDigit(c)))
                    {
                        //violation is detected
                        return false;
                    }
                }
                // no violation detected returns true.
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method that does the math opperations with the correct opperator.
        /// </summary>
        /// <param name="a">The opperator that appears at the top of the stack</param>
        /// <param name="OpStack">The operator stack used in the program</param>
        /// <param name="ValStack">The value stack that is used in the program</param>
        /// <returns></returns>
        private static double Opperation(char a, Stack<char> OpStack, Stack<double> ValStack)
        {
            double result = 0;
            double y = ValStack.Pop();
            double x = ValStack.Pop();
            OpStack.Pop();

            switch (a)
            {
                case '+':
                    result = x + y;
                    break;

                case '-':
                    result = x - y;
                    break;

                case '*':
                    result = x * y;
                    break;

                case '/':
                    //no check needed as it is handled in the evaluate method.
                    result = x / y;
                    break;
            }


            return result;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }

        public List<String> GetEquationTokens()
        {
            return equation;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
 
            String FormulaString = "";

            foreach(String token in equation)
            {
                if(Double.TryParse(token, out double number))
                {
                    FormulaString = FormulaString + number.ToString();
                }
                else
                {
                    FormulaString = FormulaString + norm(token);
                }
            }

            return FormulaString;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            //TODO: for loop that checks each token equality
            if(!(obj is Formula))
            {
                return false;
            }
            
            String[] ObjList = (obj as Formula).GetEquationTokens().ToArray();
            String[] ThisList = equation.ToArray();
            if (ObjList.Length == ThisList.Length)
            {
                for(int i = 0; i < equation.Count; i++)
                {
                    String s1 = ObjList[i];
                    String s2 = ThisList[i];
                    //case where both are a number
                    if(Double.TryParse(s1, out double n1) && Double.TryParse(s2, out double n2))
                    {
                        //are they the same string?
                        if (!(n1.ToString().Equals(n2.ToString())))
                        {
                            // case that the values are not the same in string form
                            return false;
                        }

                    }
                    if (!Double.TryParse(s1, out double x) && !Double.TryParse(s2, out double y))
                    {


                        //case must be that they are both strings
                        if (!(norm(s1).Equals(norm(s2))))
                        {
                            //case that string 1 returned from norm is not the same as string 2
                            return false;
                        }
                    }
                }
                // if each token cannot be proven by counterexample then it must be true.
                return true;
            }
            //case that the arrays are different lengths therefore different equations
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            //TODO:
            if(ReferenceEquals(null, f1))
            {
                return ReferenceEquals(null, f2);
            }

            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if(f1 == f2)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }
    /// <summary>
    /// used to extend the use of stack
    /// </summary>
    public static class EvaluatorExtension
    {
        /// <summary>
        /// this method extends the stack class and provides a way to determine if a specific item is on the top of the stack.
        /// </summary>
        /// <param name="s">Stack being used to check its contents</param>
        /// <param name="c">Character that needs to be found</param>
        /// <returns></returns>
        public static bool IsOnTop(this Stack<char> s, char c)
        {
            if (s.Count == 0)
            {
                return false;
            }
            return s.Peek() == c;
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

