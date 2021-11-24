namespace ModelExporter
{
    public class Animation : NintendoSystemBinary
    {
        public AnimationData Data;
        
        public Animation(string path) : base(path)
        {
            
        }
    }
    
    public struct AnimationData
    {
        public string Name;
        public string FoundIn;
        public int Frames;
        public int NumberOfObjects;
    }
}