module Domain.ScheduleContext

open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Storage.ValueConversion
open Models

type ScheduleContext =
    inherit DbContext
    
    [<DefaultValue>]
    val mutable _events     : DbSet<Event>
    member x.Events 
        with get () = x._events 
        and set v = x._events <- v
    
    [<DefaultValue>]
    val mutable identites  : DbSet<Identity>
    member x.Identites 
        with get () = x.identites 
        and set v = x.identites <- v
    
    [<DefaultValue>]
    val mutable subjects   : DbSet<Subject>
    member x.Subjects 
        with get () = x.subjects 
        and set v = x.subjects <- v
    
    [<DefaultValue>]
    val mutable teachers   : DbSet<Teacher>
    member x.Teachers 
        with get () = x.teachers 
        and set v = x.teachers <- v
    
    new() = { inherit DbContext() }
    new(options : DbContextOptions<ScheduleContext>) = { inherit DbContext(options) }
    
    override __.OnModelCreating modelBuilder =
        let identityTypeConvert = ValueConverter<IdentityType, int>((fun v -> LanguagePrimitives.EnumToValue v),
                                                                    (fun v -> LanguagePrimitives.EnumOfValue v))
        modelBuilder.Entity<Identity>().Property(fun e -> e.Type).HasConversion(identityTypeConvert) |> ignore
        
        modelBuilder.Entity<Teacher>().HasKey([|"TeacherId"; "SubjectId"; "GroupId"; "EventType"|]) |> ignore
        modelBuilder.Entity<Identity>().HasKey([|"Id"; "Type"|]) |> ignore