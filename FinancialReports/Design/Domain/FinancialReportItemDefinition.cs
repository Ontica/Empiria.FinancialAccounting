/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportItemDefinition              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Base class for report rows or cells.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Base class for report rows or cells.</summary>
  public abstract class FinancialReportItemDefinition : BaseObject {

    #region Constructors and parsers

    protected FinancialReportItemDefinition() {
      // Required by Empiria Framework.
    }


    static public FinancialReportItemDefinition Parse(int id) {
      return BaseObject.ParseId<FinancialReportItemDefinition>(id);
    }


    static public FinancialReportItemDefinition Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportItemDefinition>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_CONCEPTO")]
    public FinancialConcept FinancialConcept {
      get; private set;
    }


    [DataField("ETIQUETA")]
    public string Label {
      get; private set;
    }


    [DataField("FORMATO")]
    public string Format {
      get; private set;
    }


    [DataField("FILA")]
    public int RowIndex {
      get; private set;
    }

    #endregion Properties

  }  // class FinancialReportItemDefinition

} // namespace Empiria.FinancialAccounting.FinancialReports
