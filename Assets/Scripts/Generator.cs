using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject chunk;
    public GameObject water;

    public float GlobalSeed;

    public Transform referencePoints;

    public Dictionary<string, GameObject> chuncks;

    public Dictionary<string, List<string>> Modifications;
    public Dictionary<string, List<Vector4>> Buildings;

    public int ChunckRange;

    public int xSize, zSize;

    int[] triangles;

    int LastX = 1, LastZ = 1;

    bool test;

    public Player Manager;

    void Start()
    {
        Time.timeScale = 1;

        GlobalSeed = Random.Range(0, 99999);

        chuncks = new Dictionary<string, GameObject>();
        Modifications = new Dictionary<string, List<string>>();
        Buildings = new Dictionary<string, List<Vector4>>();
        test = false;

        triangles = new int[6 * xSize * zSize];

        int v = 0, t = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[t] = v;
                triangles[t + 1] = v + xSize + 1;
                triangles[t + 2] = v + 1;

                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + xSize + 1;
                triangles[t + 5] = v + xSize + 2;

                v++; t += 6;
            }
            v++;
        }
    }

    void Update()
    {
        float x = referencePoints.position.x, z = referencePoints.position.z;

        int xPosT = Mathf.FloorToInt(x / 100);
        int zPosT = Mathf.FloorToInt(z / 100);

        if ((xPosT != LastX || zPosT != LastZ) || test)
        {
            for (int i = -ChunckRange; i <= ChunckRange; i++)
            {
                int zPos = zPosT + ChunckRange;
                int xPos = i + xPosT;
                string key = xPos.ToString() + "_" + zPos.ToString();
                if (chuncks.ContainsKey(key))
                {
                    DestroyImmediate(chuncks[key].GetComponent<MeshFilter>().sharedMesh);
                    Destroy(chuncks[key].gameObject);
                    chuncks.Remove(key);
                }

                zPos = -2 + zPosT;
                key = xPos.ToString() + "_" + zPos.ToString();
                if (chuncks.ContainsKey(key))
                {
                    DestroyImmediate(chuncks[key].GetComponent<MeshFilter>().sharedMesh);
                    Destroy(chuncks[key].gameObject);
                    chuncks.Remove(key);
                }

                if(i>=-2)
                {
                    xPos = xPosT - ChunckRange;
                    zPos = i + zPosT;

                    key = xPos.ToString() + "_" + zPos.ToString();
                    if (chuncks.ContainsKey(key))
                    {
                        DestroyImmediate(chuncks[key].GetComponent<MeshFilter>().sharedMesh);
                        Destroy(chuncks[key].gameObject);
                        chuncks.Remove(key);
                    }

                    xPos = xPosT + ChunckRange;
                    key = xPos.ToString() + "_" + zPos.ToString();
                    if (chuncks.ContainsKey(key))
                    {
                        DestroyImmediate(chuncks[key].GetComponent<MeshFilter>().sharedMesh);
                        Destroy(chuncks[key].gameObject);
                        chuncks.Remove(key);
                    }
                }
            }

            for (int xp = -ChunckRange+1; xp < ChunckRange; xp++)
            {
                for (int zp = -1; zp < ChunckRange; zp++) 
                {
                    int xPos = xp + xPosT, zPos = zp + zPosT;

                    string key = xPos.ToString() + "_" + zPos.ToString();

                    if (!chuncks.ContainsKey(key))
                    {
                        GameObject ch = Instantiate(chunk, new Vector3(xPos * 100, 0, zPos * 100), Quaternion.identity);
                        chuncks.Add(key, ch);
                        Chunck c = ch.GetComponent<Chunck>();
                        c.home = this;
                        c.key = key;

                        if (Modifications.ContainsKey(key))
                        {
                            c.Modified = true;
                        }
                        else
                        {
                            c.Modified = false;
                        }

                        c.StartChunk();
                        c.seedX = GlobalSeed + xPos * xSize; c.seedZ = GlobalSeed + zPos * zSize;
                        c.GenerateChunk();
                        c.chunk.triangles = triangles;
                        c.chunk.RecalculateNormals();
                        test = true;
                        return;
                    }
                }
            }
        }

        LastX = xPosT; LastZ = zPosT;
        test = false;
    }
}
