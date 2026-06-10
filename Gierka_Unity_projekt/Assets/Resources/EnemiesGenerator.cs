using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemiesGenerator : MonoBehaviour
{
    //====co potrzebuje zmieniaæ w TileManager
    //gradients
    //colors
    //enemy_pos_save

    //====co potrzebuje temp odzcytaæ co jest na czerwono
    //UnitPerFrame
    //tileWidth
    //TileHeight
    //enemy size
    float MtileWidth;
    float MtileHeight;
    int MUnitPerFrame;
    Vector2 MenemySize;

    // ===================================== PERLIN ====================================================================
    //na iloœæ pokoi osobny perlin
    //by nie zapisywaæ wszystkiego robiæ na ka¿dy pokój, zapisaæ pozycje przeciwników i wyczyœciæ
    //skoro perlin dzieli komówki na dziesiêtnê to tablica pokoju dzielone 
    public void ReadValues(float tileWidth, float tileHeight, int UnitPerFrame, Vector2 enemySize)
    {
        MtileHeight = tileHeight;
        MtileWidth = tileWidth;
        MenemySize = enemySize;
        MUnitPerFrame = UnitPerFrame;
    }

    public void Perlin(float[,] gradients,float[,] colors,int rel_height, int rel_width) //gradients, colors
    {
        int UnitPerFrame = 16;
        for (int z = 0; z < gradients.GetLength(0); z++)
        {
            for (int c = 0; c < gradients.GetLength(1); c++)
            {
                float pozX = UnityEngine.Random.Range(-1, 1);
                float pozY = UnityEngine.Random.Range(-1, 1);
                gradients[z, c] = (float)Math.Sqrt(Math.Pow(pozX, 2) + Math.Pow(pozY, 2));
            }
        }
        //Debug.Log("rel_width" + rel_width);
        //Debug.Log("rel_Height" + rel_height);

        //Debug.Log("GRADIENT_width" + gradients.GetLength(0));
        //Debug.Log("GRADIENT_height" + gradients.GetLength(1));

        //Debug.Log("COLOR_width" + colors.GetLength(0));
        //Debug.Log("COLOR_Height" + colors.GetLength(1));

        for (int x = 0; x < gradients.GetLength(0) - 1; x++)
        {

            for (int y = 0; y < gradients.GetLength(1) - 1; y++)
            {

                //wartosc kazdego z naroznikow
                float vecup1 = gradients[x, y];
                float vecup2 = gradients[x + 1, y];
                float vecdown1 = gradients[x, y + 1];
                float vecdown2 = gradients[x + 1, y + 1];

                float up;
                float down;

                //petle wewnatrz jednego kwadracika
                for (int x_min = 0; x_min < UnitPerFrame; x_min++)
                {
                    float Tx = (float)x_min / rel_width; // pozycja kratki w kratce x

                    for (int y_min = 0; y_min < UnitPerFrame; y_min++)
                    {
                        float Ty = (float)y_min / rel_height; // pozycja kratki w kratce x

                        up = Mathf.Lerp(vecup1, vecup2, Tx); //wbudowana funkcja na liczenie indukcji
                        down = Mathf.Lerp(vecdown1, vecdown2, Tx);

                        //Debug.Log("X: " + (x_min + (x * rel_width)) + ", Y: " + (y_min + (y * rel_height)));
                        colors[x_min + (x * UnitPerFrame), y_min + (y * UnitPerFrame)] = Mathf.Lerp(up, down, Ty); //przydzielenie koloru szaroœci
                    }

                }
                
            }
        }

    }
    public void CountNeighbours(float[,] colors, List<Vector2> enemy_pos_save, List<Vector2> enemies_spawn,List<Vector2> enemies_rooms,
        Vector2Int posi, int n) //colors, enemy_pos_save, enemies_rooms, enemies_spawn
    {
        float[,] neighbour_count = new float[Mathf.RoundToInt(MtileWidth) * MUnitPerFrame, Mathf.RoundToInt(MtileHeight) * MUnitPerFrame]; // 
        float avg_much = 0f;
        float avg_value = 0f;
        for (int x = 0; x < colors.GetLength(0); x++)
        {
            for (int y = 0; y < colors.GetLength(1); y++)
            {
                //ca³y warunek dodawania s¹siedztwa----------
                float much = 0;
                float value = 0;
                if (x - 1 >= 0)
                {
                    if (y - 1 >= 0)
                    {//(x-1, y-1)
                        much += 1;
                        value += colors[x - 1, y - 1];
                    }
                    if (y + 1 < colors.GetLength(1))
                    { //(x-1, y+1)
                        much += 1;
                        value += colors[x - 1, y + 1];
                    }
                    //(x-1, y)
                    much += 1;
                    value += colors[x - 1, y];

                }
                if (x + 1 < colors.GetLength(0))
                {
                    if (y - 1 >= 0)
                    { //(x+1, y-1)
                        much += 1;
                        value += colors[x + 1, y - 1];
                    }
                    if (y + 1 < colors.GetLength(1))
                    { //(x+1, y+1)
                        much += 1;
                        value += colors[x + 1, y + 1];
                    }
                    //(x+1, y)
                    much += 1;
                    value += colors[x + 1, y];
                }
                //dla zwyk³ego x
                if (y - 1 >= 0)
                { //(x, y-1)
                    much += 1;
                    value += colors[x, y - 1];
                }
                if (y + 1 < colors.GetLength(1))
                { //(x, y+1)
                    much += 1;
                    value += colors[x, y + 1];
                }
                //warunek x, y
                much += 1;
                value += colors[x, y];
                //---------------

                neighbour_count[x, y] = (value / much);
                if (neighbour_count[x, y] > 0.1)
                {
                    avg_much += 1;
                    avg_value += neighbour_count[x, y];
                }
            }
        }
        float avg = avg_value / avg_much;
        List<Vector2Int> posible_enemy = new List<Vector2Int>();
        for (int x = 0; x < neighbour_count.GetLength(0); x++)
        {
            for (int y = 0; y < neighbour_count.GetLength(1); y++)
            {
                if (neighbour_count[x, y] >= avg)
                {
                    posible_enemy.Add(new Vector2Int(x, y)); //doda mo¿liwoœæ istnienia przeciwnika na polu

                    //podejœcie jest z mo¿liwoœci¹ pojawienia siê obok siebie
                }
            }
        }
        int spawnCount = Mathf.Min(n, posible_enemy.Count);
        //Debug.Log("SPAWN_COUNT: "+spawnCount);
        for (int i = 0; i < spawnCount; i++)
        {
            int r = UnityEngine.Random.Range(0, posible_enemy.Count);

            Vector2Int local = posible_enemy[r];

            float localX = ((float)local.x / colors.GetLength(0)) * MtileWidth;
            float localY = ((float)local.y / colors.GetLength(1)) * MtileHeight;

            //zapobieganie wychodzenia za pokój/kamere
            float halfEnemyX = MenemySize.x * 0.7f;
            float halfEnemyY = MenemySize.y * 0.7f;

            localX = Mathf.Clamp(
                localX,
                halfEnemyX,
                MtileWidth - halfEnemyX
            );

            localY = Mathf.Clamp(
                localY,
                halfEnemyY,
                MtileHeight - halfEnemyY
            );
            //tutaj dane do zapisu tak jak zapisywa³am rozmieszczenie pokoju
            //enemy_pos_save.Add(new Vector2(localX, localY));

            

            Vector2 worldPos = new Vector2(
                posi.x * MtileWidth - MtileWidth * 0.5f + localX,
                posi.y * MtileHeight - MtileHeight * 0.5f + localY
            );
            //zapisanie wobec pozycji œwiata
           
            enemy_pos_save.Add(worldPos);
            enemies_spawn.Add(worldPos);
            enemies_rooms.Add(posi);
            //Debug.Log("Enemy at " + worldPos);

            posible_enemy.RemoveAt(r);
        }


    }

    public void ClearGradient(float[,] gradients, float[,] colors) //gradients, colors
    {
        for (int x = 0; x < gradients.GetLength(0); x++)
        {
            for (int y = 0; y < gradients.GetLength(1); y++)
            {
                gradients[x, y] = 0f;
            }
        }
        for (int x = 0; x < colors.GetLength(0); x++)
        {
            for (int y = 0; y < colors.GetLength(1); y++)
            {
                colors[x, y] = 0f;
            }
        }
    }
}
