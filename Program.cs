namespace MapGen
{
    static class Vars
    {
        //variables to change
        public static int mountainnes = 5; //percentage of mountains
        public static int waterness = 10; //percentage of lakes
        public static int mapsizeMult = 1; //size of map (1-default - fullscreen map , no scrolling possible) (2 and more - screen width*number = pixels width ...)
        public static int chanceMountainTwist = 15;
        public static int chanceChangeSize = 1;
        public static int currentMountainSize = 2;
        public static int chanceToBranch = 5;
        public static int chanceToBranchRiver = 3;
        public static int numberOfRivers = 100; // how many rivers to do
        public static int numberOfAproximationsHeight = 2;
        public static int numberOfPerlinAprox = 2;
        public static int chanceRiverTwist = 30;
        public static int numberOfMountainAprox = 1;
        public static int mapWidth = 210;
        public static int mapHeight = 300;

        // global variables, dont change anything
        public static char[,] Sq = new char[1, 1];
        public static int[,] AproxHeight1 = new int[1, 1];
        public static int[,] AproxHeight2 = new int[1, 1];
        public static int[] HeightGroups = new int[10000];
        public static int posX = 0;
        public static int posY = 0;
        public static bool mountainIncrease = true;
    }
    class Program
    {
        public static int rotationLimitCheckerSetter1(int meanrotation, int newrotation)
        {
            int rotat = newrotation;
            if (!(rotat >= meanrotation - 1 && rotat <= meanrotation + 1)) //if not in safe space
            {
                if (rotat < meanrotation)
                {
                    rotat = meanrotation-1;
                }
                else
                {
                    rotat = meanrotation+1;
                }
            }
            if (rotat > 7)
            {
                rotat -= 8;
            }
            if (rotat < 0)
            {
                rotat += 8;
            }
            return rotat;
        }
        public static int rotationLimitCheckerSetter(int meanrotation, int newrotation)
        {
            int rotat = newrotation;
            if (!(rotat >= meanrotation - 2 && rotat <= meanrotation + 2)) //if not in safe space
            {
                if (rotat < meanrotation)
                {
                    rotat += 2;
                }
                else
                {
                    rotat -= 2;
                }
            }
            if (rotat > 7)
            {
                rotat -= 8;
            }
            if (rotat < 0)
            {
                rotat += 8;
            }
            return rotat;
        }
        public static int rotationChanger2(int rotation, Random r1)
        {
            int rotat = rotation;
            if (r1.Next(0, 1) == 0)
            {
                rotat -= 2;
                if (rotat == -1)
                {
                    rotat = 7;
                }
                if (rotat == -2)
                {
                    rotat = 6;
                }
            }
            else
            {
                rotat += 2;
                if (rotat == 8)
                {
                    rotat = 0;
                }
                if (rotat == 9)
                {
                    rotat = 1;
                }
            }
            return rotat;
        }
        public static void mountainBranchGen(int currentSize, int rotation, Random r1, int chanceBranch, int meanrotation)
        {
            int changeSize = 0;
            int branchposY = Vars.posY;
            int branchposX = Vars.posX;
            //todo
            while (currentSize != 0)
            {
                //size changer
                if (r1.Next(0, 10000) <= changeSize)
                {
                    currentSize--;
                    changeSize = 0;
                    if (currentSize == 0)
                    {
                        break;
                    }
                }
                else
                {
                    changeSize += 80;
                }

                //next mountain in mountain chain
                if (r1.Next(0, 100) <= chanceBranch)
                {
                    int tmp = rotationChanger2(rotation, r1);
                    mountainBranchGen(currentSize, tmp, r1, chanceBranch / 2, tmp);
                }


                if (r1.Next(0, 100) <= Vars.chanceMountainTwist)
                {
                    if (r1.Next(0, 1) == 0)
                    {
                        rotation--;
                        rotation = rotationLimitCheckerSetter(meanrotation, rotation);
                    }
                    else
                    {
                        rotation++;
                        rotation = rotationLimitCheckerSetter(meanrotation, rotation);
                    }
                }

                //rotation affector
                switch (rotation)
                {
                    case 0:
                        branchposY--;
                        break;
                    case 1:
                        branchposY--;
                        branchposX++;
                        break;
                    case 2:
                        branchposX++;
                        break;
                    case 3:
                        branchposY++;
                        branchposX++;
                        break;
                    case 4:
                        branchposY++;
                        break;
                    case 5:
                        branchposY++;
                        branchposX--;
                        break;
                    case 6:
                        branchposX--;
                        break;
                    case 7:
                        branchposY--;
                        branchposX--;
                        break;
                    default:
                        Console.WriteLine("shouldnt happen");
                        break;
                }

                //new mountain completely out of bounds
                if (branchposX == -1 || branchposX == Vars.mapWidth || branchposY == -1 || branchposY == Vars.mapHeight) //out of map bounds, new mountain range
                {
                    break;
                }

                //number of tiles (width and height) setter
                int numberOfTilesXAxis = (2 * currentSize) - 1;
                if (currentSize == 1)
                {
                    numberOfTilesXAxis += 2;
                }

                //generate next/new blob
                for (int y = 0; y < 3 + (2 * currentSize); y++)
                {

                    for (int x = 0; x < numberOfTilesXAxis; x++)
                    {
                        int heightPos = branchposY + y - ((2 + (2 * currentSize)) / 2);
                        int widthPos = branchposX + x - ((numberOfTilesXAxis - 1) / 2);
                        if (heightPos < Vars.mapHeight && heightPos >= 0)
                        {
                            if (widthPos < Vars.mapWidth && widthPos >= 0)
                            {
                                if (widthPos == branchposX && heightPos == branchposY)
                                {
                                    Vars.Sq[heightPos, widthPos] = '▓';
                                }
                                if (currentSize == 2 || currentSize == 3)
                                {
                                    if ((heightPos == branchposY) && ((widthPos == branchposX - 1) || (widthPos == branchposX + 1)))
                                    {
                                        Vars.Sq[heightPos, widthPos] = '▓';
                                    }
                                    if ((widthPos == branchposX) && ((heightPos == branchposY - 1) || (heightPos == branchposY + 1)))
                                    {
                                        Vars.Sq[heightPos, widthPos] = '▓';
                                    }
                                }
                                if (currentSize == 3)
                                {
                                    if ((heightPos == branchposY - 1) && ((widthPos == branchposX - 1) || (widthPos == branchposX + 1)))
                                    {
                                        Vars.Sq[heightPos, widthPos] = '▓';
                                    }
                                    if ((heightPos == branchposY + 1) && ((widthPos == branchposX - 1) || (widthPos == branchposX + 1)))
                                    {
                                        Vars.Sq[heightPos, widthPos] = '▓';
                                    }
                                }
                                if (Vars.Sq[heightPos, widthPos] != '▓')
                                {
                                    Vars.Sq[heightPos, widthPos] = '▒';
                                }
                            }
                        }
                    }

                    //number of tiles changer
                    if (currentSize == 1)
                    {
                        if (y == 0)
                        {
                            numberOfTilesXAxis += 2;
                        }
                        if (y == 3)
                        {
                            numberOfTilesXAxis -= 2;
                        }
                    }
                    else if (currentSize == 2)
                    {
                        if (y == 0 || y == 1)
                        {
                            numberOfTilesXAxis += 2;
                        }
                        if (y == 4 || y == 5)
                        {
                            numberOfTilesXAxis -= 2;
                        }
                    }
                    else if (currentSize == 3)
                    {
                        if (y == 0 || y == 1)
                        {
                            numberOfTilesXAxis += 2;
                        }
                        if (y == 6 || y == 7)
                        {
                            numberOfTilesXAxis -= 2;
                        }
                    }
                }
            }
        }
        public static int rotationLakeToRiver(int posx, int posy)
        {
            int count = 0;
            int[] rotationScoring = new int[8];
            for (int localy = 0; localy < 3; localy++)
            {
                for (int localx = 0; localx < 3; localx++)
                {
                    int heightPos = posy + localy - 1;
                    int widthPos = posx + localx - 1;
                    if (heightPos < Vars.mapHeight && heightPos >= 0)
                    {
                        if (widthPos < Vars.mapWidth && widthPos >= 0)
                        {
                            if (Vars.Sq[heightPos, widthPos] == '░')
                            {
                                switch (count)
                                {
                                    case 8:
                                        rotationScoring[2]++;
                                        rotationScoring[3]++;
                                        rotationScoring[4]++;
                                        break;
                                    case 7:
                                        rotationScoring[3]++;
                                        rotationScoring[4]++;
                                        rotationScoring[5]++;
                                        break;
                                    case 6:
                                        rotationScoring[4]++;
                                        rotationScoring[5]++;
                                        rotationScoring[6]++;
                                        break;
                                    case 5:
                                        rotationScoring[1]++;
                                        rotationScoring[2]++;
                                        rotationScoring[3]++;
                                        break;
                                    case 3:
                                        rotationScoring[5]++;
                                        rotationScoring[6]++;
                                        rotationScoring[7]++;
                                        break;
                                    case 2:
                                        rotationScoring[0]++;
                                        rotationScoring[1]++;
                                        rotationScoring[2]++;
                                        break;
                                    case 1:
                                        rotationScoring[7]++;
                                        rotationScoring[0]++;
                                        rotationScoring[1]++;
                                        break;
                                    case 0:
                                        rotationScoring[6]++;
                                        rotationScoring[7]++;
                                        rotationScoring[0]++;
                                        break;
                                    default:
                                        break;
                                }
                            }


                        }
                    }
                    count++;
                }
            }
            int max = 0; int maxindex = 0;
            for (int i = 0; i < 8; i++)
            {
                if (rotationScoring[i] > max)
                {
                    max = rotationScoring[i];
                    maxindex = i;
                }
            }
            return maxindex;
        }
        public static bool HTinRange(int posy, int posx)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int widthpos = posx - 1 + j;
                    int heightpos = posy - 1 + i;
                    if (heightpos >= Vars.mapHeight || heightpos < 0 || widthpos >= Vars.mapWidth || widthpos < 0)
                    {
                        return true;
                    }
                    else if (Vars.Sq[heightpos, widthpos] == '▓')
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static int nextMainRiverTile(int posx, int posy, int rotation, int anglecount)
        {
            int rotat = 0;
            int max = 0;

            for (int i = 0; i < 3; i++)
            {
                if (getHeightAtOrientation(posx, posy, rotation - 1 + i) > max)
                {
                    max = getHeightAtOrientation(posx, posy, rotation - 1 + i);
                    rotat = rotation - 1 + i;
                }
            }
            if ((getHeightAtOrientation(posx, posy, rotation - 1) == getHeightAtOrientation(posx, posy, rotation)) && (getHeightAtOrientation(posx, posy, rotation) == max))
            {
                rotat++;
            }
            if (rotat == -1)
            {
                rotat = 7;
            }
            if (rotat == 8)
            {
                rotat = 0;
            }
            if (anglecount <= 2)
            {
                rotat = rotation;
            }
            return rotat;
        }
        public static int getHeightAtOrientation(int posx, int posy, int orientation)
        {
            switch (orientation)
            {
                case -1:
                    posy--;
                    posx--;
                    break;
                case 0:
                    posy--;
                    break;
                case 1:
                    posy--;
                    posx++;
                    break;
                case 2:
                    posx++;
                    break;
                case 3:
                    posy++;
                    posx++;
                    break;
                case 4:
                    posy++;
                    break;
                case 5:
                    posy++;
                    posx--;
                    break;
                case 6:
                    posx--;
                    break;
                case 7:
                    posy--;
                    posx--;
                    break;
                case 8:
                    posy--;
                    break;
                default:
                    Console.WriteLine("shouldnt happen");
                    break;
            }
            return Vars.AproxHeight1[posy, posx];
        }
        public static bool waterinRange(int posy, int posx)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int widthpos = posx - 1 + j;
                    int heightpos = posy - 1 + i;
                    if (heightpos >= Vars.mapHeight || heightpos < 0 || widthpos >= Vars.mapWidth || widthpos < 0)
                    {
                        return false;
                    }
                    if (Vars.Sq[heightpos, widthpos] == '~')
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //************************************************************************
        static void Main(string[] args)
        {
            //init
            Vars.Sq = new char[Vars.mapHeight + 1, Vars.mapWidth + 1]; //tiles/board
            Vars.AproxHeight1 = new int[Vars.mapHeight + 1, Vars.mapWidth + 1];
            Vars.AproxHeight2 = new int[Vars.mapHeight + 1, Vars.mapWidth + 1];

            //spawn LT low tile
            for (int j = 0; j < Vars.mapHeight; j++)
            {
                for (int k = 0; k < Vars.mapWidth; k++)
                {
                    Vars.Sq[j, k] = '░';
                }
            }
            //spawn HT high tile and MT medium tile
            int numberOfMountains = (((Vars.mapWidth * Vars.mapHeight) / 100) * Vars.mountainnes) / 12;
            bool newmountain = true;
            int rotation = 0;
            int meanrotation = 0;
            Random r1 = new Random();
            for (int i = 0; i < numberOfMountains; i++)
            {
                //size changer
                if (r1.Next(0, 10000) <= Vars.chanceChangeSize)
                {
                    if (Vars.mountainIncrease == true)
                    {
                        Vars.currentMountainSize++;
                    }
                    else
                    {
                        Vars.currentMountainSize--;
                    }
                    if (Vars.currentMountainSize == 3)
                    {
                        Vars.mountainIncrease = false;
                    }
                    if (Vars.currentMountainSize == 0)
                    {
                        newmountain = true;
                        Vars.mountainIncrease = true;
                    }
                    Vars.chanceChangeSize = 0;
                }
                else
                {
                    Vars.chanceChangeSize += 80;
                }

                //new mountain range location
                if (newmountain == true)
                {
                    Vars.currentMountainSize = 1;
                    Vars.posX = r1.Next(0, Vars.mapWidth - 1);
                    Vars.posY = r1.Next(0, Vars.mapHeight - 1);
                    newmountain = false;
                    rotation = r1.Next(0, 7);
                    meanrotation = rotation;
                }

                //next mountain in mountain chain
                else
                {
                    if (r1.Next(0, 100) <= Vars.chanceToBranch)
                    {
                        int tmp = rotationChanger2(rotation, r1);
                        mountainBranchGen(Vars.currentMountainSize, tmp, r1, Vars.chanceToBranch / 2, tmp);
                    }
                    if (r1.Next(0, 100) <= Vars.chanceMountainTwist)
                    {
                        if (r1.Next(0, 1) == 0)
                        {
                            rotation--;
                            rotation = rotationLimitCheckerSetter(meanrotation, rotation);
                        }
                        else
                        {
                            rotation++;
                            rotation = rotationLimitCheckerSetter(meanrotation, rotation);
                        }
                    }
                }

                //rotation affector
                switch (rotation)
                {
                    case 0:
                        Vars.posY--;
                        break;
                    case 1:
                        Vars.posY--;
                        Vars.posX++;
                        break;
                    case 2:
                        Vars.posX++;
                        break;
                    case 3:
                        Vars.posY++;
                        Vars.posX++;
                        break;
                    case 4:
                        Vars.posY++;
                        break;
                    case 5:
                        Vars.posY++;
                        Vars.posX--;
                        break;
                    case 6:
                        Vars.posX--;
                        break;
                    case 7:
                        Vars.posY--;
                        Vars.posX--;
                        break;
                    default:
                        Console.WriteLine("shouldnt happen");
                        break;
                }

                //new mountain completely out of bounds
                if (Vars.posX == -1 || Vars.posX == Vars.mapWidth || Vars.posY == -1 || Vars.posY == Vars.mapHeight) //out of map bounds, new mountain range
                {
                    i--;
                    newmountain = true;
                    continue;
                }

                //number of tiles (width and height) setter
                int numberOfTilesXAxis = (2 * Vars.currentMountainSize) - 1;
                if (Vars.currentMountainSize == 1)
                {
                    numberOfTilesXAxis += 2;
                }

                //generate next/new blob
                for (int y = 0; y < 3 + (2 * Vars.currentMountainSize); y++)
                {

                    for (int x = 0; x < numberOfTilesXAxis; x++)
                    {
                        int heightPos = Vars.posY + y - ((2 + (2 * Vars.currentMountainSize)) / 2);
                        int widthPos = Vars.posX + x - ((numberOfTilesXAxis - 1) / 2);
                        if (heightPos < Vars.mapHeight && heightPos >= 0)
                        {
                            if (widthPos < Vars.mapWidth && widthPos >= 0)
                            {
                                if (widthPos == Vars.posX && heightPos == Vars.posY)
                                {
                                    Vars.Sq[heightPos, widthPos] = '▓';
                                }
                                if (Vars.currentMountainSize == 2 || Vars.currentMountainSize == 3)
                                {
                                    if ((heightPos == Vars.posY) && ((widthPos == Vars.posX - 1) || (widthPos == Vars.posX + 1)))
                                    {
                                        Vars.Sq[heightPos, widthPos] = '▓';
                                    }
                                    if ((widthPos == Vars.posX) && ((heightPos == Vars.posY - 1) || (heightPos == Vars.posY + 1)))
                                    {
                                        Vars.Sq[heightPos, widthPos] = '▓';
                                    }
                                }
                                if (Vars.currentMountainSize == 3)
                                {
                                    if ((heightPos == Vars.posY - 1) && ((widthPos == Vars.posX - 1) || (widthPos == Vars.posX + 1)))
                                    {
                                        Vars.Sq[heightPos, widthPos] = '▓';
                                    }
                                    if ((heightPos == Vars.posY + 1) && ((widthPos == Vars.posX - 1) || (widthPos == Vars.posX + 1)))
                                    {
                                        Vars.Sq[heightPos, widthPos] = '▓';
                                    }
                                }
                                if (Vars.Sq[heightPos, widthPos] != '▓')
                                {
                                    Vars.Sq[heightPos, widthPos] = '▒';
                                }
                            }
                        }
                    }

                    //number of tiles changer
                    if (Vars.currentMountainSize == 1)
                    {
                        if (y == 0)
                        {
                            numberOfTilesXAxis += 2;
                        }
                        if (y == 3)
                        {
                            numberOfTilesXAxis -= 2;
                        }
                    }
                    else if (Vars.currentMountainSize == 2)
                    {

                        if (y == 0 || y == 1)
                        {
                            numberOfTilesXAxis += 2;
                        }
                        if (y == 4 || y == 5)
                        {
                            numberOfTilesXAxis -= 2;
                        }
                    }
                    else if (Vars.currentMountainSize == 3)
                    {
                        if (y == 0 || y == 1)
                        {
                            numberOfTilesXAxis += 2;
                        }
                        if (y == 6 || y == 7)
                        {
                            numberOfTilesXAxis -= 2;
                        }
                    }


                }
            }

        
            //water start
            //sq to aproxheight1
            for (int i = 0; i < Vars.mapHeight - 1; i++)
            {
                for (int j = 0; j < Vars.mapWidth - 1; j++)
                {
                    if (Vars.Sq[i, j] == '▓')
                    {
                        Vars.AproxHeight1[i, j] = 10000;
                    }
                    if (Vars.Sq[i, j] == '▒')
                    {
                        Vars.AproxHeight1[i, j] = 2000;
                    }
                    if (Vars.Sq[i, j] == '░')
                    {
                        Vars.AproxHeight1[i, j] = 0;
                    }
                }
            }

            //aprox mountains
            for (int i = 0; i < Vars.numberOfMountainAprox; i++)
            {
                for (int y = 0; y < Vars.mapHeight; y++)
                {
                    for (int x = 0; x < Vars.mapWidth; x++)
                    {
                        int tilesCount = 0;
                        int heightCount = 0;
                        int numberOfTilesXAxis = 3;
                        for (int localy = 0; localy < 7; localy++)
                        {
                            for (int localx = 0; localx < numberOfTilesXAxis; localx++)
                            {
                                int heightPos = y + localy - 3;
                                int widthPos = x + localx - ((numberOfTilesXAxis - 1) / 2);
                                if (heightPos < Vars.mapHeight && heightPos >= 0)
                                {
                                    if (widthPos < Vars.mapWidth && widthPos >= 0)
                                    {
                                        tilesCount++;
                                        heightCount += Vars.AproxHeight1[heightPos, widthPos];
                                    }
                                }
                            }

                            //number of tiles changer
                            if (localy == 0)
                            {
                                numberOfTilesXAxis += 2;
                            }
                            if (localy == 1)
                            {
                                numberOfTilesXAxis += 2;
                            }
                            if (localy == 4)
                            {
                                numberOfTilesXAxis -= 2;
                            }
                            if (localy == 5)
                            {
                                numberOfTilesXAxis -= 2;
                            }
                        }
                        Vars.AproxHeight2[y, x] = heightCount / tilesCount;
                    }

                }
                Vars.AproxHeight1 = Vars.AproxHeight2.Clone() as int[,];
            }
            //adjust lvl1 mountain
            for (int i = 0; i < Vars.mapHeight - 1; i++)
            {
                for (int j = 0; j < Vars.mapWidth - 1; j++)
                {
                    if (Vars.Sq[i, j] == '▓')
                    {
                        Vars.AproxHeight1[i, j] += 4000;
                    }
                }
            }

            //paint MT HT
            for (int i = 0; i < Vars.mapHeight - 1; i++)
            {
                for (int j = 0; j < Vars.mapWidth - 1; j++)
                {
                    if (Vars.AproxHeight1[i, j] >= 4000)
                    {
                        Vars.Sq[i, j] = '▓';
                    }
                    else if (Vars.AproxHeight1[i, j] >= 1100)
                    {
                        Vars.Sq[i, j] = '▒';
                    }
                    else
                    {
                        Vars.Sq[i, j] = '░';
                    }
                }
            }
            Vars.AproxHeight1 = new int[Vars.mapHeight + 1, Vars.mapWidth + 1];
            Vars.AproxHeight2 = new int[Vars.mapHeight + 1, Vars.mapWidth + 1];

            //sq to aproxheight1
            for (int i = 0; i < Vars.mapHeight - 1; i++)
            {
                for (int j = 0; j < Vars.mapWidth - 1; j++)
                {
                    if (Vars.Sq[i, j] == '▓')
                    {
                        Vars.AproxHeight1[i, j] = 10000;
                    }
                    if (Vars.Sq[i, j] == '▒')
                    {
                        Vars.AproxHeight1[i, j] = 2000;
                    }
                    if (Vars.Sq[i, j] == '░')
                    {
                        Vars.AproxHeight1[i, j] = 0;
                    }
                }
            }
            //average heights
            for (int i = 0; i < Vars.numberOfAproximationsHeight; i++)
            {
                for (int y = 0; y < Vars.mapHeight; y++)
                {
                    for (int x = 0; x < Vars.mapWidth; x++)
                    {
                        int tilesCount = 0;
                        int heightCount = 0;
                        int numberOfTilesXAxis = 3;
                        for (int localy = 0; localy < 7; localy++)
                        {
                            for (int localx = 0; localx < numberOfTilesXAxis; localx++)
                            {
                                int heightPos = y + localy - 3;
                                int widthPos = x + localx - ((numberOfTilesXAxis - 1) / 2);
                                if (heightPos < Vars.mapHeight && heightPos >= 0)
                                {
                                    if (widthPos < Vars.mapWidth && widthPos >= 0)
                                    {
                                        tilesCount++;
                                        heightCount += Vars.AproxHeight1[heightPos, widthPos];
                                    }
                                }
                            }

                            //number of tiles changer
                            if (localy == 0)
                            {
                                numberOfTilesXAxis += 2;
                            }
                            if (localy == 1)
                            {
                                numberOfTilesXAxis += 2;
                            }
                            if (localy == 4)
                            {
                                numberOfTilesXAxis -= 2;
                            }
                            if (localy == 5)
                            {
                                numberOfTilesXAxis -= 2;
                            }
                        }
                        Vars.AproxHeight2[y, x] = heightCount / tilesCount;
                    }

                }
                Vars.AproxHeight1 = Vars.AproxHeight2.Clone() as int[,];
            }

            //perlin noise
            for (int i = 0; i < Vars.mapHeight - 1; i++)
            {
                for (int j = 0; j < Vars.mapWidth - 1; j++)
                {
                    Vars.AproxHeight1[i, j]+=r1.Next(0,50);
                }
            }
            // aprox for perlin
            for (int i = 0; i < Vars.numberOfPerlinAprox; i++)
            {
                for (int y = 0; y < Vars.mapHeight; y++)
                {
                    for (int x = 0; x < Vars.mapWidth; x++)
                    {
                        int tilesCount = 0;
                        int heightCount = 0;
                        int numberOfTilesXAxis = 3;
                        for (int localy = 0; localy < 7; localy++)
                        {
                            for (int localx = 0; localx < numberOfTilesXAxis; localx++)
                            {
                                int heightPos = y + localy - 3;
                                int widthPos = x + localx - ((numberOfTilesXAxis - 1) / 2);
                                if (heightPos < Vars.mapHeight && heightPos >= 0)
                                {
                                    if (widthPos < Vars.mapWidth && widthPos >= 0)
                                    {
                                        tilesCount++;
                                        heightCount += Vars.AproxHeight1[heightPos, widthPos];
                                    }
                                }
                            }

                            //number of tiles changer
                            if (localy == 0)
                            {
                                numberOfTilesXAxis += 2;
                            }
                            if (localy == 1)
                            {
                                numberOfTilesXAxis += 2;
                            }
                            if (localy == 4)
                            {
                                numberOfTilesXAxis -= 2;
                            }
                            if (localy == 5)
                            {
                                numberOfTilesXAxis -= 2;
                            }
                        }
                        Vars.AproxHeight2[y, x] = heightCount / tilesCount;
                    }

                }
                Vars.AproxHeight1 = Vars.AproxHeight2.Clone() as int[,];
            }
                

            //catalog heights
            for (int i = 0; i < Vars.mapHeight - 1; i++)
            {
                for (int j = 0; j < Vars.mapWidth - 1; j++)
                {
                    if (Vars.AproxHeight1[i, j]>=10000)
                    {
                        Vars.AproxHeight1[i, j] = 9999;
                    }
                    Vars.HeightGroups[Vars.AproxHeight1[i, j]]++;
                }
            }
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    Vars.HeightGroups[i] += Vars.HeightGroups[j];
                }
            }
            int waterLevel = 0;
            int numberofWaterTiles = ((Vars.mapHeight * Vars.mapWidth) / 100) * Vars.waterness;
            for (int i = 0; i < 10000; i++)
            {
                if (Vars.HeightGroups[i] > numberofWaterTiles)
                {
                    waterLevel = i-1;
                    break;
                }
            }
            if (waterLevel == -1 && Vars.waterness != 0)
            {
                for (int i = 0; i < 10000; i++)
                {
                    if (Vars.HeightGroups[i] != 0)
                    {
                        waterLevel = i;
                        break;
                    }
                }
            }

            //lakes
            for (int i = 0; i < Vars.mapHeight - 1; i++)
            {
                for (int j = 0; j < Vars.mapWidth - 1; j++)
                {
                    if (Vars.AproxHeight1[i, j] <= waterLevel+1)
                    {
                        Vars.Sq[i, j] = '~';
                    }
                }
            }

            //get river candidates
            int[,] Rivercandid1 = new int[2, Vars.numberOfRivers];
            int[,] Rivercandid2 = new int[2, Vars.numberOfRivers];
            int riverarray1index = 0;
            int riverarray2index = 0;
            int riverindex = 0;
            for (riverindex = waterLevel + 1; riverindex < 10000; riverindex++)
            {
                if (Vars.HeightGroups[riverindex] - Vars.HeightGroups[waterLevel] >= Vars.numberOfRivers)
                {
                    for (int i = 0; i < Vars.mapHeight - 1; i++)
                    {
                        for (int j = 0; j < Vars.mapWidth - 1; j++)
                        {
                            if (Vars.AproxHeight1[i, j] == riverindex)
                            {
                                riverarray2index++;
                            }
                        }
                    }
                    Rivercandid2 = new int[2, riverarray2index + 1];
                    riverarray2index = 0;
                    for (int i = 0; i < Vars.mapHeight - 1; i++)
                    {
                        for (int j = 0; j < Vars.mapWidth - 1; j++)
                        {
                            if (Vars.AproxHeight1[i, j] == riverindex)
                            {
                                if (waterinRange(i,j)==true)
                                {
                                    Rivercandid2[0, riverarray2index] = i;
                                    Rivercandid2[1, riverarray2index] = j;
                                    riverarray2index++;
                                }
                                
                            }
                        }
                    }
                    break;
                }
                else
                {
                    for (int i = 0; i < Vars.mapHeight - 1; i++)
                    {
                        for (int j = 0; j < Vars.mapWidth - 1; j++)
                        {
                            if (Vars.AproxHeight1[i, j] == riverindex)
                            {
                                Rivercandid1[0, riverarray1index] = i;
                                Rivercandid1[1, riverarray1index] = j;
                                riverarray1index++;
                            }
                        }
                    }
                }

            }
            //river array 1 correction
            for (int i = riverarray1index; i < Vars.numberOfRivers; i++)
            {
                int tmprandom = r1.Next(0, Rivercandid2.Length/2);
                Rivercandid1[0, i] = Rivercandid2[0, tmprandom];
                Rivercandid1[1, i] = Rivercandid2[1, tmprandom];
            }


            //rivers 
            for (int i = 0; i < Vars.numberOfRivers; i++)
            {
                int posx = Rivercandid1[1, i];
                int posy = Rivercandid1[0, i];
                int riverrotation = rotationLakeToRiver(posx, posy);
                int meanriverRotation = riverrotation;
                int twistcount = 0;
                int loweringcount = 0;
                int newriverrotat = 0;
                char riverchar = '/';
                while (HTinRange(posy, posx) == false)
                {
                    if (r1.Next(0,100)<Vars.chanceRiverTwist)
                    {
                        if (r1.Next(0,1)==0)
                        {
                            newriverrotat--;
                            if (newriverrotat == -1)
                            {
                                newriverrotat = 7;
                            }
                        }
                        else
                        {
                            newriverrotat++;
                            if (newriverrotat==8)
                            {
                                newriverrotat = 0;
                            }
                        }
                    }
                    else
                    {
                        newriverrotat = nextMainRiverTile(posx, posy, riverrotation, twistcount);
                    }
                    if (newriverrotat!=riverrotation)
                    {
                        twistcount=0;
                    }
                    else
                    {
                        twistcount++;
                    }
                    
                    riverrotation = rotationLimitCheckerSetter1(meanriverRotation, newriverrotat);
                    int oldx = posx;
                    int oldy = posy;
                    
                    switch (newriverrotat)
                    {
                        case 0:
                            posy--;
                            riverchar = '|';
                            break;
                        case 1:
                            posy--;
                            posx++;
                            riverchar = '/';
                            break;
                        case 2:
                            posx++;
                            riverchar = '-';
                            break;
                        case 3:
                            posy++;
                            posx++;
                            riverchar = '\\';
                            break;
                        case 4:
                            posy++;
                            riverchar = '|';
                            break;
                        case 5:
                            posy++;
                            posx--;
                            riverchar = '/';
                            break;
                        case 6:
                            posx--;
                            riverchar = '-';
                            break;
                        case 7:
                            posy--;
                            posx--;
                            riverchar = '\\';
                            break;
                        default:
                            Console.WriteLine("shouldnt happen");
                            break;
                    }
                    if (Vars.AproxHeight1[oldy, oldx] > Vars.AproxHeight1[posy, posx])
                    {
                        loweringcount++;
                    }
                    else
                    {
                        loweringcount = 0;
                    }
                    if (loweringcount==4)
                    {
                        break;
                    }
                    if (Vars.Sq[posy, posx]!= '░' && Vars.Sq[posy, posx] != '▒')
                    {
                        break;
                    }
                    if (Vars.Sq[oldy, oldx] != '~')
                    {
                        Vars.Sq[oldy, oldx] = riverchar;
                    }
                }
            }
            for (int i = 0; i < Vars.mapHeight - 1; i++)
            {
                for (int j = 0; j < Vars.mapWidth - 1; j++)
                {
                    Console.Write(Vars.Sq[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
