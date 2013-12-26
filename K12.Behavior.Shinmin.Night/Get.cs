﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;

namespace K12.Behavior.Shinmin.Night
{
    public static class Get
    {
        public static DSResponse GetDiscipline(DSRequest request)
        {
            return FISCA.Authentication.DSAServices.CallService("SmartSchool.Student.Discipline.GetDiscipline", request);
        }

        public static DSResponse GetAttendance(DSRequest request)
        {
            return FISCA.Authentication.DSAServices.CallService("SmartSchool.Student.Attendance.GetAttendance", request);
        }
    }
}
