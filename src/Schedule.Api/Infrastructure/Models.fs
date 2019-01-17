module Infrastructure.Models

open System

type EventType =
    | Lecture = 1
    | Practice = 2
    | Lab = 3
    | Consultation = 4
    | Credit = 5
    | Exam = 6
    | CourseWork = 7
    
let mapEventType = function
    | "Лк"      -> EventType.Lecture
    | "Пз"      -> EventType.Practice
    | "Лб"      -> EventType.Lab
    | "Конс"    -> EventType.Consultation
    | "Зал"     -> EventType.Credit
    | "ІспКомб" -> EventType.Exam
    | _         -> EventType.CourseWork

type TeacherTypeGroup = {
    Teacher   : long
    EventType : EventType
    Group     : string
}

type SubjectModel = {
    Id                : long
    Brief             : string
    Title             : string
    TeacherTypeGroups : TeacherTypeGroup list
}

type EventModel = {
    TimeStart    : DateTime
    TimeEnd      : DateTime
    Auditory     : string
    GroupsName   : string
    SubjectBrief : string
    EventType    : EventType
}