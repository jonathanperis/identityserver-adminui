using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicOidcProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Authority = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ClientId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ClientSecret = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ResponseType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Scopes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CallbackPath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GetClaimsFromUserInfoEndpoint = table.Column<bool>(type: "boolean", nullable: false),
                    SaveTokens = table.Column<bool>(type: "boolean", nullable: false),
                    MetadataAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RequireHttpsMetadata = table.Column<bool>(type: "boolean", nullable: false),
                    Scheme = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    ProviderType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicOidcProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicSamlProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpEntityId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IdpEntityId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IdpSingleSignOnUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IdpMetadataUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AcsPath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IdpCertificate = table.Column<string>(type: "text", nullable: true),
                    SpCertificate = table.Column<string>(type: "text", nullable: true),
                    SpCertificatePassword = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SignAuthenticationRequests = table.Column<bool>(type: "boolean", nullable: false),
                    WantAssertionsSigned = table.Column<bool>(type: "boolean", nullable: false),
                    NameIdFormat = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BindingType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Scheme = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    ProviderType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSamlProviders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicOidcProviders_Scheme",
                table: "DynamicOidcProviders",
                column: "Scheme",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSamlProviders_Scheme",
                table: "DynamicSamlProviders",
                column: "Scheme",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicOidcProviders");

            migrationBuilder.DropTable(
                name: "DynamicSamlProviders");
        }
    }
}
