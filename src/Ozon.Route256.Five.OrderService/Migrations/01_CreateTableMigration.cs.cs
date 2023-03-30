using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Migrations;

[Migration(1)]
public class CreateTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("region")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("stockcoordinate").AsCustom("point").NotNullable();

        Create.Table("customer")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("first_name").AsString().NotNullable()
            .WithColumn("last_name").AsString().NotNullable()
            .WithColumn("phone_number").AsString().NotNullable();

        Create.Table("order")
            .WithColumn("id").AsInt64().PrimaryKey()
            .WithColumn("date_create").AsDateTimeOffset().NotNullable()
            .WithColumn("date_update").AsDateTimeOffset().NotNullable()
            .WithColumn("state").AsInt32().NotNullable()
            .WithColumn("type").AsInt32().NotNullable()
            .WithColumn("address").AsCustom("jsonb").NotNullable()
            .WithColumn("shipments_count").AsInt32().NotNullable()
            .WithColumn("price").AsDouble().NotNullable()
            .WithColumn("weight").AsInt64().NotNullable()
            .WithColumn("customer_id").AsInt32().ForeignKey("customer", "id")
            .WithColumn("region_id").AsInt32().ForeignKey("region", "id");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}

