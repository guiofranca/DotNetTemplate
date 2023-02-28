using FluentMigrator;
using Template.Data.Repositories.Shared;
using Template.Core.Models;

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

        Insert.IntoTable(TableName.Of<Role>()).Row(new
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        });
    }

    public override void Down()
    {
        Delete.Table(TableName.Of<Role>());
    }
}
