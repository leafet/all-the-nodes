using System.Numerics;
using EveryNode.Api.Models;

namespace EveryNode.Api.MockData;

public static class MockDataUtils
{
    static Random r = new Random();

    private const int NUMBER_OF_RANDOM_USERS = 10;
    private const int MAX_NODES_BUDGET = 3;
    
    private static int nodesIdStore = 0;
    private static int edgesIdStore = 0;
    private static int usersIdStore = 0;

    private static List<User> randomUsers = new List<User>();
    
    private static float RandomFloatFromRange(float min, float max)
    {
        return (float)r.NextDouble() * (max - min) + min;
    }
    
    public static void PopulateRandomUsers()
    {
        for (int i = 0; i < NUMBER_OF_RANDOM_USERS; i++)
        {
            int newUserId = usersIdStore++;
            
            randomUsers.Add(new User
            {
                Id = newUserId,
                Username = "Bob" + newUserId,
                NodesBudget = r.Next(0, MAX_NODES_BUDGET)
            });
        }
    }
    
    private static Node RandomNode()
    {
        int randomUserId = r.Next(0, randomUsers.Count);
        
        return new Node
        {
            Id = nodesIdStore++,
            X =  RandomFloatFromRange(-100f, 100f),
            Y =  RandomFloatFromRange(-100f, 100f),
            Owner = randomUsers[randomUserId]
        };
    }

    public static List<User> GetRandomUsers()
    {
        return randomUsers;
    }

    public static User GetRandomUser(List<User> users)
    {
        return users[r.Next(0, randomUsers.Count)];
    }
    
    public static void PopulateField(int numberOfNodes, int maxNumberOfEdges, Field field)
    {
        PopulateRandomUsers();
        
        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();
        
        for (int i = 0; i < numberOfNodes; i++)
        {
            nodes.Add(RandomNode());
        }

        for (int i = 0; i < maxNumberOfEdges; i++)
        {
            int randomStartId = r.Next(0, nodes.Count);
            int randomEndId = r.Next(0, nodes.Count);

            if (randomStartId == randomEndId)
            {
                continue;
            }
            
            edges.Add(new Edge
            {
                Id = edgesIdStore++,
                Start = nodes[randomStartId],
                End = nodes[randomEndId]
            });
        }

        field.Nodes = nodes;
        field.Edges = edges;
    }
}