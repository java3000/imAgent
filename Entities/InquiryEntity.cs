using System;
using System.Collections.Generic;

namespace ImAgent.Entities
{
    [Serializable]
    public class InquiryEntity
    {
        public string Method { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TaskFile { get; set; }
        public string OutputFolder { get; set; }
        
        //for comparsion
        public string File1 { get; set; }
        public string File2 { get; set; }

        public List<TaskEntity> Tasks { get; set; }
    }
}
