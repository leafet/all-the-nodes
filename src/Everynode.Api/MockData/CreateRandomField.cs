using System.Numerics;
using EveryNode.Api.Models;

namespace EveryNode.Api.MockData;

public static class CreateRandomField
{
    static Random r = new Random();

    private const int NUMBER_OF_RANDOM_USERS = 10;
    
    private static int nodesIdStore = 0;
    private static int edgesIdStore = 0;
    private static int usersIdStore = 0;

    private static List<User> randomUsers = new List<User>();
    
    private static float RandomFloatFromRange(float min, float max)
    {
        return (float)r.NextDouble() * (max - min) + min;
    }
    
    private static void PopulateRandomUsers()
    {
        int newUserId = usersIdStore++;
        
        randomUsers.Add(new User
        {
            Id = newUserId,
            Username = "Bob" + newUserId,
        });
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
    
    public static Field Create(int numberOfNodes, int maxNumberOfEdges)
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
        
        Field randomField = new Field
        {
            Nodes =  nodes,
            Edges = edges,
        };
        
        return randomField;
    }
}