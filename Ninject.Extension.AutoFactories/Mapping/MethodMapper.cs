﻿using Boxed.Mapping;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactory.Models;

namespace Ninject.AutoFactory.Mapping
{
    internal class MethodMapper : IMapper<ConstructorDeclarationSyntax, MethodModel>
    {
        private readonly IMapper<ParameterSyntax, ParameterModel> m_parameterMapper;

        public MethodMapper()
        {
            m_parameterMapper = new ParameterMapper();
        }

        public void Map(ConstructorDeclarationSyntax source, MethodModel destination)
        {
            ParameterSyntax[] parameters = source.ParameterList.Parameters
                .Where(p => !SyntaxHelpers.HasAttribute(p, "FromFactoryAttribute"))
                .ToArray();

            destination.Parameters = m_parameterMapper.MapList(parameters);
        }
    }
}
