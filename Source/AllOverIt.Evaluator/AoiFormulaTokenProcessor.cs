using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Stack;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    internal sealed class AoiFormulaTokenProcessor : IAoiFormulaTokenProcessor
    {
        private readonly IAoiFormulaProcessor _formulaProcessor;
        private readonly IList<AoiFormulaTokenProcessorContext> _tokenProcessors;
        private readonly IAoiFormulaReader _formulaReader;
        private readonly IAoiFormulaExpressionFactory _expressionFactory;
        private readonly IAoiArithmeticOperationFactory _operationFactory;

        public AoiFormulaTokenProcessor(IAoiFormulaProcessor formulaProcessor, IAoiFormulaReader formulaReader,
            IAoiFormulaExpressionFactory formulaExpressionFactory, IAoiArithmeticOperationFactory operationFactory)
            : this(new List<AoiFormulaTokenProcessorContext>(), formulaProcessor, formulaReader, formulaExpressionFactory, operationFactory)
        {
        }

        internal AoiFormulaTokenProcessor(IList<AoiFormulaTokenProcessorContext> tokenProcessors, IAoiFormulaProcessor formulaProcessor,
            IAoiFormulaReader formulaReader, IAoiFormulaExpressionFactory expressionFactory, IAoiArithmeticOperationFactory operationFactory)
        {
            _tokenProcessors = tokenProcessors.WhenNotNull(nameof(tokenProcessors));
            _formulaProcessor = formulaProcessor.WhenNotNull(nameof(formulaProcessor));
            _formulaReader = formulaReader.WhenNotNull(nameof(formulaReader));
            _expressionFactory = expressionFactory.WhenNotNull(nameof(expressionFactory));
            _operationFactory = operationFactory.WhenNotNull(nameof(operationFactory));

            RegisterTokenProcessors();
        }

        public void ProcessOperators(IAoiStack<string> operators, IAoiStack<Expression> expressions, Func<bool> condition)
        {
            _ = operators.WhenNotNull(nameof(operators));
            _ = expressions.WhenNotNull(nameof(expressions));
            _ = condition.WhenNotNull(nameof(condition));

            while (operators.Any() && condition.Invoke())
            {
                var nextOperator = operators.Pop();
                var operation = _operationFactory.GetOperation(nextOperator);
                var expression = _expressionFactory.CreateExpression(operation, expressions);

                expressions.Push(expression);
            }
        }

        public bool ProcessToken(char token, bool isUserMethod)
        {
            var processor = _tokenProcessors
              .SkipWhile(p => !p.Predicate.Invoke(token, isUserMethod))
              .First();     // process the first match found

            // returns true to indicate processing should continue
            return processor.Processor.Invoke(token, isUserMethod);
        }

        private void RegisterTokenProcessors()
        {
            // start of a new scope
            Register(
              (token, isUserDefined) => token == '(',
              (token, isUserDefined) =>
              {
                  _formulaReader.ReadNext();   // consume the '('
                  _formulaProcessor.ProcessScopeStart();
                  return true;
              });

            // end of a scope
            Register(
              (token, isUserDefined) => token == ')',
              (token, isUserDefined) =>
              {
                  _formulaReader.ReadNext();   // consume the ')'
                  return _formulaProcessor.ProcessScopeEnd(isUserDefined);
              });

            // arguments of a method
            Register(
              (token, isUserDefined) => isUserDefined && token == ',',
              (token, isUserDefined) =>
              {
                  _formulaReader.ReadNext();
                  _formulaProcessor.ProcessMethodArgument();
                  return true;
              });

            // an operator
            Register(
              (token, isUserDefined) => _operationFactory.IsCandidate(token),
              (token, isUserDefined) =>
              {
                  _formulaProcessor.ProcessOperator();
                  return true;
              });

            // numerical constant
            Register(
              (token, isUserDefined) => AoiFormulaReader.IsNumericalCandidate(token),
              (token, isUserDefined) =>
              {
                  _formulaProcessor.ProcessNumerical();
                  return true;
              });

            // everything else - variables and methods
            Register(
              (token, isUserDefined) => true,
              (token, isUserDefined) =>
              {
                  _formulaProcessor.ProcessNamedOperand();
                  return true;
              });
        }

        // The predicate is used to determine if the associated processor will be invoked. The input arguments of the predicate include the
        // next token to be read and a flag to indicate if the token is within the context of a user defined method. The processor 
        // is invoked if the predicate returns true.
        // The input arguments of the processor include the next token to be read and a flag to indicate if the token is within the context
        // of a user defined method. The processor returns true to indicate processing is to continue or false to indicate processing of the
        // current scope is complete (such as reading arguments of a user defined method).
        private void Register(Func<char, bool, bool> predicate, Func<char, bool, bool> processor)
        {
            _tokenProcessors.Add(new AoiFormulaTokenProcessorContext(predicate, processor));
        }
    }
}