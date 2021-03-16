using System.Collections.Generic;
using ImAgent.Entities;

namespace ImAgent.Module
{
    interface IFinder
    {
        public bool Recursive {get; set; }

        abstract List<FileEntity> Search(string where, string what, string machine);
        abstract List<FileEntity> Search(TaskEntity task);
    }
}
