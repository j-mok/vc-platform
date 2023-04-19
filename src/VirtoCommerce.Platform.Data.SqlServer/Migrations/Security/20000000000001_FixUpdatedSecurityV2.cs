using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VirtoCommerce.Platform.Security.Repositories;

namespace VirtoCommerce.Platform.Data.SqlServer.Migrations.Security
{
    [DbContext(typeof(SecurityDbContext))]
    [Migration("20000000000001_FixUpdatedSecurityV2")]
    public class FixUpdatedSecurityV2 : Migration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                    IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.Foundation.Data.Security.Identity.SecurityDbContext'))
                    BEGIN
                        -- #### ASP.NET IDENTITY ####

                        -- Drop FK constraints
                        alter table dbo.AspNetUserClaims
                            drop constraint [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]

                        alter table dbo.AspNetUserLogins
                            drop constraint [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]

                        alter table dbo.AspNetUserRoles
                            drop constraint [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]

                        alter table dbo.AspNetUserTokens
                            drop constraint [FK_AspNetUserTokens_AspNetUsers_UserId]

                        alter table dbo.AspNetUserRoles
                            drop constraint [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]

                        alter table dbo.AspNetRoleClaims
                            drop constraint [FK_AspNetRoleClaims_AspNetRoles_RoleId]

                        -- Update AspNetRoles
                        alter table dbo.AspNetRoles
                            alter column Name nvarchar(256) null

                        alter table dbo.AspNetRoles
                            drop constraint [PK_dbo.AspNetRoles]

                        alter table dbo.AspNetRoles
                            add constraint PK_AspNetRoles
                                primary key (Id)

                        -- Update AspNetUserClaims
                        create index IX_AspNetUserClaims_UserId
                            on dbo.AspNetUserClaims (UserId)

                        drop index IX_UserId on dbo.AspNetUserClaims

                        alter table dbo.AspNetUserClaims
                            drop constraint [PK_dbo.AspNetUserClaims]

                        alter table dbo.AspNetUserClaims
                            add constraint PK_AspNetUserClaims
                                primary key (Id)

                        -- Update AspNetUserLogins
                        create index IX_AspNetUserLogins_UserId
                            on dbo.AspNetUserLogins (UserId)

                        drop index IX_UserId on dbo.AspNetUserLogins

                        alter table dbo.AspNetUserLogins
                            drop constraint [PK_dbo.AspNetUserLogins]

                        alter table dbo.AspNetUserLogins
                            add constraint PK_AspNetUserLogins
                                primary key (LoginProvider, ProviderKey)

                        -- Update AspNetUserRoles
                        create index IX_AspNetUserRoles_RoleId
                            on dbo.AspNetUserRoles (RoleId)

                        drop index IX_RoleId on dbo.AspNetUserRoles

                        drop index IX_UserId on dbo.AspNetUserRoles

                        alter table dbo.AspNetUserRoles
                            drop constraint [PK_dbo.AspNetUserRoles]

                        alter table dbo.AspNetUserRoles
                            add constraint PK_AspNetUserRoles
                                primary key (UserId, RoleId)

                        -- Update AspNetUsers
                        alter table dbo.AspNetUsers
                            drop column LockoutEndDateUtc

                        alter table dbo.AspNetUsers
                            alter column UserName nvarchar(256) null

                        declare @defaultConstraintName sysname

                        -- Drop default value on IsAdministrator
                        select @defaultConstraintName = name from sys.default_constraints
                            where parent_object_id = object_id('dbo.AspNetUsers')
                                and parent_column_id = (
                                    select column_id from sys.columns
                                    where name = 'IsAdministrator' and object_id = object_id('dbo.AspNetUsers'))
                        exec('alter table dbo.AspNetUsers drop constraint ' + @defaultConstraintName)

                        -- Drop default value on PasswordExpired
                        select @defaultConstraintName = name from sys.default_constraints
                        where parent_object_id = object_id('dbo.AspNetUsers')
                          and parent_column_id = (
                            select column_id from sys.columns
                            where name = 'PasswordExpired' and object_id = object_id('dbo.AspNetUsers'))
                        exec('alter table dbo.AspNetUsers drop constraint ' + @defaultConstraintName)

                        create index EmailIndex
                            on dbo.AspNetUsers (NormalizedEmail)

