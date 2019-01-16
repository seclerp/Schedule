module Infrastructure.Models

open System.Collections.Generic

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


type TeachersPerGroup = {
    Teacher   : long
    EventType : EventType
    Group     : string
}

type SubjectModel = {
    Id              : long
    Brief           : string
    Title           : string
    TeachersByGroup : TeachersPerGroup list
}