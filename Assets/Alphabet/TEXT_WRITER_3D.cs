using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEXT_WRITER_3D : MonoBehaviour
{
    [System.Serializable]
    public struct Letter
    {
        public GameObject obj;

        public float size;
    }

    [System.Serializable]
    public struct GameText
    {
        public List<string> ThisText;
        public Vector3 pos;
    }

    public Letter[] Alphabet;
    public GameText[] TEXT;

    public float space, Line_Height;

    void Start()
    {
        for (int i = 0; i < TEXT.Length; i++)
        {
            Write(TEXT[i].ThisText, TEXT[i].pos);
        }
    }

    public void Write(List<string> input, Vector3 pos)
    {
        List<float> lengths = new List<float>();
        List<Transform> lines = new List<Transform>(); 

        GameObject text = new GameObject("3D_TEXT");

        float MaxLength = 0;

        for (int i = 0; i < input.Count; i++)
        {
            GameObject line = new GameObject("Line" + (i + 1).ToString());

            float endPoint = 0;

            for (int j = 0; j < input[i].Length; j++)
            {
                if (input[i][j] != ' ')
                {
                    int letterID = (int)input[i][j] - 97;

                    GameObject letter = Instantiate(Alphabet[letterID].obj, new Vector3(endPoint, 0, 0), Quaternion.identity);

                    letter.transform.SetParent(line.transform);

                    endPoint += Alphabet[letterID].size + space;
                }
                else
                {
                    endPoint += space + space;
                }
            }
            endPoint -= space;

            line.transform.position = new Vector3(0, (input.Count - i - 1) * Line_Height, 0);

            lengths.Add(endPoint);

            if (MaxLength < endPoint) 
            {
                MaxLength = endPoint;
            }

            line.transform.SetParent(text.transform);
            lines.Add(line.transform);
        }

        for (int i = 0; i < lines.Count; i++)
        {
            float delta = MaxLength - lengths[i];

            lines[i].transform.localPosition += new Vector3(delta / 2f, 0, 0);
        }

        text.transform.position = pos;
    }
}
