using System.Text.Json.Serialization;

namespace Feature.Locations.FetchAll;

public record LocationNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Depth { get; set; }

    public int ParentId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? LocationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DisplayOrder { get; set; }


    [JsonIgnore] public List<LocationNode> Children { get; set; } = new(); // List of child nodes

    public LocationNode(int id, string name, int depth, int parentId, int displayOrder)
    {
        Id = id;
        Name = name;
        Depth = depth;
        ParentId = parentId;
        DisplayOrder = displayOrder;
    }

    // Helper method to add child nodes
    public void AddChild(LocationNode child)
    {
        Children.Add(child);
    }
}

public static class LocationNodeExtensions
{
    public static List<LocationNode> Traverse(this LocationNode root)
    {
        var result = new List<LocationNode>();
        if (root == null) return result;

        // Initialize the queue with the root node
        Queue<LocationNode> queue = new Queue<LocationNode>();
        queue.Enqueue(root);

        // Perform BFS traversal
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            result.Add(currentNode);

            foreach (var child in currentNode.Children)
            {
                queue.Enqueue(child);
            }
        }

        return result;
    }
}