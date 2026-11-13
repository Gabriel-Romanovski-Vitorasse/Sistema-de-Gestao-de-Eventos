using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestão.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Capacidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Capacidade = table.Column<int>(type: "int", nullable: false),
                    PrecoInscricao = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrganizadorId = table.Column<int>(type: "int", nullable: false),
                    LocalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventos_Locais_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Eventos_Organizadores_OrganizadorId",
                        column: x => x.OrganizadorId,
                        principalTable: "Organizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PessoasFisicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PessoasFisicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PessoasFisicas_Organizadores_Id",
                        column: x => x.Id,
                        principalTable: "Organizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PessoasJuridicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PessoasJuridicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PessoasJuridicas_Organizadores_Id",
                        column: x => x.Id,
                        principalTable: "Organizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Palestrantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    MiniBio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Palestrantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Palestrantes_Pessoas_Id",
                        column: x => x.Id,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Instituicao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participantes_Pessoas_Id",
                        column: x => x.Id,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PessoaId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificados_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificados_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conferencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Tema = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NumeroSessoes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conferencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conferencias_Eventos_Id",
                        column: x => x.Id,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workshops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CargaHoraria = table.Column<int>(type: "int", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workshops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workshops_Eventos_Id",
                        column: x => x.Id,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoPalestrante",
                columns: table => new
                {
                    EventosId = table.Column<int>(type: "int", nullable: false),
                    PalestrantesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPalestrante", x => new { x.EventosId, x.PalestrantesId });
                    table.ForeignKey(
                        name: "FK_EventoPalestrante_Eventos_EventosId",
                        column: x => x.EventosId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoPalestrante_Palestrantes_PalestrantesId",
                        column: x => x.PalestrantesId,
                        principalTable: "Palestrantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inscricoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataInscricao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ParticipanteId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscricoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscricoes_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscricoes_Participantes_ParticipanteId",
                        column: x => x.ParticipanteId,
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metodo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    InscricaoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Inscricoes_InscricaoId",
                        column: x => x.InscricaoId,
                        principalTable: "Inscricoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_Codigo",
                table: "Certificados",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_EventoId",
                table: "Certificados",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_PessoaId_EventoId",
                table: "Certificados",
                columns: new[] { "PessoaId", "EventoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventoPalestrante_PalestrantesId",
                table: "EventoPalestrante",
                column: "PalestrantesId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_LocalId",
                table: "Eventos",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_OrganizadorId",
                table: "Eventos",
                column: "OrganizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_EventoId",
                table: "Inscricoes",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_ParticipanteId_EventoId",
                table: "Inscricoes",
                columns: new[] { "ParticipanteId", "EventoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizadores_Email",
                table: "Organizadores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_InscricaoId",
                table: "Pagamentos",
                column: "InscricaoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Cpf",
                table: "Pessoas",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Email",
                table: "Pessoas",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PessoasFisicas_Cpf",
                table: "PessoasFisicas",
                column: "Cpf",
                unique: true,
                filter: "[Cpf] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PessoasJuridicas_Cnpj",
                table: "PessoasJuridicas",
                column: "Cnpj",
                unique: true,
                filter: "[Cnpj] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificados");

            migrationBuilder.DropTable(
                name: "Conferencias");

            migrationBuilder.DropTable(
                name: "EventoPalestrante");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "PessoasFisicas");

            migrationBuilder.DropTable(
                name: "PessoasJuridicas");

            migrationBuilder.DropTable(
                name: "Workshops");

            migrationBuilder.DropTable(
                name: "Palestrantes");

            migrationBuilder.DropTable(
                name: "Inscricoes");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Participantes");

            migrationBuilder.DropTable(
                name: "Locais");

            migrationBuilder.DropTable(
                name: "Organizadores");

            migrationBuilder.DropTable(
                name: "Pessoas");
        }
    }
}
