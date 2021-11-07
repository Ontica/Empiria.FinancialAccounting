/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : ReportTypeDto                                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : DTO used to describe financial accounting system's report types.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>DTO used to describe financial accounting system's report types.</summary>
  public class ReportTypeDto {

    public string UID {
      get; internal set;
    } = string.Empty;


    public string Name {
      get; internal set;
    } = string.Empty;


    public string Group {
      get; internal set;
    } = string.Empty;


    public string PayloadType {
      get; internal set;
    } = string.Empty;


    public FileType[] ExportTo {
      get; internal set;
    } = new FileType[0];


  } // class ReportTypeDto

} // namespace Empiria.FinancialAccounting.Reporting
