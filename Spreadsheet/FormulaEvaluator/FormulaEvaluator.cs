using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// This class is an Infix expression evaluator. This class uses stacks to evaluate basic math calcualtions.
    /// </summary>
    public static class Evaluator
    {
        // TODO: Follow the PS1 instructions

        public delegate int Lookup(String v);

        /// <summary>
        /// This method takes in a string which contains a math expression 
        /// and a look up function for any variables that could be present, and returns the solution.
        /// The method uses stacks to calculate the infix math expresstions.
        /// </summary>
        /// <param name="exp">String of the given expression to calculate</param>
        /// <param name="variableEvaluator">Function that looks up each variable value and assigns it it's numerical value</param>
        /// <returns>Integer value of the calculated expression</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            Stack<char> OperatorStack = new Stack<char>();
            Stack<int> ValueStack = new Stack<int>();


            //split string into an array using regex.split
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");


            //for each loop to extract each value

            foreach (String s in substrings)
            {
                //for tokens with leading white space.
                String input = s.Trim();
                //check for leading and empty chars.
                if (s == "" || s == " ")
                {
                    continue;
                }
               

                if (int.TryParse(s, out int number))
                {

                    
                    int val2;
                    bool StackEmpty = false;

                    if (ValueStack.Count == 0)
                    {
                        StackEmpty = true;
                    }

                    if (OperatorStack.IsOnTop('*'))
                    {
                        if (StackEmpty)
                        {
                            throw new ArgumentException("Format is incorrect");
                        }

                        val2 = ValueStack.Pop();
                        OperatorStack.Pop();

                        ValueStack.Push(number * val2);


                    }
                    else if (OperatorStack.IsOnTop('/'))
                    {
                        if (StackEmpty)
                        {
                            throw new ArgumentException("Format is incorrect");
                        }
                        else if (number == 0)
                        {
                            throw new ArgumentException("cannot divide by zero");
                        }

                        val2 = ValueStack.Pop();
                        OperatorStack.Pop();

                        ValueStack.Push(val2 / number);
                    }
                    else
                    {
                        ValueStack.Push(number);
                    }


                }
                //variable lookup
                else if (IsVariable(input))
                {
                    //try catch used to see if variable is valid token or not
                    try
                    {
                        int val = variableEvaluator(s);
                        int val2;
                        bool StackEmpty = false;

                        if (ValueStack.Count == 0)
                        {
                            StackEmpty = true;
                        }

                        if (OperatorStack.IsOnTop('*'))
                        {
                            if (StackEmpty)
                            {
                                throw new ArgumentException("Format is incorrect");
                            }

                            val2 = ValueStack.Pop();
                            OperatorStack.Pop();

                            ValueStack.Push(val * val2);


                        }
                        else if (OperatorStack.IsOnTop('/'))
                        {
                            if (StackEmpty)
                            {
                                throw new ArgumentException("Format is incorrect");
                            }
                            else if (val == 0)
                            {
                                throw new ArgumentException("cannot divide by zero");
                            }

                            val2 = ValueStack.Pop();

                            ValueStack.Push(val2 / val);
                        }
                        else
                        {
                            ValueStack.Push(val);
                        }

                    }
                    catch
                    {
                        throw new ArgumentException("could not find value of variable");
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

                            if (!OperatorStack.IsOnTop('('))
                            {
                                throw new ArgumentException("format is incorrect. No '(' was found.");
                            }
                            OperatorStack.Pop();

                            if (OperatorStack.IsOnTop('*'))
                            {
                                ValueStack.Push(Opperation('*', OperatorStack, ValueStack));
                            }
                            else if (OperatorStack.IsOnTop('/'))
                            {
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
                            throw new ArgumentException("token is not valid");
                    }
                }

            }

            int ResultValue = 0;
            if (OperatorStack.Count == 0)
            {
                if(ValueStack.Count == 0)
                {
                    throw new ArgumentException("empty stack");
                }
                ResultValue = ValueStack.Pop();
            }
            else if (OperatorStack.IsOnTop('+'))
            {
                if (ValueStack.Count <= 1 || ValueStack.Count > 2)
                {
                    throw new ArgumentException("stack error");
                }
                ResultValue = Opperation('+', OperatorStack, ValueStack);
            }
            else if (OperatorStack.IsOnTop('-'))
            {
                if (ValueStack.Count <= 1 || ValueStack.Count > 2)
                {
                    throw new ArgumentException("stack error");
                }

                ResultValue = Opperation('-', OperatorStack, ValueStack);
            }

            return ResultValue;


        }
        /// <summary>
        /// Helper method that does the math opperations with the correct opperator.
        /// </summary>
        /// <param name="a">The opperator that appears at the top of the stack</param>
        /// <param name="OpStack">The operator stack used in the program</param>
        /// <param name="ValStack">The value stack that is used in the program</param>
        /// <returns></returns>
        public static int Opperation(char a, Stack<char> OpStack, Stack<int> ValStack)
        {
            int result = 0;
            int y = ValStack.Pop();
            int x = ValStack.Pop();
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
                    if(y == 0)
                    {
                        throw new ArgumentException("cannot divide by zero");
                    }
                    result = x / y;
                    break;
            }


            return result;
        }

        /// <summary>
        /// helper method that determines if the string is a variable token or not.
        /// </summary>
        /// <param name="s"> String that needs to be checked for validity</param>
        /// <returns>Boolean that determines if the token is valid or not</returns>
        public static bool IsVariable(String s)
        {
            // initially checks if sting meets var req letter followed by number
            if (char.IsLetter(s, 0))
            {
                int count = 0;
                //keeps cycling through string
                while (char.IsLetter(s, count) && count < s.Length - 1)
                {
                    count++;
                }
                if (char.IsNumber(s, count))
                {
                    //rethink count goes out of bounds, possible fix?
                    //check length first
                    while (char.IsNumber(s, count) && count < s.Length - 1)
                    {
                        count++;
                    }
                }
                //  if(char.IsNumber(s, ))
                return count == s.Length - 1 && char.IsNumber(s, count);
            }
            else
            {
                return false;
            }

        }



    }

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
}
