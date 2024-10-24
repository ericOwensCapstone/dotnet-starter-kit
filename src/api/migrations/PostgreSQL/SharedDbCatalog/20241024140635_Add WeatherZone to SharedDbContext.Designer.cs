﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SharedDbContextProject;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.SharedDbCatalog
{
    [DbContext(typeof(SharedDbContext))]
    [Migration("20241024140635_Add WeatherZone to SharedDbContext")]
    partial class AddWeatherZonetoSharedDbContext
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("sharedcatalog")
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FSH.Starter.WebApi.AnimalTypeCatalog.Domain.AnimalType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("ArrivalCostPerCwtMean")
                        .HasColumnType("double precision");

                    b.Property<double>("ArrivalCostPerCwtStdDev")
                        .HasColumnType("double precision");

                    b.Property<double>("ArrivalHeadCountMean")
                        .HasColumnType("double precision");

                    b.Property<double>("ArrivalHeadCountStdDev")
                        .HasColumnType("double precision");

                    b.Property<double>("ArrivalWeightMean")
                        .HasColumnType("double precision");

                    b.Property<double>("ArrivalWeightStdDev")
                        .HasColumnType("double precision");

                    b.Property<double>("CarcassYieldMean")
                        .HasColumnType("double precision");

                    b.Property<double>("CarcassYieldStdDev")
                        .HasColumnType("double precision");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<double>("DiseaseIncidenceMean")
                        .HasColumnType("double precision");

                    b.Property<double>("DiseaseIncidenceStdDev")
                        .HasColumnType("double precision");

                    b.Property<double>("FcrMean")
                        .HasColumnType("double precision");

                    b.Property<double>("FcrStdDev")
                        .HasColumnType("double precision");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<double>("LowerCriticalTemp")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("QualityGradeMean")
                        .HasColumnType("double precision");

                    b.Property<double>("QualityGradeStdDev")
                        .HasColumnType("double precision");

                    b.Property<double>("UpperCriticalTemp")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("AnimalTypes", "sharedcatalog");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.GrowthTreatment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("DollarsPerHead")
                        .HasColumnType("numeric");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("GrowthTreatments", "sharedcatalog");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.LifecycleProgram", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Rating")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("LifecyclePrograms", "sharedcatalog");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.LifecycleProgramStage", b =>
                {
                    b.Property<Guid>("LifecycleProgramId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("LifecycleStageId")
                        .HasColumnType("uuid");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.HasKey("LifecycleProgramId", "LifecycleStageId");

                    b.HasIndex("LifecycleStageId");

                    b.ToTable("LifecycleProgramStages", "sharedcatalog");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.LifecycleStageCatalog.Domain.LifecycleStage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("AdfiStdDev")
                        .HasColumnType("double precision");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("GrowthTreatmentId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<int>("MaxHead")
                        .HasColumnType("integer");

                    b.Property<int>("MergeableDuration")
                        .HasColumnType("integer");

                    b.Property<double>("MergeableWeightRange")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("PreventativeTreatmentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RationId")
                        .HasColumnType("uuid");

                    b.Property<double>("TargetAdfi")
                        .HasColumnType("double precision");

                    b.Property<double>("TargetWeight")
                        .HasColumnType("double precision");

                    b.Property<double>("TargetWeightRangeForSort")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("GrowthTreatmentId");

                    b.HasIndex("PreventativeTreatmentId");

                    b.HasIndex("RationId");

                    b.ToTable("LifecycleStages", "sharedcatalog");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.PreventativeTreatment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("DollarsPerHead")
                        .HasColumnType("numeric");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PreventativeTreatments", "sharedcatalog");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.RationCatalog.Domain.Ration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("DollarsPerPound")
                        .HasColumnType("numeric");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Rations", "sharedcatalog");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.WeatherZoneCatalog.Domain.WeatherZone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("DollarsPerPound")
                        .HasColumnType("numeric");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WeatherZones", "sharedcatalog");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.LifecycleProgramStage", b =>
                {
                    b.HasOne("FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.LifecycleProgram", "LifecycleProgram")
                        .WithMany("LifecycleProgramStages")
                        .HasForeignKey("LifecycleProgramId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FSH.Starter.WebApi.LifecycleStageCatalog.Domain.LifecycleStage", "LifecycleStage")
                        .WithMany()
                        .HasForeignKey("LifecycleStageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LifecycleProgram");

                    b.Navigation("LifecycleStage");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.LifecycleStageCatalog.Domain.LifecycleStage", b =>
                {
                    b.HasOne("FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.GrowthTreatment", "GrowthTreatment")
                        .WithMany()
                        .HasForeignKey("GrowthTreatmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.PreventativeTreatment", "PreventativeTreatment")
                        .WithMany()
                        .HasForeignKey("PreventativeTreatmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FSH.Starter.WebApi.RationCatalog.Domain.Ration", "Ration")
                        .WithMany()
                        .HasForeignKey("RationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GrowthTreatment");

                    b.Navigation("PreventativeTreatment");

                    b.Navigation("Ration");
                });

            modelBuilder.Entity("FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.LifecycleProgram", b =>
                {
                    b.Navigation("LifecycleProgramStages");
                });
#pragma warning restore 612, 618
        }
    }
}
