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

    public virtual DbSet<MapCollectionMaps> MapCollectionMaps { get; set; }

    public virtual DbSet<MapCollections> MapCollections { get; set; }

    public virtual DbSet<MapContributorRoles> MapContributorRoles { get; set; }

    public virtual DbSet<MapContributors> MapContributors { get; set; }

    public virtual DbSet<MapCounterCommonRules> MapCounterCommonRules { get; set; }

    public virtual DbSet<MapCounterRuleRelations> MapCounterRuleRelations { get; set; }

    public virtual DbSet<MapCounterRules> MapCounterRules { get; set; }

    public virtual DbSet<MapCounters> MapCounters { get; set; }

    public virtual DbSet<MapDamElements> MapDamElements { get; set; }

    public virtual DbSet<MapDams> MapDams { get; set; }

    public virtual DbSet<MapElements> MapElements { get; set; }

    public virtual DbSet<MapElementsMetadata> MapElementsMetadata { get; set; }

    public virtual DbSet<MapFeedbackOperators> MapFeedbackOperators { get; set; }

    public virtual DbSet<MapFeedbackRules> MapFeedbackRules { get; set; }

    public virtual DbSet<MapFeedbackTypes> MapFeedbackTypes { get; set; }

    public virtual DbSet<MapGroups> MapGroups { get; set; }

    public virtual DbSet<MapKeys> MapKeys { get; set; }

    public virtual DbSet<MapNodeCounters> MapNodeCounters { get; set; }

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

    public virtual DbSet<MapQuestionResponses> MapQuestionResponses { get; set; }

    public virtual DbSet<MapQuestionTypes> MapQuestionTypes { get; set; }

    public virtual DbSet<MapQuestionValidation> MapQuestionValidation { get; set; }

    public virtual DbSet<MapQuestions> MapQuestions { get; set; }

    public virtual DbSet<MapSections> MapSections { get; set; }

    public virtual DbSet<MapSecurities> MapSecurities { get; set; }

    public virtual DbSet<MapSkins> MapSkins { get; set; }

    public virtual DbSet<MapTypes> MapTypes { get; set; }

    public virtual DbSet<MapUsers> MapUsers { get; set; }

    public virtual DbSet<MapVpdElements> MapVpdElements { get; set; }

    public virtual DbSet<MapVpdTypes> MapVpdTypes { get; set; }

    public virtual DbSet<MapVpds> MapVpds { get; set; }

    public virtual DbSet<Maps> Maps { get; set; }

    public virtual DbSet<OauthProviders> OauthProviders { get; set; }

    public virtual DbSet<Options> Options { get; set; }

    public virtual DbSet<Phinxlog> Phinxlog { get; set; }

    public virtual DbSet<QCumulative> QCumulative { get; set; }

    public virtual DbSet<ScenarioMaps> ScenarioMaps { get; set; }

    public virtual DbSet<Scenarios> Scenarios { get; set; }

    public virtual DbSet<ScopeTypes> ScopeTypes { get; set; }

    public virtual DbSet<SecurityRoles> SecurityRoles { get; set; }

    public virtual DbSet<SecurityUsers> SecurityUsers { get; set; }

    public virtual DbSet<Servers> Servers { get; set; }

    public virtual DbSet<SjtResponse> SjtResponse { get; set; }

    public virtual DbSet<Statements> Statements { get; set; }

    public virtual DbSet<StatisticsUserDatesave> StatisticsUserDatesave { get; set; }

    public virtual DbSet<StatisticsUserResponses> StatisticsUserResponses { get; set; }

    public virtual DbSet<StatisticsUserSessions> StatisticsUserSessions { get; set; }

    public virtual DbSet<StatisticsUserSessiontraces> StatisticsUserSessiontraces { get; set; }

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

    public virtual DbSet<TwitterCredits> TwitterCredits { get; set; }

    public virtual DbSet<UserBookmarks> UserBookmarks { get; set; }

    public virtual DbSet<UserGroups> UserGroups { get; set; }

    public virtual DbSet<UserNotes> UserNotes { get; set; }

    public virtual DbSet<UserResponses> UserResponses { get; set; }

    public virtual DbSet<UserSessions> UserSessions { get; set; }

    public virtual DbSet<UserSessiontraces> UserSessiontraces { get; set; }

    public virtual DbSet<UserState> UserState { get; set; }

    public virtual DbSet<UserTypes> UserTypes { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<Vocablets> Vocablets { get; set; }

    public virtual DbSet<WebinarGroups> WebinarGroups { get; set; }

    public virtual DbSet<WebinarMacros> WebinarMacros { get; set; }

    public virtual DbSet<WebinarMaps> WebinarMaps { get; set; }

    public virtual DbSet<WebinarNodePoll> WebinarNodePoll { get; set; }

    public virtual DbSet<WebinarPoll> WebinarPoll { get; set; }

    public virtual DbSet<WebinarSteps> WebinarSteps { get; set; }

    public virtual DbSet<WebinarUsers> WebinarUsers { get; set; }

    public virtual DbSet<Webinars> Webinars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
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
                .HasDefaultValueSql("current_timestamp()");
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

        modelBuilder.Entity<MapCollectionMaps>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Collection).WithMany(p => p.MapCollectionMaps).HasConstraintName("map_collectionmaps_ibfk_1");

            entity.HasOne(d => d.Map).WithMany(p => p.MapCollectionMaps).HasConstraintName("map_collectionmaps_ibfk_2");
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

        modelBuilder.Entity<MapCounterRules>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.CounterNavigation).WithMany(p => p.MapCounterRules).HasConstraintName("map_counter_rules_ibfk_1");
        });

        modelBuilder.Entity<MapCounters>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Visible).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Map).WithMany(p => p.MapCounters)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("map_counters_ibfk_1");
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

        modelBuilder.Entity<MapGroups>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Group).WithMany(p => p.MapGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mp_ibfk_group");

            entity.HasOne(d => d.Map).WithMany(p => p.MapGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mp_ibfk_map");
        });

        modelBuilder.Entity<MapKeys>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.MapKeys).HasConstraintName("map_keys_ibfk_1");
        });

        modelBuilder.Entity<MapNodeCounters>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Display).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.Node).WithMany(p => p.MapNodeCounters).HasConstraintName("map_node_counters_ibfk_1");
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

        modelBuilder.Entity<MapQuestionResponses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("map_question_responses_ibfk_2");

            entity.HasOne(d => d.Question).WithMany(p => p.MapQuestionResponses)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("map_question_responses_ibfk_1");
        });

        modelBuilder.Entity<MapQuestionTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MapQuestionValidation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Question).WithMany(p => p.MapQuestionValidation).HasConstraintName("map_question_validation_ibfk_1");
        });

        modelBuilder.Entity<MapQuestions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.NumTries).HasDefaultValueSql("-1");

            entity.HasOne(d => d.Map).WithMany(p => p.MapQuestions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("map_questions_ibfk_1");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("map_questions_ibfk_2");
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
                .HasDefaultValueSql("current_timestamp()");
        });

        modelBuilder.Entity<QCumulative>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Map).WithMany(p => p.QCumulative).HasConstraintName("q_cumulative_ibfk_2");

            entity.HasOne(d => d.Question).WithMany(p => p.QCumulative).HasConstraintName("q_cumulative_ibfk_1");
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

        modelBuilder.Entity<SecurityRoles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SecurityUsers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Servers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SjtResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Response).WithMany(p => p.SjtResponse).HasConstraintName("sjt_response_ibfk_1");
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
            entity.Property(e => e.IsMedia).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsShared).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsSystem).HasDefaultValueSql("'0'");
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

            entity.Property(e => e.NumTries).HasDefaultValueSql("-1");

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

        modelBuilder.Entity<TwitterCredits>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<UserBookmarks>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Node).WithMany(p => p.UserBookmarks).HasConstraintName("user_bookmarks_ibfk_2");

            entity.HasOne(d => d.Session).WithMany(p => p.UserBookmarks).HasConstraintName("user_bookmarks_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.UserBookmarks).HasConstraintName("user_bookmarks_ibfk_3");
        });

        modelBuilder.Entity<UserGroups>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Iss).HasDefaultValueSql("'olab'");

            entity.HasOne(d => d.Group).WithMany(p => p.UserGroups).HasConstraintName("user_groups_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.UserGroups).HasConstraintName("user_groups_ibfk_1");
        });

        modelBuilder.Entity<UserNotes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Session).WithOne(p => p.UserNotes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_notes_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_notes_ibfk_1");

            entity.HasOne(d => d.Webinar).WithMany(p => p.UserNotes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_notes_ibfk_3");
        });

        modelBuilder.Entity<UserResponses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Question).WithMany(p => p.UserResponses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_responses_ibfk_1");

            entity.HasOne(d => d.Session).WithMany(p => p.UserResponses).HasConstraintName("user_responses_ibfk_2");
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

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.IsLti).HasDefaultValueSql("'0'");
            entity.Property(e => e.VisualEditorAutosaveTime).HasDefaultValueSql("'50000'");
        });

        modelBuilder.Entity<Vocablets>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<WebinarGroups>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<WebinarMacros>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<WebinarMaps>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.StepNavigation).WithMany(p => p.WebinarMaps).HasConstraintName("webinar_maps_ibfk_1");
        });

        modelBuilder.Entity<WebinarNodePoll>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Node).WithMany(p => p.WebinarNodePoll).HasConstraintName("webinar_node_poll_ibfk_2");

            entity.HasOne(d => d.Webinar).WithMany(p => p.WebinarNodePoll).HasConstraintName("webinar_node_poll_ibfk_1");
        });

        modelBuilder.Entity<WebinarPoll>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.OnNodeNavigation).WithMany(p => p.WebinarPollOnNodeNavigation).HasConstraintName("webinar_poll_ibfk_1");

            entity.HasOne(d => d.ToNodeNavigation).WithMany(p => p.WebinarPollToNodeNavigation).HasConstraintName("webinar_poll_ibfk_2");
        });

        modelBuilder.Entity<WebinarSteps>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<WebinarUsers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.User).WithMany(p => p.WebinarUsers).HasConstraintName("webinar_users_ibfk_1");
        });

        modelBuilder.Entity<Webinars>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
