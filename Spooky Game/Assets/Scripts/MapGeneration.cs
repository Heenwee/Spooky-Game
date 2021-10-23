using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public int loops;
    public float sideSectionLength, upSectionLength;
    public int branchLenghtMin, branchLenghtMax;
    Transform[] points;
    int lastDir; // 1 = left, 2 = right, 3 = up

    // Start is called before the first frame update
    void Start()
    {
        GameObject pointObj = new GameObject("point");
        points = new Transform[loops];
        for(int i = 0; i < loops; i++)
        {
            var p = Instantiate(pointObj, transform.position, Quaternion.identity);
            points[i] = p.transform;
        }
        lastDir = 0;

        points[1].position = new Vector2(0, 0);
        StartCoroutine(GenerateMain());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Generate(Transform[] p, int lenght)
    {
        for (int i = 1; i < lenght; i++)
        {
            int dir = Random.Range(1, 4);

            if ((dir == 1 && lastDir != 2) ||
                (dir == 2 && lastDir != 1) ||
                dir == 3)
            {
                Vector3 change = Vector3.zero;
                if (dir == 1)
                {
                    change = Vector2.left * sideSectionLength;
                }
                if (dir == 2)
                {
                    change = Vector2.right * sideSectionLength;
                }
                if (dir == 3)
                {
                    change = Vector2.up * upSectionLength;
                }
                p[i].position = p[i - 1].position + change;

                lastDir = dir;
            }
            else i--;
        }
        LineRenderer mainLine = new GameObject().AddComponent<LineRenderer>();
        mainLine.positionCount = lenght;
        for (int j = 0; j < mainLine.positionCount; j++)
        {
            mainLine.SetPosition(j, p[j].position);
        }
    }
    IEnumerator GenerateMain()
    {
        Generate(points, loops);
        yield return new WaitForSeconds(1);
        GenerateBranches();
    }
    void GenerateBranches()
    {
        for (int j = 0; j < 5; j++)
        {
            int branchLenght = Random.Range(branchLenghtMin, branchLenghtMax + 1);

            GameObject pointObj = new GameObject("branchPoint");
            Transform[] bPoints = new Transform[branchLenght];
            for (int i = 1; i < branchLenght; i++)
            {
                var p = Instantiate(pointObj, transform.position, Quaternion.identity);
                bPoints[i] = p.transform;
            }
            bPoints[0] = points[Random.Range(0, points.Length)];

            Generate(bPoints, branchLenght);
        }
    }
}
