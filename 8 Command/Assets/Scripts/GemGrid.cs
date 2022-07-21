using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace GPP
{
    public class GemGrid
    {
        // A class representing a change in the grid
        // i.e. a gem moved, was destroyed, or was created
        public struct Change
        {
            public enum Kind
            {
                Moved, Created, Destroyed
            }

            public readonly Gem Gem;
            public readonly Vector2Int Position;
            public readonly Kind Type;

            public Change(Kind type, Gem gem, Vector2Int position)
            {
                Type = type;
                Gem = gem;
                Position = position;
            }

            // Some helper factory methods to make reading things a little cleaner...
            public static Change Create(Gem gem, int x, int y)
            {
                return new Change(Kind.Created, gem, new Vector2Int(x, y));
            }

            public static Change Destroy(Gem gem, int x, int y)
            {
                return new Change(Kind.Destroyed, gem, new Vector2Int(x, y));
            }

            public static Change Move(Gem gem, int x, int y)
            {
                return new Change(Kind.Moved, gem, new Vector2Int(x, y));
            }

        }

        public Gem[,] Gems { get; private set; }

        public int Width
        {
            get { return Gems.GetLength(0); }
        }

        public int Height
        {
            get { return Gems.GetLength(1); }
        }

        public GemGrid(int width, int height)
        {
            Gems = new Gem[width, height];
        }

        public Gem[,] CopyGems()
        {
            var gems = new Gem[Width, Height];
            Array.Copy(Gems, gems, Gems.Length);
            return gems;
        }

        public IEnumerable<Change> Init()
        {
            var changes = new List<Change>();
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var i = Random.Range(0, Gem.Kinds.Length - 1);
                    var gem = new Gem(Gem.Kinds[i]);
                    Gems[x, y] = gem;
                    changes.Add(Change.Create(gem, x, y));
                }
            }

            return changes;
        }

        public IEnumerable<Change> Set(Gem[,] gems)
        {
            Assert.IsTrue(Gems.GetLength(0) == gems.GetLength(0) && Gems.GetLength(1) == gems.GetLength(1));

            var changes = new List<Change>();

            var width = Gems.GetLength(0);
            var height = Gems.GetLength(1);

            var oldGems = Gems;
            var newGems = gems;

            // go through the two sets of gems in lockstep to determine the differences
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var oldGem = Gems[x, y];
                    var newGem = gems[x, y];

                    if (oldGem != newGem)
                    {
                        // if the "new" one wasn't in the old grid's column then it was created
                        var rowForNew = RowForGemInColumn(newGem, x, oldGems);

                        if (rowForNew == -1)
                        {
                            changes.Add(Change.Create(newGem, x, y));
                        }
                        // Otherwise it moved to this spot
                        else
                        {
                            changes.Add(Change.Move(newGem, x, y));
                        }

                        if (oldGem != Gem.None)
                        {
                            // check to see if the old one has moved
                            var rowForOld = RowForGemInColumn(oldGem, x, newGems);
                            // if the old one is not in the new grid then it was destroyed...
                            if (rowForOld == -1)
                            {
                                changes.Add(Change.Destroy(oldGem, x, y));
                            }
                        }
                    }
                }
            }

            Gems = gems;
            return changes;
        }

        private static int RowForGemInColumn(Gem gem, int column, Gem[,] gems)
        {
            for (var y = 0; y < gems.GetLength(1); y++)
            {
                if (gems[column, y] == gem) return y;
            }
            return -1;
        }

        public IEnumerable<Change> ClearGroupAt(int x, int y)
        {
            var changes = new List<Change>();

            // Get the locations of all gems that are in the group
            // that matches the gem at selected location
            var group = new HashSet<Vector2Int>();

            // Check if its in bounds and is not empty cell
            if (IsInBounds(x, y) && Gems[x, y] != Gem.None)
                FindMatching(Gems[x, y].Type, x, y, group);
            else
                return changes;

            // Clear the cells
            var affectedColumns = new HashSet<int>();
            foreach (var pos in group)
            {
                var gem = Gems[pos.x, pos.y];
                changes.Add(Change.Destroy(gem, pos.x, pos.y));
                Gems[pos.x, pos.y] = Gem.None;
                affectedColumns.Add(pos.x);
            }

            // Move any cells down that are now standing on blank spaces
            foreach (var column in affectedColumns)
            {
                changes.AddRange(CompactColumn(column));
            }

            return changes;
        }

        private IEnumerable<Change> CompactColumn(int x)
        {
            var changes = new List<Change>();

            var top = 0;
            var bottom = 0;

            while (top < Height)
            {
                var gem = Gems[x, top];
                if (gem != Gem.None)
                {
                    if (bottom != top)
                    {
                        Gems[x, bottom] = gem;
                        Gems[x, top] = Gem.None;
                        changes.Add(Change.Move(gem, x, bottom));
                    }
                    bottom++;
                }
                top++;
            }
            return changes;
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private void FindMatching(Gem.Kind type, int x, int y, HashSet<Vector2Int> group)
        {
            // Bail out if we're out of bounds
            if (!IsInBounds(x, y))
                return;

            var gem = Gems[x, y];
            // Bail out if the gem is not of the type we're looking for
            if (gem.Type != type)
                return;

            var pos = new Vector2Int(x, y);
            // Bail out if we've already added this position
            if (group.Contains(pos))
                return;

            // Success! Add this position to the group
            group.Add(pos);

            FindMatching(type, x+1, y, group);
            FindMatching(type, x-1, y, group);
            FindMatching(type, x, y+1, group);
            FindMatching(type, x, y-1, group);
        }
    }
}

