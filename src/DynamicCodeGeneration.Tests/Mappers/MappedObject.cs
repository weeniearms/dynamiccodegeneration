namespace DynamicCodeGeneration.Tests.Mappers
{
    public class MappedObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((MappedObject)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Id;
                hashCode = (hashCode * 397) ^ (this.Name != null ? this.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Age;
                return hashCode;
            }
        }

        protected bool Equals(MappedObject other)
        {
            return this.Id == other.Id && string.Equals(this.Name, other.Name) && this.Age == other.Age;
        }
    }
}