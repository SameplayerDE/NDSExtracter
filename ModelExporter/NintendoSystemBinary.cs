namespace ModelExporter
{
    public enum NintendoSystemBinaryType
    {
        ModelData,
        Texture,
        Animation,
        PatternAnimation,
        MaterialAnimation
    }
    
    public class NintendoSystemBinary
    {
        public string Path;
        public string Name;
        public NintendoSystemBinaryType Type;

        public NintendoSystemBinary(string path)
        {
            Path = path;
        }
        
    }
}