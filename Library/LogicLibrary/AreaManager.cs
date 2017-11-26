using System;
using System.Collections.Generic;
using System.Text;

namespace LogicLibrary
{
    public class AreaManager
    {
        List<Player>[] playerListByArea;

        float maxMapWidth;
        float maxMapHight;

        float cellSideLength;

        int numOfElemInOneRow;
        int numOfElemInOneColumn;

        int maxAreaCode;
        const int minAreaCode = 0;
        const int falseAreaCode = -1;

        public AreaManager(float maxWidth, float maxHight, float cellSideLen)
        {
            //최대 맵 가로세로,한 구역의 길이 구역은 정사각형 맵이 딱떨어지 않는 다면 나머지 자투리부분도 그냥 정사각형으로 가정하고 할당한다.
            maxMapWidth = maxWidth;
            maxMapHight = maxHight;
            cellSideLength = cellSideLen;
            
            //위 정보로 부터 각 구역의 행렬정보와 area 코드를 계산
            numOfElemInOneRow = (int)Math.Ceiling((double)(maxMapWidth / cellSideLength));
            numOfElemInOneColumn = (int)Math.Ceiling((double)(maxMapHight / cellSideLength));
            
            maxAreaCode = numOfElemInOneRow * numOfElemInOneColumn - 1;
            
            //초기화
            playerListByArea = new List<Player>[maxAreaCode + 1];
            
            for (int i = 0; i<= maxAreaCode;++i)
            {
                playerListByArea[i] = new List<Player>();
            }
        }

        public bool AddPlayerToMapManager(Player player)
        {
            if (player == null) return false;
            string name = player.playerID;
            int code = GetAreaCodeFromPosition(player.position);

            AddPlayerToArea(player, GetAreaCodeFromPosition(player.position));

            InitNewActiveArea(player);

            return true;
        }

        bool AddPlayerToArea(Player player, int areaCode)
        {
            //아마 락을 걸어야 겠지?
            if (areaCode < minAreaCode || areaCode > maxAreaCode)
            {
                //오류로그
                return false;
            }

            //이미 있는 플레이어인지 테스트.
            //if (playerListByArea[areaCode].Exists(x => x.Id == player.Id) == true)
            if (IsExist(playerListByArea[areaCode], player))
            {
                //오류로그
                return false;
            }

            player.areaCode = areaCode;

            playerListByArea[areaCode].Add(player);

            return true;
        }

        public bool RemovePlayerFromArea(Player player, int areaCode)
        {
            //아마 락을 걸어야 겠지?
            if (areaCode < minAreaCode || areaCode > maxAreaCode)
            {
                //오류로그
                return false;
            }

            //이미 있는 플레이어인지 테스트.
            //if (playerListByArea[areaCode].Exists(x => x.Id == player.Id) == true)
            if (IsExist(playerListByArea[areaCode], player))
            {

                player.areaCode = falseAreaCode;

                playerListByArea[areaCode].Remove(player);
                return true;
            }
            else
            {
                //오류로그
                return false;
            }

        }

        public bool TransPlayersArea(Player player)
        {
            //아마 락을 걸어야 겠지?

            int oldAreaCode = player.areaCode;



            int newAreaCode = GetAreaCodeFromPosition(player.position);

            if (newAreaCode == falseAreaCode)
            {
                return false;
            }

            if (oldAreaCode == newAreaCode)
            {
                //Debug.Log("Same Area. Area Not Changed");
                return false;
            }

            bool isRemoved = RemovePlayerFromArea(player, oldAreaCode);

            if (isRemoved == false)
            {
                return false;
            }

            bool isAdded = AddPlayerToArea(player, newAreaCode);

            if (isAdded == false)
            {
                AddPlayerToArea(player, oldAreaCode);
                return false;
            }

            UpdateActiveArea(player);

            return true;
        }

