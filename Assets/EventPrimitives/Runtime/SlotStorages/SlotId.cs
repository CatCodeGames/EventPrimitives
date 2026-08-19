namespace CatCode.Collections
{
    public readonly struct SlotId
    {
        public readonly int Index;
        public readonly int Generation;

        public SlotId(int index, int generation)
        {
            Index = index;
            Generation = generation;
        }
    }
}