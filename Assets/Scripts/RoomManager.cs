using System;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public enum RoomSide : byte
    {
        TopLeft,
        BottomLeft,
        TopRight,
        BottomRight,
        SizeOf
    }

    public GameObject cinemachineCamera;

    public RoomManager topLeftRoom;
    public RoomManager bottomLeftRoom;
    public RoomManager topRightRoom;
    public RoomManager bottomRightRoom;

    public NextRoomLoader topLeftLoader;
    public NextRoomLoader bottomLeftLoader;
    public NextRoomLoader topRightLoader;
    public NextRoomLoader bottomRightLoader;

    public Transform shellContainer;


    private const int neighboursAmount = (int) RoomSide.SizeOf;


    private LevelManager levelManager;

    private readonly RoomManager[] neighbours = new RoomManager[neighboursAmount];
    private readonly NextRoomLoader[] neighbourLoaders = new NextRoomLoader[neighboursAmount];


    public void RegisterLevelManager(LevelManager manager)
    {
        levelManager = manager;
    }

    public void LoadNeighbour(NextRoomLoader nextRoomLoader)
    {
        int roomLoaderIndex = RoomLoaderIndex(nextRoomLoader);

        neighbours[roomLoaderIndex].gameObject.SetActive(true);
    }

    public void UnloadNeighbour(NextRoomLoader nextRoomLoader)
    {
        int roomLoaderIndex = RoomLoaderIndex(nextRoomLoader);

        if (levelManager.IsCurrentRoom(this))
        {
            neighbours[roomLoaderIndex].gameObject.SetActive(false);
        }
    }


    private void InitNeighbours()
    {
        void tryToAddNeighbour(RoomManager neighbour, RoomSide side)
        {
            if (neighbour)
            {
                neighbours[(int) side] = neighbour;
            }
        }

        tryToAddNeighbour(topLeftRoom, RoomSide.TopLeft);
        tryToAddNeighbour(bottomLeftRoom, RoomSide.BottomLeft);
        tryToAddNeighbour(topRightRoom, RoomSide.TopRight);
        tryToAddNeighbour(bottomRightRoom, RoomSide.BottomRight);
    }

    private void InitNeighbourLoaders()
    {
        void tryToAddNeighbourLoader(NextRoomLoader neighbourLoader, RoomSide side)
        {
            if (neighbourLoader)
            {
                neighbourLoaders[(int) side] = neighbourLoader;
                neighbourLoader.RegisterRoomManager(this);
            }
        }

        tryToAddNeighbourLoader(topLeftLoader, RoomSide.TopLeft);
        tryToAddNeighbourLoader(bottomLeftLoader, RoomSide.BottomLeft);
        tryToAddNeighbourLoader(topRightLoader, RoomSide.TopRight);
        tryToAddNeighbourLoader(bottomRightLoader, RoomSide.BottomRight);
    }

    private void CheckMatchingLoaderToRoom()
    {
        for (int i = 0; i < neighboursAmount; ++i)
        {
            if ((neighbours[i] is null && neighbourLoaders[i] is not null) ||
                (neighbours[i] is not null && neighbourLoaders[i] is null))
            {
                Debug.LogError(string.Format(
                    "Neighbour Loader {0} is set, but Neighbour Room Manager {0} is not",
                    (RoomSide) i));
            }
        }
    }

    private int RoomManagerIndex(RoomManager roomManager)
    {
        int index = Array.IndexOf(neighbours, roomManager);

        if (index == -1)
        {
            Debug.LogError("Cannot find neighbour");
            return neighboursAmount;
        }

        return index;
    }

    private int RoomLoaderIndex(NextRoomLoader nextRoomLoader)
    {
        int index = Array.IndexOf(neighbourLoaders, nextRoomLoader);

        if (index == -1)
        {
            Debug.LogError("Cannot find neighbour loader");
            return neighboursAmount;
        }

        return index;
    }

    private RoomSide RoomManagerSide(RoomManager roomManager)
    {
        return (RoomSide) RoomManagerIndex(roomManager);
    }

    private RoomSide RoomLoaderSide(NextRoomLoader nextRoomLoader)
    {
        return (RoomSide) RoomLoaderIndex(nextRoomLoader);
    }


    private void Awake()
    {
        InitNeighbours();
        InitNeighbourLoaders();
        CheckMatchingLoaderToRoom();
    }
}
