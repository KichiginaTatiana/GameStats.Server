﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BestPlayer> BestPlayers { get; set; }
        public virtual DbSet<GameMode> GameModes { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<PlayerStat> PlayerStats { get; set; }
        public virtual DbSet<PopularServer> PopularServers { get; set; }
        public virtual DbSet<RecentMatch> RecentMatches { get; set; }
        public virtual DbSet<Scoreboard> Scoreboards { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<ServerStat> ServerStats { get; set; }
        public virtual DbSet<TopFiveGameMode> TopFiveGameModes { get; set; }
        public virtual DbSet<TopFiveMap> TopFiveMaps { get; set; }
    }
}
