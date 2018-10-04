using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    wait, move, win, lose, pause
}

public enum TileKind {
    Breakable,
    Blank,
    Lock,
    Concrete,
    Slime,
    Normal
}

[System.Serializable]
public class MatchType {
    public int type;
    public string color;
}

[System.Serializable]
public class TileType {
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour {

    [Header("Scriptable Object Stuff")]
    public World world;
    public int level;

    public GameState currentState = GameState.move;

    [Header("Board Dimensions")]
    public int width;
    public int height;
    public int offSet;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject breakablePrefab;
    public GameObject lockPrefab;
    public GameObject concretePrefab;
    public GameObject slimePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;

    [Header("Layout")]
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private Breakable[,] breakableTiles;
    public Breakable[,] lockTiles;
    public Breakable[,] concreteTiles;
    private Breakable[,] slimeTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;

    [Header("Match Stuff")]
    public MatchType matchType;
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;
    public bool makeSlime = true;

    private void Awake() {
        if (PlayerPrefs.HasKey("Current Level")) {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null) {
            if (level < world.levels.Length) {
                if (world.levels[level] != null) {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }

    private void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        findMatches = FindObjectOfType<FindMatches>();
        goalManager = FindObjectOfType<GoalManager>();
        allDots = new GameObject[width, height];
        blankSpaces = new bool[width, height];
        breakableTiles = new Breakable[width, height];
        lockTiles = new Breakable[width, height];
        slimeTiles = new Breakable[width, height];
        concreteTiles = new Breakable[width, height];
        scoreManager = FindObjectOfType<ScoreManager>();

        SetUp();

        currentState = GameState.pause;
    }

    public void GenerateBlankSpaces() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.Blank) {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakables() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.Breakable) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakablePrefab, tempPosition, Quaternion.identity, 
                    transform);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<Breakable>();
            }
        }
    }

    private void GenerateLockTiles() {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Lock)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockPrefab, tempPosition, Quaternion.identity,
                    transform);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<Breakable>();
            }
        }
    }

    private void GenerateConcreteTiles() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.Concrete) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concretePrefab, tempPosition, Quaternion.identity,
                    transform);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<Breakable>();
            }
        }
    }

    private void GenerateSlimeTiles() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.Slime) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(slimePrefab, tempPosition, Quaternion.identity,
                    transform);
                slimeTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<Breakable>();
            }
        }
    }

    private void SetUp() {
        GenerateBlankSpaces();
        GenerateBreakables();
        GenerateLockTiles();
        GenerateConcreteTiles();
        GenerateSlimeTiles();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j]) {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity,
                        transform);
                    backgroundTile.name = "( " + i + ", " + j + ")";

                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100) {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity, transform);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;

                    dot.name = "( " + i + ", " + j + ")";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece) {
        if (column > 1 && row > 1) {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null) {
                if (allDots[column - 1, row].tag == piece.tag 
                        && allDots[column - 2, row].tag == piece.tag) {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null) {
                if (allDots[column, row - 1].tag == piece.tag 
                        && allDots[column, row - 2].tag == piece.tag) {
                    return true;
                }
            }
        } else if (column <= 1 || row <= 1) {
            if (row > 1) {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null) {
                    if (allDots[column, row - 1].tag == piece.tag 
                            && allDots[column, row - 2].tag == piece.tag) {
                        return true;
                    }
                }
            }
            if (column > 1) {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null) {
                    if (allDots[column - 1, row].tag == piece.tag
                            && allDots[column - 2, row].tag == piece.tag) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private MatchType ColumnOrRow() {
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;

        matchType.type = 0;
        matchType.color = "";

        for (int i = 0; i < matchCopy.Count; i++) {
            Dot thisDot = matchCopy[i].GetComponent<Dot>();

            string color = matchCopy[i].tag;
            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchCopy.Count; j++) {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if (nextDot == thisDot) {
                    continue;
                }

                if (nextDot.column == thisDot.column && nextDot.tag == color) {
                    columnMatch++;
                }

                if (nextDot.row == thisDot.row && nextDot.tag == color) {
                    rowMatch++;
                }
            }

            if (columnMatch == 4 || rowMatch == 4) {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            } else if (columnMatch == 2 && rowMatch == 2) {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            } else if (columnMatch == 3 || rowMatch == 3) {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }

        matchType.type = 0;
        matchType.color = "";
        return matchType;
    }

    private void CheckToMakeBombs() {
        if (findMatches.currentMatches.Count > 3) {
            MatchType typeOfMatch = ColumnOrRow();

            if (typeOfMatch.type == 1) {
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color) {
                    currentDot.isMatched = false;
                    currentDot.MakeColorBomb();
                } else {
                    if (currentDot.otherDot != null) {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color) {
                            otherDot.isMatched = false;
                            otherDot.MakeColorBomb();
                        }
                    }
                }
            } else if (typeOfMatch.type == 2) {
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color) {
                    currentDot.isMatched = false;
                    currentDot.MakeAdjacentBomb();
                } else {
                    if (currentDot.otherDot != null) {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color) {
                            otherDot.isMatched = false;
                            otherDot.MakeAdjacentBomb();
                        }
                    }
                }
            } else if (typeOfMatch.type == 3) {
                findMatches.CheckBombs(typeOfMatch);
            }
        }
    }

    public void BombRow(int row) {
        for (int i = 0; i < width; i++) {
            if (concreteTiles[i, row]) {
                concreteTiles[i, row].TakeDamage(1);
                if (concreteTiles[i, row].hitPoints <= 0) {
                    concreteTiles[i, row] = null;
                }
            }
        }
    }

    public void BombColumn(int column) {
        for (int i = 0; i < width; i++) {
            if (concreteTiles[column, i]) {
                concreteTiles[column, i].TakeDamage(1);
                if (concreteTiles[column, i].hitPoints <= 0) {
                    concreteTiles[column, i] = null;
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row) {
        if (allDots[column, row].GetComponent<Dot>().isMatched) {            
            if (breakableTiles[column, row] != null) {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0) {
                    breakableTiles[column, row] = null;
                }
            }

            if (lockTiles[column, row] != null) {
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0) {
                    lockTiles[column, row] = null;
                }
            }

            DamageConcrete(column, row);
            DamageSlime(column, row);

            if (goalManager != null) {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }

            if (soundManager != null) {
                soundManager.PLayRandomDestroyNoise();
            }
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, 
                Quaternion.identity);

            Destroy(particle, .5f);
            //allDots[column, row].GetComponent<Dot>().PopAnimation();
            Destroy(allDots[column, row], .5f);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    private void DamageConcrete(int column, int row) {
        if (column > 0) {
            if (concreteTiles[column - 1, row]) {
                concreteTiles[column - 1, row].TakeDamage(1);
                if (concreteTiles[column - 1, row].hitPoints <= 0) {
                    concreteTiles[column - 1, row] = null;
                }
            }
        }

        if (column < width - 1) {
            if (concreteTiles[column + 1, row]) {
                concreteTiles[column + 1, row].TakeDamage(1);
                if (concreteTiles[column + 1, row].hitPoints <= 0) {
                    concreteTiles[column + 1, row] = null;
                }
            }
        }

        if (row > 0) {
            if (concreteTiles[column, row - 1]) {
                concreteTiles[column, row - 1].TakeDamage(1);
                if (concreteTiles[column, row - 1].hitPoints <= 0) {
                    concreteTiles[column, row - 1] = null;
                }
            }
        }

        if (row < height - 1) {
            if (concreteTiles[column, row + 1]) {
                concreteTiles[column, row + 1].TakeDamage(1);
                if (concreteTiles[column, row + 1].hitPoints <= 0) {
                    concreteTiles[column, row + 1] = null;
                }
            }
        }
    }

    private void DamageSlime(int column, int row) {
        if (column > 0) {
            if (slimeTiles[column - 1, row]) {
                slimeTiles[column - 1, row].TakeDamage(1);
                if (slimeTiles[column - 1, row].hitPoints <= 0) {
                    slimeTiles[column - 1, row] = null;
                }
                makeSlime = false;
            }
        }

        if (column < width - 1) {
            if (slimeTiles[column + 1, row]) {
                slimeTiles[column + 1, row].TakeDamage(1);
                if (slimeTiles[column + 1, row].hitPoints <= 0) {
                    slimeTiles[column + 1, row] = null;
                }
                makeSlime = false;
            }
        }

        if (row > 0) {
            if (slimeTiles[column, row - 1]) {
                slimeTiles[column, row - 1].TakeDamage(1);
                if (slimeTiles[column, row - 1].hitPoints <= 0) {
                    slimeTiles[column, row - 1] = null;
                }
                makeSlime = false;
            }
        }

        if (row < height - 1) {
            if (slimeTiles[column, row + 1]) {
                slimeTiles[column, row + 1].TakeDamage(1);
                if (slimeTiles[column, row + 1].hitPoints <= 0) {
                    slimeTiles[column, row + 1] = null;
                }
                makeSlime = false;
            }
        }
    }

    public void DestroyMatches() {
        if (findMatches.currentMatches.Count >= 4) {
            CheckToMakeBombs();
        }

        findMatches.currentMatches.Clear();

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (!blankSpaces[i, j] && allDots[i ,j] == null && !concreteTiles[i, j] &&
                        !slimeTiles[i, j]) {
                    for (int k = j + 1; k < height; k++) {
                        if (allDots[i, k] != null) {
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !concreteTiles[i, j] &&
                        !slimeTiles[i, j]) {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100) {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;

                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity,
                        transform);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard() {
        findMatches.FindAllMatches();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    if (allDots[i, j].GetComponent<Dot>().isMatched) {
                        return true;
                    }
                }
            }
        }        
        return false;
    }

    private IEnumerator FillBoardCo() {
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard()) {
            streakValue++;
            DestroyMatches();
            yield break;
        }
        currentDot = null;

        CheckToMakeSlime();

        if (IsDeadLocked()) {
            StartCoroutine(ShuffleBoardCo());
        }

        yield return new WaitForSeconds(refillDelay);
        System.GC.Collect();
        if (currentState != GameState.pause) {
            currentState = GameState.move;
        }
        makeSlime = true;
        streakValue = 1;
    }

    private void CheckToMakeSlime() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (slimeTiles[i, j] != null && makeSlime) {
                    MakeNewSlime();
                }
            }
        }
    }

    private Vector2 CheckForAdjacent(int column, int row) {
        if (allDots[column + 1, row] && column < width - 1) {
            return Vector2.right;
        }
        if (allDots[column - 1, row] && column > 0) {
            return Vector2.left;
        }
        if (allDots[column, row + 1] && row < height - 1) {
            return Vector2.up;
        }
        if (allDots[column, row - 1] && row > 0) {
            return Vector2.down;
        }
        return Vector2.zero;
    }

    private void MakeNewSlime() {
        bool slime = false;
        int loops = 0;
        while (!slime && loops < 200) {
            int newX = Random.Range(0, width - 1);
            int newY = Random.Range(0, height - 1);
            if (slimeTiles[newX, newY] != null) {
                Vector2 adjacent = CheckForAdjacent(newX, newY);
                if (adjacent != Vector2.zero) {
                    int _x = newX + (int)adjacent.x;
                    int _y = newY + (int)adjacent.y;

                    Destroy(allDots[_x, _y]);
                    Vector2 tempPosition = new Vector2(_x, _y);
                    GameObject tile = Instantiate(slimePrefab, tempPosition, Quaternion.identity, transform);
                    slimeTiles[_x, _y] = tile.GetComponent<Breakable>();
                    slime = true;
                }
            }
            loops++;
        }
    }

    private void SwitchPieces(int column, int row, Vector2 direction) {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null) {
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y];
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            allDots[column, row] = holder;
        }
    }

    private bool CheckForMatches() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    if (i < width - 2) {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null) {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag
                                    && allDots[i + 2, j].tag == allDots[i, j].tag) {
                                return true;
                            }
                        }
                    }

                    if (j < height - 2) {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null) {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                                    && allDots[i, j + 2].tag == allDots[i, j].tag) {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction) {
        SwitchPieces(column, row, direction);
        if (CheckForMatches()) {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    if (i < width - 1) {
                        if (SwitchAndCheck(i, j, Vector2.right)) {
                            return false;
                        }
                    }
                    if (j < height - 1) {
                        if (SwitchAndCheck(i, j, Vector2.up)) {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoardCo() {
        yield return new WaitForSeconds(0.5f);

        List<GameObject> newBoard = new List<GameObject>();

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j]) {
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100) {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }

        if (IsDeadLocked()) {
            StartCoroutine(ShuffleBoardCo());
        }
    }

}