using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

public partial class OLabDBContext : DbContext
{
    public OLabDBContext(DbContextOptions<OLabDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuthorRights> AuthorRights { get; set; }

    public virtual DbSet<Cron> Cron { get; set; }

    public virtual DbSet<GrouproleAcls> GrouproleAcls { get; set; }

    public virtual DbSet<Groups> Groups { get; set; }

    public virtual DbSet<H5pContents> H5pContents { get; set; }

    public virtual DbSet<H5pContentsLibraries> H5pContentsLibraries { get; set; }

    public virtual DbSet<H5pContentsTags> H5pContentsTags { get; set; }

    public virtual DbSet<H5pContentsUserData> H5pContentsUserData { get; set; }

    public virtual DbSet<H5pCounters> H5pCounters { get; set; }

    public virtual DbSet<H5pEvents> H5pEvents { get; set; }

    public virtual DbSet<H5pLibraries> H5pLibraries { get; set; }

    public virtual DbSet<H5pLibrariesCachedassets> H5pLibrariesCachedassets { get; set; }

    public virtual DbSet<H5pLibrariesLanguages> H5pLibrariesLanguages { get; set; }

    public virtual DbSet<H5pLibrariesLibraries> H5pLibrariesLibraries { get; set; }

    public virtual DbSet<H5pResults> H5pResults { get; set; }

    public virtual DbSet<H5pTags> H5pTags { get; set; }

    public virtual DbSet<Languages> Languages { get; set; }

    public virtual DbSet<LinksOrder> LinksOrder { get; set; }

    public virtual DbSet<LinksToOrder> LinksToOrder { get; set; }

    public virtual DbSet<Lrs> Lrs { get; set; }

    public virtual DbSet<LrsStatement> LrsStatement { get; set; }

    public virtual DbSet<LtiConsumer> LtiConsumer { get; set; }

    public virtual DbSet<LtiConsumers> LtiConsumers { get; set; }

    public virtual DbSet<LtiContexts> LtiContexts { get; set; }

    public virtual DbSet<LtiNonces> LtiNonces { get; set; }

    public virtual DbSet<LtiProviders> LtiProviders { get; set; }

    public virtual DbSet<LtiSharekeys> LtiSharekeys { get; set; }

    public virtual DbSet<LtiUsers> LtiUsers { get; set; }

    public virtual DbSet<MapAvatars> MapAvatars { get; set; }

    public virtual DbSet<MapChatElements> MapChatElements { get; set; }

    public virtual DbSet<MapChats> MapChats { get; set; }

    public virtual DbSet<MapCollectionmaps> MapCollectionmaps { get; set; }

    public virtual DbSet<MapCollections> MapCollections { get; set; }

    public virtual DbSet<MapContributorRoles> MapContributorRoles { get; set; }

    public virtual DbSet<MapContributors> MapContributors { get; set; }

    public virtual DbSet<MapCounterCommonRules> MapCounterCommonRules { get; set; }

    public virtual DbSet<MapCounterRuleRelations> MapCounterRuleRelations { get; set; }

    public virtual DbSet<MapDamElements> MapDamElements { get; set; }

    public virtual DbSet<MapDams> MapDams { get; set; }

    public virtual DbSet<MapElements> MapElements { get; set; }

    public virtual DbSet<MapElementsMetadata> MapElementsMetadata { get; set; }

    public virtual DbSet<MapFeedbackOperators> MapFeedbackOperators { get; set; }

    public virtual DbSet<MapFeedbackRules> MapFeedbackRules { get; set; }

    public virtual DbSet<MapFeedbackTypes> MapFeedbackTypes { get; set; }

    public virtual DbSet<MapGrouproles> MapGrouproles { get; set; }

    public virtual DbSet<MapGroups> MapGroups { get; set; }

    public virtual DbSet<MapKeys> MapKeys { get; set; }

    public virtual DbSet<MapNodeGrouproles> MapNodeGrouproles { get; set; }

    public virtual DbSet<MapNodeJumps> MapNodeJumps { get; set; }

    public virtual DbSet<MapNodeLinkStylies> MapNodeLinkStylies { get; set; }

    public virtual DbSet<MapNodeLinkTypes> MapNodeLinkTypes { get; set; }

    public virtual DbSet<MapNodeLinks> MapNodeLinks { get; set; }

    public virtual DbSet<MapNodeNotes> MapNodeNotes { get; set; }

    public virtual DbSet<MapNodePriorities> MapNodePriorities { get; set; }

    public virtual DbSet<MapNodeReferences> MapNodeReferences { get; set; }

    public virtual DbSet<MapNodeSectionNodes> MapNodeSectionNodes { get; set; }

    public virtual DbSet<MapNodeSections> MapNodeSections { get; set; }

    public virtual DbSet<MapNodeTypes> MapNodeTypes { get; set; }

    public virtual DbSet<MapNodes> MapNodes { get; set; }

    public virtual DbSet<MapPopupAssignTypes> MapPopupAssignTypes { get; set; }

    public virtual DbSet<MapPopupPositionTypes> MapPopupPositionTypes { get; set; }

    public virtual DbSet<MapPopupPositions> MapPopupPositions { get; set; }

    public virtual DbSet<MapPopups> MapPopups { get; set; }

    public virtual DbSet<MapPopupsAssign> MapPopupsAssign { get; set; }

    public virtual DbSet<MapPopupsCounters> MapPopupsCounters { get; set; }

    public virtual DbSet<MapPopupsStyles> MapPopupsStyles { get; set; }

    public virtual DbSet<MapSections> MapSections { get; set; }

    public virtual DbSet<MapSecurities> MapSecurities { get; set; }

    public virtual DbSet<MapSkins> MapSkins { get; set; }

    public virtual DbSet<MapTypes> MapTypes { get; set; }

    public virtual DbSet<MapUsers> MapUsers { get; set; }

    public virtual DbSet<MapVpdElements> MapVpdElements { get; set; }

    public virtual DbSet<MapVpdTypes> MapVpdTypes { get; set; }

    public virtual DbSet<MapVpds> MapVpds { get; set; }

    public virtual DbSet<Maps> Maps { get; set; }

    public virtual DbSet<Mapswithttalkview> Mapswithttalkview { get; set; }

    public virtual DbSet<OauthProviders> OauthProviders { get; set; }

    public virtual DbSet<Options> Options { get; set; }

    public virtual DbSet<Phinxlog> Phinxlog { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<ScenarioMaps> ScenarioMaps { get; set; }

    public virtual DbSet<Scenarios> Scenarios { get; set; }

    public virtual DbSet<ScopeTypes> ScopeTypes { get; set; }

    public virtual DbSet<Servers> Servers { get; set; }

    public virtual DbSet<Statements> Statements { get; set; }

    public virtual DbSet<StatisticsUserDatesave> StatisticsUserDatesave { get; set; }

    public virtual DbSet<StatisticsUserResponses> StatisticsUserResponses { get; set; }

    public virtual DbSet<StatisticsUserSessions> StatisticsUserSessions { get; set; }

    public virtual DbSet<StatisticsUserSessiontraces> StatisticsUserSessiontraces { get; set; }

    public virtual DbSet<SystemApplications> SystemApplications { get; set; }

    public virtual DbSet<SystemConstants> SystemConstants { get; set; }

    public virtual DbSet<SystemCounterActions> SystemCounterActions { get; set; }

    public virtual DbSet<SystemCounters> SystemCounters { get; set; }

    public virtual DbSet<SystemCourses> SystemCourses { get; set; }

    public virtual DbSet<SystemFiles> SystemFiles { get; set; }

    public virtual DbSet<SystemGlobals> SystemGlobals { get; set; }

    public virtual DbSet<SystemQuestionResponses> SystemQuestionResponses { get; set; }

    public virtual DbSet<SystemQuestionTypes> SystemQuestionTypes { get; set; }

    public virtual DbSet<SystemQuestionValidation> SystemQuestionValidation { get; set; }

    public virtual DbSet<SystemQuestions> SystemQuestions { get; set; }

    public virtual DbSet<SystemScripts> SystemScripts { get; set; }

    public virtual DbSet<SystemServers> SystemServers { get; set; }

    public virtual DbSet<SystemSettings> SystemSettings { get; set; }

    public virtual DbSet<SystemThemes> SystemThemes { get; set; }

    public virtual DbSet<TodayTips> TodayTips { get; set; }

    public virtual DbSet<Ttalkqu2mapsview> Ttalkqu2mapsview { get; set; }

    public virtual DbSet<TwitterCredits> TwitterCredits { get; set; }

    public virtual DbSet<UserAcls> UserAcls { get; set; }

    public virtual DbSet<UserBookmarks> UserBookmarks { get; set; }

    public virtual DbSet<UserCounterUpdate> UserCounterUpdate { get; set; }

    public virtual DbSet<UserGrouproles> UserGrouproles { get; set; }

    public virtual DbSet<UserResponses> UserResponses { get; set; }

    public virtual DbSet<UserSessions> UserSessions { get; set; }

    public virtual DbSet<UserSessiontraces> UserSessiontraces { get; set; }

    public virtual DbSet<UserState> UserState { get; set; }

    public virtual DbSet<UserTypes> UserTypes { get; set; }

    public virtual DbSet<UserresponseCounterupdate> UserresponseCounterupdate { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<UsersessiontraceCounterupdate> UsersessiontraceCounterupdate { get; set; }

    public virtual DbSet<Vocablets> Vocablets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AuthorRights>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Cron>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Rule).WithMany(p => p.Cron).HasConstraintName("cron_ibfk_1");
        });