                        drop index UserNameIndex on dbo.AspNetUsers

                        create unique index UserNameIndex
                            on dbo.AspNetUsers (NormalizedUserName)
                            where [NormalizedUserName] IS NOT NULL

                        alter table dbo.AspNetUsers
                            drop constraint [PK_dbo.AspNetUsers]

                        alter table dbo.AspNetUsers
                            add constraint PK_AspNetUsers
                                primary key (Id)

                        -- Restore FK constraints
                        alter table dbo.AspNetUserClaims
                            add constraint FK_AspNetUserClaims_AspNetUsers_UserId
                                foreign key (UserId) references dbo.AspNetUsers
                                    on delete cascade

                        alter table dbo.AspNetUserLogins
                            add constraint FK_AspNetUserLogins_AspNetUsers_UserId
                                foreign key (UserId) references dbo.AspNetUsers
                                    on delete cascade

                        alter table dbo.AspNetUserRoles
                            add constraint FK_AspNetUserRoles_AspNetUsers_UserId
                                foreign key (UserId) references dbo.AspNetUsers
                                    on delete cascade

                        alter table dbo.AspNetUserTokens
                            add constraint FK_AspNetUserTokens_AspNetUsers_UserId
                                foreign key (UserId) references dbo.AspNetUsers
                                    on delete cascade

                        alter table dbo.AspNetUserRoles
                            add constraint FK_AspNetUserRoles_AspNetRoles_RoleId
                                foreign key (RoleId) references dbo.AspNetRoles
                                    on delete cascade

                        alter table dbo.AspNetRoleClaims
                            add constraint FK_AspNetRoleClaims_AspNetRoles_RoleId
                                foreign key (RoleId) references AspNetRoles
                                    on delete cascade

                        -- #### OPENIDDICT ####

                        -- Update OpenIddictApplications
                        alter table dbo.OpenIddictApplications
                            alter column ConcurrencyToken nvarchar(50) null

                        alter table dbo.OpenIddictApplications
                            alter column ClientId nvarchar(100) not null

                        alter table dbo.OpenIddictApplications
                            alter column Type nvarchar(25) not null

                        create unique index IX_OpenIddictApplications_ClientId
                            on dbo.OpenIddictApplications (ClientId)

                        -- Update OpenIddictAuthorizations
                        alter table dbo.OpenIddictAuthorizations
                            alter column ConcurrencyToken nvarchar(50) null

                        alter table dbo.OpenIddictAuthorizations
                            alter column Status nvarchar(25) not null

                        alter table dbo.OpenIddictAuthorizations
                            alter column Subject nvarchar(450) not null

                        alter table dbo.OpenIddictAuthorizations
                            alter column Type nvarchar(25) not null

                        create index IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type
                            on dbo.OpenIddictAuthorizations (ApplicationId, Status, Subject, Type)

                        -- Update OpenIddictScopes
                        alter table dbo.OpenIddictScopes
                            alter column ConcurrencyToken nvarchar(50) null

                        alter table dbo.OpenIddictScopes
                            alter column Name nvarchar(200) not null

                        create unique index IX_OpenIddictScopes_Name
                            on dbo.OpenIddictScopes (Name)

                        -- Update OpenIddictTokens
                        alter table dbo.OpenIddictTokens
                            alter column ConcurrencyToken nvarchar(50) null

                        alter table dbo.OpenIddictTokens
                            alter column ReferenceId nvarchar(100) null

                        alter table dbo.OpenIddictTokens
                            alter column Status nvarchar(25) not null

                        alter table dbo.OpenIddictTokens
                            alter column Subject nvarchar(450) not null

                        alter table dbo.OpenIddictTokens
                            alter column Type nvarchar(25) not null

                        create index IX_OpenIddictTokens_AuthorizationId
                            on dbo.OpenIddictTokens (AuthorizationId)

                        create unique index IX_OpenIddictTokens_ReferenceId
                            on dbo.OpenIddictTokens (ReferenceId)
                            where [ReferenceId] IS NOT NULL

                        create index IX_OpenIddictTokens_ApplicationId_Status_Subject_Type
                            on dbo.OpenIddictTokens (ApplicationId, Status, Subject, Type)
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This method defined empty
        }
    }
}
