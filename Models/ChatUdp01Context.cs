using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Client_UdpClient.Models;

public partial class ChatUdp01Context : DbContext
{
    public ChatUdp01Context()
    {
    }

    public ChatUdp01Context(DbContextOptions<ChatUdp01Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BlackList> BlackLists { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<ChatUser> ChatUsers { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ChatUDP01;Integrated Security=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlackList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlackLis__3214EC072831175F");

            entity.HasOne(d => d.BlackUser).WithMany(p => p.BlackListBlackUsers)
                .HasForeignKey(d => d.BlackUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BlackList__Black__37A5467C");

            entity.HasOne(d => d.Creator).WithMany(p => p.BlackListCreators)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("FK__BlackList__Creat__36B12243");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chats__3214EC07D25C6746");

            entity.Property(e => e.ChatName).HasMaxLength(30);

            entity.HasOne(d => d.Creator).WithMany(p => p.Chats)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("FK__Chats__CreatorId__286302EC");
        });

        modelBuilder.Entity<ChatUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatUser__3214EC07D3C4423E");

            entity.HasOne(d => d.Chat).WithMany(p => p.ChatUsers)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("FK__ChatUsers__ChatI__3B75D760");

            entity.HasOne(d => d.ChatUserNavigation).WithMany(p => p.ChatUsers)
                .HasForeignKey(d => d.ChatUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatUsers__ChatU__3C69FB99");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC0747D44F1D");

            entity.Property(e => e.Body).HasMaxLength(500);
            entity.Property(e => e.SendingTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SystemInfo).HasMaxLength(100);

            entity.HasOne(d => d.Chats).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatsId)
                .HasConstraintName("FK__Messages__ChatsI__4D94879B");

            entity.HasOne(d => d.CreatorUser).WithMany(p => p.Messages)
                .HasForeignKey(d => d.CreatorUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Creato__4CA06362");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC079FE469BF");

            entity.Property(e => e.IsSystem)
                .HasMaxLength(1)
                .HasDefaultValueSql("((0))")
                .IsFixedLength();
            entity.Property(e => e.Login).HasMaxLength(20);
            entity.Property(e => e.Passvord).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
