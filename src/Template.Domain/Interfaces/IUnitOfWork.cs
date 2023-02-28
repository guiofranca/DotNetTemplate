namespace Template.Core.Interfaces;

public interface IUnitOfWork
{
     void BeginTransaction();
     void Commit();
     void Rollback();
}
