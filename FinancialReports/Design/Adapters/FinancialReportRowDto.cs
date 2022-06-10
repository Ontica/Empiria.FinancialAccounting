/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : FinancialReportRowDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a row definition of a fixed rows type financial report.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO used to return a row definition of a fixed rows type financial report.</summary>
  public class FinancialReportRowDto {

    public string UID {
      get; internal set;
    }

    public string ConceptCode {
      get; internal set;
    }


    public string Concept {
      get; internal set;
    }


    public string Format {
      get; internal set;
    }


    public int Row {
      get; internal set;
    }


    public string FinancialConceptUID {
      get; internal set;
    }


    public string FinancialConceptGroupUID {
      get; internal set;
    }

  } // class FinancialReportRowDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
