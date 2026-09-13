using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Piece : MonoBehaviour
{
   [SerializeField] AudioSource rotateSound;
    [SerializeField] AudioSource moveSound;
    [SerializeField] AudioSource fallSound;
    
    public Board board {get; private set;}
    public TetrominoData data {get; private set;}
    public Vector3Int position {get; private set;}
    public Vector3Int[] cells {get; private set;}
    public int rotationIndex {get; private set;}

    public float stepDelay = 0.7f;
    public float lockDelay = 0.5f;
    float stepTime;
    float lockTime;
    public void Intialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        if(this.cells == null)
        {
            this.cells = new Vector3Int [data.cells.Length];
        }

        for(int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {

        this.board.Clear(this);

        this.lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!rotateSound.isPlaying)
            {
                rotateSound.Play();
            }
            Rotate();            
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!moveSound.isPlaying)
            {
                moveSound.Play();
            }
            Move(Vector2Int.left);
        }


       else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!moveSound.isPlaying)
            {
                moveSound.Play();
            }
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!moveSound.isPlaying)
            {
                moveSound.Play();
            }
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!fallSound.isPlaying)
            {
                fallSound.Play();
            }
            HardDrop();
        }

        if(Time.time >= this.stepTime)
        {
            Step();
        }

        this.board.Set(this);
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);

        if(this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    private int Wrap(int input, int min, int max)
    {
        if(input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

private void Rotate()
{
    int originalRotation = this.rotationIndex;
    this.rotationIndex = Wrap(this.rotationIndex + 1, 0, 4);

    ApplyRotationMatrix();

    if (!TestWallKicks(this.rotationIndex, 1))
    {
        this.rotationIndex = originalRotation;
        ApplyRotationMatrix(); // rotating back "undoes" the matrix
    }
}

private void ApplyRotationMatrix()
{
    for (int i = 0; i < this.cells.Length; i++)
    {
        Vector3 cell = this.cells[i];

        int x, y;

        switch (this.data.tetromino)
        {
            case Tetromino.I:
            case Tetromino.O:
                cell.x -= 0.5f;
                cell.y -= 0.5f;
                x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * 1) + (cell.y * Data.RotationMatrix[1] * 1));
                y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * 1) + (cell.y * Data.RotationMatrix[3] * 1));
                break;

            default:
                x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * 1) + (cell.y * Data.RotationMatrix[1] * 1));
                y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * 1) + (cell.y * Data.RotationMatrix[3] * 1));
                break;
        }

        this.cells[i] = new Vector3Int(x, y, 0);
    }
}

private bool TestWallKicks(int rotationIndex, int rotationDirection)
{
    Vector2Int[,] wallKicks = Data.WallKicks[this.data.tetromino];
    int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

    for (int i = 0; i < wallKicks.GetLength(1); i++)
    {
        Vector2Int translation = wallKicks[wallKickIndex, i];

        if (Move(translation))
        {
            return true;
        }
    }

    return false;
}

private int GetWallKickIndex(int rotationIndex, int rotationDirection)
{
    Vector2Int[,] wallKicks = Data.WallKicks[this.data.tetromino];
    int wallKickIndex = rotationIndex * 2;

    if (rotationDirection < 0)
    {
        wallKickIndex--;
    }

    return Wrap(wallKickIndex, 0, wallKicks.GetLength(0));
}


    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.position = newPosition;
            this.lockTime = 0f;
        }

        

        return valid;
    }

    public void ClearLines()
    {
        
    }
}
