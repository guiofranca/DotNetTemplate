using FluentMigrator;
using Template.Data.Repositories.Shared;
using Template.Domain.Models;

namespace Template.MigrationRunner.Migrations;

[Migration(20230101000001)]
public class CreateRolesTable : Migration
{
    public override void Up()
    {
        Create.Table(TableName.Of<Role>())
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString().Unique()
            .WithColumn("CreatedAt").AsDateTime()
            .WithColumn("UpdatedAt").AsDateTime();
    }

    public override void Down()
    {
        Delete.Table(TableName.Of<Role>());
    }
}
