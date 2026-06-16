using System;
using System.IO;
using Vogen;

namespace AutoFactories
{
    [Instance("Factory", "FactoryView")]
    [Instance("FactoryInterface", "FactoryInterfaceView")]
    [Instance("ClassAttribute", "ClassAttribute")]
    [Instance("FromFactoryAttribute", "FromFactoryAttribute")]
    [Instance("FactoryParamAttribute", "FactoryParamAttribute")]
    [ValueObject<string>(conversions: Conversions.None)]
    public readonly partial struct ViewKey
    {
        private static string NormalizeInput(string input)
            => Path.GetFileNameWithoutExtension(input);
            
    }
}
