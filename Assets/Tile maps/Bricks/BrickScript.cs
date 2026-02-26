using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Rule Tile", menuName = "Tiles/Custom Rule Tile")]
public class BrickScript : RuleTile<BrickScript.Neighbor>
{
    public bool customField;
    public TileBase Skinny;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Null = 3;
        public const int NotNull = 4;
        public const int SkinnyTile = 5;
        public const int SkinnyOrThis = 6;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.SkinnyOrThis: return tile == Skinny || tile == this;
            case Neighbor.This: return tile == this;
            case Neighbor.NotThis: return tile != this;
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
            case Neighbor.SkinnyTile: return tile == Skinny;

        }
        return base.RuleMatch(neighbor, tile);
    }
}