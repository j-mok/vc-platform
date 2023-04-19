using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Data.SqlServer.Migrations.Data
{
    [DbContext(typeof(PlatformDbContext))]
    [Migration("20000000000001_FixUpdatedPlatformV2")]
    public class FixUpdatedPlatformV2 : Migration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                    IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.Platform.Data.Repositories.Migrations.Configuration'))
                    BEGIN
                        -- #### ASSETS ####

                        -- Update AssetEntry
                        alter table dbo.AssetEntry
                            drop constraint [PK_dbo.AssetEntry]

                        alter table dbo.AssetEntry
                            alter column CreatedDate datetime2 not null

                        alter table dbo.AssetEntry
                            alter column ModifiedDate datetime2 null

                        create index IX_AssetEntry_RelativeUrl_Name
                            on dbo.AssetEntry (RelativeUrl, Name)

                        drop index IX_AssetEntry_TenantId_TenantType on dbo.AssetEntry

                        alter table dbo.AssetEntry
                            add constraint PK_AssetEntry
                                primary key (Id)


                        -- #### DYNAMIC PROPERTIES ####

                        -- Drop FK constraints
                        alter table dbo.PlatformDynamicPropertyDictionaryItem
                            drop constraint [FK_dbo.PlatformDynamicPropertyDictionaryItem_dbo.PlatformDynamicProperty_PropertyId]

                        alter table dbo.PlatformDynamicPropertyName
                            drop constraint [FK_dbo.PlatformDynamicPropertyName_dbo.PlatformDynamicProperty_PropertyId]

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            drop constraint [FK_dbo.PlatformDynamicPropertyObjectValue_dbo.PlatformDynamicProperty_PropertyId]

                        alter table dbo.PlatformDynamicPropertyDictionaryItemName
                            drop constraint [FK_dbo.PlatformDynamicPropertyDictionaryItemName_dbo.PlatformDynamicPropertyDictionaryItem_DictionaryItemId]

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            drop constraint [FK_dbo.PlatformDynamicPropertyObjectValue_dbo.PlatformDynamicPropertyDictionaryItem_DictionaryItemId]

                        -- Update PlatformDynamicProperty
                        alter table dbo.PlatformDynamicProperty
                            drop constraint [PK_dbo.PlatformDynamicProperty]

                        alter table dbo.PlatformDynamicProperty
                            alter column Id nvarchar(128) not null

                        alter table dbo.PlatformDynamicProperty
                            alter column CreatedDate datetime2 not null

                        alter table dbo.PlatformDynamicProperty
                            alter column ModifiedDate datetime2 null

                        drop index IX_PlatformDynamicProperty_ObjectType_Name on dbo.PlatformDynamicProperty

                        create unique index IX_PlatformDynamicProperty_ObjectType_Name
                            on dbo.PlatformDynamicProperty (ObjectType, Name)
                            where [ObjectType] IS NOT NULL AND [Name] IS NOT NULL

                        alter table dbo.PlatformDynamicProperty
                            add constraint PK_PlatformDynamicProperty
                                primary key (Id)

                        -- Update PlatformDynamicPropertyDictionaryItem
                        alter table dbo.PlatformDynamicPropertyDictionaryItem
                            drop constraint [PK_dbo.PlatformDynamicPropertyDictionaryItem]

                        alter table dbo.PlatformDynamicPropertyDictionaryItem
                            alter column Id nvarchar(128) not null

                        alter table dbo.PlatformDynamicPropertyDictionaryItem
                            alter column PropertyId nvarchar(128) null

                        alter table dbo.PlatformDynamicPropertyDictionaryItem
                            alter column CreatedDate datetime2 not null

                        alter table dbo.PlatformDynamicPropertyDictionaryItem
                            alter column ModifiedDate datetime2 null

                        drop index IX_PlatformDynamicPropertyDictionaryItem_PropertyId_Name on dbo.PlatformDynamicPropertyDictionaryItem

                        create unique index IX_PlatformDynamicPropertyDictionaryItem_PropertyId_Name
                            on dbo.PlatformDynamicPropertyDictionaryItem (PropertyId, Name)
                            where [PropertyId] IS NOT NULL AND [Name] IS NOT NULL

                        alter table dbo.PlatformDynamicPropertyDictionaryItem
                            add constraint PK_PlatformDynamicPropertyDictionaryItem
                                primary key (Id)

                        -- Update PlatformDynamicPropertyDictionaryItemName
                        alter table dbo.PlatformDynamicPropertyDictionaryItemName
                            alter column Id nvarchar(128) not null

                        alter table dbo.PlatformDynamicPropertyDictionaryItemName
                            alter column DictionaryItemId nvarchar(128) null

                        alter table dbo.PlatformDynamicPropertyDictionaryItemName
                            alter column CreatedDate datetime2 not null

                        alter table dbo.PlatformDynamicPropertyDictionaryItemName
                            alter column ModifiedDate datetime2 null

                        drop index IX_PlatformDynamicPropertyDictionaryItemName_DictionaryItemId_Locale_Name on dbo.PlatformDynamicPropertyDictionaryItemName

                        create unique index IX_PlatformDynamicPropertyDictionaryItemName_DictionaryItemId_Locale_Name
                            on dbo.PlatformDynamicPropertyDictionaryItemName (DictionaryItemId, Locale, Name)
                            where [DictionaryItemId] IS NOT NULL AND [Locale] IS NOT NULL AND [Name] IS NOT NULL

                        alter table dbo.PlatformDynamicPropertyDictionaryItemName
                            drop constraint [PK_dbo.PlatformDynamicPropertyDictionaryItemName]

                        alter table dbo.PlatformDynamicPropertyDictionaryItemName
                            add constraint PK_PlatformDynamicPropertyDictionaryItemName
                                primary key (Id)

                        -- Update PlatformDynamicPropertyName
                        alter table dbo.PlatformDynamicPropertyName
                            alter column Id nvarchar(128) not null

                        alter table dbo.PlatformDynamicPropertyName
                            alter column PropertyId nvarchar(128) null

                        alter table dbo.PlatformDynamicPropertyName
                            alter column CreatedDate datetime2 not null

                        alter table dbo.PlatformDynamicPropertyName
                            alter column ModifiedDate datetime2 null

                        drop index IX_PlatformDynamicPropertyName_PropertyId_Locale_Name on dbo.PlatformDynamicPropertyName

                        create unique index IX_PlatformDynamicPropertyName_PropertyId_Locale_Name
                            on dbo.PlatformDynamicPropertyName (PropertyId, Locale, Name)
                            where [PropertyId] IS NOT NULL AND [Locale] IS NOT NULL AND [Name] IS NOT NULL

                        alter table dbo.PlatformDynamicPropertyName
                            drop constraint [PK_dbo.PlatformDynamicPropertyName]

                        alter table dbo.PlatformDynamicPropertyName
                            add constraint PK_PlatformDynamicPropertyName
                                primary key (Id)

                        -- Update PlatformDynamicPropertyObjectValue
                        alter table dbo.PlatformDynamicPropertyObjectValue
                            alter column Id nvarchar(128) not null

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            alter column DateTimeValue datetime2 null

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            alter column PropertyId nvarchar(128) null

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            alter column DictionaryItemId nvarchar(128) null

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            alter column CreatedDate datetime2 not null

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            alter column ModifiedDate datetime2 null

                        drop index IX_DictionaryItemId on dbo.PlatformDynamicPropertyObjectValue

                        create index IX_PlatformDynamicPropertyObjectValue_DictionaryItemId
                            on dbo.PlatformDynamicPropertyObjectValue (DictionaryItemId)

                        create index IX_PlatformDynamicPropertyObjectValue_PropertyId
                            on dbo.PlatformDynamicPropertyObjectValue (PropertyId)

                        drop index IX_PropertyId on dbo.PlatformDynamicPropertyObjectValue

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            drop constraint [PK_dbo.PlatformDynamicPropertyObjectValue]

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            add constraint PK_PlatformDynamicPropertyObjectValue
                                primary key (Id)

                        -- Restore FK constraints
                        alter table dbo.PlatformDynamicPropertyDictionaryItemName
                            add constraint FK_PlatformDynamicPropertyDictionaryItemName_PlatformDynamicPropertyDictionaryItem_DictionaryItemId
                                foreign key (DictionaryItemId) references dbo.PlatformDynamicPropertyDictionaryItem
                                    on delete cascade

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            add constraint FK_PlatformDynamicPropertyObjectValue_PlatformDynamicPropertyDictionaryItem_DictionaryItemId
                                foreign key (DictionaryItemId) references dbo.PlatformDynamicPropertyDictionaryItem

                        alter table dbo.PlatformDynamicPropertyDictionaryItem
                            add constraint FK_PlatformDynamicPropertyDictionaryItem_PlatformDynamicProperty_PropertyId
                                foreign key (PropertyId) references dbo.PlatformDynamicProperty
                                    on delete cascade

                        alter table dbo.PlatformDynamicPropertyName
                            add constraint FK_PlatformDynamicPropertyName_PlatformDynamicProperty_PropertyId
                                foreign key (PropertyId) references dbo.PlatformDynamicProperty
                                    on delete cascade

                        alter table dbo.PlatformDynamicPropertyObjectValue
                            add constraint FK_PlatformDynamicPropertyObjectValue_PlatformDynamicProperty_PropertyId
                                foreign key (PropertyId) references dbo.PlatformDynamicProperty
                                    on delete cascade


                        -- #### OPERATION LOG ####

                        -- Update PlatformOperationLog
                        alter table dbo.PlatformOperationLog
                            alter column CreatedDate datetime2 not null

                        alter table dbo.PlatformOperationLog
                            alter column ModifiedDate datetime2 null

                        alter table dbo.PlatformOperationLog
                            drop constraint [PK_dbo.PlatformOperationLog]

                        alter table dbo.PlatformOperationLog
                            add constraint PK_PlatformOperationLog
                                primary key (Id)


                        -- #### SETTINGS ####

                        -- Drop FK constraints
                        alter table dbo.PlatformSettingValue
                            drop constraint [FK_dbo.PlatformSettingValue_dbo.PlatformSetting_SettingId]

                        -- Update PlatformSetting
                        alter table dbo.PlatformSetting
                            drop constraint [PK_dbo.PlatformSetting]

                        alter table dbo.PlatformSetting
                            alter column Id nvarchar(128) not null

                        alter table dbo.PlatformSetting
                            drop column Description

                        alter table dbo.PlatformSetting
                            alter column CreatedDate datetime2 not null

                        alter table dbo.PlatformSetting
                            alter column ModifiedDate datetime2 null

                        alter table dbo.PlatformSetting
                            add constraint PK_PlatformSetting
                                primary key (Id)

                        -- Update PlatformSettingValue
                        alter table dbo.PlatformSettingValue
                            drop constraint [PK_dbo.PlatformSettingValue]

                        alter table dbo.PlatformSettingValue
                            alter column Id nvarchar(128) not null

                        alter table dbo.PlatformSettingValue
                            alter column DateTimeValue datetime2 null

                        alter table dbo.PlatformSettingValue
                            drop column Locale

                        alter table dbo.PlatformSettingValue
                            alter column SettingId nvarchar(128) null

                        alter table dbo.PlatformSettingValue
                            alter column CreatedDate datetime2 not null

                        alter table dbo.PlatformSettingValue
                            alter column ModifiedDate datetime2 null

                        create index IX_PlatformSettingValue_SettingId
                            on dbo.PlatformSettingValue (SettingId)

                        drop index IX_SettingId on dbo.PlatformSettingValue

                        alter table dbo.PlatformSettingValue
                            add constraint PK_PlatformSettingValue
                                primary key (Id)

                        -- Restore FK constraints
                        alter table dbo.PlatformSettingValue
                            add constraint FK_PlatformSettingValue_PlatformSetting_SettingId
                                foreign key (SettingId) references dbo.PlatformSetting
                                    on delete cascade
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This method defined empty
        }
    }
}
