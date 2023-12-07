#if UNITY_EDITOR

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;
using UnityEngine.UIElements;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal class RosalinaDocumentBindingsGenerator : IRosalinaCodeGeneartor
{
    private const string DocumentFieldName = "document";
    private const string RootVisualElementPropertyName = "Root";
    private const string InitializeDocumentMethodName = "InitializeDocument";

    public RosalinaGenerationResult Generate(UIDocumentAsset document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document), "Cannot generate binding with a null document asset.");
        }

        InitializationStatement[] statements = RosalinaStatementSyntaxFactory.GenerateInitializeStatements(document, $"{RootVisualElementPropertyName}?.Q");
        PropertyDeclarationSyntax[] propertyStatements = statements.Select(x => x.Property).ToArray();
        StatementSyntax[] initializationStatements = statements.Select(x => x.Statement).ToArray();

        MethodDeclarationSyntax initializeMethod = MethodDeclaration(ParseTypeName("void"), InitializeDocumentMethodName)
            .AddModifiers(
                Token(SyntaxKind.PublicKeyword)
            )
            .WithBody(
                Block(initializationStatements)
            );

        ClassDeclarationSyntax @class = ClassDeclaration($"{document.Name}Component")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddModifiers(Token(SyntaxKind.PartialKeyword))
            .AddMembers(
                CreateDocumentVariable()
            )
            .AddMembers(propertyStatements)
            .AddMembers(
                CreateVisualElementRootProperty(),
                initializeMethod
            );

        NamespaceDeclarationSyntax @namespace = NamespaceDeclaration(ParseName("Crowdoka.Crowdungeon.Runtime.Components")).NormalizeWhitespace();

        @namespace = @namespace
            .AddUsings(
                UsingDirective(IdentifierName("UnityEngine")),
                UsingDirective(IdentifierName("UnityEngine.UIElements"))
            )
            .AddMembers(@class);

        string code = CompilationUnit()
            .AddMembers(@namespace)
            .NormalizeWhitespace()
            .ToFullString();

        return new RosalinaGenerationResult(code);
    }

    private static MemberDeclarationSyntax CreateDocumentVariable()
    {
        NameSyntax serializeFieldName = ParseName(typeof(SerializeField).Name);

        return FieldDeclaration(
                VariableDeclaration(
                        ParseName(typeof(UIDocument).Name)
                    )
                    .AddVariables(
                        VariableDeclarator(DocumentFieldName)
                    )
            )
            .AddModifiers(Token(SyntaxKind.PrivateKeyword));
    }

    private static MemberDeclarationSyntax CreateVisualElementRootProperty()
    {
        return PropertyDeclaration(
                IdentifierName(typeof(VisualElement).Name), RootVisualElementPropertyName
            )
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithExpressionBody(
                ArrowExpressionClause(
                    IdentifierName($"{DocumentFieldName}?.rootVisualElement")
                )
            )
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}

#endif