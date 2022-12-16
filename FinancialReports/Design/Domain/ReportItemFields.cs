/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : ReportItemFields                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Fields holder used to update financial report rows and cells.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Base holder used to update financial report cells or rows.</summary>
  internal class ReportItemFields {

    internal FinancialConcept FinancialConcept {
      get; set;
    }

    internal int Row {
      get; set;
    }

    internal string Label {
      get; set;
    }

    internal string Format {
      get; set;
    } = string.Empty;

  }  // class ReportItemFields



  /// <summary>Fields holder used to update financial report cells.</summary>
  internal class ReportCellFields : ReportItemFields {

    internal string Column {
      get; set;
    } = string.Empty;


    internal string DataField {
      get; set;
    } = string.Empty;


  }  // class ReportCellFields



  /// <summary>Fields holder used to update financial report rows.</summary>
  internal class ReportRowFields : ReportItemFields {

  }  // class ReportItemFields

}  // namespace Empiria.FinancialAccounting.FinancialReports
