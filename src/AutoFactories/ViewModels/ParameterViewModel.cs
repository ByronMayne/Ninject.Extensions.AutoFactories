using AutoFactories.Types;
using AutoFactories.Visitors;
using CodeCasing;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutoFactories.Models
{
    [DebuggerDisplay("{Name,nq}: {Type.Name}")]
    internal class ParameterViewModel
    {
        public class EqualityComparer : IEqualityComparer<ParameterViewModel>
        {
            bool IEqualityComparer<ParameterViewModel>.Equals(ParameterViewModel x, ParameterViewModel y)
            {
                return string.Equals(x.Name, y.Name, System.StringComparison.Ordinal) &&
                    x.Type.QualifiedName.Equals(y.Type.QualifiedName);
            }

            int IEqualityComparer<ParameterViewModel>.GetHashCode(ParameterViewModel parameter)
            {
                return parameter.GetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets the name of the parameter 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter 
        /// </summary>
        public MetadataTypeName Type { get; set; }

        /// <summary>
        /// Gets or sets if the parameter has to be provided by the invokee 
        /// </summary>
        public bool IsRequired { get; set; }

        public int Position { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }

        public ParameterViewModel()
        {
            Name = "";
        }
        
        /// <summary>
        /// See <see cref="ConstructorDeclarationVisitor.UsesFactoryParamMode"/> for mode explanation.
        /// </summary>
        public static ParameterViewModel Map(ParameterSyntaxVisitor visitor)
        {
            bool isRequired;
            if (visitor.Constructor.UsesFactoryParamMode)
            {
                isRequired = visitor.HasFactoryParamAttribute;
            }
            else
            {
                isRequired = !visitor.HasFromFactoryAttribute;
            }

            return new ParameterViewModel()
            {
                Name = visitor.Name?.ToCamelCase(),
                Type = visitor.Type,
                IsRequired = isRequired
            };
        }
    }
}
