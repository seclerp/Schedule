module Domain.Models

open System
open System.Collections.Generic
open System.ComponentModel.DataAnnotations

type EventType =
    | Lecture = 1
    | Practice = 2
    | Lab = 3
    | Consultation = 4
    | Credit = 5
    | Exam = 6
    | CourseWork = 7

//type EventType =
//    | Lecture       = 0  // лекция (базовый тип, цвет желтый, FEFEEA) – lecture;
//    | Lecture2      = 1  // лекция установочная, первая (этот подтип есть у заочников);
//    | Lecture3      = 2  // лекция установочная, предшествующая семестровому контролю
//    
//    | Practice      = 10 // практическое занятие (базовый тип, зелёный DAE9D9) – practice;
//    | Seminar       = 11 // семинар (подтип практического занятия, в ХНУРЭ нет);
//    | Practice2     = 12 // практическое занятие установочное (этот подтип есть у заочников)
//    
//    | Lab           = 20 // лабораторная работа (базовый тип, фиолетовый CDCCFF) – laboratory;
//    | Lab2          = 21 // лабораторная работа на ВЦ (используется только при расчете нагрузки, в расписании его нет);
//    | Lab3          = 22 // лабораторная работа кафедре (аналогично);
//    | Lab4          = 23 // лабораторная работа на ВЦ установочная;
//    | Lab5          = 24 // лабораторная работа на кафедре установочная.
//    
//    | Consultation  = 30 // консультация (базовый тип, цвет белый) – consultation;
//    | Consultation2 = 31 // внеучебное занятие (необязательная консультация. В расписании это пара у преподавателя без группы и предмета).
//    
//    | Credit        = 40 // зачет обычный (базовый тип, цвет коричневый, C2A0B8) – test;
//    | Credit2       = 41 // зачет дифференцированный (в ХНУРЭ сейчас нет, отменили 5 лет назад)
//    
//    | Exam          = 50 // экзамен (базовый тип, в чистом виде в ХНУРЭ не используется, цвет темно-голубой 8FD3FC) – exam;
//    | Exam2         = 51 // экзамен письменный;
//    | Exam3         = 52 // экзамен устный;
//    | Exam4         = 53 // экзамен комбинированный;
//    | Exam5         = 54 // экзамен тестовый;
//    | Exam6         = 56 // экзамен модульный.
//    
//    | CourseWork    = 60 // КП/КР (базовый тип) – course_work.

type IdentityType =
    | Teacher = 0
    | Group = 1
    | Auditory = 2

type [<CLIMutable>] Event = {
    Id            : long
    Date          : DateTime
    Teachers      : string   // csv list of long
    Groups        : string   // csv list of long
    Auditory      : Auditory
    Subject       : Subject
    Type          : EventType
}
and [<CLIMutable>] Identity = {
    Id            : long
    Name          : string
    Type          : IdentityType
    IsAlternative : bool
}
and [<CLIMutable>] Auditory = {
    Id            : long
    Name          : string
    Floor         : int
    HasPower      : bool
}
and [<CLIMutable>] Subject = {
    Id            : long
    Brief         : string
    Title         : string
}
and [<CLIMutable>] Teacher = {
    SubjectId     : long
    TeacherId     : long
    EventType     : int
    GroupId       : long
}