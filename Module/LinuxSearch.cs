using ImAgent.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImAgent.Module
{
    class LinuxSearch : IFinder
    {
        public bool Recursive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<FileEntity> Search(string where, string what, string machine)
        {
            throw new NotImplementedException();
        }

        public IList<FileEntity> Search(TaskEntity task)
        {
            throw new NotImplementedException();
        }
    }
}
