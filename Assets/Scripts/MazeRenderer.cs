using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{

    [SerializeField]
    [Range(1, 50)]
    public int width = 10;

    [SerializeField]
    [Range(1, 50)]
    public int height = 10;

    [SerializeField]
    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform goalPrefab = null;

    public List<GameObject> goals = new List<GameObject>();

    public WallState[,] maze;
    private GameObject[,,] walls;


    public void GenerateMaze(Transform parent)
    {
        walls = new GameObject[width, height, 4];
        maze = MazeGenerator.Generate(width, height);
        Draw(maze, parent);
    }

    public void ReDrawMaze()
    {
        maze = MazeGenerator.Generate(width, height);
        ReDraw(maze);
    }

    public void RemoveGoal(GameObject goal)
    {
        goal.SetActive(false);
    }

    public void ResetGoals()
    {
        foreach(GameObject goal in goals) 
        {
            goal.SetActive(true);
        }
    }

    private void Draw(WallState[,] maze, Transform parent)
    {
        for (int i = 0; i < width; ++i)
        {

            for (int j = 0; j < height; ++j)
            {
                var position = new Vector3(-width / 2 + i, -height / 2 + j);

                if (!((i == width / 2) && (j == height / 2)))
                {
                    var goal = Instantiate(goalPrefab, parent);
                    goal.localPosition = new Vector3(i - width / 2, j - height / 2);
                    goals.Add(goal.gameObject);
                }

                var topWall = Instantiate(wallPrefab, parent);
                topWall.localPosition = position + new Vector3(0, size / 2);
                walls[i, j, 0] = topWall.gameObject;

                var leftWall = Instantiate(wallPrefab, parent) as Transform;
                leftWall.localPosition = position + new Vector3(-size / 2, 0);
                leftWall.eulerAngles = new Vector3(0, 0, 90);
                walls[i, j, 1] = leftWall.gameObject;

                var rightWall = Instantiate(wallPrefab, parent) as Transform;
                rightWall.localPosition = position + new Vector3(size / 2, 0);
                rightWall.eulerAngles = new Vector3(0, 0, 90);
                walls[i, j, 2] = rightWall.gameObject;

                var bottomWall = Instantiate(wallPrefab, parent) as Transform;
                bottomWall.localPosition = position + new Vector3(0, -size / 2);
                walls[i, j, 3] = bottomWall.gameObject;

            }
        }        
    }

    private void ReDraw(WallState[,] maze)
    {
        ResetWalls();

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                for (int k = 0; k < 4; k++)
                {

                    var cell = maze[i, j];
                    var position = new Vector3(-width / 2 + i, -height / 2 + j);

                    var wall = walls[i, j, k].transform;

                    if (cell.HasFlag(WallState.UP))
                    {
                        var localPosition = position + new Vector3(0, size / 2);
                        if (wall.localPosition.Equals(localPosition))
                        {
                            wall.gameObject.SetActive(true);
                        }
                    }

                    if (cell.HasFlag(WallState.LEFT))
                    {
                        var localPosition = position + new Vector3(-size / 2, 0);
                        var eulerAngles = new Vector3(0, 0, 90);
                        if (wall.localPosition.Equals(localPosition) && wall.eulerAngles.Equals(eulerAngles))
                        {
                            wall.gameObject.SetActive(true);
                        }
                    }

                    if (i == width - 1)
                    {
                        if (cell.HasFlag(WallState.RIGHT))
                        {
                            var localPosition = position + new Vector3(size / 2, 0);
                            var eulerAngles = new Vector3(0, 0, 90);
                            if (wall.localPosition.Equals(localPosition) && wall.eulerAngles.Equals(eulerAngles))
                            {
                                wall.gameObject.SetActive(true);
                            }
                        }
                    }

                    if (j == 0)
                    {
                        if (cell.HasFlag(WallState.DOWN))
                        {
                            var localPosition = position + new Vector3(0, -size / 2);
                            if (wall.localPosition.Equals(localPosition))
                            {
                                wall.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }


    private void ResetWalls()
    {
        foreach (GameObject goal in goals)
        {
            goal.SetActive(true);
        }
        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
        }
    }
}