using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    //====co potrzebuje zmieniaæ w TileManager
    //pos_details
    //danger_rooms
    //path to door
    //safe_rooms

    //====co potrzebuje temp odzcytaæ co jest na czerwono
    //current
    //width
    //height
    int Mwidth;
    int Mheight;
    Vector2Int Mcurrent;

    public void ReadValues(int width, int height, Vector2Int current) { 
        Mwidth = width;
        Mheight = height;
        Mcurrent = current;
    }

    public void Drunkard(int danger_rooms, List<Vector2Int> pos_details, int[,] pos) //danger_room, pos_detail,pos

    {
        int n = UnityEngine.Random.Range(4, 7); //iloœæ pokoi
        danger_rooms = n + 1;
        Vector2Int new_current = Mcurrent;
        pos_details.Add(new Vector2Int(new_current.x, new_current.y));
        for (int i = 0; i < n; i++)
        {
            int attempts = 0; //limit dla jednego przypadku bycia optoczonym
            bool not_possible = true;
            while (not_possible && attempts < 10)
            {
                attempts++;
                Vector2Int side = direction(new_current.x, new_current.y);
                //dla x - estetyczne rozdzielenie
                if (side.x >= 0 && side.x < Mwidth)
                {
                    //dla y + czy na obecnym polu jest 1
                    if (side.y >= 0 && side.y < Mheight && pos[side.x, side.y] != 1)
                    {
                        pos[side.x, side.y] = 1;
                        new_current = side;
                        pos_details.Add(new Vector2Int(side.x, side.y));
                        not_possible = false;
                    }
                }
            }
        }
    }
    // funkcja na koñcowy pokój wyjsciowy - pomocy robilam to solidna 1h po 23, my brain is melting 
    public void The_longest_path(int path_to_door, int[,] pos ,int x, int y) //pos, path_to_door
    {
        int[,] count = new int[Mwidth, Mheight];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(x, y));
        count[x, y] = 1;

        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
        };

        //BFS - algorytm na znajdowanie najdaleszej wartoœci od pocz¹tkowego obiektu
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (next.x >= 0 && next.x < Mwidth &&
                    next.y >= 0 && next.y < Mheight)
                {
                    if (pos[next.x, next.y] == 1 && count[next.x, next.y] == 0)
                    {
                        count[next.x, next.y] = count[current.x, current.y] + 1;
                        queue.Enqueue(next);
                    }
                }
            }
        }

        int max = 0;
        int maxX = x;
        int maxY = y;

        for (int a = 0; a < Mwidth; a++)
        {
            for (int b = 0; b < Mheight; b++)
            {
                if (count[a, b] > max)
                {
                    max = count[a, b];
                    maxX = a;
                    maxY = b;
                }
            }
        }
        path_to_door = max;
        Vector2Int new_current = new Vector2Int(maxX, maxY);

        //Debug.Log("COUNT");
        for (int a = 0; a < Mwidth; a++)
        {
            string line = "";
            for (int b = 0; b < Mheight; b++)
            {
                line += count[a, b].ToString() + " ";
            }
            line += "[" + a + "]";
            //Debug.Log(line);
        }

        bool placed = false;
        int attempts = 0;

        while (!placed && attempts < 20)
        {
            attempts++;

            Vector2Int side = direction(new_current.x, new_current.y);

            if (side.x >= 0 && side.x < Mwidth &&
                side.y >= 0 && side.y < Mheight &&
                pos[side.x, side.y] != 1)
            {
                pos[side.x, side.y] = 2; // place door
                placed = true;
            }
        }
    }

    public void bezpieczne_pokoje(int safe_rooms, List<Vector2Int> pos_details, int[,] pos) //safe_rooms, pod_details, pos
    {
        int n = UnityEngine.Random.Range(2, 4); //iloœæ pokoi
        safe_rooms = n;
        List<Vector2Int> pos_posibilities = new List<Vector2Int>();
        foreach (Vector2Int posi in pos_details)
        {
            if (posi.x + 1 < Mwidth && pos[posi.x + 1, posi.y] == 0)
            { //prawy s¹siad
                pos_posibilities.Add(new Vector2Int(posi.x + 1, posi.y));
            }
            if (posi.x - 1 > 0 && pos[posi.x - 1, posi.y] == 0)
            { //lewy s¹siad
                pos_posibilities.Add(new Vector2Int(posi.x - 1, posi.y));
            }
            if (posi.y + 1 < Mheight && pos[posi.x, posi.y + 1] == 0)
            { //górny s¹siad
                pos_posibilities.Add(new Vector2Int(posi.x, posi.y + 1));
            }
            if (posi.y - 1 > 0 && pos[posi.x, posi.y - 1] == 0)
            { //dolny s¹siad
                pos_posibilities.Add(new Vector2Int(posi.x, posi.y - 1));
            }
        }

        //rozsrawienie safe pokoi po wylosowanym slocie s¹siaduj¹cym niebezpieczny pokój
        for (int i = 0; i < n; i++)
        {
            bool not_done = true;
            while (not_done)
            {
                int r = UnityEngine.Random.Range(0, pos_posibilities.Count - 1);
                if (pos[pos_posibilities[r].x, pos_posibilities[r].y] == 0)
                { //czy zosta³ zabrany przez wczeœniejsz¹ pêtle
                    pos[pos_posibilities[r].x, pos_posibilities[r].y] = 3;
                    not_done = false;
                }
            }
        }
    }

    Vector2Int direction(int x, int y)
    {
        int kierunek = UnityEngine.Random.Range(0, 4);

        if (kierunek == 0)
        {
            return new Vector2Int(x + 1, y);
        }
        if (kierunek == 1)
        {
            return new Vector2Int(x - 1, y);
        }
        if (kierunek == 2)
        {
            return new Vector2Int(x, y - 1);
        }
        else
        {
            return new Vector2Int(x, y + 1);
        }
    }
    // funkcja przed kafelkami na test lochu
    public void Test_print(int[,] pos)
    {
        for (int y = Mheight - 1; y >= 0; y--)
        {
            string line = "";

            for (int x = 0; x < Mwidth; x++)
            {
                line += pos[x, y].ToString() + " ";
            }
            line += "[" + y + "]";
            //Debug.Log(line);
        }
    }

}
