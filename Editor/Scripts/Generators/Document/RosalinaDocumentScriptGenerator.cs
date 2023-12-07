#if UNITY_EDITOR

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal class RosalinaDocumentScriptGenerator : IRosalinaCodeGeneartor
{
    private const string InitializeMethodName = "InitializeDocument";

    public RosalinaGenerationResult Generate(UIDocumentAsset documentAsset)
    {
        if (documentAsset is null)
        {
            throw new ArgumentNullException(nameof(documentAsset), "Cannot generate binding with a null document asset.");
        }

        ClassDeclarationSyntax @class = ClassDeclaration($"{documentAsset.FileSetting.FilePrefix}{documentAsset.Name}{documentAsset.FileSetting.FileSuffix}")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddModifiers(Token(SyntaxKind.PartialKeyword))
            .AddBaseListTypes(
                SimpleBaseType(ParseName(typeof(MonoBehaviour).Name))
            )
            .AddMembers(
                // private void Awake()
                MethodDeclaration(ParseTypeName("void"), "Awake")
                    .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                    .WithBody(
                        Block(
                            ExpressionStatement(
                                InvocationExpression(IdentifierName(InitializeMethodName))
                            )
                        )
                    )
            );

        NamespaceDeclarationSyntax @namespace = NamespaceDeclaration(ParseName(documentAsset.FileSetting.Namespace)).NormalizeWhitespace();

        @namespace = @namespace
            .AddUsings(
                UsingDirective(IdentifierName("UnityEngine"))
            )
            .AddMembers(@class);

        string code = CompilationUnit()
            .AddMembers(@namespace)
            .NormalizeWhitespace()
            .ToFullString();

        return new RosalinaGenerationResult(code, false);
    }
}

#endif