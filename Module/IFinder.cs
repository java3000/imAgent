using System.Collections.Generic;
using ImAgent.Entities;

namespace ImAgent.Module
{
    interface IFinder
    {
        public bool Recursive {get; set; }

        abstract IList<FileEntity> Search(string where, string what, string machine);
        abstract IList<FileEntity> Search(TaskEntity task);
    }
}
