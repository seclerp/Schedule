using System.Collections.Generic;
using NURE.Schedule.Common;

namespace NURE.Schedule.Api.StaticData
{
  public class EventTypeStaticData
  {
    public static Dictionary<EventType, string> Colors = new Dictionary<EventType, string>()
    {
      {EventType.Lecture, "FEFEEA"},
      {EventType.Lecture2, "FEFEEA"},
      {EventType.Lecture3, "FEFEEA"},

      {EventType.Practice, "DAE9D9"},
      {EventType.Seminar, "DAE9D9"},
      {EventType.Practice2, "DAE9D9"},

      {EventType.Laboratory, "CDCCFF"},
      {EventType.Laboratory2, "CDCCFF"},
      {EventType.Laboratory3, "CDCCFF"},
      {EventType.Laboratory4, "CDCCFF"},
      {EventType.Laboratory5, "CDCCFF"},

      {EventType.Consultation, "FFFFFF"},
      {EventType.Consultation2, "FFFFFF"},

      {EventType.Test, "C2A0B8"},
      {EventType.Test2, "C2A0B8"},

      {EventType.Exam, "8FD3FC"},
      {EventType.ExamWriting, "8FD3FC"},
      {EventType.ExamAudition, "8FD3FC"},
      {EventType.ExamCombined, "8FD3FC"},
      {EventType.ExamTest, "8FD3FC"},
      {EventType.ExamModule, "8FD3FC"},

      {EventType.CourseWork, "8FD3FC"}
    };

    public static Dictionary<EventType, string> Names = new Dictionary<EventType, string>()
    {
      {EventType.Lecture, "Лекция"},
      {EventType.Lecture2, "Лекция"},
      {EventType.Lecture3, "Лекция"},

      {EventType.Practice, "Практические занятие"},
      {EventType.Seminar, "Семинар"},
      {EventType.Practice2, "Практические занятие"},

      {EventType.Laboratory, "Лабораторная работа"},
      {EventType.Laboratory2, "Лабораторная работа"},
      {EventType.Laboratory3, "Лабораторная работа"},
      {EventType.Laboratory4, "Лабораторная работа"},
      {EventType.Laboratory5, "Лабораторная работа"},

      {EventType.Consultation, "Консультация"},
      {EventType.Consultation2, "Консультация"},

      {EventType.Test, "Зачёт"},
      {EventType.Test2, "Зачёт"},

      {EventType.Exam, "Экзамен"},
      {EventType.ExamWriting, "Экзамен (письменный)"},
      {EventType.ExamAudition, "Экзамен (устный)"},
      {EventType.ExamCombined, "Экзамен (комбинированный)"},
      {EventType.ExamTest, "Экзамен (тест)"},
      {EventType.ExamModule, "Экзамен (модульный)"},

      {EventType.CourseWork, "КП/КР"}
    };
  }
}