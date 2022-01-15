﻿using Assets.Editor.Scripts.Generator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

internal static class RosalinaGenerator
{
    private static readonly string GeneratedCodeHeader = @$"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rosalina Code Generator tool.
//     Version: {RosalinaConstants.Version}
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

";

    private const string DocumentFieldName = "_document";
    private const string DocumentRootVisualElementFieldName = "rootVisualElement";
    private const string RootVisualElementPropertyName = "Root";
    private const string RootVisualElementQueryMethodName = "Q";
    private const string InitializeDocumentMethodName = "InitializeDocument";
    private static readonly UsingDirectiveSyntax[] DefaultUsings = new UsingDirectiveSyntax[]
    {
        SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("UnityEngine")),
        SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("UnityEngine.UIElements"))
    };

    /// <summary>
    /// Generates the UI document code behind.
    /// </summary>
    /// <param name="uiDocumentPath">UI Document path.</param>
    public static void Generate(UIDocumentAsset document)
    {
        // Parse document
        UxmlNode uiDocumentRootNode = RosalinaUXMLParser.ParseUIDocument(document.FullPath);
        IEnumerable<UIPropertyDescriptor> namedNodes = uiDocumentRootNode.Children
            .FlattenTree(x => x.Children)
            .Where(x => x.HasName)
            .Select(x => new UIPropertyDescriptor(x.Type, x.Name))
            .ToList();

        MemberDeclarationSyntax documentVariable = CreateDocumentVariable();
        MemberDeclarationSyntax visualElementProperty = CreateVisualElementRootProperty();

        InitializationStatement[] statements = GenerateInitializeStatements(namedNodes);
        FieldDeclarationSyntax[] privateFieldsStatements = statements.Select(x => x.PrivateField).ToArray();
        StatementSyntax[] initializationStatements = statements.Select(x => x.Statement).ToArray();

        MethodDeclarationSyntax initializeMethod = RosalinaSyntaxFactory.CreateMethod("void", InitializeDocumentMethodName, SyntaxKind.PublicKeyword)
            .WithBody(SyntaxFactory.Block(initializationStatements));

        MemberDeclarationSyntax[] classMembers = new[] { documentVariable }
            .Concat(privateFieldsStatements)
            .Append(visualElementProperty)
            .Append(initializeMethod)
            .ToArray();

        ClassDeclarationSyntax @class = SyntaxFactory.ClassDeclaration(document.Name)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseName(typeof(MonoBehaviour).Name))
            )
            .AddMembers(classMembers);

        CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit()
            .AddUsings(DefaultUsings)
            .AddMembers(@class);

        string code = compilationUnit
            .NormalizeWhitespace()
            .ToFullString();
        string generatedCode = GeneratedCodeHeader + code;

        File.WriteAllText(document.GeneratedFileOutputPath, generatedCode);
    }

    private static MemberDeclarationSyntax CreateDocumentVariable()
    {
        string documentPropertyTypeName = typeof(UIDocument).Name;
        NameSyntax serializeFieldName = SyntaxFactory.ParseName(typeof(SerializeField).Name);

        FieldDeclarationSyntax documentField = RosalinaSyntaxFactory.CreateField(documentPropertyTypeName, DocumentFieldName, SyntaxKind.PrivateKeyword)
            .AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(serializeFieldName)
                    )
                )
            );

        return documentField;
    }

    private static MemberDeclarationSyntax CreateVisualElementRootProperty()
    {
        string propertyTypeName = typeof(VisualElement).Name;
        string documentFieldName = $"{DocumentFieldName}?";

        return RosalinaSyntaxFactory.CreateProperty(propertyTypeName, RootVisualElementPropertyName, SyntaxKind.PublicKeyword)
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(
                    SyntaxKind.GetAccessorDeclaration,
                    SyntaxFactory.Block(
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.Token(SyntaxKind.ReturnKeyword),
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName(documentFieldName),
                                SyntaxFactory.IdentifierName(DocumentRootVisualElementFieldName)),
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                        )
                    )
                )
            );
    }

    private static MemberAccessExpressionSyntax CreateRootQueryMethodAccessor()
    {
        return SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName($"{RootVisualElementPropertyName}?"),
            SyntaxFactory.Token(SyntaxKind.DotToken),
            SyntaxFactory.IdentifierName(RootVisualElementQueryMethodName)
        );
    }

    private static InitializationStatement[] GenerateInitializeStatements(IEnumerable<UIPropertyDescriptor> properties)
    {
        var documentQueryMethodAccess = CreateRootQueryMethodAccessor();
        var statements = new List<InitializationStatement>();

        foreach (var property in properties)
        {
            string fieldName = property.PrivateName;
            Type uiPropertyType = UIPropertyTypes.GetUIElementType(property.Type);

            if (uiPropertyType is null)
            {
                Debug.LogWarning($"[Rosalina]: Failed to get property type: '{property.Type}' field: '{property.Name}'. Property will be ignored.");
                continue;
            }

            FieldDeclarationSyntax field = RosalinaSyntaxFactory.CreateField(uiPropertyType.Name, fieldName, SyntaxKind.PrivateKeyword);

            var argumentList = SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(property.Name)
                    )
                )
            });
            var cast = SyntaxFactory.CastExpression(
                SyntaxFactory.ParseTypeName(uiPropertyType.Name),
                SyntaxFactory.InvocationExpression(documentQueryMethodAccess, SyntaxFactory.ArgumentList(argumentList))
            );
            var statement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(fieldName),
                    cast
                )
            );

            statements.Add(new InitializationStatement(statement, field));
        }

        return statements.ToArray();
    }

    private struct InitializationStatement
    {
        public StatementSyntax Statement { get; }

        public FieldDeclarationSyntax PrivateField { get; }

        public InitializationStatement(StatementSyntax statement, FieldDeclarationSyntax privateField)
        {
            Statement = statement;
            PrivateField = privateField;
        }
    }
}