        modelBuilder.Entity<GrouproleAcls>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Acl2).HasDefaultValueSql("b'0'");

            entity.HasOne(d => d.Group).WithMany(p => p.GrouproleAcls)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ifk_gra_group");

            entity.HasOne(d => d.Role).WithMany(p => p.GrouproleAcls)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ifk_gra_role");
        });

        modelBuilder.Entity<Groups>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<H5pContents>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
        });

        modelBuilder.Entity<H5pContentsLibraries>(entity =>
        {
            entity.HasKey(e => new { e.ContentId, e.LibraryId, e.DependencyType })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
        });

        modelBuilder.Entity<H5pContentsTags>(entity =>
        {
            entity.HasKey(e => new { e.ContentId, e.TagId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
        });

        modelBuilder.Entity<H5pContentsUserData>(entity =>
        {
            entity.HasKey(e => new { e.ContentId, e.UserId, e.SubContentId, e.DataId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
        });

        modelBuilder.Entity<H5pCounters>(entity =>
        {
            entity.HasKey(e => new { e.Type, e.LibraryName, e.LibraryVersion })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
        });

        modelBuilder.Entity<H5pEvents>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<H5pLibraries>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
        });

        modelBuilder.Entity<H5pLibrariesCachedassets>(entity =>
        {
            entity.HasKey(e => new { e.LibraryId, e.Hash })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
        });

        modelBuilder.Entity<H5pLibrariesLanguages>(entity =>
        {
            entity.HasKey(e => new { e.LibraryId, e.LanguageCode })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
        });

        modelBuilder.Entity<H5pLibrariesLibraries>(entity =>
        {
            entity.HasKey(e => new { e.LibraryId, e.RequiredLibraryId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
        });

        modelBuilder.Entity<H5pResults>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<H5pTags>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Languages>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<LinksOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<LinksToOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Lrs>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<LrsStatement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Lrs).WithMany(p => p.LrsStatement).HasConstraintName("lrs_statement_ibfk_1");

            entity.HasOne(d => d.Statement).WithMany(p => p.LrsStatement).HasConstraintName("lrs_statement_ibfk_2");
        });

        modelBuilder.Entity<LtiConsumer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Role).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<LtiConsumers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Role).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<LtiContexts>(entity =>
        {
            entity.HasKey(e => e.ConsumerKey).HasName("PRIMARY");
        });

        modelBuilder.Entity<LtiNonces>(entity =>
        {
            entity.HasKey(e => e.ConsumerKey).HasName("PRIMARY");
        });

        modelBuilder.Entity<LtiProviders>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<LtiSharekeys>(entity =>
        {
            entity.HasKey(e => e.ShareKeyId).HasName("PRIMARY");
        });

        modelBuilder.Entity<LtiUsers>(entity =>
        {
            entity.HasKey(e => e.ConsumerKey).HasName("PRIMARY");

            entity.HasOne(d => d.ConsumerKeyNavigation).WithOne(p => p.LtiUsers).HasConstraintName("lti_users_ibfk_2");
        });

        modelBuilder.Entity<MapAvatars>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapAvatars).HasConstraintName("map_avatars_ibfk_1");
        });

        modelBuilder.Entity<MapChatElements>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Chat).WithMany(p => p.MapChatElements).HasConstraintName("map_chat_elements_ibfk_1");
        });

        modelBuilder.Entity<MapChats>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapChats).HasConstraintName("map_chats_ibfk_1");
        });

        modelBuilder.Entity<MapCollectionmaps>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Collection).WithMany(p => p.MapCollectionmaps).HasConstraintName("map_collectionmaps_ibfk_1");

            entity.HasOne(d => d.Map).WithMany(p => p.MapCollectionmaps).HasConstraintName("map_collectionmaps_ibfk_2");
        });

        modelBuilder.Entity<MapCollections>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapContributorRoles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapContributors>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Order).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.Map).WithMany(p => p.MapContributors).HasConstraintName("map_contributors_ibfk_1");
        });

        modelBuilder.Entity<MapCounterCommonRules>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapCounterCommonRules).HasConstraintName("map_counter_common_rules_ibfk_1");
        });

        modelBuilder.Entity<MapCounterRuleRelations>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapDamElements>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Order).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Dam).WithMany(p => p.MapDamElements).HasConstraintName("map_dam_elements_ibfk_1");
        });

        modelBuilder.Entity<MapDams>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapDams).HasConstraintName("map_dams_ibfk_1");
        });

        modelBuilder.Entity<MapElements>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.HeightType).HasDefaultValueSql("'px'");
            entity.Property(e => e.IsShared).HasDefaultValueSql("'1'");
            entity.Property(e => e.WidthType).HasDefaultValueSql("'px'");

            entity.HasOne(d => d.Map).WithMany(p => p.MapElements).HasConstraintName("map_elements_ibfk_1");
        });

        modelBuilder.Entity<MapElementsMetadata>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapFeedbackOperators>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapFeedbackRules>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapFeedbackRules).HasConstraintName("map_feedback_rules_ibfk_1");
        });

        modelBuilder.Entity<MapFeedbackTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapGrouproles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Group).WithMany(p => p.MapGrouproles)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("map_grouproles_groups_FK");

            entity.HasOne(d => d.Map).WithMany(p => p.MapGrouproles).HasConstraintName("map_grouproles_maps_FK");

            entity.HasOne(d => d.Role).WithMany(p => p.MapGrouproles)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("map_grouproles_roles_FK");
        });

        modelBuilder.Entity<MapGroups>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Group).WithMany(p => p.MapGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("map_groups_groups_FK");

            entity.HasOne(d => d.Map).WithMany(p => p.MapGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("map_groups_maps_FK");
        });

        modelBuilder.Entity<MapKeys>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapKeys).HasConstraintName("map_keys_ibfk_1");
        });

        modelBuilder.Entity<MapNodeGrouproles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Group).WithMany(p => p.MapNodeGrouproles).HasConstraintName("mngr_ibfk_group");

            entity.HasOne(d => d.Node).WithMany(p => p.MapNodeGrouproles).HasConstraintName("mngr_ibfk_node");

            entity.HasOne(d => d.Role).WithMany(p => p.MapNodeGrouproles).HasConstraintName("mngr_ibfk_role");
        });

        modelBuilder.Entity<MapNodeJumps>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Hidden).HasDefaultValueSql("'0'");
            entity.Property(e => e.Order).HasDefaultValueSql("'1'");
            entity.Property(e => e.Probability).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Map).WithMany(p => p.MapNodeJumps).HasConstraintName("map_jumps_ibfk_1");

            entity.HasOne(d => d.Node).WithMany(p => p.MapNodeJumps).HasConstraintName("map_jumps_ibfk_4");
        });

        modelBuilder.Entity<MapNodeLinkStylies>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapNodeLinkTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapNodeLinks>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Hidden).HasDefaultValueSql("'0'");
            entity.Property(e => e.Order).HasDefaultValueSql("'1'");
            entity.Property(e => e.Probability).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Map).WithMany(p => p.MapNodeLinks).HasConstraintName("map_node_links_ibfk_1");

            entity.HasOne(d => d.NodeId1Navigation).WithMany(p => p.MapNodeLinksNodeId1Navigation).HasConstraintName("map_node_links_ibfk_4");

            entity.HasOne(d => d.NodeId2Navigation).WithMany(p => p.MapNodeLinksNodeId2Navigation).HasConstraintName("map_node_links_ibfk_5");
        });

        modelBuilder.Entity<MapNodeNotes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.MapNode).WithMany(p => p.MapNodeNotes).HasConstraintName("fk_map_node");
        });

        modelBuilder.Entity<MapNodePriorities>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapNodeReferences>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapNodeSectionNodes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Node).WithMany(p => p.MapNodeSectionNodes).HasConstraintName("map_node_section_nodes_ibfk_3");

            entity.HasOne(d => d.Section).WithMany(p => p.MapNodeSectionNodes).HasConstraintName("map_node_section_nodes_ibfk_1");
        });

        modelBuilder.Entity<MapNodeSections>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapNodeSections).HasConstraintName("map_node_sections_ibfk_1");
        });

        modelBuilder.Entity<MapNodeTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapNodes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.ForceReload).HasDefaultValueSql("'0'");
            entity.Property(e => e.LinkTypeId).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.LinkStyle).WithMany(p => p.MapNodes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("map_nodes_ibfk_2");

            entity.HasOne(d => d.Map).WithMany(p => p.MapNodes).HasConstraintName("map_nodes_ibfk_1");
        });

        modelBuilder.Entity<MapPopupAssignTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapPopupPositionTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapPopupPositions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapPopups>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapPopupsAssign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.RedirectTypeId).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<MapPopupsCounters>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Popup).WithMany(p => p.MapPopupsCounters).HasConstraintName("map_popups_counters_ibfk_1");
        });

        modelBuilder.Entity<MapPopupsStyles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.IsDefaultBackgroundColor).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<MapSections>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapSecurities>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapSkins>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapUsers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapUsers).HasConstraintName("map_users_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.MapUsers).HasConstraintName("map_users_ibfk_2");
        });

        modelBuilder.Entity<MapVpdElements>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Vpd).WithMany(p => p.MapVpdElements).HasConstraintName("map_vpd_elements_ibfk_1");
        });

        modelBuilder.Entity<MapVpdTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapVpds>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.VpdType).WithMany(p => p.MapVpds).HasConstraintName("map_vpds_ibfk_1");
        });

        modelBuilder.Entity<Maps>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.IsTemplate).HasDefaultValueSql("'0'");
            entity.Property(e => e.Keywords).HasDefaultValueSql("''''''");
            entity.Property(e => e.LanguageId).HasDefaultValueSql("'1'");
            entity.Property(e => e.ReminderMsg).HasDefaultValueSql("''");

            entity.HasOne(d => d.Language).WithMany(p => p.Maps).HasConstraintName("maps_ibfk_7");

            entity.HasOne(d => d.Section).WithMany(p => p.Maps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("maps_ibfk_6");

            entity.HasOne(d => d.Security).WithMany(p => p.Maps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("maps_ibfk_2");

            entity.HasOne(d => d.Type).WithMany(p => p.Maps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("maps_ibfk_3");
        });

        modelBuilder.Entity<Mapswithttalkview>(entity =>
        {
            entity.ToView("mapswithttalkview");
        });

        modelBuilder.Entity<OauthProviders>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Options>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Autoload).HasDefaultValueSql("'yes'");
            entity.Property(e => e.Name).HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Phinxlog>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PRIMARY");

            entity.Property(e => e.Version).ValueGeneratedNever();
            entity.Property(e => e.EndTime).HasDefaultValueSql("'0000-00-00 00:00:00'");
            entity.Property(e => e.StartTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.System).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<ScenarioMaps>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Map).WithMany(p => p.ScenarioMaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_scenario_maps_maps");

            entity.HasOne(d => d.Scenario).WithMany(p => p.ScenarioMaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_scenario_maps_scenarios");
        });

        modelBuilder.Entity<Scenarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<ScopeTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Servers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Statements>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Initiator).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.Session).WithMany(p => p.Statements)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("statements_ibfk_1");
        });

        modelBuilder.Entity<StatisticsUserDatesave>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<StatisticsUserResponses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<StatisticsUserSessions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<StatisticsUserSessiontraces>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SystemApplications>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SystemConstants>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SystemCounterActions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Counter).WithMany(p => p.SystemCounterActions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_counter_action_counter");

            entity.HasOne(d => d.Map).WithMany(p => p.SystemCounterActions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_counter_action_map");
        });

        modelBuilder.Entity<SystemCounters>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Visible).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<SystemCourses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SystemFiles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.HeightType).HasDefaultValueSql("'px'");
            entity.Property(e => e.Media).HasDefaultValueSql("'1'");
            entity.Property(e => e.Shared).HasDefaultValueSql("'1'");
            entity.Property(e => e.System).HasDefaultValueSql("'0'");
            entity.Property(e => e.WidthType).HasDefaultValueSql("'px'");
        });

        modelBuilder.Entity<SystemGlobals>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SystemQuestionResponses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("system_question_responses_ibfk_2");

            entity.HasOne(d => d.Question).WithMany(p => p.SystemQuestionResponses)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("system_question_responses_ibfk_1");
        });

        modelBuilder.Entity<SystemQuestionTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SystemQuestionValidation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Question).WithMany(p => p.SystemQuestionValidation).HasConstraintName("system_question_validation_ibfk_1");
        });

        modelBuilder.Entity<SystemQuestions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.NumTries).HasDefaultValueSql("'-1'");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("system_questions_ibfk_2");
        });

        modelBuilder.Entity<SystemScripts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.IsRaw).HasDefaultValueSql("b'0'");
        });

        modelBuilder.Entity<SystemServers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SystemSettings>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SystemThemes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<TodayTips>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Ttalkqu2mapsview>(entity =>
        {
            entity.ToView("ttalkqu2mapsview");
        });

        modelBuilder.Entity<TwitterCredits>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<UserAcls>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Acl2).HasDefaultValueSql("b'0'");
        });

        modelBuilder.Entity<UserBookmarks>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Node).WithMany(p => p.UserBookmarks).HasConstraintName("user_bookmarks_ibfk_2");

            entity.HasOne(d => d.Session).WithMany(p => p.UserBookmarks).HasConstraintName("user_bookmarks_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.UserBookmarks).HasConstraintName("user_bookmarks_ibfk_3");
        });

        modelBuilder.Entity<UserCounterUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<UserGrouproles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Iss).HasDefaultValueSql("'olab'");

            entity.HasOne(d => d.Group).WithMany(p => p.UserGrouproles).HasConstraintName("user_grouproles_ibfk_2");

            entity.HasOne(d => d.Role).WithMany(p => p.UserGrouproles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_grouproles_ibfk_3");

            entity.HasOne(d => d.User).WithMany(p => p.UserGrouproles).HasConstraintName("user_grouproles_ibfk_1");
        });

        modelBuilder.Entity<UserResponses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Question).WithMany(p => p.UserResponses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_responses_ibfk_1");

            entity.HasOne(d => d.Session).WithMany(p => p.UserResponses).HasConstraintName("user_responses_ibfk_3");
        });

        modelBuilder.Entity<UserSessions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.UserSessions).HasConstraintName("user_sessions_ibfk_2");
        });

        modelBuilder.Entity<UserSessiontraces>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.UserSessiontraces).HasConstraintName("user_sessiontraces_ibfk_3");

            entity.HasOne(d => d.Node).WithMany(p => p.UserSessiontraces).HasConstraintName("user_sessiontraces_ibfk_6");

            entity.HasOne(d => d.Session).WithMany(p => p.UserSessiontraces).HasConstraintName("user_sessiontraces_ibfk_5");
        });

        modelBuilder.Entity<UserState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.UserState).HasConstraintName("map_fk");

            entity.HasOne(d => d.MapNode).WithMany(p => p.UserState).HasConstraintName("map_node_fk");
        });

        modelBuilder.Entity<UserTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<UserresponseCounterupdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Counterupdate).WithMany(p => p.UserresponseCounterupdate).HasConstraintName("urcu_fk_cu");

            entity.HasOne(d => d.Userresponse).WithMany(p => p.UserresponseCounterupdate).HasConstraintName("urcu_fk_ur");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Lti).HasDefaultValueSql("'0'");
            entity.Property(e => e.VisualEditorAutosaveTime).HasDefaultValueSql("'50000'");
        });

        modelBuilder.Entity<UsersessiontraceCounterupdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Counterupdate).WithMany(p => p.UsersessiontraceCounterupdate).HasConstraintName("stcu_fk_cu");

            entity.HasOne(d => d.Sessiontrace).WithMany(p => p.UsersessiontraceCounterupdate).HasConstraintName("stcu_fk_st");
        });

        modelBuilder.Entity<Vocablets>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
