using Ninject;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class GenericFactoryTests : BaseFactoryTest
    {
        public GenericFactoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            // Not need to bind anything 
        }

        [Fact]
        public Task Internal_Constructor_Is_Populated()
            => CaptureAsync(
                notes: ["The constructor is internal but should still generate a factory method"],
                verifySource: ["City.HouseFactory"],
                source: ["""
                    using AutoFactories;
                    using System.Collections.Generic;

                    namespace City
                    {
                        [AutoFactory]
                        public class House
                        {
                            internal House(string address, int? unitNumber)
                            {}
                        }
                    }
                """]);

        [Fact]
        public Task Nullable_Arguments()
            => CaptureAsync(
                notes: ["Create should have the name 'StringComparer'"],
                verifySource: ["World.PersonFactory"],
                source: ["""
                    using AutoFactories;
                    using System.Collections.Generic;

                    namespace World
                    {
               
                        [AutoFactory]
                        public class Person 
                        {
                            public Person(int? age, string? name)
                            {}
                        }
                    }
                """]);



        [Fact]
        public Task Type_With_Name_Of_Attribute_Generates()
            => CaptureAsync(
                notes: ["Create should have the name 'StringComparer'"],
                verifySource: ["World.Factory"],
                source: ["""
                    using AutoFactories;
                    using System.Collections.Generic;

                    namespace World
                    {
                        public partial class Factory 
                        {}

                        [AutoFactory(typeof(Factory), $"{nameof(System.StringComparer)}")]
                        public class Person 
                        {
                            public Person([FromFactory] IEqualityComparer<string?> comparer)
                            {}
                        }
                    }
                    """]);


        [Fact]
        public Task Factory_Method_Name_Using_InvocationExpression_Produces_Expected()
         => CaptureAsync(
             notes: ["The Factory should have a method called `Person`"],
             verifySource: ["World.Factory"],
             source: ["""
                using AutoFactories;
                using System.Collections.Generic;

                namespace World
                {
                    public partial class Factory 
                    {}

                    [AutoFactory(typeof(Factory), nameof(Person))]
                    public class Person 
                    {
                        public Person([FromFactory] IEqualityComparer<string?> comparer)
                        {}
                    }
                }
                """]);


        [Fact]
        public Task Factory_Method_Name_Using_InterpolatedString_Produces_Expected()
         => CaptureAsync(
             notes: ["The Factory should have a method called `Person`"],
             verifySource: ["World.Factory"],
             source: ["""
                using AutoFactories;
                using System.Collections.Generic;

                namespace World
                {
                    public partial class Factory 
                    {}

                    [AutoFactory(typeof(Factory), $"{nameof(Person)}")]
                    public class Person 
                    {
                        public Person([FromFactory] IEqualityComparer<string?> comparer)
                        {}
                    }
                }
                """]);


        [Fact]
        public Task Factory_With_NullableStructParameter_EmitsValidSignature()
            => CaptureAsync(
                notes: ["Nullable<T> for a struct (e.g. `Id?`) must not emit `Nullable<T>?` (CS0453)."],
                verifySource: ["Sample.WidgetSelectFactory"],
                source: ["""
                    using AutoFactories;

                    namespace Sample
                    {
                        public struct Id { public int Value; }

                        [AutoFactory]
                        public class WidgetSelect
                        {
                            public WidgetSelect(Id? preselectedValue) { }
                        }
                    }
                """]);

        [Fact]
        public Task Static_Constructor_Is_Ignored()
            => CaptureAsync(
                notes: ["Static constructor should be ignored and only instance constructor should generate factory method"],
                verifySource: ["Widgets.WidgetFactory"],
                source: ["""
                    using AutoFactories;

                    namespace Widgets
                    {
                        [AutoFactory]
                        public class Widget
                        {
                            static Widget()
                            {
                            }

                            public Widget(string name)
                            {
                            }
                        }
                    }
                """]);

        [Fact]
        public Task Shared_Factory_Merges_Duplicate_Type_FromFactory_Parameters()
            => CaptureAsync(
                notes: ["Two classes sharing a factory with the same [FromFactory] type should emit a single merged field"],
                verifySource: ["World.Factory"],
                source: ["""
                    using AutoFactories;
                    using System.Collections.Generic;

                    namespace World
                    {
                        public partial class Factory
                        {}

                        [AutoFactory(typeof(Factory))]
                        public class Person
                        {
                            public Person(string name, [FromFactory] IEqualityComparer<string> comparer)
                            {}
                        }

                        [AutoFactory(typeof(Factory))]
                        public class Robot
                        {
                            public Robot(int id, [FromFactory] IEqualityComparer<string> comparer)
                            {}
                        }
                    }
                    """]);

        [Fact]
        public Task Shared_Factory_Gives_Unique_Names_To_Conflicting_FromFactory_Parameters()
            => CaptureAsync(
                notes: ["Two classes sharing a factory with same-named but different-typed [FromFactory] parameters should get unique fields"],
                verifySource: ["World.Factory"],
                source: ["""
                    using AutoFactories;
                    using System.Collections.Generic;

                    namespace World
                    {
                        public partial class Factory
                        {}

                        [AutoFactory(typeof(Factory))]
                        public class Person
                        {
                            public Person(string name, [FromFactory] IEqualityComparer<string> service)
                            {}
                        }

                        [AutoFactory(typeof(Factory))]
                        public class Robot
                        {
                            public Robot(int id, [FromFactory] IComparer<string> service)
                            {}
                        }
                    }
                    """]);
    }
}
