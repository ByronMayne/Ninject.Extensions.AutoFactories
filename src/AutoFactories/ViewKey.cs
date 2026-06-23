using System.IO;

namespace AutoFactories
{
    public readonly partial struct ViewKey
    {
        public static ViewKey Factory = new ViewKey("FactoryView");
        public static ViewKey FactoryInterface = new ViewKey("FactoryInterfaceView");
        public static ViewKey ClassAttribute = new ViewKey("ClassAttribute");
        public static ViewKey ParameterAttribute = new ViewKey("ParameterAttribute");


        public readonly string Value;

        private ViewKey(string value)
        {
            Value = value;
        }

        public static ViewKey From(string input)
            => new ViewKey(NormalizeInput(input));

        private static string NormalizeInput(string input)
            => Path.GetFileNameWithoutExtension(input);
            
    }
}
