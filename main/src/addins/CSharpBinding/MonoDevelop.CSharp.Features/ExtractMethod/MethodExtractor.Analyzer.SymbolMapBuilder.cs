﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.LanguageServices;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ICSharpCode.NRefactory6.CSharp.ExtractMethod
{
    abstract partial class MethodExtractor
    {
        protected abstract partial class Analyzer
        {
            private class SymbolMapBuilder : SyntaxWalker
            {
                private readonly SemanticModel _semanticModel;
                //private readonly ISyntaxFactsService _service;
                private readonly TextSpan _span;
                private readonly Dictionary<ISymbol, List<SyntaxToken>> _symbolMap;
                private readonly CancellationToken _cancellationToken;

                public static Dictionary<ISymbol, List<SyntaxToken>> Build(
                  //  ISyntaxFactsService service,
                    SemanticModel semanticModel,
                    SyntaxNode root,
                    TextSpan span,
                    CancellationToken cancellationToken)
                {
                    //Contract.ThrowIfNull(semanticModel);
//                    Contract.ThrowIfNull(service);
                    //Contract.ThrowIfNull(root);

                    var builder = new SymbolMapBuilder(/*service, */semanticModel, span, cancellationToken);
                    builder.Visit(root);

                    return builder._symbolMap;
                }

                private SymbolMapBuilder(
                //    ISyntaxFactsService service,
                    SemanticModel semanticModel,
                    TextSpan span,
                    CancellationToken cancellationToken)
                    : base(SyntaxWalkerDepth.Token)
                {
                    _semanticModel = semanticModel;
                //    _service = service;
                    _span = span;
                    _symbolMap = new Dictionary<ISymbol, List<SyntaxToken>>();
                    _cancellationToken = cancellationToken;
                }

                protected override void VisitToken(SyntaxToken token)
                {
                    if (token.IsMissing ||
                        token.Width() <= 0 ||
						!token.IsIdentifier() ||
                        !_span.Contains(token.Span) ||
						token.Parent.IsNamedParameter())
                    {
                        return;
                    }

                    var symbolInfo = _semanticModel.GetSymbolInfo(token, _cancellationToken);
                    foreach (var sym in symbolInfo.GetAllSymbols())
                    {
                        // add binding result to map
                        var list = _symbolMap.GetOrAdd(sym, _ => new List<SyntaxToken>());
                        list.Add(token);
                    }
                }
            }
        }
    }
}
