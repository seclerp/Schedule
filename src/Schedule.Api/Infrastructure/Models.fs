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
    Teachers : Dictionary<EventType, TeacherModel list>
}
and TeacherModel = {
    Name : string
    Groups : string list
}