/* Empiria OnePoint ******************************************************************************************
*                                                                                                            *
*  Module   : Reporting                                    Component : Adapters                              *
*  Assembly : Empiria.OnePoint.Security.dll                Pattern   : Output DTO                            *
*  Type     : LogEntryDto                                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Describes a log entry.                                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

namespace Empiria.OnePoint.Security.Reporting {

  public class LogEntryDto {

    public string UserName {
      get {
        return User.FullName;
      }
    }


    public string EmployeeID {
      get {
        if (User is Person person) {
          return person.EmployeeNo == string.Empty ? person.EMail : person.EmployeeNo;
        } else {
          return User.EMail;
        }
      }
    }

    [DataField("UserId")]
    public Contact User {
      get;
      internal set;
    }


    [DataField("SessionId", ConvertFrom=typeof(long))]
    public int SessionId {
      get;
      internal set;
    }


    [DataField("UserHostAddress")]
    public string UserHostAddress {
      get;
      internal set;
    } = string.Empty;


    [DataField("LogTimeStamp")]
    public DateTime DateTime {
      get;
      internal set;
    }


    [DataField("OperationName")]
    public string Operation {
      get;
      internal set;
    } = string.Empty;


    [DataField("OperationDescription")]
    public string Description {
      get;
      internal set;
    } = string.Empty;



    [DataField("SubjectId")]
    public Contact Subject {
      get;
      internal set;
    }


    [DataField("SubjectObject")]
    public string SubjectObject {
      get;
      internal set;
    }

    public string SubjectEmployeeID {
      get {
        if (Subject is Person person) {
          return person.EmployeeNo == string.Empty ? person.EMail : person.EmployeeNo;
        } else {
          return Subject.EMail;
        }
      }
    }

    [DataField("ErrorDescription")]
    public string Exception {
      get;
      internal set;
    } = string.Empty;

  }  // class LogEntryDto

} // namespace Empiria.OnePoint.Security.Reporting
