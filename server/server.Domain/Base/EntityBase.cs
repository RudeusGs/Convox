namespace server.Domain.Base
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual DateTime? DeletedDate { get; set; }
        public bool IsDeleted => DeletedDate.HasValue;
        public void MarkUpdated()
        {
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkDeleted()
        {
            DeletedDate = DateTime.UtcNow;
            MarkUpdated();
        }
    }
}