        public void InitNewActiveArea(Player player)
        {

            int areaCode = player.areaCode;

            if (areaCode < minAreaCode || areaCode > maxAreaCode)
            {
                return;
            }

            for (int i = 0; i < player.activeArea.Length; ++i)
            {
                player.oldActiveArea[i] = player.activeArea[i];
            }

            int[] newActiveArea = new int[9];

            #region northWest
            int northWest = areaCode - numOfElemInOneRow - 1;
            if (northWest >= minAreaCode)
            {
                newActiveArea[0] = northWest;
            }
            else
            {
                newActiveArea[0] = falseAreaCode;
            }
            #endregion

            #region north
            int north = areaCode - numOfElemInOneRow;
            if (north >= minAreaCode)
            {
                newActiveArea[1] = north;
            }
            else
            {
                newActiveArea[1] = falseAreaCode;
            }
            #endregion

            #region northEast
            int northEast = areaCode - numOfElemInOneRow + 1;
            if (northEast >= minAreaCode)
            {
                newActiveArea[2] = northEast;
            }
            else
            {
                newActiveArea[2] = falseAreaCode;
            }
            #endregion

            #region west
            int west = areaCode - 1;
            if (west >= minAreaCode)
            {
                newActiveArea[3] = west;
            }
            else
            {
                newActiveArea[3] = falseAreaCode;
            }

            #endregion

            #region center
            newActiveArea[4] = areaCode;
            #endregion

            #region east
            int east = areaCode + 1;
            if (west <= maxAreaCode)
            {
                newActiveArea[5] = east;
            }
            else
            {
                newActiveArea[5] = falseAreaCode;
            }
            #endregion

            #region southWest
            int southWest = areaCode + numOfElemInOneRow - 1;

            if (southWest <= maxAreaCode)
            {
                newActiveArea[6] = southWest;
            }
            else
            {
                newActiveArea[6] = falseAreaCode;
            }
            #endregion

            #region south
            int south = areaCode + numOfElemInOneRow;

            if (south <= maxAreaCode)
            {
                newActiveArea[7] = south;
            }
            else
            {
                newActiveArea[7] = falseAreaCode;
            }
            #endregion

            #region southEast

            int southEast = areaCode + numOfElemInOneRow + 1;

            if (southEast <= maxAreaCode)
            {
                newActiveArea[8] = southEast;
            }
            else
            {
                newActiveArea[8] = falseAreaCode;
            }
            #endregion

            #region Update Active Area
            for (int i = 0; i < player.activeArea.Length; ++i)
            {
                player.activeArea[i] = newActiveArea[i];
            }
            #endregion

        }

        public void UpdateActiveArea(Player player)
        {


            InitNewActiveArea(player);

            #region Update InActive Area
            for (int i = 0; i < 5; ++i)
            {
                player.inActiveArea[i] = -1;
            }

            int newInActiveCount = 0;



            for (int i = 0; i < player.oldActiveArea.Length; ++i)
            {



                if (player.oldActiveArea[i] == falseAreaCode) continue;

                int j = 0;

                for (; j < player.activeArea.Length; ++j)
                {
                    if (player.oldActiveArea[i] == player.activeArea[j])
                    {
                        break;
                    }
                }

                if (j == 9)
                {
                    player.inActiveArea[newInActiveCount++] = player.oldActiveArea[i];
                }
            }

            #endregion

            return;
        }

        int GetAreaCodeFromPosition(Vector3 position)
        {
            int posXToRow = (int)(position.x / cellSideLength);
            int posYToCol = (int)(position.z / cellSideLength);

            //Debug.Log(posXToRow + " ," + posYToCol);

            if (posXToRow == numOfElemInOneRow)
            {
                posXToRow -= 1;
            }

            if (posYToCol == numOfElemInOneColumn)
            {
                posYToCol -= 1;
            }

            int areaCode = posXToRow + posYToCol * numOfElemInOneRow;

            if (areaCode < minAreaCode || areaCode > maxAreaCode)
            {
                //오류 로그
                return falseAreaCode;
            }

            return areaCode;
        }

        bool IsExist(List<Player> list, Player player)
        {
            if (list == null || player == null)
            {
                return false;
            }

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].playerID == player.playerID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
