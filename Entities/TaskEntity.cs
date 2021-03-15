using System;

namespace ImAgent.Entities
{
    [Serializable]
    public class TaskEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
    }
}
