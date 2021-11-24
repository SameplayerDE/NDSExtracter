namespace ModelExporter.NintendoSystemBinaries.Data
{
    public struct NintendoSystemBinaryModelData
    {
        public string Name;
        public string FoundIn;
        public int NumberOfPieces;

        public NintendoSystemBinaryModelData(string name, string foundIn, int numberOfPieces)
        {
            Name = name;
            FoundIn = foundIn;
            NumberOfPieces = numberOfPieces;
        }
    }
}