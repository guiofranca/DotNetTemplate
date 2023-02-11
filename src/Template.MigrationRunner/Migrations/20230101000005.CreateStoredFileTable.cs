using FluentMigrator;
using Template.Data.Repositories.Shared;
using Template.Domain.Models;

namespace Template.MigrationRunner.Migrations;

[Migration(20230101000005)]
public class CreateStoredFileTable : Migration
{
    private readonly string Table = TableName.Of<StoredFile>();
    public override void Up()
    {
        Create.Table(Table)
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString()
            .WithColumn("Size").AsInt32()
            .WithColumn("Owner").AsString().Nullable()
            .WithColumn("OwnerId").AsGuid().Nullable()
            .WithColumn("CreatedAt").AsDateTime()
            .WithColumn("UpdatedAt").AsDateTime();

        Create.Index().OnTable(Table).OnColumn("OwnerId");
    }

    public override void Down()
    {
        Delete.Table(Table);
    }
}
