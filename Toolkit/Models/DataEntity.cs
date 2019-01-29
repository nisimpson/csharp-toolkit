using System;

namespace Nerdshoe.Models
{
    public abstract class DataEntityWithId<TDerived>
        : IEquatable<DataEntityWithId<TDerived>>
    {
        public int Id { get; set; }

        public override bool Equals(object obj) => this.EqualsStrict(obj);
      
        public bool Equals(DataEntityWithId<TDerived> other) => Id == other?.Id;

        public override int GetHashCode() => Id.GetHashCode();     
    }

    public abstract class DataEntityWithGuid<TDerived>
        : IEquatable<DataEntityWithGuid<TDerived>>
    {
        public DataEntityWithGuid(string guid) { Guid = guid; }

        public string Guid { get; set; }

        public override bool Equals(object obj) => this.EqualsStrict(obj);

        public bool Equals(DataEntityWithGuid<TDerived> other) => Guid == other?.Guid;

        public override int GetHashCode() => Guid.GetHashCode();
    }
}
