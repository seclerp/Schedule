module Infrastructure.Models

open System.Collections.Generic

type EventType =
    | Lecture
    | Practice
    | Lab
    | Consultation
    | Credit
    | Exam
    | CourseWork 
    with static member map = function
                             | "Лк"      -> Lecture
                             | "Пз"      -> Practice
                             | "Лб"      -> Lab
                             | "Конс"    -> Consultation
                             | "Зал"     -> Credit
                             | "ІспКомб" -> Exam
                             | _         -> CourseWork
type SubjectModel = {
    Id : long
    Brief : string
    Title : string
    TeachersByGroup : Dictionary<string, Dictionary<EventType, string list>>
}