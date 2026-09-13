using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPiece : MonoBehaviour
{

    public Tile tile;
    public Board board;
    public Piece trackingPiece;
    public Tilemap tilemap {get; private set;}
    public Vector3Int[] cells {get; private set;}
     public Vector3Int position {get; private set;}

    void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }

    void Drop()
    {
        Vector3Int position = this.trackingPiece.position;
        int current = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Clear(this.trackingPiece);

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            if(this.board.IsValidPosition(this.trackingPiece, position))
            {
                this.position = position;
            }
            else
            {
                break;
            }
        }

        this.board.Set(this.trackingPiece);
    }

    void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }
}
