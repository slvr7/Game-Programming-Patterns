using System;

#pragma warning disable 660,661

namespace GPP
{
    public struct Gem
    {
        private static ulong _nextId;

        public static Gem None = new Gem(Kind.None);

        public enum Kind : byte
        {
            Red,
            Green,
            Blue,
            Yellow,
            None
        }

        public static readonly Kind[] Kinds = (Kind[])Enum.GetValues(typeof(Kind));

        public static bool operator ==(Gem left, Gem right)
        {
            return left.Id == right.Id;
        }

        public static bool operator !=(Gem left, Gem right)
        {
            return left.Id != right.Id;
        }

        public readonly Kind Type;
        public readonly ulong Id;

        public Gem(Kind type)
        {
            Type = type;
            Id = _nextId++;
        }

    }
}
