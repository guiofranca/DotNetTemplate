using FluentMigrator;
using Template.Data.Repositories.Shared;
using Template.Core.Models;

namespace Template.MigrationRunner.Migrations;

[Migration(20230101000003)]
public class CreateBlogPostTable : Migration
{
    public override void Up()
    {
        Create.Table(TableName.Of<BlogPost>())
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("UserId").AsGuid()
            .WithColumn("Title").AsString().NotNullable()
            .WithColumn("Content").AsString(500).NotNullable()
            .WithColumn("CreatedAt").AsDateTime()
            .WithColumn("UpdatedAt").AsDateTime();

        Create.ForeignKey()
            .FromTable(TableName.Of<BlogPost>()).ForeignColumn("UserId")
            .ToTable(TableName.Of<User>()).PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table(TableName.Of<BlogPost>());
    }
}
