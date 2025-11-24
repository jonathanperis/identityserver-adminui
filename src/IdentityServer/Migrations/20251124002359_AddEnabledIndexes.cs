using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class AddEnabledIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DynamicSamlProviders_Enabled",
                table: "DynamicSamlProviders",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicOidcProviders_Enabled",
                table: "DynamicOidcProviders",
                column: "Enabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DynamicSamlProviders_Enabled",
                table: "DynamicSamlProviders");

            migrationBuilder.DropIndex(
                name: "IX_DynamicOidcProviders_Enabled",
                table: "DynamicOidcProviders");
        }
    }
}
