using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ImAgent.Entities
{
    [Serializable]
    public class FileEntity : IEquatable<FileEntity>
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }

        public string ApplicationName { get; set; }
        public string Author { get; set; }
        public string Company { get; set; }
        public string Copyright { get; set; }
        public string Version { get; set; }
        public string FileDescription { get; set; }
        public string FileExtension { get; set; }
        public string ItemType { get; set; }
        public string Language { get; set; }
        public string SoftwareUsed { get; set; }
        public string Crc32 { get; set; }

        public DateTime DateModified { get; set; }
        public DateTime DateAccessed { get; set; }
        public DateTime DateCreated { get; set; }

        public override string ToString()
        {
            //todo correct tostring
            return base.ToString();
        }

        public override bool Equals(object obj) => Equals(obj as FileEntity);

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Name);
            hash.Add(Path);
            hash.Add(Size);
            hash.Add(DateModified);
            hash.Add(DateAccessed);
            hash.Add(DateCreated);
            hash.Add(Crc32);
            return hash.ToHashCode();
        }

        public bool Equals([AllowNull] FileEntity other)
        {
            if (other is null) return false;

            return Name == other.Name &&
                   Path == other.Path &&
                   Size == other.Size &&
                   DateModified == other.DateModified &&
                   DateAccessed == other.DateAccessed &&
                   DateCreated == other.DateCreated &&
                   Crc32 == other.Crc32;
        }

        public static bool operator ==(FileEntity left, FileEntity right)
        {
            return EqualityComparer<FileEntity>.Default.Equals(left, right);
        }

        public static bool operator !=(FileEntity left, FileEntity right)
        {
            return !(left == right);
        }
    }
}
